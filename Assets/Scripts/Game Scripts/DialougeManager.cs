﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pathfinding;

public class DialougeManager : MonoBehaviour
{
    [SerializeField] private float typingSpeed = 0.05f;
    private bool dialougeStarted = false;
    private int index;
    private float speechBubbleAnimationDelay = 1f;

    [Header("TMP")]
    [SerializeField] private TextMeshProUGUI dialougeText;
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("In Game Text")]
    [SerializeField] private string[] names;
    [TextArea]
    [SerializeField] private string[] dialougeSentences;
    

    [Header("Continue Button")]
    [SerializeField] private GameObject continueButton;

    [Header("Animator")]
    [SerializeField] private Animator speechBubbleAnimator;

    private GameObject SFX;
    private GameObject OST;
    private GameObject switchCamera;
    public GameObject controlScreen;


    public void Start()
    {
        switchCamera = GameObject.Find("CameraController");
        SFX = GameObject.Find("SFX");
        OST = GameObject.Find("OST");
        StartCoroutine(StartDialouge());
    }

    private void Update()
    {
        if(continueButton.activeSelf)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                ContinueDialouge();
            }    
        }
    }

    public IEnumerator StartDialouge()
    {
        Cursor.visible = true;
        speechBubbleAnimator.SetTrigger("Open");

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<AIPath>().enabled = false;
            enemy.GetComponent<SoldierAStarAI>().enabled = false;
            enemy.GetComponentInChildren<Animator>().enabled = false;
            foreach (SpriteRenderer sr in enemy.GetComponentsInChildren<SpriteRenderer>())
            {
                if (sr.gameObject.name == "MiniMapIndicator")
                {
                    sr.enabled = false;
                }
            }
        }

        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in player)
        {
            p.GetComponent<PlayerController>().enabled = false;
            p.GetComponent<Animator>().enabled = false;
        }


        yield return new WaitForSeconds(speechBubbleAnimationDelay);
        StartCoroutine(TypeDialouge());

    }

    

    private IEnumerator TypeDialouge()
    {
        if(index == 0 || !(names[index - 1].ToString().Equals(names[index].ToString())))
        {
            foreach (char letter in names[index].ToCharArray())
            {
                nameText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        foreach (char letter in dialougeSentences[index].ToCharArray())
        {
            dialougeText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        continueButton.SetActive(true);
    }


    public void ContinueDialouge()
    {
        if(index >= dialougeSentences.Length - 1)
        {
            nameText.text = string.Empty;
            dialougeText.text = string.Empty;

            Cursor.visible = false;

            if (gameObject.name.Equals("FirstCutscene"))
            {
                controlScreen.SetActive(false);
                OST.GetComponent<OST>().PlayBattleTheme();
            }
            else
                speechBubbleAnimator.SetTrigger("Close");


            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<AIPath>().enabled = true;
                enemy.GetComponent<SoldierAStarAI>().enabled = true;
                enemy.GetComponentInChildren<Animator>().enabled = true;
                
            }
            GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject p in player)
            {
                p.GetComponent<PlayerController>().enabled = true;
                p.GetComponent<Animator>().enabled = true;
            }
        }
        else
        {
            continueButton.SetActive(false);
            if (index < dialougeSentences.Length - 1)
            {

                index++;
                if (!names[index - 1].ToString().Equals(names[index].ToString()))
                {
                    nameText.text = string.Empty;
                }
                dialougeText.text = string.Empty;

                if(gameObject.name.Equals("FirstCutscene"))
                {
                    if (index == 2)
                    {
                        GameObject.Find("throneRoomFire").SetActive(false);
                        SFX.GetComponent<SFX>().PlayHorn();
                        switchCamera.GetComponent<Animator>().SetBool("cutscene1", true);
                        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                        foreach (GameObject enemy in enemies)
                        {
                            foreach (SpriteRenderer sr in enemy.GetComponentsInChildren<SpriteRenderer>())
                            {
                                if (sr.gameObject.name == "MiniMapIndicator")
                                {
                                    sr.enabled = true;
                                }
                            }
                        }
                    }

                    else if(index == 3)
                    {
                        switchCamera.GetComponent<Animator>().SetBool("cutscene1", false);
                        speechBubbleAnimator.SetTrigger("Close");
                        controlScreen.SetActive(true);
                    }
                        
                }
                StartCoroutine(TypeDialouge());
            }
        }
        
    }
}
