using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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

        
    }

    // Update is called once per frame
    void Update()
    {
        // for 16:9
        // check if y >= 4 => can't go up
        // check if y <= -3 => can't go down
        // check if x <= -9 ==> can't go left
        // check if x >= 9 ==> can't go right
        // check if a neighbor exists ==> can't go there 
        // otherwise: pick a random direction and grow

        MazeUnit lastMazeUnit = _maze.LastOrDefault(unit => unit.traversed == false);
        if (lastMazeUnit != null)
        {

            // Array positionValues = Enum.GetValues(typeof(Position));
            


            Vector3 lastMazeUnitPosition = lastMazeUnit.mazePosition;

            List<Direction> positionValues = _CheckIfBordering(lastMazeUnitPosition);

            if (positionValues.Count > 0)
            {
                System.Random _random = new System.Random();
                Direction randomPosition = (Direction)positionValues[_random.Next(positionValues.Count)];
                _ExtendMaze(randomPosition, lastMazeUnit);
            } else
            {
                lastMazeUnit.traversed = true;
                //MazeUnit nextToLastMazeUnit = _maze.LastOrDefault(unit => unit.traversed == false);
            }
        }
    }
    
    private List<Direction> _CheckIfBordering(Vector3 lastMazeUnitPosition)
    {
        List<Direction> positionValues = new List<Direction>();

        bool hasNeighborLeft = _maze.Any(unit => unit.mazePosition == (lastMazeUnitPosition + new Vector3(-1, 0, 0)));
        bool hasNeighborRight = _maze.Any(unit => unit.mazePosition == (lastMazeUnitPosition + new Vector3(1, 0, 0)));
        bool hasNeighborUp = _maze.Any(unit => unit.mazePosition == (lastMazeUnitPosition + new Vector3(0, 1, 0)));
        bool hasNeighborDown = _maze.Any(unit => unit.mazePosition == (lastMazeUnitPosition + new Vector3(0, -1, 0)));

        if (lastMazeUnitPosition.x < 9 && !hasNeighborRight)
        {
            positionValues.Add(Direction.right);
        }
        if (lastMazeUnitPosition.x > -9 && !hasNeighborLeft)
        {
            positionValues.Add(Direction.left);
        }
        if (lastMazeUnitPosition.y < 4 && !hasNeighborUp)
        {
            positionValues.Add(Direction.up);
        }
        if (lastMazeUnitPosition.y > -3 && !hasNeighborDown)
        {
            positionValues.Add(Direction.down);
        }
        return positionValues;
    }

    enum Direction
    {
        up,
        down,
        left,
        right,
    }

    private void _ExtendMaze(Direction direction, MazeUnit lastMazeUnit)
    {
        // get the last square of the maze
        // MazeUnit lastMazeUnit = _maze.LastOrDefault();
        if (lastMazeUnit != null)
        {
        int lastMazeUnitIndex = _maze.LastIndexOf(lastMazeUnit);
        Vector3 lastMazeUnitPosition = lastMazeUnit.mazeUnitGameObject.transform.position;

            Vector3 directionToMoveTo = new Vector3(0, 0, 0);
            string oldMazeUnitWallName = "";
            string newMazeUnitWallName = "";

            // check the direction to extend the maze to 
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

            // get the last unit in the maze to remove one of the walls
            Transform lastMazeUnitWallToRemove = lastMazeUnit.mazeUnitGameObject.transform.Find(oldMazeUnitWallName);
            if (lastMazeUnitWallToRemove != null)
            {
                // remove the wall in the maze that borders the extension
                lastMazeUnitWallToRemove.parent = null;
                Destroy(lastMazeUnitWallToRemove.gameObject);
                // create the extension of the maze 
                GameObject newMazeUnitGameObject = Instantiate(_mazeUnitGameObject, lastMazeUnitPosition + directionToMoveTo, Quaternion.identity);
                if (newMazeUnitGameObject != null)
                {
                    Transform newMazeUnitWallToRemove = newMazeUnitGameObject.transform.Find(newMazeUnitWallName);
                    // remove the wall in the extension that borders the maze 
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
