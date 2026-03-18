using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum EstadoJogo
    {
        Superficie,
        Submerso,
        GameOver
    }
    [Header("Estados do Jogo")]
    public EstadoJogo estadoJogo;
    [SerializeField] private bool superficie;
    public static event Action<EstadoJogo> OnEstadoJogoMudou; //Evento para notificar mudanças de estado do jogo

    [Header("Gerenciamento de Pontuação")]
    [SerializeField] private int pontuacaoTotal;
    [SerializeField] private int vidasJogador = 3;                              //Fora a que começa, o jogador tem 3 vidas extras
    [SerializeField] private const int PONTOS_VIDA_EXTRA = 10000;               //REGRA: Ganha 1 vida a cada 10.000 pontos
    [SerializeField] private int humanosColetados;
    [SerializeField] private const int MAX_HUMANOS = 6;                         //REGRA
    public static bool jogadorCheio;
    [SerializeField] private int pontosPeixe;
    [SerializeField] private int pontosSubmarino;
    [SerializeField] private int pontosHumano;
    [SerializeField] private const int MAX_PONTOS_HUMANOS = 1000;               //REGRA
    [SerializeField] private const int MAX_PONTOS_INIMIGOS = 90;                //REGRA
    [SerializeField] private int rodadasComSucesso = 1;                         //cada vez que o jogador sobe a superfície E tem 6 humanos, os pontos aumentam
    
    #region INSCRIÇÃO_AÇÕES
    void OnEnable()
    {
        Acoes.JogadorMorto += JogadorMorto; //OUVIR - BaseInimigo da o sinal
        Acoes.InimigoMorto += InimigoMorto;
        Acoes.ColetouHumano += ColetouHumano;
        Acoes.Superficie += Superficie;
    }

    void OnDisable()
    {
        Acoes.JogadorMorto -= JogadorMorto;
        Acoes.InimigoMorto -= InimigoMorto;
        Acoes.ColetouHumano -= ColetouHumano;
        Acoes.Superficie -= Superficie;
    }
    #endregion

    void JogadorMorto(int pontos)
    {
        pontosSubmarino = 20;
        pontosPeixe = 20;
        pontuacaoTotal += pontos;
        vidasJogador--;
        Debug.LogWarning($"GameManager: O jogador morreu! Pontuação Total: {pontuacaoTotal} / Vidas: {vidasJogador}");

        //reset loop do jogo, ou seja, voltar para o estado Superficie, resetar inimigos, etc.
        if (vidasJogador <= 0)
        {
            GameOver();
        }
    }

    void InimigoMorto(int pontos)
    {
        pontosSubmarino = 20;
        pontosPeixe = 20;
        pontuacaoTotal += pontos;
        Debug.LogWarning($"GameManager: O inimigo morreu! Pontos ganhos: {pontos} / Pontuação Total: {pontuacaoTotal}");
    }

    void ColetouHumano(int pontos)
    {
        pontosHumano = pontos;
        if (humanosColetados < MAX_HUMANOS)
        {
            //pontuacaoTotal += pontos; //Pontuação somente na superfície
            humanosColetados++;
            Debug.Log($"COLETADO! Humanos Coletados: {humanosColetados}");

            if (humanosColetados == MAX_HUMANOS)
            {
                jogadorCheio = true;
                Debug.Log("JOGADOR CHEIO! HUMANO NÃO COLETADO.");
            }
        }else
        {
            //Ativar Beep
            //Sprites inventário piscam
            //Humano não some!!!
        }
    }

    void Superficie()
    {
        superficie = true;
        if (superficie)
        {
            Debug.Log("SUPERFÍCIE");
            estadoJogo = EstadoJogo.Superficie;

            if (humanosColetados == MAX_HUMANOS) // REGRA: rodada de sucesso, dificuldade e pontos aumentam;
            {
                for (int i = 1; i <= MAX_HUMANOS; i++) //aumenta um por um visualmente
                {
                    pontuacaoTotal += pontosHumano * rodadasComSucesso;
                    humanosColetados--;
                    //UI Update aqui
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
                    GameOver();
                }
                
            }
            else // REGRA: perde um humano por cada vez que sobe a superfície sem estar cheio
            {
                humanosColetados --;
                //UI Upadate aqui
            }

            estadoJogo = EstadoJogo.Submerso;
        }

        superficie = false;
    }

    void GameOver()
    {
        estadoJogo = EstadoJogo.GameOver;
        OnEstadoJogoMudou?.Invoke(estadoJogo); //Notificar ouvintes sobre a mudança de estado do jogo
        Debug.LogError("Game Over! O jogador perdeu todas as vidas.");
    }
}