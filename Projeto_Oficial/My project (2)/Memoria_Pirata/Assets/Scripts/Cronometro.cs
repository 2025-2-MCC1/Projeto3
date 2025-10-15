// Isso aqui � pra avisar o Unity que a gente vai usar aquele texto mais bonito, o TextMeshPro.
using TMPro;
using UnityEngine;

// Aqui come�a a nossa "receita de bolo" pro cron�metro.
public class CronometroJogo : MonoBehaviour
{
    // Crio uma "caixinha" p�blica no Inspector pra eu poder arrastar o objeto de texto da tela pra c�.
    public TextMeshProUGUI textoCronometro;

    // Aqui eu defino quanto tempo o jogador vai ter. 120 segundos = 2 minutos.
    [SerializeField] private float tempoInicial = 120f;

    // Essa vari�vel vai guardar o tempo que vai diminuindo.
    private float tempoRestante;
    // Essa aqui funciona como um interruptor pra dizer se o cron�metro est� ligado ou n�o.
    private bool cronometroAtivo = false;

    // Essa parte do c�digo roda uma �nica vez, bem quando o objeto aparece na cena (tipo no "Play").
    void Start()
    {
        // J� manda ligar o cron�metro logo de cara.
        IniciarCronometro();
    }

    // Essa fun��o � um loop infinito, o Unity fica chamando ela o tempo todo, v�rias vezes por segundo.
    void Update()
    {
        // S� faz alguma coisa se o nosso "interruptor" estiver ligado.
        if (cronometroAtivo)
        {
            // Se o tempo restante ainda for maior que zero...
            if (tempoRestante > 0)
            {
                // ... a m�gica acontece aqui! A gente diminui um pouquinho do tempo a cada frame.
                tempoRestante -= Time.deltaTime;

                // Manda a fun��o l� de baixo atualizar o que aparece na tela.
                AtualizarDisplay(tempoRestante);
            }
            // Se o tempo n�o for maior que zero, significa que acabou!
            else
            {
                // Zera o tempo pra n�o ficar negativo e desliga o "interruptor".
                tempoRestante = 0;
                cronometroAtivo = false;

                // Manda atualizar a tela uma �ltima vez (pra mostrar "00:00").
                AtualizarDisplay(tempoRestante);

                // Agora, chama a fun��o que faz as coisas de "fim de jogo".
                FimDeJogo();
            }
        }
    }

    // Essa � a fun��o que a gente chama pra dar o "start" no cron�metro.
    public void IniciarCronometro()
    {
        // Pega o tempo inicial que a gente definiu l� em cima e coloca no tempo restante.
        tempoRestante = tempoInicial;
        // Liga o nosso "interruptor".
        cronometroAtivo = true;
    }

    // Essa fun��o serve pra pegar o n�mero "quebrado" do tempo e deixar ele bonitinho na tela.
    private void AtualizarDisplay(float tempo)
    {
        // Uma seguran�a pra n�o mostrar tempo negativo na tela, tipo "-1 segundos".
        if (tempo < 0)
        {
            tempo = 0;
        }

        // Pega o tempo total em segundos e quebra ele em minutos e segundos.
        float minutos = Mathf.FloorToInt(tempo / 60);
        float segundos = Mathf.FloorToInt(tempo % 60);

        // Monta o texto no formato "00:00" e manda l� pra caixa de texto da tela.
        textoCronometro.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    // Aqui a gente define o que acontece quando o tempo zera.
    private void FimDeJogo()
    {
        // Manda uma mensagem pro Console do Unity (s� pra gente, o dev, saber que funcionou).
        Debug.Log("O JOGO ACABOU!");

        // Muda o texto na tela do jogo para o jogador ver.
        textoCronometro.text = "TEMPO ESGOTADO!";

        // DICA: Aqui seria um bom lugar pra colocar o c�digo que impede o jogador de continuar clicando nas cartas.
    }
}

