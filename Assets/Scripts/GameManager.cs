using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [SerializeField]
    public static bool finishedGeneratingMaze = false;
    public static float mazeUnitSize;
    public static float mazeSize;

    // Start is called before the first frame update
    private void Start()
    {
        finishedGeneratingMaze = false;
    }

    // Update is called once per frame
    void Update()
    {
        // return to the start screen after pressing 'R' when finished
        // generating the maze 
        if (Input.GetKeyDown(KeyCode.R) && finishedGeneratingMaze)
        {
            SceneManager.LoadScene(0);
        }
    }
}
