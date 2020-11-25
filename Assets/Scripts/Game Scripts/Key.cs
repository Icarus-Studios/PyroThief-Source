using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private KeyType keyType;
  public enum KeyType
    {
        room1,
        room2,
        room3,
        room4,
        Hades
    }

    public KeyType GetKeyType()
    {
        return keyType;
    }
}
