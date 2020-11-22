using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    private Text roundsSurvived;
    private Text timePlayed;

    // Start is called before the first frame update
    public void OnEnable()
    {
        roundsSurvived = GameObject.Find("RoundsSurvivedText").GetComponent<Text>();
        timePlayed = GameObject.Find("TimePlayedText").GetComponent<Text>();
        roundsSurvived.text = "Keys Collected: " + GameManager.Instance.getRounds();
        timePlayed.text = GameManager.Instance.getTimePlayed();
    }
}
