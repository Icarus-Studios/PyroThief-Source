using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int rounds = 0;
    private int playerLevel = 1;
    private float minutes, seconds;
    public int goldAmount = 0;
    private Text roundsSurvived;
    private Text timePlayed;
    private Text playerLvl;
    private Text goldText;


    //these probably should go in their own script but I'm putting them here now as a UI proof of concept
    private Image weaponSelect;
    public Sprite swordSelected;
    public Sprite bowSelected;
    private bool swordActive = true;

    // Start is called before the first frame update
    void Start()
    {
        roundsSurvived = GameObject.Find("roundsSurvived").GetComponent<Text>();
        timePlayed = GameObject.Find("timePlayed").GetComponent<Text>();
        playerLvl = GameObject.Find("playerLevel").GetComponent<Text>();
        goldText = GameObject.Find("goldAmount").GetComponent<Text>();
        roundsSurvived.text = "Rounds Survived: " + rounds;
        timePlayed.text = "Time Played: 00:00";
        playerLvl.text = "Lv. " + playerLevel;
        goldText.text = goldAmount.ToString();

        weaponSelect = GameObject.Find("weaponSelect").GetComponent<Image>();
        weaponSelect.sprite = swordSelected;

    }

    // Update is called once per frame
    void Update()
    {
        updateTime();

    }

    void updateTime()
    {
        minutes = (int)(Time.timeSinceLevelLoad / 60f);
        seconds = (int)(Time.timeSinceLevelLoad % 60f);
        timePlayed.text = "Time Played: " + minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    public void updateRounds()
    {
        rounds++;
        roundsSurvived.text = "Rounds Survived: " + rounds;
    }

    public void updatePlayerLevel()
    {
        playerLevel++;
        playerLvl.text = "Lv. " + playerLevel;
    }

    public void addGold(int amount)
    {
        goldAmount += amount;
        goldText.text = goldAmount.ToString();
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
}
