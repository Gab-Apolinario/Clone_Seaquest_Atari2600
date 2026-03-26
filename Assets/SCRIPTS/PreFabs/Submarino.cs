using UnityEngine;
using System.Collections;

public class Submarino : BaseInimigo
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject tiroSubmarinoPrefab;
    [SerializeField] private bool podeAtirar;
    [SerializeField] private float delayTiro;
    
    protected override void Start()
    {
        base.Start(); // Chama o Start da classe base (BaseInimigo) para 
                      // garantir que a lógica de movimento e destruição funcione corretamente.
        pontos = 20; // Define os pontos específicos para o Peixe
        podeAtirar = true;
    }

    protected override void Update()
    {
        base.Update(); // Chama o Update da classe base para manter a lógica de movimento e destruição.
        if (podeAtirar)
        {
            StartCoroutine(Atirar());
        }
    }

    private IEnumerator Atirar(){

        podeAtirar = false;

        //Sorteia se o submarino vai atirar, evita tiro continuo
        int  chanceAtirar = Random.Range(0, 10);
        if(chanceAtirar <= 3) //30% de chance de atirar
        {
            //Debug.Log("Submarino não atira desta vez.");
            podeAtirar = true;
            yield break; //Sai da coroutine sem atirar
        }
        else
        {
        GameObject tiroSubmarino = Instantiate(tiroSubmarinoPrefab, spawnPoint.position, Quaternion.identity);
        
        if (spriteRenderer.flipX == false) //Se o jogador estiver virado para a esquerda
        {
            tiroSubmarino.GetComponent<TiroSubmarino>().AtirarEsquerda(); //Faz o tiro ir para a esquerda
        }

        //Debug.Log("Submarino Atirou!");

        yield return new WaitForSeconds(delayTiro); // Tempo de recarga entre os tiros
        
        podeAtirar = true;
        }
    }

}
