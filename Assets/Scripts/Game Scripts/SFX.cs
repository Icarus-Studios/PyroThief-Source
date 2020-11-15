using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    public AudioSource swordSwing;
    public AudioSource damageSound;
    public AudioSource errorSound;
    public AudioSource pickupSound;
    public AudioSource horn;
    public void PlaySwordSwing()
    {
       swordSwing.Play();
    }

    public void PlayDamageSound()
    {
        damageSound.Play();
    }

    public void PlayErrorSound()
    {
        errorSound.Play();
    }

    public void PlayPickupSound()
    {
        pickupSound.Play();
    }

    public void PlayHorn()
    {
        horn.Play();
    }


}
