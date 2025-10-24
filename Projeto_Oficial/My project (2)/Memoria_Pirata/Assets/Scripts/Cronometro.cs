using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CronometroJogo : MonoBehaviour
{
    public TextMeshProUGUI textoCronometro;
    [SerializeField] private float tempoInicial = 120f;

    private float tempoRestante;
    private bool cronometroAtivo = false;

    // Índice da cena de Game Over
    private int indiceCenaGameOver = 2;

    // Delay antes de trocar de cena (segundos)
    [SerializeField] private float delayAntesTrocarCena = 1f;

    void Start()
    {
        tempoRestante = tempoInicial;
        cronometroAtivo = true;
        AtualizarDisplay(tempoRestante);

        if (textoCronometro == null)
            Debug.LogWarning("CronometroJogo: Texto Cronometro não foi atribuído no Inspector!");
    }

    void Update()
    {
        if (!cronometroAtivo) return;

        tempoRestante -= Time.deltaTime;

        if (tempoRestante <= 0f)
        {
            tempoRestante = 0f;
            cronometroAtivo = false;
            AtualizarDisplay(tempoRestante);
            StartCoroutine(TrocarCenaComDelay());
        }
        else
        {
            AtualizarDisplay(tempoRestante);
        }
    }

    private void AtualizarDisplay(float tempo)
    {
        int minutos = Mathf.FloorToInt(tempo / 60);
        int segundos = Mathf.FloorToInt(tempo % 60);

        if (textoCronometro != null)
            textoCronometro.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    private IEnumerator TrocarCenaComDelay()
    {
        // Mostra a mensagem final
        if (textoCronometro != null)
            textoCronometro.text = "TEMPO ESGOTADO!";

        Debug.Log("CronometroJogo: tempo zerou, aguardando " + delayAntesTrocarCena + "s antes de trocar de cena.");
        yield return new WaitForSeconds(delayAntesTrocarCena);

        // Verifica se o índice existe na Build Settings
        if (indiceCenaGameOver >= 0 && indiceCenaGameOver < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("CronometroJogo: carregando cena de índice " + indiceCenaGameOver);
            SceneManager.LoadScene(indiceCenaGameOver);
        }
        else
        {
            Debug.LogError("CronometroJogo: índice da cena inválido! Verifique Build Settings.");
        }
    }
}
