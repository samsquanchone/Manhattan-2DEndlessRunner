using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameMenu : MonoBehaviour
{
    [SerializeField] GameObject menuPanel;
    [SerializeField] Button resumeButton;
    [SerializeField] Button quitButton;

    private void Start()
    {
        menuPanel.SetActive(false);
        resumeButton.onClick.AddListener(delegate { HideMenu(); });
        quitButton.onClick.AddListener(delegate { QuitToMenu(); });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowMenu();
        }
    }

    void ShowMenu() 
    {
        GameManager.Instance.SetGameState(GameStateEnum.MenuOpen);
        menuPanel.SetActive(true);
    }

    void HideMenu()
    {
        GameManager.Instance.SetGameState(GameStateEnum.Normal);
        menuPanel.SetActive(false);
    }

    void QuitToMenu()
    {
        SceneManager.LoadScene(0);
    }


}
