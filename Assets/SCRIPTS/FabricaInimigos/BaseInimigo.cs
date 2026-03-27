using UnityEngine;
using System.Collections;

public class BaseInimigo : MonoBehaviour
{
    [Header("Variáveis Comuns para Herdar")]
    [SerializeField] protected float velocidade;
    [SerializeField] protected float multiplicadorVelocidade;
    [SerializeField] protected int pontos;
    [SerializeField] private int tempoVida;
    [SerializeField] protected bool irDireita;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    protected virtual void Start()
    {
        Destroy(gameObject, tempoVida);
        Seguranças();
        multiplicadorVelocidade = GameManager.multiplicadorDificuldade; //REGRA: a cada rodada de sucesso, os inimigos ficam mais rápidos
        velocidade *= multiplicadorVelocidade;
    }

    protected virtual void Update()
    {
        Mover();
    }

    #region MOVIMENTO_INIMIGO
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
        public void IrDireita()
    {
        irDireita = true;
        spriteRenderer.flipX = true; //se o sprite original estiver virado para a direita
    }
    #endregion

    #region COLISÕES_INIMIGO
    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            //Matar o jogador
            Acoes.JogadorMorto?.Invoke(pontos);                 // TRANSMITIR - '?.Invoke()' = if (ouvinte != null) { Acao.Invoke(); }
            Destroy(gameObject);                                //destroi o inimigo quando colidir com o jogador
            Debug.LogWarning("COLIDIU COM PLAYER");
            //Somar pontuação na UI
        }
        else if (col.gameObject.CompareTag("TiroJogador"))
        {
            Acoes.InimigoMorto?.Invoke(pontos);                 //'?.Invoke()' = if (ouvinte != null) { Acao.Invoke(); }
            Destroy(gameObject);
            Destroy(col.gameObject);                            //destroi o tiro do jogador
            Debug.LogWarning("ATINGIDO PELO TIRO.");
            //Somar pontuação na UI
        }
    }

    #endregion

    protected void Seguranças()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}
