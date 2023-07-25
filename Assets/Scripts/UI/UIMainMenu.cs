using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIMainMenu : MonoBehaviour
{
    
    [SerializeField] Button _startGame;
    [SerializeField] Button _howToPlayButton;
    // Start is called before the first frame update
    void Start()
    {
        _startGame.onClick.AddListener(StartNewGame);
        _howToPlayButton.onClick.AddListener(HowToPlay);
        Debug.Log("buttonClicked");
    }

    private void StartNewGame()
    {
        SceneManager.LoadScene(1);
    }

    private void HowToPlay()
    {
        SceneManager.LoadScene(3);
    }



 
}
