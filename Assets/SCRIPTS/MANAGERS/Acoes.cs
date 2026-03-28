using UnityEngine;
using System;
public static class Acoes
{
    public static Action<int> JogadorMorto;
    public static Action<int> InimigoMorto;
    public static Action<int> ColetouHumano;
    public static Action<bool, Vector2> MoverJogador;
    public static Action<bool> Superficie;

    //UI
    public static Action<int> UIResolverPontuacao;
    public static Action<int> UIHumanos;
    public static Action<int> UIColetouHumano;
    public static Action<float> UIOxigenio;

    public static Action<int> UIVidaJogador;

    public static Action<bool> PiscarOxigenio;
    public static Action<bool> PiscarHumanos;

    //SPAWN INIMIGOS
    public static Action<bool> AtivarSpawn;
    public static Action<bool> SpawnarSubPatrulheiro;

    //SFX
    public static Action OxigenioEnchendo;
    public static Action OxigenioDescendo;
    public static Action PeixeMorto;
    public static Action SubmarinoMorto;
    public static Action TiroJogador;
}