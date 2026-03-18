using UnityEngine;

public class BaseInimigo : MonoBehaviour
{
    [Header("Variáveis Comuns para Herdar")]
    [SerializeField] protected int velocidade = 5;
    [SerializeField] protected int pontuacao = 20;
    [SerializeField] private int tempoVida = 5;
    [SerializeField] protected bool irDireita;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    protected virtual void Start()
    {
        Destroy(gameObject, tempoVida);
    }

    protected virtual void Update()
    {
        Mover();
    }

    protected virtual void Mover()
    {
        if (irDireita)
        {
            transform.Translate(Vector2.right * velocidade * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.left * velocidade * Time.deltaTime);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Matar o jogador
            Debug.LogWarning("COLIDIU COM PLAYER");
            Destroy(gameObject);
            //Somar pontuação na UI
        }
        else if (collision.gameObject.CompareTag("TiroJogador"))
        {
            Destroy(gameObject);
            Debug.LogWarning("ATINGIDO PELO TIRO.");
            //Somar pontuação na UI
        }
    }

    public void IrDireita()
    {
        irDireita = true;
        spriteRenderer.flipX = true; //se o sprite original estiver virado para a direita
    }
}
