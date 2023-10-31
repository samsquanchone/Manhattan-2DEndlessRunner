using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScoreHolder
{
    private static int score;
    public static void HoldScore(int newScore)
    {
        score = newScore;
    }
    public static int GetScore()
    {
        int _score = score;
        Debug.Log("Score");
        return _score;
    }
}


