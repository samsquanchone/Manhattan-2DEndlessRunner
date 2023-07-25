using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HowToPlayUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button menuButton;

    // Start is called before the first frame update
    void Start()
    {
        playButton.onClick.AddListener(delegate { PlayGame(); });
        menuButton.onClick.AddListener(delegate { ToMenu(); });
    }

    void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    void ToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
