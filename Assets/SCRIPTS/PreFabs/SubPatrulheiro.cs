using System.Threading;
using UnityEngine;

public class SubPatrulheiro : MonoBehaviour
{
    [SerializeField] private float velocidade;
    [SerializeField] private float velocidadeOriginal;
    [SerializeField] private bool jogadorSuperficie;
    [SerializeField] private bool velocidadeAumentada;
    private Vector2 foraTela = new Vector2(-8, 0);

    void OnEnable()
    {
        Acoes.Superficie += Superficie;
        velocidadeOriginal = velocidade;
    }

    void OnDisable()
    {
        Acoes.Superficie -= Superficie;
    }

    void Update()
    {
        transform.Translate(Vector2.left * velocidade * Time.deltaTime);

        if (transform.position.x < foraTela.x)
        {
            Acoes.SpawnarSubPatrulheiro?.Invoke(true);
            Destroy(gameObject);
        }
    }

    void Superficie(bool valor)
    {
        jogadorSuperficie = valor;
        
        if (jogadorSuperficie && !velocidadeAumentada)
        {
            velocidade *= 1.2f;
            velocidadeAumentada = true;
        }
        else if (!jogadorSuperficie && velocidadeAumentada)
        {
            velocidade = velocidadeOriginal;
            velocidadeAumentada = false;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Acoes.JogadorMorto?.Invoke(0);
            Destroy(gameObject);
            Acoes.SpawnarSubPatrulheiro?.Invoke(true); //Informa o spawner para spawnar outro subpatrulheiro
        }
    }

}
