using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

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

    [Header("Jogador")]
    [SerializeField] private Transform jogadorTransform;
    [SerializeField] private int vidasJogador = 3;                              //Fora a que começa, o jogador tem 3 vidas extras
    public static bool jogadorCheio;
    [SerializeField] private int humanosColetados;
    [SerializeField] private const int MAX_HUMANOS = 6;                         //REGRA
    [SerializeField] private bool jogadorPodeMover;
    [SerializeField] private bool superficiePorMorte;

    [Header("Oxegênio")]
    [SerializeField] private float oxigenioSubmarino;
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

        if(jogadorTransform.position.y < 2.8f && estadoJogo == EstadoJogo.Superficie)
        {
            MudarEstadoJogo(EstadoJogo.Submerso);
        }
        else if (jogadorTransform.position.y >= 2.8f && estadoJogo == EstadoJogo.Submerso)
        {
            MudarEstadoJogo(EstadoJogo.Superficie);
        }
        OxigenioSubmarino();
        GanharVidaExtra();
    }

    #region COLISÕES
    void JogadorMorto(int pontos)
    {
        //reset loop do jogo
        superficiePorMorte = true;
        MudarEstadoJogo(EstadoJogo.Superficie);
        pontosSubmarino = 20;
        pontosPeixe = 20;
        oxigenioSubmarino = 0;
        
        if (rodadasComSucesso == 0)
        {
            pontuacaoTotal += pontos;
            Acoes.UIResolverPontuacao?.Invoke(pontuacaoTotal); //Atualiza UI
        }
        else
        {
            int pontuacaoSomar;
            pontuacaoSomar = pontos + (10 * rodadasComSucesso);
            if(pontuacaoSomar >= MAX_PONTOS_INIMIGOS)
            {
                pontuacaoTotal += MAX_PONTOS_INIMIGOS;
                Acoes.UIResolverPontuacao?.Invoke(pontuacaoTotal); //Atualiza UI
            }
            else
            {
                pontuacaoTotal += pontuacaoSomar;
                Acoes.UIResolverPontuacao?.Invoke(pontuacaoTotal); //Atualiza UI
            }
        }

        vidasJogador--;
        Acoes.UIVidaJogador?.Invoke(vidasJogador); //Atualiza UI de vidas
        Debug.Log($"Jogador Morreu! Pontuação Total: {pontuacaoTotal} / Vidas: {vidasJogador}");

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
            Acoes.UIResolverPontuacao?.Invoke(pontuacaoTotal); //Atualiza UI
        }
        else
        {
            int pontuacaoSomar;
            pontuacaoSomar = pontos + (10 * rodadasComSucesso);
            if(pontuacaoSomar >= MAX_PONTOS_INIMIGOS)
            {
                pontuacaoTotal += MAX_PONTOS_INIMIGOS;
                Acoes.UIResolverPontuacao?.Invoke(pontuacaoTotal); //Atualiza UI
            }
            else
            {
                pontuacaoTotal += pontuacaoSomar;
                Acoes.UIResolverPontuacao?.Invoke(pontuacaoTotal); //Atualiza UI
            }
        }

        Debug.Log($"Inimigo Morto: {pontos} / Pontuação Total: {pontuacaoTotal}");
    }

    void ColetouHumano(int pontos)
    {
        pontosHumano = pontos;
        if (humanosColetados < MAX_HUMANOS)
        {
            humanosColetados++;
            Acoes.UIColetouHumano?.Invoke(humanosColetados); //Atualiza UI
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
        Acoes.UIVidaJogador?.Invoke(vidasJogador);          //Atualiza UI de vidas

        if (oxigenioSubmarino < OXIGENIO_MAXIMO)            //Oxigênio não está cheio
        {
            //Iniciar preenchemento do oxigênio
            Acoes.UIOxigenio?.Invoke(oxigenioSubmarino);    //Atualiza UI do oxigênio
            jogadorPodeMover = false;                       //impede o jogador de se mover enquanto o oxigênio estiver sendo preenchido
            Acoes.MoverJogador?.Invoke(jogadorPodeMover);
        }
        else
        {
            jogadorPodeMover = true;                        //permite o jogador se mover normalmente
            Acoes.MoverJogador?.Invoke(jogadorPodeMover);
        }
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

                GameOver();
                break;
        }
    }

    void ResolverHumanos() //PONTUAÇÃO E DIFICULDADE
    {
        if (humanosColetados == MAX_HUMANOS) // REGRA: rodada de sucesso, dificuldade e pontos aumentam;
        {
            StartCoroutine(RodadaComSucesso());
        }
        else if (humanosColetados == 0 && !superficiePorMorte) //REGRA: Se o jogador subir a superfície sem coletar nenhum humano, perde 1 vida
        {
            vidasJogador--;
            Acoes.UIVidaJogador?.Invoke(vidasJogador); //Atualiza UI de vidas
            Debug.Log($"O jogador subiu a superfície sem coletar humanos! Vidas restantes: {vidasJogador}");
            if (vidasJogador <= 0)
            {
                MudarEstadoJogo(EstadoJogo.GameOver);
            }
        }
        else // REGRA: perde um humano por cada vez que sobe a superfície sem estar cheio
        {
            if (humanosColetados > 0)
            {
                humanosColetados--;
                Acoes.UIColetouHumano?.Invoke(humanosColetados); //Atualiza UI
            }
        }
    }

    IEnumerator RodadaComSucesso() //Superfície com 6 humanos coletados
    {

        for (int i = 0; i < humanosColetados; i++)
        {
            int pontuacaoSomar;
            pontuacaoSomar = pontosHumano * (rodadasComSucesso + 1); //HUMANO MULTIPLICA PORQUE VALOR DE INCREMENTO == PONTOS
            if(pontuacaoSomar >= MAX_PONTOS_HUMANOS)
            {
                pontuacaoTotal += MAX_PONTOS_HUMANOS;
                Acoes.UIResolverPontuacao?.Invoke(pontuacaoTotal); //Atualiza UI
                Acoes.UIHumanos?.Invoke((humanosColetados - 1) - i);
            }
            else
            {
                pontuacaoTotal += pontuacaoSomar;
                Acoes.UIResolverPontuacao?.Invoke(pontuacaoTotal); //Atualiza UI
                Acoes.UIHumanos?.Invoke((humanosColetados - 1) - i);
            }
            Debug.Log($"PONTUAÇÃO HUMANOS = {pontuacaoSomar}");
            yield return new WaitForSeconds(0.5f); // Pequeno delay para garantir que a pontuação seja atualizada antes de atualizar os ícones
        }

        humanosColetados = 0; //reseta humanos coletados para a próxima rodada
        jogadorCheio = false;
        rodadasComSucesso++; //aumenta pontos e velocidade dos inimigos (DIFICULDADE)
    }

    void OxigenioSubmarino()
    {
        //Lógica para preencher o oxigênio do submarino quando o jogador estiver na superfície
        //Pode ser um aumento gradual do oxigênio ao longo do tempo ou um preenchimento instantâneo
        //Quando o oxigênio atingir o máximo, permitir que o jogador se mova normalmente
        if (estadoJogo == EstadoJogo.Superficie && oxigenioSubmarino < OXIGENIO_MAXIMO)
        {
            //Exemplo de preenchimento gradual
            oxigenioSubmarino += 35f * Time.deltaTime; //Aumenta o oxigênio
            Acoes.UIOxigenio?.Invoke(oxigenioSubmarino); //Atualiza UI do oxigênio

            if (oxigenioSubmarino >= OXIGENIO_MAXIMO)
            {
                oxigenioSubmarino = OXIGENIO_MAXIMO;
                jogadorPodeMover = true; //Permite o jogador se mover normalmente
                Acoes.MoverJogador?.Invoke(jogadorPodeMover); //'grita' que o jogador pode se mover
                Debug.Log("Oxigênio cheio! O jogador pode se mover normalmente.");
            }
        }
        else if(estadoJogo == EstadoJogo.Submerso)
        {
            oxigenioSubmarino -= 3f * Time.deltaTime; //Diminui oxigênio
            Acoes.UIOxigenio?.Invoke(oxigenioSubmarino); //Atualiza UI do oxigênio

            if (oxigenioSubmarino <= 0)
            {
                oxigenioSubmarino = 0;
                JogadorMorto(0); //O jogador morre por falta de oxigênio, mas não perde pontos
                Debug.Log("Falta de oxigênio! O jogador morreu.");
            }
        }
    }

    void GanharVidaExtra()
    {
        if (pontuacaoTotal >= pontuacaoVidaExtra)
        {
            vidasJogador++;
            Acoes.UIVidaJogador?.Invoke(vidasJogador); //Atualiza UI de vidas
            pontuacaoVidaExtra += PONTOS_VIDA_EXTRA;
            Debug.LogWarning($"VIDA EXTRA. Vidas Atuais: {vidasJogador}");
        }
    }
    
    void GameOver()
    {
        Debug.LogError("Game Over! O jogador perdeu todas as vidas.");
        Time.timeScale = 0;
        oxigenioSubmarino = 0;
    }
    public void OnReiniciar()
    {
        if (inputActions.Player.Reiniciar.WasPressedThisFrame())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //Recarrega a cena atual
        }
    }
}