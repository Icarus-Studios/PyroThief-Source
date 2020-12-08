using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class miniBossSpawn
{
    public string Name = "___ Corner";
    public GameObject miniBoss;
    public Vector2 locations;
}



public class miniBossScript : MonoBehaviour
{
    [Header("Door Attributes")]
    public GameObject door;


    [Header("Minibosses")]
    public miniBossSpawn[] miniBosses;

    [Header("Reward")]
    public GameObject reward;
    //public Vector2 rewardLocation;

    bool inBattle = false;
    int prevEnemiesActive;

    void Start()
    {
        prevEnemiesActive = GameManager.Instance.enemiesActive;

        //MAKE SURE THE OBJECT ATTACTHED TO THIS SCRIPT IS IN THE MIDDLE OF THE ROOM
    }

    private void Update()
    {
        if(inBattle)
        {
            if(GameManager.Instance.enemiesActive <= prevEnemiesActive)
            {
                Instantiate(reward, transform.position, Quaternion.identity);
                door.SetActive(false);;
                Destroy(this);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(inBattle == false)
            {
                door.SetActive(true);


                GameManager.Instance.enemiesActive += miniBosses.Length;
                inBattle = true;

                for (int i = 0; i < miniBosses.Length; i++)
                {
                    Instantiate(miniBosses[i].miniBoss, new Vector3(miniBosses[i].locations.x, miniBosses[i].locations.y, 3), Quaternion.identity);
                }
            }
            
        }
    }
}
