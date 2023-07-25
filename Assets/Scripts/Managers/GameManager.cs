using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameStateEnum {Normal, MenuOpen };

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => m_instance;
    private static GameManager m_instance;
    private GameStateEnum gameState = GameStateEnum.Normal;
   

    private bool isPlaying = true;
    

    private void Start()
    {
        m_instance = this;
    }
    public void GameOverState()
    {
        //Stop spawning, stop time, show score, provide reset button 
        isPlaying = false;
        UIManager.Instance.SetEndScore();
        SpawnManager.Instance.StopRoutine();
        UIManager.Instance.DeActivateGameUI();
        SceneManager.LoadScene(2);
    }

    public bool GameState()
    {
        bool state = isPlaying;

        return state;
    }

    public GameStateEnum GetGameState()
    {
        return gameState;
    }

    public void SetGameState(GameStateEnum state)
    {
        gameState = state;
    }
}
