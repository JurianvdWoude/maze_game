using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// THe individual squares that make up a maze 
/// </summary>
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
        MazeUnit mazeUnit = new MazeUnit(new Vector3(0,0,0), false, mazeUnitGameObject);
        _maze.Add(mazeUnit);

        _ExtendMaze(Direction.up);
        _ExtendMaze(Direction.left);
        _ExtendMaze(Direction.up);
        _ExtendMaze(Direction.right);
        _ExtendMaze(Direction.right);
        _ExtendMaze(Direction.down);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    enum Direction
    {
        up,
        down,
        left,
        right,
    }

    private void _ExtendMaze(Direction direction)
    {
        // get the last unit of the maze
        MazeUnit lastMazeUnit = _maze.LastOrDefault();
        if (lastMazeUnit != null)
        {
        int lastMazeUnitIndex = _maze.LastIndexOf(lastMazeUnit);
        Vector3 lastMazeUnitPosition = lastMazeUnit.mazeUnitGameObject.transform.position;

            Vector3 directionToMoveTo = new Vector3(0, 0, 0);
            string oldMazeUnitWallName = "";
            string newMazeUnitWallName = "";

            // check the direction
            switch (direction)
            {
                case Direction.up:
                    directionToMoveTo = new Vector3(0, 1, 0);
                    oldMazeUnitWallName = "TopWall";
                    newMazeUnitWallName = "BottomWall";
                    break;
                case Direction.down:
                    directionToMoveTo = new Vector3(0, -1, 0);
                    oldMazeUnitWallName = "BottomWall";
                    newMazeUnitWallName = "TopWall";
                    break;
                case Direction.left:
                    directionToMoveTo = new Vector3(-1, 0, 0);
                    oldMazeUnitWallName = "LeftWall";
                    newMazeUnitWallName = "RightWall";
                    break;
                case Direction.right:
                    directionToMoveTo = new Vector3(1, 0, 0);
                    oldMazeUnitWallName = "RightWall";
                    newMazeUnitWallName = "LeftWall";
                    break;
                default:
                    Debug.Log("No direction was given");
                    break;
            }

            // get the transform of the last unit in the maze to remove one of the walls
            Transform lastMazeUnitWallToRemove = lastMazeUnit.mazeUnitGameObject.transform.Find(oldMazeUnitWallName);
            if (lastMazeUnitWallToRemove != null)
            {
                lastMazeUnitWallToRemove.parent = null;
                Destroy(lastMazeUnitWallToRemove.gameObject);
                GameObject newMazeUnitGameObject = Instantiate(_mazeUnitGameObject, lastMazeUnitPosition + directionToMoveTo, Quaternion.identity);
                if (newMazeUnitGameObject != null)
                {
                    Transform newMazeUnitWallToRemove = newMazeUnitGameObject.transform.Find(newMazeUnitWallName);
                    newMazeUnitWallToRemove.parent = null;
                    Destroy(newMazeUnitWallToRemove.gameObject);
                    MazeUnit newMazeUnit = new MazeUnit(newMazeUnitGameObject.transform.position, false, newMazeUnitGameObject);
                    _maze.Add(newMazeUnit);
                }
                else
                {
                    Debug.Log("Could not instantiate newGameObject");
                }
            }
            else
            {
                Debug.Log("LeftWall doesn't exist");
            }
        }
    }
}
