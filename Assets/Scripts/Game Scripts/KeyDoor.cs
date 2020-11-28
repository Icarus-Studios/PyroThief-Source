using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoor : MonoBehaviour
{
    [SerializeField] private Key.KeyType keyType;

    public GameObject miniMapIndicator;
    public Key.KeyType GetKeyType()
    {
        return keyType;
    }

    public void OpenDoor()
    {
        if(miniMapIndicator != null)
            Destroy(miniMapIndicator);
        gameObject.SetActive(false);
    }

   
}
