using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] Button _startGame;
    // Start is called before the first frame update
    void Start()
    {
        _startGame.onClick.AddListener(StartNewGame);
        Debug.Log("buttonClicked");
    }

    private void StartNewGame()
    {
        SceneManager.LoadScene(1);
    }



 
}
