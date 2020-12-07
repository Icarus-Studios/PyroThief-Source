using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyArrow : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Enemy" && collision.gameObject.tag != "enemyPro")
        {
            if (collision.gameObject.tag == "Player")
                GameManager.Instance.updateHP(-5);
            Destroy(this.gameObject);
        }
            
    }
}
