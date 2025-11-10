using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CronometroJogo : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste aqui o texto do cronômetro (TextMeshProUGUI) que aparece na tela.")]
    public TextMeshProUGUI textoCronometro;

    [Header("Configurações de Tempo")]
    [SerializeField] private float tempoInicial = 60f; // Tempo total em segundos

    private float tempoRestante;
    private bool cronometroAtivo;

    [Header("Configurações de Cena")]
    [SerializeField] private int indiceCenaGameOver = 2;
    [SerializeField] private float delayAntesTrocarCena = 1f;

    void Start()
    {
        // Inicializa o tempo e garante que o texto não vai gerar erros
        tempoRestante = tempoInicial;
        cronometroAtivo = true;

        if (textoCronometro != null)
            AtualizarDisplay(tempoRestante);
        else
            Debug.Log("⚠️ Cronômetro iniciado sem texto atribuído no Inspector. O jogo continuará normalmente.");
    }

    void Update()
    {
        if (!cronometroAtivo)
            return;

        // Contagem regressiva
        tempoRestante -= Time.deltaTime;

        if (tempoRestante <= 0f)
        {
            tempoRestante = 0f;
            cronometroAtivo = false;

            if (textoCronometro != null)
                AtualizarDisplay(tempoRestante);

            // Inicia troca de cena com delay
            StartCoroutine(TrocarCenaComDelay());
        }
        else
        {
            if (textoCronometro != null)
                AtualizarDisplay(tempoRestante);
        }
    }

    private void AtualizarDisplay(float tempo)
    {
        int minutos = Mathf.FloorToInt(tempo / 60);
        int segundos = Mathf.FloorToInt(tempo % 60);

        textoCronometro.text = $"{minutos:00}:{segundos:00}";
    }

    private IEnumerator TrocarCenaComDelay()
    {
        if (textoCronometro != null)
            textoCronometro.text = "TEMPO ESGOTADO!";

        Debug.Log($"⏰ Cronômetro zerou — trocando para cena {indiceCenaGameOver} em {delayAntesTrocarCena}s.");

        yield return new WaitForSeconds(delayAntesTrocarCena);

        if (indiceCenaGameOver >= 0 && indiceCenaGameOver < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(indiceCenaGameOver);
        }
        else
        {
            Debug.LogError("❌ Índice da cena inválido. Verifique em File > Build Settings.");
        }
    }
}
