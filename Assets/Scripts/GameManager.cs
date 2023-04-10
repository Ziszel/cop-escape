using System;
using UnityEngine;
using UnityEngine.SceneManagement;

enum GameState
{
    MainLoop,
    PlayerSeen,
    PlayerWon
}

public class GameManager : MonoBehaviour
{

    private GameState _gmState;

    private void Start()
    {
        _gmState = GameState.MainLoop;
    }

    private void Update()
    {
        if (_gmState == GameState.MainLoop)
        {
            // do something if required else remove this if statement later
        }
        else if (_gmState == GameState.PlayerSeen)
        {
            // stop game loop
            // show end game UI
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ResetLevel();
            }
        }
        else if (_gmState == GameState.PlayerWon)
        {
            EndGame();
        }
    }

    // This function will be called when the player has been seen
    // Potentially split this out further into game 'states', and call different functions depending on the 
    public static void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void EndGame()
    {
        // Update any global game vars here (best time, best score, etc...)
        Debug.Log("Game over!");
        ShowWinUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EndGame();
        }
    }

    private void ShowWinUI()
    {
        // implement later
    }
}
