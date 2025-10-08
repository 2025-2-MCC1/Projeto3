using UnityEngine;
using System.Collections;

public class CardFlip : MonoBehaviour
{
    // Vari�vel para controlar a dura��o da anima��o (em segundos)
    public float flipDuration = 0.35f;

    // Vari�vel tempor�ria para a carta clicada no Update
    private GameObject cardToFlip = null;

    // Refer�ncia ao GameManager
    public GameManager gameManager;

    // Vari�vel para evitar que a anima��o seja interrompida por cliques repetidos
    private bool isAnimating = false;

    private void Start()
    {
        // Encontra o gamemanager na cena:
        GameObject managerObject = GameObject.Find("GameManager");
        if (managerObject != null)
        {
            gameManager = managerObject.GetComponent<GameManager>();
        }
        else
        {
            Debug.LogError("GameManager n�o encontrado na cena. Verifique se o nome est� correto e o componente existe.");
        }
    }

    void Update()
    {
        // Garante que a carta n�o est� no meio de outra anima��o
        if (isAnimating || gameManager == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Camera mainCamera = Camera.main;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 1. Encontra o objeto PAI (a carta completa)
                GameObject clickedObject = hit.collider.gameObject;
                if (clickedObject.transform.parent != null)
                {
                    cardToFlip = clickedObject.transform.parent.gameObject;

                    // 2. CHECA se a carta J� FOI VIRADA ou se J� H� DUAS viradas
                    if (cardToFlip == gameManager.cardFliped1 || cardToFlip == gameManager.cardFliped2)
                    {
                        Debug.Log("Voc� j� virou esta carta.");
                        cardToFlip = null;
                        return;
                    }
                    if (gameManager.cardFliped1 != null && gameManager.cardFliped2 != null)
                    {
                        Debug.Log("Duas cartas j� est�o viradas. Aguarde a compara��o.");
                        cardToFlip = null;
                        return;
                    }

                    // 3. REGISTRA A CARTA
                    bool cardRegistered = false;
                    if (gameManager.cardFliped1 == null)
                    {
                        gameManager.cardFliped1 = cardToFlip;
                        Debug.Log("Virou a primeira carta: " + cardToFlip.name);
                        cardRegistered = true;
                    }
                    else if (gameManager.cardFliped2 == null)
                    {
                        gameManager.cardFliped2 = cardToFlip;
                        Debug.Log("Virou a segunda carta: " + cardToFlip.name);
                        gameManager.CheckForMatch(); // NOVO: Chama o GameManager para checar
                        cardRegistered = true;
                    }

                    // 4. ANIMA
                    if (cardRegistered)
                    {
                        // Usa a Coroutine original que l� a vari�vel de classe cardToFlip
                        StartCoroutine(FlipAnimation());
                    }
                }
            }
        }
    }

    // O GameManager chama este m�todo, passando a carta que deve ser desvirada
    public void StartFlipBack(GameObject cardToUnflip)
    {
        // Garante que n�o est� animando antes de iniciar
        if (!isAnimating)
        {
            // Chama a nova Coroutine que usa o objeto passado como par�metro
            StartCoroutine(FlipAnimationObject(cardToUnflip));
        }
    }

    // Coroutine usada pelo clique (Update)
    private IEnumerator FlipAnimation()
    {
        // Verifica se a vari�vel de classe est� definida (deve estar, pois o Update a preencheu)
        if (cardToFlip == null)
        {
            yield break;
        }

        // Esta Coroutine usa a vari�vel de classe 'cardToFlip'
        yield return StartCoroutine(PerformFlip(cardToFlip));

        // Zera a refer�ncia AP�S a primeira virada
        cardToFlip = null;
    }

    // Coroutine usada pelo GameManager (StartFlipBack)
    private IEnumerator FlipAnimationObject(GameObject card)
    {
        // Esta Coroutine usa o objeto passado como par�metro
        yield return StartCoroutine(PerformFlip(card));
    }

    // Coroutine central para a l�gica de flip, usada por ambas
    private IEnumerator PerformFlip(GameObject card)
    {
        if (card == null) yield break;

        isAnimating = true;

        Quaternion startRotation = card.transform.rotation;
        // Rotaciona 180 graus a partir da rota��o atual (serve para virar e desvirar)
        Quaternion endRotation = startRotation * Quaternion.Euler(180f, 0f, 0f);

        float elapsedTime = 0f;

        while (elapsedTime < flipDuration)
        {
            card.transform.rotation = Quaternion.Slerp(
                startRotation,
                endRotation,
                elapsedTime / flipDuration
            );

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        card.transform.rotation = endRotation;
        isAnimating = false;
    }
}