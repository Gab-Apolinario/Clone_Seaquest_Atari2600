using UnityEngine;
using System;
public static class Acoes
{
    public static Action<int> JogadorMorto;
    public static Action<int> InimigoMorto;
    public static Action<int> ColetouHumano;
    public static Action<bool> MoverJogador;

    public static Action<int> UIResolverPontuacao;
    public static Action<int> UIHumanos;
    public static Action<int> UIColetouHumano;
    public static Action<float> UIOxigenio;

    public static Action<int> UIVidaJogador;
}