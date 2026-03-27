using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class Player : MonoBehaviour
{
    private InputSystem_Actions inputActions;

    [Header("Referencias de Componentes")]
    [SerializeField] private Rigidbody2D rb;
    

    [Header("Configurações do Jogador")]
    [SerializeField] private float velocidadeJogador;
    [SerializeField] private SpriteRenderer spriteJogador;
    [SerializeField] private float limiteTelaEsquerda;
    [SerializeField] private float limiteTelaDireita;
    [SerializeField] private float limiteTelaCima;
    [SerializeField] private float limiteTelaBaixo;
    [SerializeField] private bool podeMover = true;
    [SerializeField] private Vector2 localTravado;

      [Header("Atirar")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject tiroPrefab;
    [SerializeField] private float delayTiro;
    [SerializeField] private bool superficie;
    private bool podeAtirar = true;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Enable();
        Acoes.MoverJogador += PodeMover;
        Acoes.Superficie += Superficie;
    }

    void OnDisable()
    {
        inputActions.Disable();
        Acoes.MoverJogador -= PodeMover;
        Acoes.Superficie -= Superficie;
    }

    private void Update()
    {
        if (!superficie)
        {
            OnAtirar();
        }
    }
    private void FixedUpdate()
    {
        if (!podeMover)
        {
            rb.MovePosition(localTravado);
        }
        else
        {
            OnMover();
        }
    }

    #region CONTROLE_JOGADOR
    public void OnMover()
    {

        Debug.Log("MOVENDO");
        //Lê o valor do input(Vector2)
        Vector2 direction = inputActions.Player.Mover.ReadValue<Vector2>();

        //Move usando a direção lida anteriormente
        rb.position = new Vector2(Mathf.Clamp(rb.position.x, limiteTelaEsquerda, limiteTelaDireita), 
                                  Mathf.Clamp(rb.position.y, limiteTelaBaixo, limiteTelaCima));

        rb.MovePosition(rb.position + direction * velocidadeJogador * Time.fixedDeltaTime);

        // Flipar Sprite
        if (direction.x < 0) //indo para a esquerda
        {
            spriteJogador.flipX = false; // Flipar para a esquerda
        }
        else if (direction.x > 0) //indo para a direita
        {
            spriteJogador.flipX = true; // Flipar para a direita
        }

    }

    public void OnAtirar()
    {
        if (inputActions.Player.Atirar.IsPressed())
        {
            if (podeAtirar)
            {
                StartCoroutine(Atirar());
            }
        }
    }

    private IEnumerator Atirar()
    {
        podeAtirar = false; //inicia o delay do tiro

        GameObject tiroJogador = Instantiate(tiroPrefab, spawnPoint.position, Quaternion.identity);
        
        if (spriteJogador.flipX == false) //Se o jogador estiver virado para a esquerda
        {
            tiroJogador.GetComponent<TiroJogador>().AtirarEsquerda(); //Faz o tiro ir para a esquerda
        }

        Debug.Log("Atirando!");

        yield return new WaitForSeconds(delayTiro); // Tempo de recarga entre os tiros
        podeAtirar = true;
    }

    void PodeMover(bool valor, Vector2 travado)
    {
        podeMover = valor;
        if (!podeMover)
        {
            localTravado = travado;
        }
    }
        void Superficie(bool valor)
    {
        superficie = valor;
    }

    #endregion

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("TiroSubmarino"))
        {
            Acoes.JogadorMorto?.Invoke(0); // TRANSMITIR
            Destroy(col.gameObject); //destroi o tiro do submarino
            Debug.LogWarning("COLIDIU COM TIRO DO SUBMARINO");
        }
    }
}