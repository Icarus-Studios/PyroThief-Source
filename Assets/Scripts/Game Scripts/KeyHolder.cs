using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyHolder : MonoBehaviour
{
    private List<Key.KeyType> keyList;

    private void Awake()
    {
        keyList = new List<Key.KeyType>();
    }

    public void AddKey(Key.KeyType keyType)
    {
        Debug.Log("Added Key: " + keyType);
        keyList.Add(keyType);
    }

    public void RemoveKey(Key.KeyType keyType)
    {
        keyList.Remove(keyType);
    }

    public bool ContainsKey(Key.KeyType keyType)
    {
        return keyList.Contains(keyType);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Key key = collider.GetComponent<Key>();
        if (key != null)
        {
            AddKey(key.GetKeyType());
            Destroy(key.gameObject);
        }

        KeyDoor keyDoor = collider.GetComponent<KeyDoor>();
        if(keyDoor != null)
        {
            if (ContainsKey(keyDoor.GetKeyType()))
            {
                keyDoor.OpenDoor();
            }
            else
            {
                string room = keyDoor.GetKeyType().ToString();
                if (room.Equals("room1"))
                    room = "Room 1";
                else if (room.Equals("room2"))
                    room = "Room 2";
                else if (room.Equals("room3"))
                    room = "Room 3";
                else if (room.Equals("room4"))
                    room = "Room 4";
                else if (room.Equals("Hades"))
                    room = "Hades";
                else
                    room = "ERROR";

                Toast.Instance.Show("You need the key for " + room + "!", 5f);
            }
        }
        
    }
}
