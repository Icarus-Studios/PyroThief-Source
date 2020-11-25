using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnableShop : MonoBehaviour
{
    public GameObject shop;
    public GameObject shopTitle;
    public string shopName;
    public float safeRange = 1f;
    public LayerMask enemyLayers;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Collider2D[] enemiesInArea = Physics2D.OverlapCircleAll(transform.position, safeRange, enemyLayers);

        if (collision.gameObject.tag == "Player" && enemiesInArea.Length < 1)
        {
            Cursor.visible = true;
            shopTitle.GetComponent<Text>().text = shopName;
            shop.SetActive(true);
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Cursor.visible = false;
        shop.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, safeRange);
    }
}
