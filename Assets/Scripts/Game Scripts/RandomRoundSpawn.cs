using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRoundSpawn : MonoBehaviour
{
    public GameObject[] enemy;
    [Space]
    public GameObject reward;
    float randX;
    float randY;
    Vector2 whereToSpawn;
    public float spawnRate = 2f;
    public int amountOfRounds;
    public int minAmountToSpawn;
    public int maxAmountToSpawn;

    private int amountSpawning;
    private int selectedEnemy;
    float nextSpawn = 0.0f;
    int prevEnemiesActive = 0;
    int currentRound;

    [Header("Ranges")]
    public float leftX;
    public float rightX;
    public float upY;
    public float downY;

    // Start is called before the first frame update
    void Start()
    {
        prevEnemiesActive = GameManager.Instance.enemiesActive;
        amountSpawning = Random.Range(minAmountToSpawn, maxAmountToSpawn);
        GameManager.Instance.enemiesActive += amountSpawning;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawn && amountSpawning > 0)
        {
            nextSpawn = Time.time + spawnRate;
            randX = Random.Range(leftX, rightX);
            randY = Random.Range(upY, downY);
            selectedEnemy = Random.Range(0, enemy.Length);
            Instantiate(enemy[selectedEnemy], new Vector3(randX, randY, 3), Quaternion.identity);
            amountSpawning--;

        }
        else if (GameManager.Instance.enemiesActive == prevEnemiesActive)
        {
            if (currentRound >= amountOfRounds)
            {
                Instantiate(reward, new Vector3(transform.position.x, transform.position.y, 3), Quaternion.identity);
                Destroy(this);
            }
            else
            {
                currentRound++;
                amountSpawning = Random.Range(minAmountToSpawn, maxAmountToSpawn);
                GameManager.Instance.enemiesActive += amountSpawning;
            }
        }
    }
}
