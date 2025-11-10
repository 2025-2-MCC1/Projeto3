using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardFlip : MonoBehaviour
{
    public float flipDuration = 0.35f; // Duração da animação
    private GameObject cardToFlip = null;
    public GameManager gameManager;

    private void Start()
    {
        // Encontra o GameManager na cena
        gameManager = GameObject.FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager não encontrado!");
        }
        else
        {
            // Inicia a sequência automática de mostrar as cartas
            StartCoroutine(ShowAllCardsAtStart());
        }
    }

    void Update()
    {
        // Bloqueia cliques se o GameManager não permitir
        if (gameManager == null || !gameManager.canClick)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Camera mainCamera = Camera.main;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                GameObject parent = clickedObject.transform.parent != null
                    ? clickedObject.transform.parent.gameObject
                    : clickedObject;

                // Ignora clique se não for uma carta
                if (parent.GetComponent<Card>() == null)
                {
                    Debug.Log("Clique fora da carta ignorado.");
                    return;
                }

                cardToFlip = parent;

                // Impede clicar na mesma carta ou em mais de duas
                if (cardToFlip == gameManager.cardFliped1 || cardToFlip == gameManager.cardFliped2)
                    return;

                if (gameManager.cardFliped1 != null && gameManager.cardFliped2 != null)
                    return;

                bool registered = false;
                if (gameManager.cardFliped1 == null)
                {
                    gameManager.cardFliped1 = cardToFlip;
                    registered = true;
                }
                else if (gameManager.cardFliped2 == null)
                {
                    gameManager.cardFliped2 = cardToFlip;
                    registered = true;
                    StartCoroutine(CheckMatchCoroutine());
                }

                if (registered)
                    StartCoroutine(FlipAnimation());
            }
        }
    }

    // Chamado pelo GameManager para desvirar cartas
    public void StartFlipBack(GameObject cardToUnflip)
    {
        if (cardToUnflip != null)
        {
            StartCoroutine(FlipAnimationObject(cardToUnflip));
        }
    }

    private IEnumerator FlipAnimation()
    {
        if (cardToFlip == null || !cardToFlip.activeInHierarchy)
        {
            cardToFlip = null;
            yield break;
        }

        yield return StartCoroutine(PerformFlip(cardToFlip));
        cardToFlip = null;
    }

    private IEnumerator FlipAnimationObject(GameObject card)
    {
        if (card == null || !card.activeInHierarchy)
            yield break;

        yield return StartCoroutine(PerformFlip(card));
    }

    // Faz a rotação da carta (vira ou desvira)
    private IEnumerator PerformFlip(GameObject card)
    {
        if (card == null || !card.activeInHierarchy)
            yield break;

        Quaternion startRotation = card.transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(180f, 0f, 0f);

        float elapsedTime = 0f;

        while (elapsedTime < flipDuration)
        {
            card.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / flipDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        card.transform.rotation = endRotation;
    }

    //  Corrigido: sem yield dentro de finally
    private IEnumerator CheckMatchCoroutine()
    {
        gameManager.canClick = false;
        yield return new WaitForSeconds(0.6f);

        bool liberarClique = true;

        try
        {
            GameObject card1 = gameManager.cardFliped1;
            GameObject card2 = gameManager.cardFliped2;

            if (card1 == null || card2 == null)
            {
                Debug.LogWarning("Alguma carta foi destruída antes da verificação.");
                yield break;
            }

            Card c1 = card1.GetComponent<Card>();
            Card c2 = card2.GetComponent<Card>();

            if (c1 == null || c2 == null)
            {
                Debug.LogWarning("Uma das cartas não tem o componente Card.");
                yield break;
            }

            bool isMatch = false;

            // Compara cardId se existir, senão usa tag
            if (!string.IsNullOrEmpty(c1.cardId) && !string.IsNullOrEmpty(c2.cardId))
            {
                isMatch = string.Equals(c1.cardId.Trim(), c2.cardId.Trim(), System.StringComparison.Ordinal);
            }
            else
            {
                isMatch = string.Equals(card1.tag?.Trim(), card2.tag?.Trim(), System.StringComparison.Ordinal);
            }

            if (isMatch)
            {
                yield return new WaitForSeconds(0.25f);
                if (card1 != null) Destroy(card1);
                if (card2 != null) Destroy(card2);
            }
            else
            {
                if (card1 != null) StartCoroutine(FlipAnimationObject(card1));
                if (card2 != null) StartCoroutine(FlipAnimationObject(card2));
            }
        }
        finally
        {
            // Não pode usar yield aqui
            gameManager.cardFliped1 = null;
            gameManager.cardFliped2 = null;
            gameManager.canClick = true;
            liberarClique = false;
        }

        if (liberarClique)
        {
            // Garante pequeno delay fora do finally
            yield return new WaitForSeconds(0.2f);
            gameManager.canClick = true;
        }
    }

    //  Vira todas as cartas juntas no início (2 segundos viradas)
    private IEnumerator ShowAllCardsAtStart()
    {
        gameManager.canClick = false;

        Card[] allCards = Object.FindObjectsByType<Card>(FindObjectsSortMode.None);

        // Vira todas simultaneamente
        List<Coroutine> flips = new List<Coroutine>();
        foreach (Card c in allCards)
            flips.Add(StartCoroutine(PerformFlip(c.gameObject)));

        foreach (Coroutine flip in flips)
            yield return flip;

        // Mantém 2 segundos viradas
        yield return new WaitForSeconds(2f);

        // Desvira todas simultaneamente
        flips.Clear();
        foreach (Card c in allCards)
            flips.Add(StartCoroutine(PerformFlip(c.gameObject)));

        foreach (Coroutine flip in flips)
            yield return flip;

        // Libera cliques
        gameManager.canClick = true;
    }
}
