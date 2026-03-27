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
    [SerializeField] private bool esperandoSpawn;
    [SerializeField] private bool spawnAtivo;
    [SerializeField] private int probabilidadeSpawn;
    [SerializeField] private float spawnTick;

    [Header("Sorteio PreFab")]
    [SerializeField] int probabilidadePeixe;
    [SerializeField] int probabilidadeSubmarino;
    [SerializeField] int probabilidadeHumano;
    private int probabilidadeTotal;

    void Start()
    {
        esperandoSpawn = true;
        probabilidadeTotal = probabilidadePeixe + probabilidadeSubmarino + probabilidadeHumano;
    }

    void OnEnable()
    {
        Acoes.AtivarSpawn += SpawnAtivo;
    }

    void OnDisable()
    {
        Acoes.AtivarSpawn -= SpawnAtivo;
    }

    void Update()
    {
        if (spawnAtivo && esperandoSpawn)
        {
            StartCoroutine(TickSpawn());
        }
    }

    private IEnumerator TickSpawn()
    {
        esperandoSpawn = false;
        Spawn();
        //yield return new WaitForSeconds(Mathf.Max(spawnTick/GameManager.multiplicadorDificuldade, 0.2f));
        yield return new WaitForSeconds(spawnTick);
        esperandoSpawn = true;
    }
    
    void Spawn()
    {
        int sorteioSpawn = Random.Range(0, 100);

        if (sorteioSpawn < probabilidadeSpawn)
        {
            //lógica para sortear o inimigo com base nas probabilidades
            int resultadoSorteioPreFab = Random.Range(0, probabilidadeTotal);
            bool canSpawnSubmarino = GameManager.multiplicadorDificuldade >= 1.5f; //REGRA: submarinos começam a aparecer a partir da segunda rodada com sucesso

            if(resultadoSorteioPreFab < probabilidadePeixe)
            {
             SpawnConfigurar(peixePreFab);
                //Debug.Log("Spawn Peixe");
            }
            else if(resultadoSorteioPreFab < probabilidadePeixe + probabilidadeSubmarino && canSpawnSubmarino)
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

    }

    void SpawnAtivo (bool valor)
    {
        spawnAtivo = valor;
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
