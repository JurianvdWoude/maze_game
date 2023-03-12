using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static bool finishedGeneratingMaze = false;
    public static float mazeUnitSize;
    public static float mazeWidth;
    public static float mazeHeight;

    // Start is called before the first frame update
    private void Start()
    {
        finishedGeneratingMaze = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
