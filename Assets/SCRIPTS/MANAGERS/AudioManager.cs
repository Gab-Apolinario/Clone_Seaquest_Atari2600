using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum EfeitosSonoros
    {
        ColetouHumano,
        HumanoSalvo,
        JogadorMorto,
        OxigenioBaixo,
        OxigenioDescendo,
        OxigenioEnchendo,
        PeixeMorto,
        SubInimigoMorto,
        TiroJogador
    }

    [Header("Referências")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource audioSourceLoop;
    [SerializeField] private AudioClip[] efeitosSonoros;

    void OnEnable()
    {
        Acoes.ColetouHumano += SFXColetouHumano;
        Acoes.UIHumanos += SFXHumanoSalvo;
        Acoes.JogadorMorto += SFXJogadorMorto;
        Acoes.PiscarOxigenio += SFXOxigenioBaixo;
        Acoes.OxigenioEnchendo += SFXOxigenioEnchendo;
        Acoes.OxigenioDescendo += SFXOxigenioDescendo;
        Acoes.PeixeMorto += SFXPeixeMorto;
        Acoes.SubmarinoMorto += SFXSubmarinoMorto;
        Acoes.TiroJogador += SFXTiroJogador;
    }

    void OnDisable()
    {
        Acoes.ColetouHumano -= SFXColetouHumano;
        Acoes.UIHumanos -= SFXHumanoSalvo;
        Acoes.JogadorMorto -= SFXJogadorMorto;
        Acoes.PiscarOxigenio -= SFXOxigenioBaixo;
        Acoes.OxigenioEnchendo -= SFXOxigenioEnchendo;
        Acoes.OxigenioDescendo -= SFXOxigenioDescendo;
        Acoes.PeixeMorto -= SFXPeixeMorto;
        Acoes.SubmarinoMorto -= SFXSubmarinoMorto;
        Acoes.TiroJogador -= SFXTiroJogador;
    }
    void SFXColetouHumano(int pontos) //somente para receber o mesmo parâmetro do evento que já existe, não é necessário usar
    {
        TocarSFX(efeitosSonoros[(int)EfeitosSonoros.ColetouHumano]);
    }

    void SFXHumanoSalvo(int index)
    {
        TocarSFX(efeitosSonoros[(int)EfeitosSonoros.HumanoSalvo]);
    }

    void SFXJogadorMorto(int pontos)
    {
        TocarSFX(efeitosSonoros[(int)EfeitosSonoros.JogadorMorto]);
    }

    void SFXOxigenioBaixo(bool tocarSFX)
    {
        if (tocarSFX)
        {
            audioSourceLoop.clip = efeitosSonoros[(int)EfeitosSonoros.OxigenioBaixo];
            audioSourceLoop.loop = true;
            audioSourceLoop.Play();
        }
        else
        {
            audioSourceLoop.Stop();
        }
    }

    void SFXOxigenioEnchendo()
    {
        TocarSFX(efeitosSonoros[(int)EfeitosSonoros.OxigenioEnchendo]);
    }

    void SFXOxigenioDescendo()
    {
        TocarSFX(efeitosSonoros[(int)EfeitosSonoros.OxigenioDescendo]);
    }

    void SFXPeixeMorto()
    {
        TocarSFX(efeitosSonoros[(int)EfeitosSonoros.PeixeMorto]);
    }

    void SFXSubmarinoMorto()
    {
        TocarSFX(efeitosSonoros[(int)EfeitosSonoros.SubInimigoMorto]);
    }

    void SFXTiroJogador()
    {
        TocarSFX(efeitosSonoros[(int)EfeitosSonoros.TiroJogador]);
    }

    void TocarSFX(AudioClip sfx)
    {
        audioSource.PlayOneShot(sfx);
    }
}
