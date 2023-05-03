using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static bool finishedRenderingMaze;
    public static bool playingMazeGame;

    public static float mazeUnitSize;
    public static float mazeWidth;
    public static float mazeHeight;

    // event that goes off when the maze has finished generating
    public static event Action FinishGeneratingMazeEvent;
    // event that goes off when the first person maze game is being played
    public static event Action StartMazeGameEvent;
    // event that goes off when the player presses a button to exit the first person maze game
    public static event Action StopMazeGameEvent;
    // event that goes off when the player reaches the goal in the first person maze game
    public static event Action FinishMazeGameEvent;

    // Start is called before the first frame update
    private void Start()
    {
        // events are set to null so that the same functions aren't
        // subscribed to them again when the game gets restarted
        FinishGeneratingMazeEvent = null;
        StartMazeGameEvent = null;
        StopMazeGameEvent = null;
        FinishMazeGameEvent = null;
        finishedRenderingMaze = false;
        playingMazeGame = false;
    }

    private void Update()
    {
        // when you press the escape key
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(playingMazeGame)
            {
                // Invoke the StopMazeGameEvent
                OnStopMazeGame();
            } 
            else
            {
                // Quit the game if you're in the maze generating menu and pressed 'Esc'
                QuitGame();
            }
        }
    }

    public static void QuitGame()
    {
        Application.Quit();
    }

    public static void LoadGame()
    {
        // reload the maze generating scene
        SceneManager.LoadScene(1);
    }

    public static void OnFinishGeneratingMaze()
    {
        // this flag is needed so that the higher-poly maze only gets rendered once
        if(!finishedRenderingMaze)
        {
            FinishGeneratingMazeEvent?.Invoke();
            finishedRenderingMaze = true;
        }
    }

    public static void OnStartMazeGame()
    {
        StartMazeGameEvent?.Invoke();
        playingMazeGame = true;
    }

    public static void OnFinishMazeGame()
    {
        FinishMazeGameEvent?.Invoke();
        playingMazeGame = false;
    }

    public static void OnStopMazeGame()
    {
        StopMazeGameEvent?.Invoke();
        playingMazeGame = false;
    }
}
