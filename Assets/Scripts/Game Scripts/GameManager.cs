using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private int rounds = 0;
    private int playerLevel = 1;
    private float experience = 0f;
    private float xpForNextLevel = 100f;
    private float minutes, seconds;
    public int goldAmount = 100;
    private Text roundsSurvived;
    private Text timePlayed;
    private Text playerLvl;
    private static Text goldText;
    public Image UITurretPrompt;
    public int enemiesActive = 0;

    //these probably should go in their own script but I'm putting them here now as a UI proof of concept
    private Image weaponSelect;
    public Sprite swordSelected;
    public Sprite bowSelected;
    public bool swordActive = true;


    public Image circleBar;
    public Image extraHealth;
    public Image expBar;

    public float currentHealth = 0;
    public float maxHealth = 100;

    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    public GameObject settingsUI;
    public GameObject gameOverScreen;
    private ShakeBehavior cameraShake;

    public GameObject shop;

    //How much of the whole health bar is the circular part
    public float circlePercent = 0.3f;
    //How much of the circle part is used in the HealthBar
    private const float circleFillAmount = 0.75f;

    private ParticleSystem hpRegained;
    private Image tint;

    public static GameManager instance = null;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        tint = GameObject.Find("GreenTint").GetComponent<Image>();
        tint.enabled = false;
        roundsSurvived = GameObject.Find("roundsSurvived").GetComponent<Text>();
        timePlayed = GameObject.Find("timePlayed").GetComponent<Text>();
        playerLvl = GameObject.Find("playerLevel").GetComponent<Text>();
        goldText = GameObject.Find("goldAmount").GetComponent<Text>();
        roundsSurvived.text = "Keys Collected: " + rounds;
        timePlayed.text = "Time Played: 00:00";
        playerLvl.text = "Lv. " + playerLevel;
        goldText.text = goldAmount.ToString();

        weaponSelect = GameObject.Find("weaponSelect").GetComponent<Image>();
        weaponSelect.sprite = swordSelected;

        cameraShake = GameObject.Find("Main Camera").GetComponent<ShakeBehavior>();
        hpRegained = GameObject.Find("HPRecoveryParticles").GetComponent<ParticleSystem>();
       
        expBar.fillAmount = experience;

        //UITurretPrompt = GameObject.Find("TurretPrompt").GetComponent<Image>();


        pauseMenuUI.SetActive(false);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemiesActive++;
        }

    }

    // Update is called once per frame
    void Update()
    {
        updateTime();
        circleFill();
        extraFill();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            changeToSword();

        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            changeToBow();
        //Debug.LogWarning("Enemies Active: " + enemiesActive);
    }

    

    void OnEnable()
    {
        Resume();
    }

    public void Resume()
    {
        Cursor.visible = false;
        pauseMenuUI.SetActive(false);
        settingsUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.visible = true;
    }

    void updateTime()
    {
        minutes = (int)(Time.timeSinceLevelLoad / 60f);
        seconds = (int)(Time.timeSinceLevelLoad % 60f);
        timePlayed.text = "Time Played: " + minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    public string getTimePlayed() { return timePlayed.text; }

    public void updateRounds()
    {
        rounds++;
        roundsSurvived.text = "Keys Collected: " + rounds;
    }

    public int getRounds() { return rounds; }

    public void updatePlayerLevel()
    {
        playerLevel++;
        playerLvl.text = "Lv. " + playerLevel;
    }

    public void addExperince(int expAmount)
    {
        experience += expAmount;
        
        if (experience >= xpForNextLevel)
        {
            updatePlayerLevel();
            //xpForNextLevel = Mathf.Abs((playerLevel ^ 2 + playerLevel) / 2 * 100 - (playerLevel * 100));
            xpForNextLevel = (playerLevel / 10 + playerLevel % 10) * 100 * Mathf.Pow(10, playerLevel / 10);
            experience = 0;
            
        }
        float expFill = experience / xpForNextLevel;
        expBar.fillAmount = expFill;
        //Debug.Log("Experience:" + experience + ", XPforNextLevel: " + xpForNextLevel + ", expFill: " + expFill.ToString());
    }

   

    public void addGold(int amount)
    {
        goldAmount += amount;
        goldText.text = goldAmount.ToString();

        if(amount <= 0)
        {
            if (goldAmount < 20)
                UITurretPrompt.GetComponent<Image>().color = new Color32(54, 54, 54, 255);
        }
        else
        {
            if (goldAmount >= 20)
                UITurretPrompt.GetComponent<Image>().color = Color.white;
        }
    }

    public void addGold()
    {
        goldAmount ++;
        goldText.text = goldAmount.ToString();
    }

    public void changeWeapon()
    {
        if(swordActive)
        {
            swordActive = false;
            weaponSelect.sprite = bowSelected;
        }
        else
        {
            swordActive = true;
            weaponSelect.sprite = swordSelected;
        }
    }

    public void changeToSword()
    {
        swordActive = true;
        weaponSelect.sprite = swordSelected;
    }

    public void changeToBow()
    {
        swordActive = false;
        weaponSelect.sprite = bowSelected;
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
        GameObject player = GameObject.Find("Promethesus");

        if (amount < 0)
        {
            CinemachineShake.Instance.ShakeCamera(5f, .3f);
            player.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
        }
        else
        {
            if (currentHealth < maxHealth)
            {
                tint.enabled = true;
                StartCoroutine(FadeImage(false));
                hpRegained.Play();
                StartCoroutine(FadeImage(true));
            }
        }

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Time.timeScale = 0f;
            Cursor.visible = true;
            gameOverScreen.SetActive(true);
            isPaused = true;
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




