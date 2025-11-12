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
            // Mostra todas as cartas no início
            StartCoroutine(ShowAllCardsAtStart());
        }
    }

    void Update()
    {
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

                // Garante que é uma carta
                if (parent.GetComponent<Card>() == null)
                    return;

                cardToFlip = parent;

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

    public void StartFlipBack(GameObject cardToUnflip)
    {
        if (cardToUnflip != null)
            StartCoroutine(FlipAnimationObject(cardToUnflip));
    }

    private IEnumerator FlipAnimation()
    {
        if (cardToFlip == null || !cardToFlip.activeInHierarchy)
            yield break;

        yield return StartCoroutine(PerformFlip(cardToFlip));
        cardToFlip = null;
    }

    private IEnumerator FlipAnimationObject(GameObject card)
    {
        if (card == null || !card.activeInHierarchy)
            yield break;

        yield return StartCoroutine(PerformFlip(card));
    }

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

    private IEnumerator CheckMatchCoroutine()
    {
        gameManager.canClick = false;
        yield return new WaitForSeconds(0.6f);

        try
        {
            GameObject card1 = gameManager.cardFliped1;
            GameObject card2 = gameManager.cardFliped2;

            if (card1 == null || card2 == null)
                yield break;

            Card c1 = card1.GetComponent<Card>();
            Card c2 = card2.GetComponent<Card>();

            if (c1 == null || c2 == null)
                yield break;

            bool isMatch = false;

            if (!string.IsNullOrEmpty(c1.cardId) && !string.IsNullOrEmpty(c2.cardId))
                isMatch = string.Equals(c1.cardId.Trim(), c2.cardId.Trim());
            else
                isMatch = string.Equals(card1.tag?.Trim(), card2.tag?.Trim());

            if (isMatch)
            {
                // Marca e desativa — não destrói
                c1.MarkMatched();
                c2.MarkMatched();

                card1.SetActive(false);
                card2.SetActive(false);
            }
            else
            {
                if (card1 != null) StartCoroutine(FlipAnimationObject(card1));
                if (card2 != null) StartCoroutine(FlipAnimationObject(card2));
            }
        }
        finally
        {
            gameManager.cardFliped1 = null;
            gameManager.cardFliped2 = null;
            gameManager.canClick = true;
        }
    }

    private IEnumerator ShowAllCardsAtStart()
    {
        gameManager.canClick = false;

        Card[] allCards = Object.FindObjectsByType<Card>(FindObjectsSortMode.None);

        List<Coroutine> flips = new List<Coroutine>();
        foreach (Card c in allCards)
            flips.Add(StartCoroutine(PerformFlip(c.gameObject)));

        foreach (Coroutine flip in flips)
            yield return flip;

        yield return new WaitForSeconds(2f);

        flips.Clear();
        foreach (Card c in allCards)
            flips.Add(StartCoroutine(PerformFlip(c.gameObject)));

        foreach (Coroutine flip in flips)
            yield return flip;

        gameManager.canClick = true;
    }
}
