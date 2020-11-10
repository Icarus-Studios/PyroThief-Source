using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addHPAmount : MonoBehaviour
{
    public int amountWorth;
    public void addHPToCount()
    {
        GameManager.Instance.updateHP(amountWorth);
    }
}
