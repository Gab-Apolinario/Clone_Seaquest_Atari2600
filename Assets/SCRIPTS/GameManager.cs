using UnityEngine;

public class GameManager : MonoBehaviour
{
    void OnEnable()
    {
        Acoes.JogadorMorto += JogadorMorto;
        Acoes.InimigoMorto += InimigoMorto;
    }

    void OnDisable()
    {
        Acoes.JogadorMorto -= JogadorMorto;
        Acoes.InimigoMorto -= InimigoMorto;
    }

    void JogadorMorto(int pontos)
    {
        Debug.LogError($"GameManager: O jogador morreu! Pontos ganhos: {pontos}");
    }

    void InimigoMorto(int pontos)
    {
        Debug.LogError($"GameManager: O inimigo morreu! Pontos ganhos: {pontos}");
    }
}