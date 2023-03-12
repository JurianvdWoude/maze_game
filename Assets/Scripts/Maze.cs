using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Profiling;
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
    [SerializeField]
    private GameObject _mainCamera;
    [SerializeField]
    private float _mazeWidth;
    [SerializeField]
    private float _mazeHeight;
    [SerializeField]
    private float _mazeUnitHeight;
    [SerializeField]
    private float _mazeUnitWidth;
    [SerializeField]
    private int _startingXCoordinate = 0;
    [SerializeField]
    private int _startingYCoordinate = 0;
    [SerializeField]
    private GameObject _staticBatch;
    private GameObject[] _gos;
    private int _frames;

    // Start is called before the first frame update
    void Start()
    {
        // temporarily add the maze values here
        _mazeWidth = GameManager.mazeWidth;
        _mazeHeight = GameManager.mazeHeight;
        _mazeUnitHeight = GameManager.mazeUnitSize;
        _mazeUnitWidth = GameManager.mazeUnitSize;
        _frames = 0;

        // adjust the camera
        float largestWidthOrHeight = _mazeWidth > _mazeHeight ? _mazeWidth : _mazeHeight;
        _mainCamera.transform.position = new Vector3(_mazeWidth / 2, _mazeHeight / 2, -1 * largestWidthOrHeight - 5);

        _mazeUnitGameObject.transform.localScale = new Vector3(_mazeUnitWidth, _mazeUnitHeight, 1);
        GameObject mazeUnitGameObject = Instantiate(_mazeUnitGameObject, new Vector3(0, 0, 0), Quaternion.identity);
        MazeUnit mazeUnit = new MazeUnit(new Vector3(0,0,0), false, mazeUnitGameObject);
        _maze.Add(mazeUnit);
    }

    // Update is called once per frame
    void Update()
    {
        // static batching the maze to reduce CPU usage when rendering
        _frames++;
        if (_frames % 60 == 0)
        {
            _gos = _maze.ConvertAll<GameObject>(unit => unit.mazeUnitGameObject).ToArray<GameObject>();
            if (_gos != null)
            {
                UnityEngine.StaticBatchingUtility.Combine(_gos, _staticBatch);
            }
        }
        
        MazeUnit lastMazeUnit = _maze.LastOrDefault(unit => unit.traversed == false);
        if (lastMazeUnit != null)
        {
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
        } else
        {
            GameManager.finishedGeneratingMaze = true;
            //return;
        }
    }
    
    private List<Direction> _CheckIfBordering(Vector3 lastMazeUnitPosition)
    {
        List<Direction> positionValues = new List<Direction>();

        bool hasNeighborLeft = _maze.Any(unit => unit.mazePosition == (lastMazeUnitPosition + new Vector3(-1 * _mazeUnitWidth, 0, 0)));
        bool hasNeighborRight = _maze.Any(unit => unit.mazePosition == (lastMazeUnitPosition + new Vector3(_mazeUnitWidth, 0, 0)));
        bool hasNeighborUp = _maze.Any(unit => unit.mazePosition == (lastMazeUnitPosition + new Vector3(0, _mazeUnitHeight, 0)));
        bool hasNeighborDown = _maze.Any(unit => unit.mazePosition == (lastMazeUnitPosition + new Vector3(0, -1 * _mazeUnitHeight, 0)));

        if (_mazeHeight < 5)
        {
            _mazeHeight = 5;
        }
        if (_mazeWidth < 5)
        {
            _mazeWidth = 5;
        }

        if (lastMazeUnitPosition.x < _mazeWidth && !hasNeighborRight)
        {
            positionValues.Add(Direction.right);
        }
        if (lastMazeUnitPosition.x > 0 && !hasNeighborLeft)
        {
            positionValues.Add(Direction.left);
        }
        if (lastMazeUnitPosition.y < _mazeHeight && !hasNeighborUp)
        {
            positionValues.Add(Direction.up);
        }
        if (lastMazeUnitPosition.y > 0 && !hasNeighborDown)
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
                    directionToMoveTo = new Vector3(0, _mazeUnitHeight, 0);
                    oldMazeUnitWallName = "TopWall";
                    newMazeUnitWallName = "BottomWall";
                    break;
                case Direction.down:
                    directionToMoveTo = new Vector3(0, -1 * _mazeUnitHeight, 0);
                    oldMazeUnitWallName = "BottomWall";
                    newMazeUnitWallName = "TopWall";
                    break;
                case Direction.left:
                    directionToMoveTo = new Vector3(-1 * _mazeUnitWidth, 0, 0);
                    oldMazeUnitWallName = "LeftWall";
                    newMazeUnitWallName = "RightWall";
                    break;
                case Direction.right:
                    directionToMoveTo = new Vector3(_mazeUnitWidth, 0, 0);
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
