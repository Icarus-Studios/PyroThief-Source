using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{

    public GameObject enemy;
    public GameObject reward;
    float randX;
    float randY;
    Vector2 whereToSpawn;
    public float spawnRate = 2f;
    public int amountToSpawn;
    float nextSpawn = 0.0f;

    [Header("Ranges")]
    public float leftX;
    public float rightX;
    public float upY;
    public float downY;
    
    
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.enemiesActive += amountToSpawn;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawn && amountToSpawn > 0)
        {
            nextSpawn = Time.time + spawnRate;
            randX = Random.Range(leftX, rightX);
            randY = Random.Range(upY, downY);
            whereToSpawn = new Vector2(randX, randY);
            Instantiate(enemy, whereToSpawn, Quaternion.identity);
            amountToSpawn--;
            
        }
        else if (GameManager.Instance.enemiesActive == 0)
        {
            //Debug.Log("Heres a reward");
            Instantiate(reward, transform.position, Quaternion.identity);
            Destroy(this);

        }
    }
}
