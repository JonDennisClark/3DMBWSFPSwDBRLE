using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] animalPrefabs;
    private float maxWait = 1.0f;
    float spawnRangeX = 11;
    float spawnRangeZ = 20;
    void Start()
    {
        InvokeRepeating("Spawn", 2.0f, maxWait);
    }

    void Spawn()
    {
        int animalIndex;
        Vector3 spawnRange = new Vector3(Random.Range(-spawnRangeX, spawnRangeX), 0, spawnRangeZ);
        animalIndex = Random.Range(0, animalPrefabs.Length);
        Instantiate(animalPrefabs[animalIndex], spawnRange, animalPrefabs[animalIndex].transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
}
