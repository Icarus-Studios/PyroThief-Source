using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSwitch : MonoBehaviour
{
    private GameObject OST;
    // Start is called before the first frame update
    void Start()
    {
        OST = GameObject.Find("OST");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            OST.GetComponent<OST>().switchMusic();
    }
}
