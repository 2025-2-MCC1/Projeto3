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
        gameManager = GameObject.FindFirstObjectByType<GameManager>();
        if (gameManager == null)
            Debug.LogError("GameManager não encontrado!");
    }

    void Update()
    {
        if (isAnimating || gameManager == null || !gameManager.canClick) return;

        if (Input.GetMouseButtonDown(0))
        {
            Camera mainCamera = Camera.main;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                GameObject parent = clickedObject.transform.parent != null ? clickedObject.transform.parent.gameObject : clickedObject;

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

    private IEnumerator PerformFlip(GameObject card)
    {
        if (card == null || !card.activeInHierarchy)
            yield break;

        isAnimating = true;

        Quaternion startRot = card.transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(180f, 0f, 0f);
        float elapsed = 0f;

        while (elapsed < flipDuration)
        {
            card.transform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / flipDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        card.transform.rotation = endRot;
        isAnimating = false;
    }
}
