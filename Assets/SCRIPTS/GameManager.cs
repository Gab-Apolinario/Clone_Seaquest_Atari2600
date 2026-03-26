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
    [SerializeField] private bool piscandoOxigenio;

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
    [SerializeField] private bool contabilizandoPontos;
    #endregion

    #region INICIAÇÕES

    void Awake()
    {
        jogadorTransform.position = new Vector2(0, 3);      //Posição inicial do jogador na superfície
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
        DestruirPrefabs(); //Limpa a tela
        jogadorTransform.position = new Vector2(0, 3);      //Posição inicial do jogador na superfície

        superficiePorMorte = true;
        MudarEstadoJogo(EstadoJogo.Superficie);
        pontosSubmarino = 20;
        pontosPeixe = 20;
        oxigenioSubmarino = 0;

        if (humanosColetados < MAX_HUMANOS)
        {
            jogadorCheio = false;
            Acoes.PiscarHumanos?.Invoke(jogadorCheio); //PARA DE PISCAR HUMANOS
        }
        
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
                Acoes.PiscarHumanos?.Invoke(jogadorCheio);
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
        Acoes.UIVidaJogador?.Invoke(vidasJogador);          //Atualiza UI de vidas

        if (oxigenioSubmarino < OXIGENIO_MAXIMO)            //Oxigênio não está cheio
        {
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

                //PARAR DE PISCAR OXIGENIO
                if (piscandoOxigenio) //Para de piscar assim que começa a encher
                {
                    piscandoOxigenio = false;
                    Acoes.PiscarOxigenio?.Invoke(piscandoOxigenio);
                }

                IniciarJogo();
                Debug.Log("SUPERFÍCIE");

                if (estadoAnterior == EstadoJogo.Submerso)
                {
                    ResolverHumanos();
                }
                break;
            //ESTADO_SUBMERSO
            case EstadoJogo.Submerso:

                Debug.Log("SUBMERSO");
                superficiePorMorte = false;
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
        if (humanosColetados == MAX_HUMANOS && !superficiePorMorte) // REGRA: rodada de sucesso, dificuldade e pontos aumentam;
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
        contabilizandoPontos = true;
        Acoes.PiscarHumanos?.Invoke(false);

        DestruirPrefabs(); //Limpa a tela

        jogadorPodeMover = false;
        Acoes.MoverJogador?.Invoke(jogadorPodeMover); //impede o jogador de se mover enquanto a pontuação é atualizada

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
            yield return new WaitForSeconds(0.5f); //Delay para garantir que a pontuação seja atualizada antes de atualizar os ícones
        }

        humanosColetados = 0; //reseta humanos coletados para a próxima rodada
        jogadorCheio = false;
        //Acoes.PiscarHumanos?.Invoke(jogadorCheio);
        rodadasComSucesso++; //aumenta pontos e velocidade dos inimigos (DIFICULDADE)

        contabilizandoPontos = false;
        jogadorPodeMover = true;
        Acoes.MoverJogador?.Invoke(jogadorPodeMover);
    }

    void OxigenioSubmarino()         //Lógica para preencher o oxigênio do submarino quando o jogador estiver na superfície
    {
        if (estadoJogo == EstadoJogo.Superficie && oxigenioSubmarino < OXIGENIO_MAXIMO)
        {
            //Exemplo de preenchimento gradual
            oxigenioSubmarino += 50f * Time.deltaTime; //Aumenta o oxigênio
            Acoes.UIOxigenio?.Invoke(oxigenioSubmarino); //Atualiza UI do oxigênio

            if (oxigenioSubmarino >= OXIGENIO_MAXIMO && !contabilizandoPontos)
            {   
                oxigenioSubmarino = OXIGENIO_MAXIMO; //Se passar, volta pro máximo
                jogadorPodeMover = true; //Permite o jogador se mover normalmente
                Acoes.MoverJogador?.Invoke(jogadorPodeMover); //'grita' que o jogador pode se mover

                Debug.Log("Oxigênio cheio! O jogador pode se mover normalmente.");
            }
        }
        else if(estadoJogo == EstadoJogo.Submerso)
        {
            oxigenioSubmarino -= 5f * Time.deltaTime; //Diminui oxigênio
            Acoes.UIOxigenio?.Invoke(oxigenioSubmarino); //Atualiza UI do oxigênio

            if (oxigenioSubmarino <= 0)
            {
                oxigenioSubmarino = 0;
                JogadorMorto(0); //O jogador morre por falta de oxigênio, mas não perde pontos
                Debug.Log("Falta de oxigênio! O jogador morreu.");
            }
            else if(oxigenioSubmarino <= 35 && !piscandoOxigenio) //PISCAR OXIGENIO
            {
                piscandoOxigenio = true;
                Acoes.PiscarOxigenio?.Invoke(piscandoOxigenio);
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

    void DestruirPrefabs()
    {
        BaseInimigo[] inimigos = GameObject.FindObjectsByType<BaseInimigo>(FindObjectsSortMode.None);
        foreach (BaseInimigo inimigo in inimigos)
        {
            Destroy(inimigo.gameObject);
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