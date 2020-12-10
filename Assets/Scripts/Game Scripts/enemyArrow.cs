using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyArrow : MonoBehaviour
{
    private GameObject player;
    PlayerController script;
    void Start()
    {
        player = GameObject.Find("Promethesus");
        script = player.GetComponent<PlayerController>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Enemy" && collision.gameObject.tag != "enemyPro")
        {
            Debug.Log("Is shield pressed?:" + PlayerController.shieldNum);
            if (collision.gameObject.tag == "Player" && !(PlayerController.shieldNum == 1))
            {
                GameManager.Instance.updateHP(-5);
            }
            Destroy(this.gameObject);
        }
            
    }
}
