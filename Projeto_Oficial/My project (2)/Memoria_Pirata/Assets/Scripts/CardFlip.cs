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
            Debug.LogError("GameManager não encontrado!");
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
                GameObject parent = clickedObject.transform.parent != null ? clickedObject.transform.parent.gameObject : clickedObject;

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
                    gameManager.CheckForMatch();
                    registered = true;
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
            StartCoroutine(FlipAnimationObject(cardToUnflip));
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

        isAnimating = true;

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
        isAnimating = false;
    }
}
