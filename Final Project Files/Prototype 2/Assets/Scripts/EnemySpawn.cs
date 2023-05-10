using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySpawn: MonoBehaviour
{
    public float timeVal;
    public TextMeshProUGUI timeDisplay;
    private int waveNumber = 0;
    public int enemySpawnAmount = 0;
    public int enemiesKilled = 0;

    public GameObject[] spawners;
    public GameObject[] enemies;
    public GameObject enemy;

    private void Start()
    {
        timeVal = 3;
        timeDisplay.gameObject.SetActive(true);
        spawners = new GameObject[8];
        //enemies = new GameObject[2];

        for(int i = 0; i < spawners.Length; i++)
        {
            spawners[i] = transform.GetChild(i).gameObject;
        }

        //3 is hard coded in rather than referring to timeVal because timeVal will constantly decrease until reaching 0. This logic stands for all further uses of Invoke.
        Invoke(nameof(StartWave), 3);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            NextWave();
        }
        if (timeVal > 0)
        {
            timeVal -= Time.deltaTime;

        }
        else
        {
            timeVal = 0;
        }

        DisplayTime();

        if (timeVal == 0)
        {
            timeDisplay.gameObject.SetActive(false);
        }
        if (enemiesKilled >= enemySpawnAmount && enemySpawnAmount != 0)
        {
            timeVal = 3;
            timeDisplay.gameObject.SetActive(true);
            enemiesKilled = 0;
            Invoke(nameof(NextWave), 3);
        }
    }

    private void SpawnEnemy()
    {
        int spawnerID = Random.Range(0, spawners.Length);
        int randEnemy = Random.Range(0, enemies.Length);
        Instantiate(enemies[randEnemy], spawners[spawnerID].transform.position, spawners[spawnerID].transform.rotation);
    }

    private void StartWave()
    {
        waveNumber = 1;
        enemySpawnAmount = 8;
        enemiesKilled = 0;

        for (int i = 0; i < enemySpawnAmount; i++)
        {
            SpawnEnemy();
        }
    }

    private void NextWave()
    {
        waveNumber++;
        enemySpawnAmount += 2;
        Debug.Log(enemySpawnAmount);
        for (int i = 0; i < enemySpawnAmount; i++)
        {
            SpawnEnemy();
        }
    }

    public void DisplayTime()
    {
        timeDisplay.text = string.Format("Next round begins in: {0:0}", timeVal);
    }
}