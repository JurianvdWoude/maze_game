using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _maze;
    [SerializeField]
    private GameObject _mazeUnit;
    // Start is called before the first frame update
    void Start()
    {
        _maze.Add(Instantiate(_mazeUnit, new Vector3(0, 0, 0), Quaternion.identity));
        // Transform childToRemove = _maze[0].transform.Find("LeftWall");
        // childToRemove.parent = null;
        // Destroy(childToRemove.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        GameObject lastMazeUnit = _maze.LastOrDefault<GameObject>();
        int lastMazeUnitIndex = _maze.LastIndexOf(lastMazeUnit);
        Vector3 lastMazeUnitPosition = lastMazeUnit.transform.position;
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            Transform lastMazeUnitChildToRemove = lastMazeUnit.transform.Find("LeftWall");
            lastMazeUnitChildToRemove.parent = null;
            Destroy(lastMazeUnitChildToRemove.gameObject);
            GameObject newMazeUnit = Instantiate(_mazeUnit, lastMazeUnitPosition + new Vector3(-1, 0, 0), Quaternion.identity);
            Transform newMazeUnitChildToRemove = newMazeUnit.transform.Find("RightWall");
            newMazeUnitChildToRemove.parent = null;
            Destroy(newMazeUnitChildToRemove.gameObject);
            _maze.Add(newMazeUnit);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            Transform lastMazeUnitChildToRemove = lastMazeUnit.transform.Find("TopWall");
            lastMazeUnitChildToRemove.parent = null;
            Destroy(lastMazeUnitChildToRemove.gameObject);
            GameObject newMazeUnit = Instantiate(_mazeUnit, lastMazeUnitPosition + new Vector3(0, 1, 0), Quaternion.identity);
            Transform newMazeUnitChildToRemove = newMazeUnit.transform.Find("BottomWall");
            newMazeUnitChildToRemove.parent = null;
            Destroy(newMazeUnitChildToRemove.gameObject);
            _maze.Add(newMazeUnit);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            Transform lastMazeUnitChildToRemove = lastMazeUnit.transform.Find("RightWall");
            lastMazeUnitChildToRemove.parent = null;
            Destroy(lastMazeUnitChildToRemove.gameObject);
            GameObject newMazeUnit = Instantiate(_mazeUnit, lastMazeUnitPosition + new Vector3(1, 0, 0), Quaternion.identity);
            Transform newMazeUnitChildToRemove = newMazeUnit.transform.Find("LeftWall");
            newMazeUnitChildToRemove.parent = null;
            Destroy(newMazeUnitChildToRemove.gameObject);
            _maze.Add(newMazeUnit);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            Transform lastMazeUnitChildToRemove = lastMazeUnit.transform.Find("BottomWall");
            lastMazeUnitChildToRemove.parent = null;
            Destroy(lastMazeUnitChildToRemove.gameObject);
            GameObject newMazeUnit = Instantiate(_mazeUnit, lastMazeUnitPosition + new Vector3(0, -1, 0), Quaternion.identity);
            Transform newMazeUnitChildToRemove = newMazeUnit.transform.Find("TopWall");
            newMazeUnitChildToRemove.parent = null;
            Destroy(newMazeUnitChildToRemove.gameObject);
            _maze.Add(newMazeUnit);
        }
    }
}
