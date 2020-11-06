using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    public AudioSource swordSwing;
    public AudioSource damageSound;

    public void PlaySwordSwing()
    {
        swordSwing.Play();
    }

    public void PlayDamageSound()
    {
        damageSound.Play();
    }

    

}
