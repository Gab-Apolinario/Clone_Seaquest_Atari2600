using System;
using UnityEngine;

public class Humano : BaseInimigo
{
    protected override void Start()
    {
        base.Start(); // Chama o Start da classe base (BaseInimigo)
        pontos = 50;
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (GameManager.jogadorCheio == false)
            {
                Acoes.ColetouHumano?.Invoke(pontos);
                Destroy(gameObject);
            }
            else
            {
                return; // Jogador cheio, não pode coletar mais humanos
            }
        }

        if (col.gameObject.CompareTag("TiroJogador"))
        {
            return; //Humano não é afetado por tiros, apenas coletado pelo jogador
                    //TALVEZ IMPLEMENTAR MECANICA QUE TIRA PONTO DO JOGADOR DE ATIRAR NUM HUMANO
        }
    }
}
