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
        pointsText.text = "Highscore: " + UIManager.Instance.points;
    }

}
