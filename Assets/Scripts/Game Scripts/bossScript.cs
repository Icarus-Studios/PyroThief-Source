using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossScript : MonoBehaviour
{
    [Header("Door Attributes")]
    public GameObject door;


    [Header("Boss")]
    public GameObject boss;
    public Vector2 location;

    [Header("Reward")]
    public GameObject reward;
    //public Vector2 rewardLocation;

    bool inBattle = false;
    int prevEnemiesActive;
    private GameObject OST;

    void Start()
    {
        prevEnemiesActive = GameManager.Instance.enemiesActive;
        OST = GameObject.Find("OST");
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


                GameManager.Instance.enemiesActive++;
                inBattle = true;
                OST.GetComponent<OST>().switchToBossMusic();
                Instantiate(boss, new Vector3(location.x, location.y, 3), Quaternion.identity);
            }
            
        }
    }
}
