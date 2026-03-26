using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private TextMeshProUGUI pontuacaoText;
    [SerializeField] private Image[] vidasJogador;
    [SerializeField] private Image[] humanosColetados;
    [SerializeField] private Image barraOxigenio;
    [SerializeField] private Coroutine piscarOxigenioCoroutine;
    [SerializeField] private Coroutine piscarHumanosCoroutine;

    void OnEnable()
    {
        Acoes.UIResolverPontuacao += ResolverPontuacao;
        Acoes.UIHumanos += UIHumanos;
        Acoes.UIColetouHumano += ColetouHumano;
        Acoes.UIOxigenio += Oxigenio;
        Acoes.UIVidaJogador += UIVidaJogador;
        Acoes.PiscarOxigenio += PiscarOxigenio;
        Acoes.PiscarHumanos += PiscarHumanos;
    }

    void OnDisable()
    {
        Acoes.UIResolverPontuacao -= ResolverPontuacao;
        Acoes.UIHumanos -= UIHumanos;
        Acoes.UIColetouHumano -= ColetouHumano;
        Acoes.UIOxigenio -= Oxigenio;
        Acoes.UIVidaJogador -= UIVidaJogador;
        Acoes.PiscarOxigenio -= PiscarOxigenio;
        Acoes.PiscarHumanos -= PiscarHumanos;
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
        barraOxigenio.fillAmount = oxigenioSubmarino / 100f;
    }

    void PiscarOxigenio(bool piscarOxigenio)
    {
        if (piscarOxigenio)
        {
            piscarOxigenioCoroutine = StartCoroutine(PiscarOxigenioCoroutine());
        }
        else
        {
            barraOxigenio.gameObject.SetActive(true);
            StopCoroutine(piscarOxigenioCoroutine);
        }
    }

    IEnumerator PiscarOxigenioCoroutine()
    {
        while (true)
        {
            barraOxigenio.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.4f);
            barraOxigenio.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.4f);
        }
    }

    void PiscarHumanos(bool piscarHumanos)
    {
        if (piscarHumanos)
        {
            piscarHumanosCoroutine = StartCoroutine(PiscarHumanosCoroutine());
        }
        else
        {
            StopCoroutine(piscarHumanosCoroutine);
        }
    }

    IEnumerator PiscarHumanosCoroutine()
    {
        while (true)
        {
            for (int i = 0; i < humanosColetados.Length; i++)
            {
                humanosColetados[i].gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(0.4f);

            for (int i = 0; i < humanosColetados.Length; i++)
            {
                humanosColetados[i].gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(0.4f);
        }
    }
}
