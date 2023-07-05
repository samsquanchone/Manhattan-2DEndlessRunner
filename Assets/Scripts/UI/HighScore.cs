using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScore : MonoBehaviour
{
    [SerializeField] private TMP_Text pointsText;

    // Start is called before the first frame update
    void Start()
    {

        if (ScoreHolder.GetScore() > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", ScoreHolder.GetScore());
            pointsText.text = "New High Score: " + UIManager.Instance.points;
        }

        else
        {
            pointsText.text = "High Score: " + PlayerPrefs.GetInt("HighScore");
        }
    }

}
