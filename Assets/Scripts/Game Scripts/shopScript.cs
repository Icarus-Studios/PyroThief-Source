using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shopScript : MonoBehaviour
{

    [Header("Number Attributes")]
    private int turretLvl = 1;
    public int turretCost;
    public int attackCost;
    public int moreHPCost;
    public int restoreHPCost;
    [Space]
    private int turretUpgrade = 0;
    private int attackUpgrade = 0;
    private float hpUpgrade = 0f;
    [Space]
    public double costMultiplyer;
    public double upgradeMultiplyer;

    [Header("Text Attributes")]
    public Text turretInfo;
    public Text turretCostText;
    public Text attackInfo;
    public Text attackCostText;
    public Text moreHPInfo;
    public Text moreHPCostText;
    public Text restoreHPInfo;
    public Text restoreHPCostText;

    private GameObject player;
    private GameObject hud;
    private GameObject turretButton;
    private GameObject attackButton;
    private GameObject HPUpgradeButton;
    private GameObject HPRestoreButton;
    private GameObject shopTitle;
    public GameObject UITurret;
    private GameObject SFX;


    // Start is called before the first frame update
    void Start()
    {
        turretUpgrade = player.GetComponent<PlayerController>().turretDamage + (int)(player.GetComponent<PlayerController>().turretDamage * upgradeMultiplyer);
        attackUpgrade = player.GetComponent<PlayerController>().attackDamage + (int)(player.GetComponent<PlayerController>().attackDamage * upgradeMultiplyer);
        hpUpgrade = (float)(hud.GetComponent<GameManager>().maxHealth + (hud.GetComponent<GameManager>().maxHealth * upgradeMultiplyer));

        turretInfo.text = "Lv. 1 (" + player.GetComponent<PlayerController>().turretDamage.ToString() + " Attack)";
        attackInfo.text = "Raise Attack from " + player.GetComponent<PlayerController>().attackDamage + " to " + attackUpgrade;
        moreHPInfo.text = "Raise HP from " + hud.GetComponent<GameManager>().maxHealth + " to " + hpUpgrade;
        

        turretCostText.text = turretCost.ToString();
        attackCostText.text = attackCost.ToString();
        moreHPCostText.text = moreHPCost.ToString();
        restoreHPCostText.text = restoreHPCost.ToString();
    }

    void colorButtons()
    {
        if (hud.GetComponent<GameManager>().goldAmount < turretCost)
            turretButton.GetComponent<Image>().color = new Color32(44, 44, 44, 255);
        else
            turretButton.GetComponent<Image>().color = new Color32(135, 89, 54, 255);

        if (hud.GetComponent<GameManager>().goldAmount < attackCost)
            attackButton.GetComponent<Image>().color = new Color32(44, 44, 44, 255);
        else
            attackButton.GetComponent<Image>().color = new Color32(135, 89, 54, 255);

        if (hud.GetComponent<GameManager>().goldAmount < moreHPCost)
            HPUpgradeButton.GetComponent<Image>().color = new Color32(44, 44, 44, 255);
        else
            HPUpgradeButton.GetComponent<Image>().color = new Color32(135, 89, 54, 255);

        if (hud.GetComponent<GameManager>().goldAmount < restoreHPCost || (hud.GetComponent<GameManager>().currentHealth == hud.GetComponent<GameManager>().maxHealth))
            HPRestoreButton.GetComponent<Image>().color = new Color32(44, 44, 44, 255);
        else
            HPRestoreButton.GetComponent<Image>().color = new Color32(135, 89, 54, 255);
    }
    

    private void OnEnable()
    {
        player = GameObject.Find("Promethesus");
        hud = GameObject.Find("HUD");
        turretButton = GameObject.Find("turretButton");
        attackButton = GameObject.Find("attackButton");
        HPUpgradeButton = GameObject.Find("moreHPButton");
        HPRestoreButton = GameObject.Find("replenishHPButton");
        shopTitle = GameObject.Find("shopTitle");
        SFX = GameObject.Find("SFX");
        restoreHPInfo.text = "Restore HP from " + hud.GetComponent<GameManager>().currentHealth + " to " + hud.GetComponent<GameManager>().maxHealth;
        colorButtons();
    }

    private bool enoughGold(int amount)
    {
        return (amount <= hud.GetComponent<GameManager>().goldAmount);
    }

    public void buyTurret()
    {
        if(enoughGold(turretCost))
        {
            hud.GetComponent<GameManager>().addGold(-turretCost);
            turretCost += (int)(turretCost * costMultiplyer);
            player.GetComponent<PlayerController>().turretDamage = turretUpgrade;
            turretUpgrade += (int)(player.GetComponent<PlayerController>().turretDamage * upgradeMultiplyer);
            if (turretLvl == 1)
            {
                UITurret.SetActive(true);
                turretCost = 40;
                turretLvl++;
                player.GetComponent<PlayerController>().turretDamage = 10;
            }
            turretInfo.text = player.GetComponent<PlayerController>().turretDamage + " -> " + turretUpgrade + " Attack";
            turretCostText.text = turretCost.ToString();
            colorButtons();
        }
        else
        {
            SFX.GetComponent<SFX>().PlayErrorSound();
            CinemachineShake.Instance.ShakeCamera(10f, .3f);
        }
    }

    public void buyAttack()
    {
        if(enoughGold(attackCost))
        {
            hud.GetComponent<GameManager>().addGold(-attackCost);
            attackCost += (int)(attackCost * costMultiplyer);
            player.GetComponent<PlayerController>().attackDamage = attackUpgrade;
            attackUpgrade += (int)(player.GetComponent<PlayerController>().attackDamage * upgradeMultiplyer);
            attackInfo.text = "Raise Attack from " + player.GetComponent<PlayerController>().attackDamage + " to " + attackUpgrade;
            attackCostText.text = attackCost.ToString();
            colorButtons();
        }
        else
        {
            SFX.GetComponent<SFX>().PlayErrorSound();
            CinemachineShake.Instance.ShakeCamera(10f, .3f);
        }
    }

    public void buyHPUpgrade()
    {
        if (enoughGold(moreHPCost))
        {
            hud.GetComponent<GameManager>().addGold(-moreHPCost);
            moreHPCost += (int)(moreHPCost * costMultiplyer);
            hud.GetComponent<GameManager>().maxHealth = hpUpgrade;
            hud.GetComponent<GameManager>().currentHealth = hpUpgrade;
            hpUpgrade += (int)(hud.GetComponent<GameManager>().maxHealth * upgradeMultiplyer);
            moreHPInfo.text = "Raise HP from " + hud.GetComponent<GameManager>().maxHealth + " to " + hpUpgrade;
            restoreHPInfo.text = "Restore HP from " + hud.GetComponent<GameManager>().currentHealth + " to " + hud.GetComponent<GameManager>().maxHealth;
            moreHPCostText.text = moreHPCost.ToString();
            colorButtons();
        }
        else
        {
            SFX.GetComponent<SFX>().PlayErrorSound();
            CinemachineShake.Instance.ShakeCamera(10f, .3f);
        }
    }

    public void restoreHP()
    {
        if(enoughGold(restoreHPCost) && (hud.GetComponent<GameManager>().currentHealth != hud.GetComponent<GameManager>().maxHealth))
        {
            hud.GetComponent<GameManager>().addGold(-restoreHPCost);
            hud.GetComponent<GameManager>().currentHealth = hud.GetComponent<GameManager>().maxHealth;
            restoreHPInfo.text = "Restore HP from " + hud.GetComponent<GameManager>().currentHealth + " to " + hud.GetComponent<GameManager>().maxHealth;
            colorButtons();
        }
        else
        {
            SFX.GetComponent<SFX>().PlayErrorSound();
            CinemachineShake.Instance.ShakeCamera(10f, .3f);
        }
    }
}
