using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image circleBar;
    public Image extraHealth;

    public float currentHealth = 0;
    public float maxHealth = 100;

    private ShakeBehavior cameraShake;

    //How much of the whole health bar is the circular part
    public float circlePercent = 0.3f;
    //How much of the circle part is used in the HealthBar
    private const float circleFillAmount = 0.75f;

    private ParticleSystem hpRegained;
    private Image tint;

    private void Start()
    {
        cameraShake = GameObject.Find("Main Camera").GetComponent<ShakeBehavior>();
        hpRegained = GameObject.Find("HPRecoveryParticles").GetComponent<ParticleSystem>();
        tint = GameObject.Find("GreenTint").GetComponent<Image>();
        tint.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        circleFill();
        extraFill();
    }

    void circleFill()
    {
        float healthPercent = currentHealth / maxHealth;
        float circleFill = healthPercent / circlePercent;
        circleFill *= circleFillAmount;
        circleFill = Mathf.Clamp(circleFill, 0, circleFillAmount);
        circleBar.fillAmount = circleFill;
    }

    void extraFill()
    {
        float circleAmount = circlePercent * maxHealth;
        float barHealth = currentHealth - circleAmount;
        float barTotalHealth = maxHealth - circleAmount;
        float barFill = barHealth / barTotalHealth;
        barFill = Mathf.Clamp(barFill, 0, 1);
        extraHealth.fillAmount = barFill;
    }

    public void updateHP(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, 100);
        if (amount < 0)
            cameraShake.TriggerShake();
        else
        {
            tint.enabled = true;
            StartCoroutine(FadeImage(false));
            hpRegained.Play();
            StartCoroutine(FadeImage(true));
        }
    }

    IEnumerator FadeImage(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 5 second backwards
            for (float i = 5; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                tint.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 5 second
            for (float i = 0; i <= 5; i += Time.deltaTime)
            {
                // set color with i as alpha
                tint.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
    }
}
