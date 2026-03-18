using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FabricaInimigos : MonoBehaviour
{
    public enum Direcao
    {
        Esquerda,
        Direita
    }
    [SerializeField] private Direcao direcaoSpawn;

    [Header("Configuração Spawn")]
    [SerializeField] private GameObject peixePreFab;
    [SerializeField] private GameObject submarinoPreFab;
    [SerializeField] private GameObject humanoPreFab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool canSpawn = true;
    [SerializeField] private int spawnTick;

    [Header("Sorteio PreFab")]
    [SerializeField] int probabilidadePeixe;
    [SerializeField] int probabilidadeSubmarino;
    [SerializeField] int probabilidadeHumano;
    private int probabilidadeTotal;

    void Start()
    {
        probabilidadeTotal = probabilidadePeixe + probabilidadeSubmarino + probabilidadeHumano;
    }

    void Update()
    {
        if (canSpawn)
        {
            StartCoroutine(TickSpawn());
        }
    }

    private IEnumerator TickSpawn()
    {
        canSpawn = false;
        Sorteio();
        yield return new WaitForSeconds(spawnTick);
        canSpawn = true;
    }
    
    void Sorteio()
    {
        //lógica para sortear o inimigo com base nas probabilidades
        int resultadoSorteio = Random.Range(0, probabilidadeTotal);

        if(resultadoSorteio < probabilidadePeixe)
        {
            SpawnConfigurar(peixePreFab);
            //Debug.Log("Spawn Peixe");
        }
        else if(resultadoSorteio < probabilidadePeixe + probabilidadeSubmarino)
        {
            SpawnConfigurar(submarinoPreFab);
            //Debug.Log("Spawn Submarino");
        }
        else
        {
            SpawnConfigurar(humanoPreFab);
            //Debug.Log("Spawn Humano");
        }
    }

    private void SpawnConfigurar(GameObject inimigoPrefab)
    {
        GameObject inimigoPreFab = Instantiate(inimigoPrefab, spawnPoint.position, Quaternion.identity);
        if(direcaoSpawn == Direcao.Direita)
        {
            inimigoPreFab.GetComponent<BaseInimigo>().IrDireita();
        }
    }
}
