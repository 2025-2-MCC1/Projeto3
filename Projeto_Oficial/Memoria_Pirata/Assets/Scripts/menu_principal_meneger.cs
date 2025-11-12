using UnityEngine;

// Para importar as açoes do jogo
using UnityEngine.SceneManagement;
public class menu_principal_meneger : MonoBehaviour
{
    [SerializeField]private string nomeDoLevelDeJogo;
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;


    public void Jogar()
    {
        SceneManager.LoadScene(1);
    }

    public void AbrirOpcoes()
    {
        painelMenuInicial.SetActive(false);
        painelOpcoes.SetActive(true);
    }

    public void FecharOpcoes()
    {
        painelOpcoes.SetActive(false);
        painelMenuInicial.SetActive(true);
    }

    public void SairJogo()
    {
        // o application quit so funciona com o jogo compilado, então o debug.log da um feedback
        Debug.Log("Sair do jogo");
        Application.Quit();
    }
}
