using UnityEngine;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.XR;

public class CardRandomizer : MonoBehaviour
{
    [Header("Configurações")]
    public string cardLayerName = "Card";  // Layer onde estão as cartas
    public string slotTagName = "Slot";    // Tag dos slots
    public float revealTime = 4f;          // Tempo que ficam viradas no início
    public float flipDuration = 0.35f;     // Duração da animação de virar
    [SerializeField]
    private GameObject[] cards;
    [SerializeField]
    private Transform[] slots;
    int rnd;
    int cont;
    int a = 0;
    int b = 0;
    int c = 0;
    int d = 0;
    int e = 0;
    int f = 0;
    int g = 0;
    int h = 0;
    int j = 0;

    void Start()
    {
        
        Debug.Log("spawCarta");
       // StartCoroutine(SetupCards());
        for (int i = 0; i < slots.Length; i++)
        {
            

           GerarCartas(i);
           
            
        }
    }

    void GerarCartas(int i)
    {
        bool cartaGerada = false;

        while (!cartaGerada)
        {
            rnd = Random.Range(0, cards.Length); // note que aqui vai até cards.Length, não -1

            switch (rnd)
            {
                case 0:
                    if (a < 2)
                    {
                        a++;
                        Instantiate(cards[rnd], slots[i].transform.position, Quaternion.identity);
                        cartaGerada = true;
                    }
                    break;
                case 1:
                    if (b < 2)
                    {
                        b++;
                        Instantiate(cards[rnd], slots[i].transform.position, Quaternion.identity);
                        cartaGerada = true;
                    }
                    break;
                case 2:
                    if (c < 2)
                    {
                        c++;
                        Instantiate(cards[rnd], slots[i].transform.position, Quaternion.identity);
                        cartaGerada = true;
                    }
                    break;
                case 3:
                    if (d < 2)
                    {
                        d++;
                        Instantiate(cards[rnd], slots[i].transform.position, Quaternion.identity);
                        cartaGerada = true;
                    }
                    break;
                case 4:
                    if (e < 2)
                    {
                        e++;
                        Instantiate(cards[rnd], slots[i].transform.position, Quaternion.identity);
                        cartaGerada = true;
                    }
                    break;
                case 5:
                    if (f < 2)
                    {
                        f++;
                        Instantiate(cards[rnd], slots[i].transform.position, Quaternion.identity);
                        cartaGerada = true;
                    }
                    break;
                case 6:
                    if (g < 2)
                    {
                        g++;
                        Instantiate(cards[rnd], slots[i].transform.position, Quaternion.identity);
                        cartaGerada = true;
                    }
                    break;
                case 7:
                    if (h < 2)
                    {
                        h++;
                        Instantiate(cards[rnd], slots[i].transform.position, Quaternion.identity);
                        cartaGerada = true;
                    }
                    break;
                case 8:
                    if (j < 2)
                    {
                        j++;
                        Instantiate(cards[rnd], slots[i].transform.position, Quaternion.identity);
                        cartaGerada = true;
                    }
                    break;
            }
        



}
    }

    IEnumerator SetupCards()
    {
        int cardLayer = LayerMask.NameToLayer(cardLayerName);

        // Pega todas as cartas na layer configurada
      
        // Pega todos os slots pela tag
       

        if (cards.Length == 0 || slots.Length == 0)
        {
            Debug.LogError("Cartas ou slots não encontrados!");
            yield break;
        }

        // Embaralha as cartas e os slots
        cards = cards.OrderBy(x => Random.value).ToArray();
        slots = slots.OrderBy(x => Random.value).ToArray();

        // Garante que não tente acessar fora do limite
        int count = Mathf.Min(cards.Length, slots.Length);

        // Posiciona as cartas nos slots
        for (int i = 0; i < count; i++)
        {
            cards[i].transform.position = slots[i].position;
            cards[i].transform.rotation = Quaternion.identity;
        }

        // Vira todas as cartas para mostrar
        foreach (var card in cards)
        {
            StartCoroutine(FlipCard(card, true));
        }

        // Espera o tempo definido
        yield return new WaitForSeconds(revealTime);

        // Desvira todas as cartas
        foreach (var card in cards)
        {
            if (card != null)
                StartCoroutine(FlipCard(card, false));
        }
    }

    IEnumerator FlipCard(GameObject card, bool faceUp)
    {
        if (card == null) yield break;

        Quaternion startRot = card.transform.rotation;
        Quaternion endRot = faceUp
            ? Quaternion.Euler(180f, 0f, 0f)
            : Quaternion.identity;

        float t = 0f;
        while (t < flipDuration)
        {
            card.transform.rotation = Quaternion.Slerp(startRot, endRot, t / flipDuration);
            t += Time.deltaTime;
            yield return null;
        }

        card.transform.rotation = endRot;
    }
}
