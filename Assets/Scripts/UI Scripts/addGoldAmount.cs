using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addGoldAmount : MonoBehaviour
{
    public int amountWorth;
    public void addGoldToCount()
    {
        GameManager.Instance.addGold(amountWorth);
    }
}
