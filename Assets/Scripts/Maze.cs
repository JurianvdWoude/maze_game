using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The individual squares that make up a maze 
/// </summary>
public class MazeUnit
{
    public bool traversed { get; set; }
    public GameObject mazeUnitGameObject { get; set; }
    public Vector3 mazePosition { get; set; }
    public MazeUnit(Vector3 mazePosition, bool traversed, GameObject mazeUnitGameObject)
    {
        this.traversed = traversed;
        this.mazeUnitGameObject = mazeUnitGameObject;
        this.mazePosition = mazePosition;
    }
}

public class Maze : MonoBehaviour
{
    [SerializeField]
    private List<MazeUnit> _maze = new List<MazeUnit>();
    [SerializeField]
    private GameObject _mazeUnitGameObject;
    [SerializeField]
    private GameObject _highPolyMazeUnitGameObject;
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
    private Material _wallMaterials;
    [SerializeField]
    private GameObject _staticBatch;
    private GameObject[] _gos;
    private int _frames;

    // Start is called before the first frame update
    void Start()
    {
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
        MazeUnit mazeUnit = new MazeUnit(new Vector3(0, 0, 0), false, mazeUnitGameObject);
        _maze.Add(mazeUnit);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.finishedGeneratingMaze)
        {
            return;
        }
        // static batching the maze to reduce CPU usage when rendering
        _frames++;
        if (_frames % 120 == 0)
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
            List<Direction> positionValues = _CheckIfBordering(lastMazeUnit);

            if (positionValues.Count > 0)
            {
                System.Random _random = new System.Random();
                Direction randomPosition = (Direction)positionValues[_random.Next(positionValues.Count)];
                _ExtendMaze(randomPosition, lastMazeUnit);
            } else
            {
                lastMazeUnit.traversed = true;

                Renderer[] renderers = lastMazeUnit.mazeUnitGameObject.GetComponentsInChildren<Renderer>();
                if (renderers != null)
                {
                    foreach (Renderer renderer in renderers)
                    {
                        renderer.material = _wallMaterials;
                    }
                }
            }
        } else
        {
            GameManager.finishedGeneratingMaze = true;
        }
    }
    
    private List<Direction> _CheckIfBordering(MazeUnit lastMazeUnit)
    {
        Vector3 lastMazeUnitPosition = lastMazeUnit.mazePosition;
        List<Direction> positionValues = new List<Direction>();
        MazeUnit neighborLeft = _maze.Find(unit => unit.mazePosition == (lastMazeUnitPosition + new Vector3(-1 * _mazeUnitWidth, 0, 0)));
        MazeUnit neighborUp = _maze.Find(unit => unit.mazePosition == (lastMazeUnitPosition + new Vector3(0, _mazeUnitHeight, 0)));
        bool hasNeighborLeft = false;
        bool hasNeighborUp = false;
        if (neighborLeft != null) {
            hasNeighborLeft = true;
        }
        if (neighborUp != null)
        {
            hasNeighborUp = true;
        }

        bool hasNeighborRight = _maze.Any(unit => unit.mazePosition == (lastMazeUnitPosition + new Vector3(_mazeUnitWidth, 0, 0)));
        bool hasNeighborDown = _maze.Any(unit => unit.mazePosition == (lastMazeUnitPosition + new Vector3(0, -1 * _mazeUnitHeight, 0)));

        if (_mazeHeight < 5)
        {
            _mazeHeight = 5;
        }
        if (_mazeWidth < 5)
        {
            _mazeWidth = 5;
        }

        if(hasNeighborLeft)
        {
            Transform neightborLeftRightWall = neighborLeft.mazeUnitGameObject.transform.Find("RightWall");
            Transform lastMazeUnitLeftWall = lastMazeUnit.mazeUnitGameObject.transform.Find("LeftWall");
            if (neightborLeftRightWall != null && lastMazeUnitLeftWall != null && neightborLeftRightWall.position == lastMazeUnitLeftWall.position)
            {
                neightborLeftRightWall.parent = null;
                Destroy(neightborLeftRightWall.gameObject);
            }
        }
        if (hasNeighborUp)
        {
            Transform neightborTopBottomWall = neighborUp.mazeUnitGameObject.transform.Find("BottomWall");
            Transform lastMazeUnitTopWall = lastMazeUnit.mazeUnitGameObject.transform.Find("TopWall");
            if (neightborTopBottomWall != null && lastMazeUnitTopWall != null && neightborTopBottomWall.position == lastMazeUnitTopWall.position)
            {
                neightborTopBottomWall.parent = null;
                Destroy(neightborTopBottomWall.gameObject);
            }
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
        //MazeUnit lastMazeUnit = _maze.LastOrDefault(unit => unit.traversed == false);
        if (lastMazeUnit != null)
        {
            Vector3 lastMazeUnitPosition = lastMazeUnit.mazePosition;

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
                    if (newMazeUnitWallToRemove != null)
                    {
                        newMazeUnitWallToRemove.parent = null;
                        Destroy(newMazeUnitWallToRemove.gameObject);
                        MazeUnit newMazeUnit = new MazeUnit(newMazeUnitGameObject.transform.position,false, newMazeUnitGameObject);
                        _maze.Add(newMazeUnit);
                    }
                }
                else
                {
                    Debug.Log("Could not instantiate newGameObject");
                }
            }
            else
            {
                Debug.Log("Cannot find wall to remove in the last maze unit");
            }
        }
    }
}
