using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExitButton : MonoBehaviour
{
     [SerializeField] Button _exitGame;

    // Start is called before the first frame update
    void Start()
    {
        _exitGame.onClick.AddListener(ExitGame);
    }

    private void ExitGame(){
        Application.Quit();        
        Debug.Log("QUITbuttonClicked");
    }
}
