using UnityEngine;

public class SpawnerSubPatrulheiro : MonoBehaviour
{
    [SerializeField] private GameObject subPatrulheiroPreFab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float tickSpawn;
    [SerializeField] private float timer;
    [SerializeField] private bool spawnar;

    private bool primeiroSpawn;

    void OnEnable()
    {
        timer = tickSpawn;
        Acoes.SpawnarSubPatrulheiro += Spawnar;
    }

    void OnDisable()
    {
        Acoes.SpawnarSubPatrulheiro -= Spawnar;
    }

    void Update()
    {
        if(GameManager.multiplicadorDificuldade >= 1.6f && !primeiroSpawn)
        {
            Spawn();
            primeiroSpawn = true;
        }
        
        
        if (spawnar)
        {
            timer -= Time.deltaTime;

            if(timer <= 0)
            {
                Spawn();
                spawnar = false;
                timer = tickSpawn;
            }
        }
    }

        void Spawn()
    {
        Instantiate(subPatrulheiroPreFab, spawnPoint.position, Quaternion.identity);
        Debug.Log("Spawnou SubPatrulheiro");
    }

    void Spawnar(bool subMorto)
    {
        spawnar = subMorto;
    }
}
