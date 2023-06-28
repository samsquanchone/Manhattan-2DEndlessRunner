using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    //Singleton 
    public static UIManager Instance => m_instance;
    private static UIManager m_instance;

    //UI Refs
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private TMP_Text timeText;
    //Variables
    int points = 0;

    private void Start()
    {
        m_instance = this;  //Instantiate singleton
    }
    private void Update()
    {
        CalculateTime();
    }

    public void DeActivateGameUI()
    {
        healthText.gameObject.SetActive(false);
        pointsText.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
    }
    public void ChangePlayerHealht(int newValue)
    {
        healthText.text = "Health: " + newValue;
    }

    public void IncrementPoints(int amount)
    {
        points += amount;
        pointsText.text = "Points: " + points.ToString(); 
    }

    private void CalculateTime()
    {
        float t = Time.timeSinceLevelLoad; // time since scene loaded

        float milliseconds = (Mathf.Floor(t * 100) % 100); // calculate the milliseconds for the timer

        int seconds = (int)(t % 60); // return the remainder of the seconds divide by 60 as an int
        t /= 60; // divide current time y 60 to get minutes
        int minutes = (int)(t % 60); //return the remainder of the minutes divide by 60 as an int
        t /= 60; // divide by 60 to get hours
        int hours = (int)(t % 24); // return the remainder of the hours divided by 60 as an int

        timeText.text = string.Format("{0}:{1}:{2}.{3}", hours.ToString("00"), minutes.ToString("00"), seconds.ToString("00"), milliseconds.ToString("00"));
    }
}