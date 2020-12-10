using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OST : MonoBehaviour
{
    public AudioSource BattleTheme;
    public AudioSource Underworld;
    public AudioSource BossTheme;
    public bool inUnderworld = false;
    public void PlayBattleTheme()
    {
        BattleTheme.Play();
    }

    public void PlayUnderworld()
    {
        Underworld.Play();
    }

    public void PlayBossTheme()
    {
        BossTheme.Play();
    }

    public void switchMusic()
    {
        if (inUnderworld == false)
        {
            //Debug.LogWarning("Enter Underworld");
            StartCoroutine(StartFade(BattleTheme, 2f, 0f));
            StartCoroutine(StartFade(Underworld, 2f, 1f));
            inUnderworld = true;
            PlayUnderworld();
        }
        else
        {
            //Debug.LogWarning("Leave Underworld");
            StartCoroutine(StartFade(Underworld, 2f, 0f));
            StartCoroutine(StartFade(BattleTheme, 2f, 1f));
            inUnderworld = false;
            PlayBattleTheme();
        }
    }

    public void switchToBossMusic()
    {
        StartCoroutine(StartFade(Underworld, 2f, 0f));
        StartCoroutine(StartFade(BattleTheme, 2f, 1f));
        PlayBossTheme();
    }
    

    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
}
