using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static bool finishedGeneratingMaze = false;
    public static bool finishedRenderingMaze = false;
    public static bool firstPersonMode = false;
    public static bool wonTheGameBefore = false;
    public static bool startGame = false;
    public static bool playGame = false;
    public static bool finishedGame = false;
    public static float mazeUnitSize;
    public static float mazeWidth;
    public static float mazeHeight;

    // Start is called before the first frame update
    private void Start()
    {
        finishedGame = false;
        finishedGeneratingMaze = false;
        finishedRenderingMaze = false;
        wonTheGameBefore = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.QuitGame();
        }
    }

    public static void QuitGame()
    {
        Application.Quit();
    }

    public static void LoadGame()
    {
        SceneManager.LoadScene(1);
    }
}
