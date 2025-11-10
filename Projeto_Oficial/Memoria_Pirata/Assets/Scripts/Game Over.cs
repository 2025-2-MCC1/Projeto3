using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [Header("Painel da tela de Game Over")]
    [SerializeField] private GameObject painelGameOver;

    private bool jogoAcabou = false;

    void Start()
    {
        // Garante que o painel começa desativado
        if (painelGameOver != null)
            painelGameOver.SetActive(false);
    }

    // Chame este método quando o jogador perder
    public void AtivarGameOver()
    {
        if (jogoAcabou) return;
        jogoAcabou = true;

        Time.timeScale = 0f; // pausa o jogo
        painelGameOver.SetActive(true);
        Debug.Log("Game Over ativado!");
    }

    // Botão "Jogar novamente"
    public void JogarNovamente()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Botão "Voltar ao menu"
    public void VoltarAoMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // 0 = cena do menu principal
    }
}
