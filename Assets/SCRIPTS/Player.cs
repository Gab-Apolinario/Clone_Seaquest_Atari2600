using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

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

      [Header("Atirar")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject tiroPrefab;
    [SerializeField] private float delayTiro;
    private bool podeAtirar = true;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Enable();
        Acoes.MoverJogador += PodeMover;
    }

    void OnDisable()
    {
        inputActions.Disable();
        Acoes.MoverJogador -= PodeMover;
    }

    private void Update()
    {
        OnAtirar();
        // Verificar a direção do movimento para flipar o sprite

    }
    private void FixedUpdate()
    {
        if (podeMover)
        {
            OnMover();
        }
    }

    #region CONTROLE_JOGADOR
    public void OnMover()
    {
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

        GameObject tiro = Instantiate(tiroPrefab, spawnPoint.position, Quaternion.identity);
        
        if (spriteJogador.flipX == false) //Se o jogador estiver virado para a esquerda
        {
            tiro.GetComponent<TiroJogador>().AtirarEsquerda(); //Faz o tiro ir para a esquerda
        }

        Debug.Log("Atirando!");

        yield return new WaitForSeconds(delayTiro); // Tempo de recarga entre os tiros
        podeAtirar = true;
    }

    void PodeMover(bool valor)
    {
        podeMover = valor;
    }

    #endregion
}