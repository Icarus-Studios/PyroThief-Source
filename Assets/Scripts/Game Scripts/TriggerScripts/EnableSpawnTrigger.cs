using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableSpawnTrigger : MonoBehaviour
{
    public GameObject spawner;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        spawner.SetActive(true);
        Destroy(this);
    }
}
