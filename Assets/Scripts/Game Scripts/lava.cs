using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lava : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collider)
    {
        GameObject player = GameObject.Find("Promethesus");

        if (collider.gameObject.tag == "Player")
        { 
                GameManager.Instance.updateHP(-.25f);
                CinemachineShake.Instance.ShakeCamera(5f, .3f);
                player.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);            
        }
        else
        {
            player.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        }
    }

}
