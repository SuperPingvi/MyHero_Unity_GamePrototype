using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefabToSpawn;

    public float startDelay = 5f;

    public float spawnInterval = 2f;
    
    public int spawnAmount = 1;
    public int maxSpawnAmount = 5;

    private int _currentSpawned;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(startDelay);
        while (_currentSpawned < maxSpawnAmount)
        {
            SpawnBatch();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnBatch()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            if (_currentSpawned >= maxSpawnAmount)
                return;
            
            Instantiate(prefabToSpawn, transform.position, transform.rotation);
            _currentSpawned++;
        }
    }

}
