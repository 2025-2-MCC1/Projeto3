using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private string TentardeNovo;
    [SerializeField] private GameObject VoltaraoMenu;
    [SerializeField] private GameObject Abandonar;
    public void Play()
    {
        SceneManager.LoadScene(1);
    }
    public void Menu()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Jogador Abandonou o Jogo");
    }
}
