using UnityEngine;
using System.Collections;

public class CardFlip : MonoBehaviour
{
    public float flipDuration = 0.35f; // Duração da animação
    private GameObject cardToFlip = null;
    private bool isAnimating = false;
    public GameManager gameManager;

    private void Start()
    {
        // Encontra o GameManager na cena
        gameManager = GameObject.FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager não encontrado!");
        }
    }

    void Update()
    {
        // Bloqueia cliques durante animação ou se GameManager não existe
        if (isAnimating || gameManager == null || !gameManager.canClick)
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

                // Ignora clique se não for uma carta (ex: clicou no fundo/cubo)
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
        if (!isAnimating)
        {
            StartCoroutine(FlipAnimationObject(cardToUnflip));
        }
    }

    // Coroutine do clique
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

    // Coroutine usada pelo GameManager
    private IEnumerator FlipAnimationObject(GameObject card)
    {
        if (card == null || !card.activeInHierarchy)
            yield break;

        yield return StartCoroutine(PerformFlip(card));
    }

    // Lógica de rotação (vira ou desvira)
    private IEnumerator PerformFlip(GameObject card)
    {
        if (card == null || !card.activeInHierarchy)
            yield break;

        Quaternion startRotation = card.transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(180f, 0f, 0f);

        isAnimating = true;
        float elapsedTime = 0f;

        while (elapsedTime < flipDuration)
        {
            card.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / flipDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        card.transform.rotation = endRotation;
        isAnimating = false;
    }

    // Verifica se as duas cartas viradas são iguais
    private IEnumerator CheckMatchCoroutine()
    {
        gameManager.canClick = false;

        // Espera antes de checar
        yield return new WaitForSeconds(0.6f);

        GameObject card1 = gameManager.cardFliped1;
        GameObject card2 = gameManager.cardFliped2;

        if (card1 != null && card2 != null)
        {
            Card c1 = card1.GetComponent<Card>();
            Card c2 = card2.GetComponent<Card>();

            if (c1 == null || c2 == null)
            {
                Debug.LogWarning("Uma das cartas não tem o componente Card.");
            }
            else
            {
                bool isMatch = false;

                // Usa cardId quando disponível, senão compara tag
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
                    // Faz um pequeno atraso antes de destruir
                    yield return new WaitForSeconds(0.25f);

                    if (card1 != null) Destroy(card1);
                    if (card2 != null) Destroy(card2);
                }
                else
                {
                    // Se forem diferentes, desvira as duas
                    StartCoroutine(FlipAnimationObject(card1));
                    StartCoroutine(FlipAnimationObject(card2));
                }
            }
        }

        // Limpa e reativa cliques
        gameManager.cardFliped1 = null;
        gameManager.cardFliped2 = null;

        // Espera um pouquinho antes de liberar clique
        yield return new WaitForSeconds(0.2f);
        gameManager.canClick = true;
    }
}
