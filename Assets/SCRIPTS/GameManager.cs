using System;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region DECLARAÇÕES
    private InputSystem_Actions inputActions;

    public enum EstadoJogo
    {
        Superficie,
        Submerso,
        GameOver
    }

    [Header("Estados do Jogo")]
    public EstadoJogo estadoJogo;
    [SerializeField] private bool superficie;
    [SerializeField] private EstadoJogo estadoAnterior;
    //public static event Action<EstadoJogo> OnEstadoJogoMudou; //Evento para notificar mudanças de estado do jogo

    [Header("Jogador")]
    [SerializeField] private Transform jogadorTransform;
    [SerializeField] private int vidasJogador = 3;                              //Fora a que começa, o jogador tem 3 vidas extras
    public static bool jogadorCheio;
    [SerializeField] private int humanosColetados;
    [SerializeField] private const int MAX_HUMANOS = 6;                         //REGRA
    [SerializeField] private bool jogadorPodeMover;

    [Header("Oxegênio")]
    [SerializeField] private int oxigenioSubmarino;
    [SerializeField] private const int OXIGENIO_MAXIMO = 100;

    [Header("Gerenciamento de Pontuação")]
    [SerializeField] private int pontuacaoTotal;
    [SerializeField] private const int PONTOS_VIDA_EXTRA = 10000;               //REGRA: Ganha 1 vida a cada 10.000 pontos
    [SerializeField] private int pontuacaoVidaExtra = 0;
    [SerializeField] private int pontosPeixe;
    [SerializeField] private int pontosSubmarino;
    [SerializeField] private int pontosHumano;
    [SerializeField] private const int MAX_PONTOS_HUMANOS = 1000;               //REGRA
    [SerializeField] private const int MAX_PONTOS_INIMIGOS = 90;                //REGRA
    [SerializeField] private int rodadasComSucesso = 0;                         //cada vez que o jogador sobe a superfície E tem 6 humanos, os pontos aumentam
    #endregion

    #region INICIAÇÕES

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void Start()
    {
        MudarEstadoJogo(EstadoJogo.Superficie);
        pontuacaoVidaExtra = PONTOS_VIDA_EXTRA;
    }

    void OnEnable()
    {
        inputActions.Enable();
        Acoes.JogadorMorto += JogadorMorto; //OUVIR - BaseInimigo da o sinal
        Acoes.InimigoMorto += InimigoMorto;
        Acoes.ColetouHumano += ColetouHumano;
    }

    void OnDisable()
    {
        inputActions.Disable();
        Acoes.JogadorMorto -= JogadorMorto;
        Acoes.InimigoMorto -= InimigoMorto;
        Acoes.ColetouHumano -= ColetouHumano;
    }
    #endregion

    void Update()
    {
        OnReiniciar();

        if(jogadorTransform.position.y < 2.6f && estadoJogo == EstadoJogo.Superficie)
        {
            MudarEstadoJogo(EstadoJogo.Submerso);
        }
        else if (jogadorTransform.position.y >= 2.6f && estadoJogo == EstadoJogo.Submerso)
        {
            MudarEstadoJogo(EstadoJogo.Superficie);
        }

        GanharVidaExtra();
    }

    #region COLISÕES
    void JogadorMorto(int pontos)
    {
        //reset loop do jogo
        MudarEstadoJogo(EstadoJogo.Superficie);
        pontosSubmarino = 20;
        pontosPeixe = 20;
        
        if (rodadasComSucesso == 0)
        {
            pontuacaoTotal += pontos;
        }
        else
        {
            int pontuacaoSomar;
            pontuacaoSomar = pontos + (10 * rodadasComSucesso);
            if(pontuacaoSomar >= MAX_PONTOS_INIMIGOS)
            {
                pontuacaoTotal += MAX_PONTOS_INIMIGOS;
            }
            else
            {
                pontuacaoTotal += pontuacaoSomar;
            }
        }

        vidasJogador--;
        Debug.LogWarning($"Jogador Morreu! Pontuação Total: {pontuacaoTotal} / Vidas: {vidasJogador}");

        if (vidasJogador <= 0)
        {
            MudarEstadoJogo(EstadoJogo.GameOver);
        }
    }

    void InimigoMorto(int pontos)
    {
        pontosSubmarino = 20;
        pontosPeixe = 20;
        if (rodadasComSucesso == 0)
        {
            pontuacaoTotal += pontos;
        }
        else
        {
            int pontuacaoSomar;
            pontuacaoSomar = pontos + (10 * rodadasComSucesso);
            if(pontuacaoSomar >= MAX_PONTOS_INIMIGOS)
            {
                pontuacaoTotal += MAX_PONTOS_INIMIGOS;
            }
            else
            {
                pontuacaoTotal += pontuacaoSomar;
            }
        }

        Debug.LogWarning($"Inimigo Morto: {pontos} / Pontuação Total: {pontuacaoTotal}");
    }

    void ColetouHumano(int pontos)
    {
        pontosHumano = pontos;
        if (humanosColetados < MAX_HUMANOS)
        {
            humanosColetados++;
            Debug.Log($"COLETADO! Humanos Coletados: {humanosColetados}");

            if (humanosColetados == MAX_HUMANOS)
            {
                jogadorCheio = true;
                Debug.Log("JOGADOR CHEIO! HUMANO NÃO COLETADO.");
            }
        }
        else
        {
            //Ativar Beep
            //Sprites inventário piscam
            //Humano não some!!!
        }
    }

    #endregion

    void IniciarJogo()
    {
        jogadorTransform.position = new Vector2(0, 3);      //Posição inicial do jogador na superfície

        if (oxigenioSubmarino != OXIGENIO_MAXIMO)           //Oxigênio não está cheio
        {
            //Iniciar preenchemento do oxigênio
            jogadorPodeMover = false;                       //impede o jogador de se mover enquanto o oxigênio estiver sendo preenchido
        }
        else
        {
            jogadorPodeMover = true;                        //permite o jogador se mover normalmente
        }

        if (jogadorPodeMover && oxigenioSubmarino == OXIGENIO_MAXIMO && jogadorTransform.position.y < 2.6f) //Saiu da superfície
        {
            estadoJogo = EstadoJogo.Submerso;
        }
    }

    void GameOver()
    {
        Debug.LogError("Game Over! O jogador perdeu todas as vidas.");
    }

    void MudarEstadoJogo(EstadoJogo novoEstado)
    {
        estadoJogo = novoEstado;
        Debug.Log($"Estado do Jogo mudou para: {estadoJogo}");

        switch (estadoJogo) 
        {
            //ESTADO_SUPERFICIE
            case EstadoJogo.Superficie:
                
                if (vidasJogador == 0)
                {
                    Time.timeScale = 0;
                    //Panel cobrindo ou DestroyInimigos()
                    //Aperte R para reiniciar
                }
                else
                {
                    IniciarJogo();
                    Debug.Log("SUPERFÍCIE");

                    if (estadoAnterior == EstadoJogo.Submerso)
                    {
                        ResolverHumanos();
                    }
                }
                break;
            //ESTADO_SUBMERSO
            case EstadoJogo.Submerso:

                Debug.Log("SUBMERSO");
                estadoAnterior = EstadoJogo.Submerso;

                break;
            //ESTADO_GAME_OVER
            case EstadoJogo.GameOver:

                GameOver(); //Chama o método de Game Over para lidar com a lógica de fim de jogo OU escrever todo o código de Game Over aqui mesmo (???)
                break;
        }
    }

    void ResolverHumanos() //PONTUAÇÃO E DIFICULDADE
    {
        if (humanosColetados == MAX_HUMANOS) // REGRA: rodada de sucesso, dificuldade e pontos aumentam;
                {
                    for(int i = 0; i < humanosColetados; i++)
                    {
                        int pontuacaoSomar;
                        pontuacaoSomar = pontosHumano * (rodadasComSucesso + 1); //HUMANO MULTIPLICA PORQUE VALOR DE INCREMENTO == PONTOS
                        if(pontuacaoSomar >= MAX_PONTOS_HUMANOS)
                        {
                             pontuacaoTotal += MAX_PONTOS_HUMANOS;
                        }
                        else
                        {
                            pontuacaoTotal += pontuacaoSomar;
                        }
                        Debug.Log($"PONTUAÇÃO HUMANOS = {pontuacaoTotal}");
                    }

                    humanosColetados = 0; //reseta humanos coletados para a próxima rodada
                    jogadorCheio = false;
                    rodadasComSucesso++; //aumenta pontos e velocidade dos inimigos (DIFICULDADE)
                }
                else if (humanosColetados == 0) //REGRA: Se o jogador subir a superfície sem coletar nenhum humano, perde 1 vida
                {
                    vidasJogador--;
                    Debug.LogWarning($"O jogador subiu a superfície sem coletar humanos! Vidas restantes: {vidasJogador}");
                    //UI Update aqui

                    if (vidasJogador <= 0)
                    {
                        MudarEstadoJogo(EstadoJogo.GameOver);
                    }

                }
                else // REGRA: perde um humano por cada vez que sobe a superfície sem estar cheio
                {
                    humanosColetados--;
                    //UI Upadate aqui
                }
    }

    void GanharVidaExtra()
    {
        if (pontuacaoTotal >= pontuacaoVidaExtra)
        {
            vidasJogador++;
            pontuacaoVidaExtra += PONTOS_VIDA_EXTRA;
            Debug.Log($"VIDA EXTRA. Vidas Atuais: {vidasJogador}");
        }
    }
    public void OnReiniciar()
    {
        if (inputActions.Player.Reiniciar.WasPressedThisFrame())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //Recarrega a cena atual
        }
    }
}