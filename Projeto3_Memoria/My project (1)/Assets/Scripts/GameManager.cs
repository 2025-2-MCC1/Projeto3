using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Vari�veis p�blicas para armazenar as cartas viradas
    public GameObject cardFliped1;
    public GameObject cardFliped2;

    // O tempo de espera antes de desvirar as cartas
    public float matchCheckDelay = 1.0f;

    // O CardFlip chama este m�todo quando a segunda carta � virada
    public void CheckForMatch()
    {
        // Come�a a coroutine para verificar o par ap�s um atraso
        StartCoroutine(CheckMatchAndReset());
    }

    private IEnumerator CheckMatchAndReset()
    {
        // Espera por um momento para que o jogador veja a segunda carta
        yield return new WaitForSeconds(matchCheckDelay);

        // Certifique-se de que ambas as cartas ainda existem (n�o foram destru�das se fosse um par)
        if (cardFliped1 == null || cardFliped2 == null)
        {
            // Se algum for nulo (p. ex., j� foram destru�dos por acerto), apenas reseta o estado
            cardFliped1 = null;
            cardFliped2 = null;
            yield break;
        }

        // *************** L�GICA DE VERIFICA��O DE PAR ***************

        // ** Aqui ficaria sua l�gica real de checagem de match (por ID, Tag, etc.) **
        bool isMatch = false;

        if (!isMatch)
        {
            // Se N�O for um par, DESVIRA as duas cartas.

            CardFlip flip1 = cardFliped1.GetComponent<CardFlip>();
            CardFlip flip2 = cardFliped2.GetComponent<CardFlip>();

            if (flip1 != null)
            {
                // CHAMA O NOVO M�TODO: Passa a refer�ncia da carta para ela se desvirar
                flip1.StartFlipBack(cardFliped1);
            }
            if (flip2 != null)
            {
                // CHAMA O NOVO M�TODO: Passa a refer�ncia da carta para ela se desvirar
                flip2.StartFlipBack(cardFliped2);
            }

            // Aguarda a dura��o da anima��o para que ela complete antes de liberar o clique
            yield return new WaitForSeconds(0.4f);
        }
        else
        {
            // L�gica para quando FOR um par (p. ex., desativar ou destruir as cartas)
            Debug.Log("Match encontrado!");
            // ... (Destruir ou desativar cartas aqui) ...
        }

        // *************** A��es para resetar o GameManager ***************
        // Zera as refer�ncias DEPOIS de toda a anima��o ou l�gica de acerto.
        cardFliped1 = null;
        cardFliped2 = null;
    }
}