using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICOntroller : MonoBehaviour
{
    public TextMeshProUGUI scoretext;

    public TetrisManager tetrisManager;

    public GameObject endGamePanel;

    public GameObject winGamePanel;
    public void UpdateScore()
    {
        scoretext.text = $"SCORE: {tetrisManager.score:n0}";
    }

    public void UpdateGameOver()
    {
        //if win condition is met, display new win screen
        //win condition is based on score. Failure to meet score means there are still grey cells, meaning game over.
        if (tetrisManager.score >= 1000)
        {
            winGamePanel.SetActive(tetrisManager.gameOver);
        }

        else if (tetrisManager.score < 1000)
        {
            //when the game over Event is broadcast the end game panel will show when the game is over
            endGamePanel.SetActive(tetrisManager.gameOver);
        }
    }

    public void PlayAgain()
    {
        //setting game over to false resets game
        winGamePanel.SetActive(false);
        tetrisManager.SetGameOver(false);
    }
}
