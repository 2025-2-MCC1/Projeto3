using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject cardFliped1;
    public GameObject cardFliped2;
    public float matchCheckDelay = 1.0f;
    public bool canClick = true; // Bloqueia clique durante animação

    public void CheckForMatch()
    {
        StartCoroutine(CheckMatchAndReset());
    }

    private IEnumerator CheckMatchAndReset()
    {
        canClick = false;
        yield return new WaitForSeconds(matchCheckDelay);

        if (cardFliped1 == null || cardFliped2 == null)
        {
            cardFliped1 = null;
            cardFliped2 = null;
            canClick = true;
            yield break;
        }

        CardInfo info1 = cardFliped1.GetComponent<CardInfo>();
        CardInfo info2 = cardFliped2.GetComponent<CardInfo>();

        bool isMatch = info1 != null && info2 != null && info1.id == info2.id;

        if (!isMatch)
        {
            CardFlip flip1 = cardFliped1.GetComponent<CardFlip>();
            CardFlip flip2 = cardFliped2.GetComponent<CardFlip>();

            if (flip1 != null) flip1.StartFlipBack(cardFliped1);
            if (flip2 != null) flip2.StartFlipBack(cardFliped2);

            yield return new WaitForSeconds(0.5f); // aguarda animação desvirar
        }
        else
        {
            // Desativa antes de destruir para evitar que coroutines travem
            GameObject temp1 = cardFliped1;
            GameObject temp2 = cardFliped2;

            cardFliped1 = null;
            cardFliped2 = null;

            temp1.SetActive(false);
            temp2.SetActive(false);

            Destroy(temp1, 0.1f);
            Destroy(temp2, 0.1f);
        }

        cardFliped1 = null;
        cardFliped2 = null;
        canClick = true;
    }
}
