using UnityEngine;
using UnityEngine.SceneManagement;

public class GerenciadorDeCenas : MonoBehaviour
{
    public void IrParaJogo()
    {
        SceneManager.LoadScene(1);
    }

    public void IrParaMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void SairDoJogo()
    {
        Debug.Log("Saindo do jogo...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Para o Play Mode no Editor
#else
        Application.Quit(); // Fecha o jogo no build
#endif
    }
}
