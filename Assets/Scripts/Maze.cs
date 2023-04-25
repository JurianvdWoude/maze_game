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

        // check get the width or height of the maze, depending on which one is larger
        float largestWidthOrHeight = _mazeWidth > _mazeHeight ? _mazeWidth : _mazeHeight;
        // set the camera to the center and zoom out based on the size of the maze 
        _mainCamera.transform.position = new Vector3(_mazeWidth / 2, largestWidthOrHeight + 5, _mazeHeight / 2);

        // set the size of the maze's tiles to the values set in the menu by the player
        _mazeUnitGameObject.transform.localScale = new Vector3(_mazeUnitWidth, _mazeUnitHeight, 1);
        // create the first tile of the maze at position x=0, y=0.
        // this is done so that the list of gameobjects has at least 1 value from which to grow the maze.
        GameObject mazeUnitGameObject = Instantiate(_mazeUnitGameObject, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(90, 0, 0)));
        MazeUnit mazeUnit = new MazeUnit(new Vector3(0, 0, 0), false, mazeUnitGameObject);
        // add the first created tile in the maze to the list
        _maze.Add(mazeUnit);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.finishedGeneratingMaze)
        {
            return;
        }
        // count the number of frames
        _frames++;
        // static batching the maze to reduce CPU usage when rendering every 120 frames
        if (_frames % 120 == 0)
        {
            // gather all gameobjects from the list and convert them all to an array
            _gos = _maze.ConvertAll<GameObject>(unit => unit.mazeUnitGameObject).ToArray<GameObject>();
            if (_gos != null)
            {
                UnityEngine.StaticBatchingUtility.Combine(_gos, _staticBatch);
            }
        }
        
        // get the last tile of the maze added to the list that cannot grow a new corridor
        MazeUnit lastMazeUnit = _maze.LastOrDefault(unit => unit.traversed == false);
        if (lastMazeUnit != null)
        {
            // get a list of directions in which the maze can grow
            List<Direction> positionValues = _CheckIfBordering(lastMazeUnit);
            if (positionValues.Count > 0)
            {
                System.Random _random = new System.Random();
                Direction randomPosition = (Direction)positionValues[_random.Next(positionValues.Count)];
                _ExtendMaze(randomPosition, lastMazeUnit);
            } else
            {
                // if there are no directions, then set traversed to true
                lastMazeUnit.traversed = true;

                // get the renderers for all of the walls in that traversed tile in the maze.
                // the renderer can be used to change to color as a visual indicator that it cannot grow a coridor.
                Renderer[] renderers = lastMazeUnit.mazeUnitGameObject.GetComponentsInChildren<Renderer>();
                if (renderers != null)
                {
                    foreach (Renderer renderer in renderers)
                    {
                        // change the color of the tile
                        renderer.material = _wallMaterials;
                    }
                }
            }
        } else
        {
            // if there are no tiles in the maze that can grow, then the maze has finished generating
            GameManager.finishedGeneratingMaze = true;
        }
    }
    
    private List<Direction> _CheckIfBordering(MazeUnit lastMazeUnit)
    {
        // get the position of the tile being checked
        Vector3 lastMazeUnitPosition = lastMazeUnit.mazePosition;
        // create a list of possible directions to expand the maze to from the last tile
        List<Direction> positionValues = new List<Direction>();
        // get the neighboring tile to the left of the tile being checked, if it exists
        // the MazeUnit class is needed to remove potential duplicate walls later on
        MazeUnit neighborLeft = _maze.Find(unit => unit.mazePosition == (lastMazeUnitPosition + new Vector3(-1 * _mazeUnitWidth, 0, 0)));
        // get the neighboring tile to the left of the tile being checked, if it exists
        MazeUnit neighborUp = _maze.Find(unit => unit.mazePosition == (lastMazeUnitPosition + new Vector3(0, 0, _mazeUnitHeight)));
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
        bool hasNeighborDown = _maze.Any(unit => unit.mazePosition == (lastMazeUnitPosition + new Vector3(0, 0, -1 * _mazeUnitHeight)));

        // set a minimum width and height for the maze 
        if (_mazeHeight < 5)
        {
            _mazeHeight = 5;
        }
        if (_mazeWidth < 5)
        {
            _mazeWidth = 5;
        }

        // check if there is a tile to the left of the tile being checked
        if(hasNeighborLeft)
        {
            Transform rightWallOfNeighborLeft = neighborLeft.mazeUnitGameObject.transform.Find("RightWall");
            Transform leftWallOfLastMazeUnit = lastMazeUnit.mazeUnitGameObject.transform.Find("LeftWall");
            // check if there are overlapping walls between the tile being checked and its neighbor on the left
            if (rightWallOfNeighborLeft != null && leftWallOfLastMazeUnit != null && rightWallOfNeighborLeft.position == leftWallOfLastMazeUnit.position)
            {
                // if they overlap, then delete the wall from the neighbor
                rightWallOfNeighborLeft.parent = null;
                Destroy(rightWallOfNeighborLeft.gameObject);
            }
        }
        // check if there is a tile above the tile being checked
        if (hasNeighborUp)
        {
            Transform bottomWallOfNeighborUp = neighborUp.mazeUnitGameObject.transform.Find("BottomWall");
            Transform topWallOfLastMazeUnit = lastMazeUnit.mazeUnitGameObject.transform.Find("TopWall");
            // check if there are overlapping walls between the tile being checked and its neighbor above it
            if (bottomWallOfNeighborUp != null && topWallOfLastMazeUnit != null && bottomWallOfNeighborUp.position == topWallOfLastMazeUnit.position)
            {
                // if they overlap, then delete the wall from the neighbor
                bottomWallOfNeighborUp.parent = null;
                Destroy(bottomWallOfNeighborUp.gameObject);
            }
        }

        // if the tile being checked doesn't have a neighbor or is near the maze's border
        // in a particular direction, then put that direction as an enum value in a list 
        if (lastMazeUnitPosition.x < _mazeWidth && !hasNeighborRight)
        {
            positionValues.Add(Direction.right);
        }
        if (lastMazeUnitPosition.x > 0 && !hasNeighborLeft)
        {
            positionValues.Add(Direction.left);
        }
        if (lastMazeUnitPosition.z < _mazeHeight && !hasNeighborUp)
        {
            positionValues.Add(Direction.up);
        }
        if (lastMazeUnitPosition.z > 0 && !hasNeighborDown)
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

            // check the direction to extend the maze to from the maze tile being checked  
            // the WallNames will be used to remove walls between the created tile and the corridor
            switch (direction)
            {
                case Direction.up:
                    directionToMoveTo = new Vector3(0, 0, _mazeUnitHeight);
                    oldMazeUnitWallName = "TopWall";
                    newMazeUnitWallName = "BottomWall";
                    break;
                case Direction.down:
                    directionToMoveTo = new Vector3(0, 0, -1 * _mazeUnitHeight);
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
                GameObject newMazeUnitGameObject = Instantiate(_mazeUnitGameObject, lastMazeUnitPosition + directionToMoveTo, Quaternion.Euler(new Vector3(90, 0, 0)));
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
