using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.Rendering.VirtualTexturing;


public class UIManager : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private TextMeshProUGUI pontuacaoText;
    [SerializeField] private Image[] vidasJogador;
    [SerializeField] private Image[] humanosColetados;
    [SerializeField] private Image barraOxigenio;

    void OnEnable()
    {
        Acoes.UIResolverPontuacao += ResolverPontuacao;
        Acoes.UIHumanos += UIHumanos;
        Acoes.UIColetouHumano += ColetouHumano;
        Acoes.UIOxigenio += Oxigenio;
        Acoes.UIVidaJogador += UIVidaJogador;
    }

    void OnDisable()
    {
        Acoes.UIResolverPontuacao -= ResolverPontuacao;
        Acoes.UIHumanos -= UIHumanos;
        Acoes.UIColetouHumano -= ColetouHumano;
        Acoes.UIOxigenio -= Oxigenio;
        Acoes.UIVidaJogador -= UIVidaJogador;
    }   


    //Atualiza pontuação na UI
    void ResolverPontuacao(int pontos)
    {
        pontuacaoText.text = pontos.ToString();
    }

    void UIHumanos(int index)
    {
        if (index < humanosColetados.Length)
        {
            humanosColetados[index].gameObject.SetActive(false);
        }
    }

    //Ativa as imagens dos humanos coletados com base na pontuação
    void ColetouHumano(int pontosHumanos)
    {
        for (int i = 0; i < humanosColetados.Length; i++)
        {
            if (i < pontosHumanos)
            {
                humanosColetados[i].gameObject.SetActive(true);
            }
            else
            {
                humanosColetados[i].gameObject.SetActive(false);
            }
        }
    }

    void UIVidaJogador(int vidasRestantes)
    {
        for (int i = 0; i < vidasJogador.Length; i++)
        {
            if (i < vidasRestantes)
            {
                vidasJogador[i].gameObject.SetActive(true);
            }
            else
            {
                vidasJogador[i].gameObject.SetActive(false);
            }
        }
    }

    void Oxigenio(float oxigenioSubmarino)
    {
        barraOxigenio.fillAmount = oxigenioSubmarino / 100f; // Supondo que o oxigênio seja representado como um valor entre 0 e 100
    }
}
