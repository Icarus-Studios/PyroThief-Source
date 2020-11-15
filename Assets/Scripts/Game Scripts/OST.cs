using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OST : MonoBehaviour
{
    public AudioSource BattleTheme;

    public void PlayBattleTheme()
    {
        BattleTheme.Play();
    }
}
