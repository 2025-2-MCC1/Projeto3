using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CronometroJogo : MonoBehaviour
{
    [Header("Referências")]
    public TextMeshProUGUI textoCronometro;

    [Header("Configurações de Tempo")]
    [SerializeField] private float tempoInicial = 60f; // 60 segundos

    private float tempoRestante;
    private bool cronometroAtivo;

    [Header("Configurações de Cena")]
    [SerializeField] private int indiceCenaGameOver = 2;
    [SerializeField] private float delayAntesTrocarCena = 1f;

    void Start()
    {
        if (textoCronometro == null)
            Debug.LogWarning("CronometroJogo: Texto Cronômetro não foi atribuído no Inspector!");

        // Inicializa o tempo
        tempoRestante = tempoInicial;
        cronometroAtivo = true;

        AtualizarDisplay(tempoRestante);
    }

    void Update()
    {
        if (!cronometroAtivo)
            return;

        // Diminui o tempo de acordo com o tempo real
        tempoRestante -= Time.deltaTime;

        if (tempoRestante <= 0f)
        {
            // Garante que o tempo não fique negativo
            tempoRestante = 0f;
            cronometroAtivo = false;
            AtualizarDisplay(tempoRestante);

            // Inicia a troca de cena
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
            textoCronometro.text = $"{minutos:00}:{segundos:00}";
    }

    private IEnumerator TrocarCenaComDelay()
    {
        if (textoCronometro != null)
            textoCronometro.text = "TEMPO ESGOTADO!";

        Debug.Log($"CronometroJogo: tempo zerou, aguardando {delayAntesTrocarCena}s antes de trocar de cena.");

        yield return new WaitForSeconds(delayAntesTrocarCena);

        if (indiceCenaGameOver >= 0 && indiceCenaGameOver < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"CronometroJogo: carregando cena de índice {indiceCenaGameOver}");
            SceneManager.LoadScene(indiceCenaGameOver);
        }
        else
        {
            Debug.LogError("CronometroJogo: índice da cena inválido! Verifique Build Settings.");
        }
    }
}
