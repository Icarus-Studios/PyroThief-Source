using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player")
            //if (this.gameObject.tag == "Projectile" && collision.gameObject.tag != "Enemy")
               // Destroy(this.gameObject, 2f);
           // else
                Destroy(this.gameObject);
    }
}
