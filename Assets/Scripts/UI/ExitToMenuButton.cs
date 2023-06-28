using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExitToMenuButton : MonoBehaviour
{
    [SerializeField] Button _exitToMenu;

    // Start is called before the first frame update
    void Start()
    {
        _exitToMenu.onClick.AddListener(ExitToMenu);
    }

    private void ExitToMenu(){
       SceneManager.LoadScene(0);
    }
}
