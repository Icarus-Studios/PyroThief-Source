using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{

    public AudioMixer audioMixer;
    public Light brightness;

    public void SetMainVol(Slider slider)
    {
        //Debug.Log("Master volume set to: " + slider.value);
        bool diditdoit = audioMixer.SetFloat("MasterVolume", slider.value);
        //Debug.Log(diditdoit);
    }

    public void SetMusicVol(Slider slider)
    {
        //Debug.Log("Music volume set to: " + slider.value);
        audioMixer.SetFloat("MusicVolume", slider.value);
    }

    public void SetSFXVol(Slider slider)
    {
        //Debug.Log("SFX volume set to: " + slider.value);
        audioMixer.SetFloat("SFXVolume", slider.value);
    }

    public void SetBrightness(Slider slider)
    {
        //Debug.Log("Brightness set to: " + slider.value);
        //RenderSettings.ambientLight = new Color(slider.value, slider.value, slider.value, 1);
        brightness.intensity = slider.value;
    }

}
