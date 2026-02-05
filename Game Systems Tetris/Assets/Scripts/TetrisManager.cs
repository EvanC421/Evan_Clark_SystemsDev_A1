using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TetrisManager : MonoBehaviour
{
    public int score { get; private set; }
    public bool gameOver { get; private set; }

    public UnityEvent OnScoreChanged;
    public UnityEvent OnGameOver;

    void Start()
    {
        SetGameOver(false);
    }

    public int CalculateScore(int clearedRows)
    {
        switch (clearedRows)
        {
            case 1: return 100;
            case 2: return 300;
            case 3: return 500;
            case 4: return 800;
            default: return 0;
        }
    }

    public void changeScore(int amount)
    {
        score += amount;
        OnScoreChanged.Invoke();
    }

    public void SetGameOver (bool gameOver)
    {
        if (!gameOver)
        {
            //when game over is false, reset score
            score = 0;
            changeScore(0);
        }

        this.gameOver = gameOver;
        OnGameOver.Invoke();
    }
}
