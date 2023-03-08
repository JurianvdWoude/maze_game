using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MazeUnit
{
    public Vector3 mazePosition { get; set; }
    public bool traversed { get; set; }
    public GameObject mazeUnitGameObject { get; set; }
    public MazeUnit(Vector3 mazePosition, bool traversed, GameObject mazeUnitGameObject)
    {
        this.mazePosition = mazePosition;
        this.traversed = traversed;
        this.mazeUnitGameObject = mazeUnitGameObject;
    }
}

public class Maze : MonoBehaviour
{
    [SerializeField]
    private List<MazeUnit> _maze = new List<MazeUnit>();
    [SerializeField]
    private GameObject _mazeUnitGameObject;
    // Start is called before the first frame update
    void Start()
    {
        GameObject mazeUnitGameObject = Instantiate(_mazeUnitGameObject, new Vector3(0, 0, 0), Quaternion.identity);
        MazeUnit mazeUnit = new MazeUnit(new Vector3(0,0,0), false, _mazeUnitGameObject);
        _maze.Add(mazeUnit);
        // Transform childToRemove = _maze[0].transform.Find("LeftWall");
        // childToRemove.parent = null;
        // Destroy(childToRemove.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        MazeUnit lastMazeUnit = _maze.LastOrDefault();
        if (lastMazeUnit != null)
        {
            int lastMazeUnitIndex = _maze.LastIndexOf(lastMazeUnit);
            Vector3 lastMazeUnitPosition = lastMazeUnit.mazeUnitGameObject.transform.position;
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                Transform lastMazeUnitTransformChildToRemove = lastMazeUnit.mazeUnitGameObject.transform.Find("LeftWall");
                if (lastMazeUnitTransformChildToRemove != null)
                {
                    lastMazeUnitTransformChildToRemove.parent = null;
                    Destroy(lastMazeUnitTransformChildToRemove.gameObject);
                    GameObject newMazeUnitGameObject = Instantiate(_mazeUnitGameObject, lastMazeUnitPosition + new Vector3(-1, 0, 0), Quaternion.identity);
                    if (newMazeUnitGameObject != null)
                    {
                        Transform newMazeUnitGameObjectChildToRemove = newMazeUnitGameObject.transform.Find("RightWall");
                        newMazeUnitGameObjectChildToRemove.parent = null;
                        Destroy(newMazeUnitGameObjectChildToRemove.gameObject);
                        MazeUnit newMazeUnit = new MazeUnit(newMazeUnitGameObject.transform.position, false, newMazeUnitGameObject);
                        _maze.Add(newMazeUnit);
                    } else
                    {
                        Debug.Log("Could not instantiate newGameObject");
                    }
                } else
                {
                    Debug.Log("LeftWall doesn't exist");
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                Transform lastMazeUnitTransformChildToRemove = lastMazeUnit.mazeUnitGameObject.transform.Find("TopWall");
                if (lastMazeUnitTransformChildToRemove != null)
                {
                    lastMazeUnitTransformChildToRemove.parent = null;
                    Destroy(lastMazeUnitTransformChildToRemove.gameObject);
                    GameObject newMazeUnitGameObject = Instantiate(_mazeUnitGameObject, lastMazeUnitPosition + new Vector3(0, 1, 0), Quaternion.identity);
                    if (newMazeUnitGameObject != null)
                    {
                        Transform newMazeUnitGameObjectChildToRemove = newMazeUnitGameObject.transform.Find("BottomWall");
                        newMazeUnitGameObjectChildToRemove.parent = null;
                        Destroy(newMazeUnitGameObjectChildToRemove.gameObject);
                        MazeUnit newMazeUnit = new MazeUnit(newMazeUnitGameObject.transform.position, false, newMazeUnitGameObject);
                        _maze.Add(newMazeUnit);
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                Transform lastMazeUnitTransformChildToRemove = lastMazeUnit.mazeUnitGameObject.transform.Find("RightWall");
                if (lastMazeUnitTransformChildToRemove != null)
                {
                    lastMazeUnitTransformChildToRemove.parent = null;
                    Destroy(lastMazeUnitTransformChildToRemove.gameObject);
                    GameObject newMazeUnitGameObject = Instantiate(_mazeUnitGameObject, lastMazeUnitPosition + new Vector3(1, 0, 0), Quaternion.identity);
                    if (newMazeUnitGameObject != null)
                    {
                        Transform newMazeUnitGameObjectChildToRemove = newMazeUnitGameObject.transform.Find("LeftWall");
                        newMazeUnitGameObjectChildToRemove.parent = null;
                        Destroy(newMazeUnitGameObjectChildToRemove.gameObject);
                        MazeUnit newMazeUnit = new MazeUnit(newMazeUnitGameObject.transform.position, false, newMazeUnitGameObject);
                        _maze.Add(newMazeUnit);
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                Transform lastMazeUnitTransformChildToRemove = lastMazeUnit.mazeUnitGameObject.transform.Find("BottomWall");
                if (lastMazeUnitTransformChildToRemove != null)
                {
                    lastMazeUnitTransformChildToRemove.parent = null;
                    Destroy(lastMazeUnitTransformChildToRemove.gameObject);
                    GameObject newMazeUnitGameObject = Instantiate(_mazeUnitGameObject, lastMazeUnitPosition + new Vector3(0, -1, 0), Quaternion.identity);
                    if (newMazeUnitGameObject != null)
                    {
                        Transform newMazeUnitGameObjectChildToRemove = newMazeUnitGameObject.transform.Find("TopWall");
                        newMazeUnitGameObjectChildToRemove.parent = null;
                        Destroy(newMazeUnitGameObjectChildToRemove.gameObject);
                        MazeUnit newMazeUnit = new MazeUnit(newMazeUnitGameObject.transform.position, false, newMazeUnitGameObject);
                        _maze.Add(newMazeUnit);
                    }
                }
            }
        } else
        {
            Debug.Log("Maze list is empty");
        }
    }
}
