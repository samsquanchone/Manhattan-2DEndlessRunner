using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => m_instance;
    private static GameManager m_instance;

    [SerializeField] private SpawnManager spawnManager;

    private bool isPlaying = true;
    

    private void Start()
    {
        m_instance = this;
    }
    public void GameOverState()
    {
        //Stop spawning, stop time, show score, provide reset button 
        isPlaying = false;
        spawnManager.StopRoutine();
        UIManager.Instance.DeActivateGameUI();
    }

    public bool GameState()
    {
        bool state = isPlaying;

        return state;
    }
}
