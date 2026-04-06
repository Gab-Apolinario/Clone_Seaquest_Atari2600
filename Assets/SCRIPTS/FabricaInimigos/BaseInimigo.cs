using UnityEngine;

public class BaseInimigo : MonoBehaviour
{
    [Header("Variáveis Comuns para Herdar")]
    [SerializeField] protected float velocidade;
    [SerializeField] protected float multiplicadorVelocidade;
    [SerializeField] protected int pontos;
    [SerializeField] protected bool irDireita;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    private Vector2 foraEsquerda = new Vector2(-8, 0);
    private Vector2 foraDireita = new Vector2(8, 0);

    protected virtual void Start()
    {
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

            if (transform.position.x >= foraDireita.x)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            transform.Translate(Vector2.left * velocidade * Time.deltaTime);
            
            if (transform.position.x <= foraEsquerda.x)
            {
                Destroy(gameObject);
            }
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
        }
        else if (col.gameObject.CompareTag("TiroJogador"))
        {
            Acoes.InimigoMorto?.Invoke(pontos);                 //'?.Invoke()' = if (ouvinte != null) { Acao.Invoke(); }
            Destroy(gameObject);
            Destroy(col.gameObject);                            //destroi o tiro do jogador
            Debug.LogWarning("ATINGIDO PELO TIRO.");
            
            if (gameObject.CompareTag("Submarino"))
            {
                Acoes.SubmarinoMorto?.Invoke();
            }
            else if (gameObject.CompareTag("Peixe"))
            {
                Acoes.PeixeMorto?.Invoke();
            }
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
