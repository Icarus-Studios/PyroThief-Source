using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[Serializable]
public class enemySpawn
{
    public string Name = "Round";
    public GameObject[] enemies;
    public Vector2[] locations;
}


public class EnemySpawnScript : MonoBehaviour
{
    [Header("Enemies")]
    public float spawnRate = 2f;
    public enemySpawn[] enemyRounds;

    [Header("Reward")]
    public GameObject reward;
    public Vector2 rewardLocation;

    
    float nextSpawn = 0.0f;
    int roundNum = 0;
    int index = 0;
    int prevEnemiesActive = 0;
    // Start is called before the first frame update
    void Start()
    {
        prevEnemiesActive = GameManager.Instance.enemiesActive;
        GameManager.Instance.enemiesActive += enemyRounds[roundNum].enemies.Length;
    }
    void Update()
    {
        if (Time.time > nextSpawn && index < enemyRounds[roundNum].enemies.Length)
        {
            nextSpawn = Time.time + spawnRate;
            Instantiate(enemyRounds[roundNum].enemies[index], new Vector3(enemyRounds[roundNum].locations[index].x, enemyRounds[roundNum].locations[index].y, 3), Quaternion.identity);
            index++;

        }
        else if (GameManager.Instance.enemiesActive == prevEnemiesActive)
        {
            if(index >= enemyRounds[roundNum].enemies.Length)
            {
                if (roundNum >= (enemyRounds.Length - 1))
                {
                    Instantiate(reward, transform.position, Quaternion.identity);
                    Destroy(this);
                }
                else
                {
                    index = 0;
                    roundNum++;
                    GameManager.Instance.enemiesActive += enemyRounds[roundNum].enemies.Length;
                }
                
            }
        }
    }
}
