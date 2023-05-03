using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using Unity.VisualScripting;
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
    private GameObject _highPolyWall1;
    [SerializeField]
    private GameObject _highPolyWall2;
    [SerializeField]
    private GameObject _highPolyPillar1;
    [SerializeField]
    private GameObject _highPolyPillar2;
    [SerializeField]
    private GameObject _highPolyPillar3;
    [SerializeField]
    private GameObject _goal;

    [Space(10)]
    [SerializeField]
    private GameObject _mainCamera;
    [SerializeField]
    private GameObject _player;

    [Space(10)]
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

    [Space(10)]
    [SerializeField]
    private GameObject _staticBatch;
    private GameObject[] _gos;
    private int _frames;

    // Start is called before the first frame update
    void Start()
    {
        _goal.SetActive(false);

        GameManager.FinishGeneratingMazeEvent += RenderHighPolyMaze;
        GameManager.FinishGeneratingMazeEvent += ActivateGoal;
        GameManager.StartMazeGameEvent += SetCameraMode;
        GameManager.StopMazeGameEvent += SetCameraMode;
        GameManager.FinishMazeGameEvent += SetCameraMode;

        _mazeWidth = GameManager.mazeWidth;
        _mazeHeight = GameManager.mazeHeight;
        _mazeUnitHeight = GameManager.mazeUnitSize;
        _mazeUnitWidth = GameManager.mazeUnitSize;

        _highPolyWall1.transform.localScale = new Vector3(GameManager.mazeUnitSize * 0.25f, GameManager.mazeUnitSize * 0.25f, GameManager.mazeUnitSize * 0.25f);
        _highPolyWall2.transform.localScale = new Vector3(GameManager.mazeUnitSize * 0.25f, GameManager.mazeUnitSize * 0.25f, GameManager.mazeUnitSize * 0.25f);
        _highPolyPillar1.transform.localScale = new Vector3(GameManager.mazeUnitSize * 0.25f, GameManager.mazeUnitSize * 0.25f, GameManager.mazeUnitSize * 0.25f);
        _highPolyPillar2.transform.localScale = new Vector3(GameManager.mazeUnitSize * 0.25f, GameManager.mazeUnitSize * 0.25f, GameManager.mazeUnitSize * 0.25f);
        _highPolyPillar3.transform.localScale = new Vector3(GameManager.mazeUnitSize * 0.25f, GameManager.mazeUnitSize * 0.25f, GameManager.mazeUnitSize * 0.25f);

        _frames = 0;

        SetCameraZoom();

        // set the size of the maze's tiles to the values set in the menu by the player
        _mazeUnitGameObject.transform.localScale = new Vector3(_mazeUnitWidth, _mazeUnitHeight, 1);
        // create the first tile of the maze at position x=0, y=0.
        // this is done so that the list of gameobjects has at least 1 value from which to grow the maze.
        GameObject mazeUnitGameObject = Instantiate(_mazeUnitGameObject, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(90, 0, 0)));
        MazeUnit mazeUnit = new MazeUnit(new Vector3(0, 0, 0), false, mazeUnitGameObject);
        // add the first created tile in the maze to the list
        _maze.Add(mazeUnit);
    }

    void ActivateGoal()
    {
        // getting the x and y of the most upper-right tile, without taking into account
        // the tilesize. this allows us to take into account if the tiles are too large 
        // to all fit in the maze
        // (so when mazeLength/mazeUnitlength is not a whole number for instance)
        int xPosition = Convert.ToInt32((_mazeWidth - _mazeUnitWidth) / _mazeUnitWidth);
        int yPosition = Convert.ToInt32((_mazeHeight - _mazeUnitHeight) / _mazeUnitHeight);
        _goal.transform.position = new Vector3(xPosition * _mazeUnitWidth, 0.5f, yPosition * _mazeUnitHeight);
        // make sure that the goal is active 
        _goal.SetActive(true);
    }

    void SetCameraZoom()
    {
        // check get the width or height of the maze, depending on which one is larger
        float largestWidthOrHeight = _mazeWidth > _mazeHeight ? _mazeWidth : _mazeHeight;
        // set the camera to the center and zoom out based on the size of the maze 
        _mainCamera.transform.position = new Vector3(_mazeWidth / 2, largestWidthOrHeight + 5, _mazeHeight / 2);
    }

    void SetCameraMode()
    {
        // set the player's position to the center of the bottom left tile
        _player.transform.position = new Vector3(0, 0.5f, 0);
        // toggle the player's active flag
        _player.SetActive(!_player.activeSelf);
        // toggle the camera's active flag
        _mainCamera.SetActive(!_mainCamera.activeSelf);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.finishedRenderingMaze)
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
                // get a random value to extend the corridor to 
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
        } 
        else
        {
            if(_maze.Count > 1)
            {
                GameManager.OnFinishGeneratingMaze();
            }
        }
    }



    private void RenderHighPolyMaze()
    {
        List<GameObject> highpolymaze = new List<GameObject>();
        // go through all the maze tiles in the maze
        foreach (MazeUnit mazeUnit in _maze)
        {
            // if there is a maze tile with walls
            if(mazeUnit != null && mazeUnit.mazeUnitGameObject.transform.childCount > 0)
            {
                // go through all the walls of the selected maze tile
                for (int i = 0; i < mazeUnit.mazeUnitGameObject.transform.childCount; i++)
                {
                    // get the wall from the maze tile 
                    Transform wall = mazeUnit.mazeUnitGameObject.transform.GetChild(i);

                    GameObject newWall = null;
                    // get a random mesh for the new wall of the maze tile 
                    int randomNumber = (int)UnityEngine.Random.Range(0, 2);

                    switch(randomNumber)
                    {
                        case 0:
                            newWall = _highPolyWall1;
                            break;
                        case 1:
                            newWall = _highPolyWall2;
                            break;
                        default:
                            //Debug.Log("Randomizer for high poly walls doesn't work");
                            throw new RandomizerNumberIsTooHighException("Random Number is to high: Random number for choosing a high-poly wall exceeds the options for them");
                            break;
                    }

                    // create a new rotation based on the orientation of the original wall  
                    Vector3 rotation = new Vector3(0, 0, 0);
                    // make sure that vertical facing walls are positioned that way
                    if (wall.name == "LeftWall" || wall.name == "RightWall")
                    {
                        rotation = new Vector3(0, 90, 0);
                    }
                    // instantiate the new wall 
                    Instantiate(newWall, wall.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(rotation));
                    // remove the old wall
                    Destroy(wall.gameObject);

                    // add the new wall to a list that will be put through static batching later on
                    highpolymaze.Add(newWall);
                }
                // destroy the old maze tile
                Destroy(mazeUnit.mazeUnitGameObject);
            }
        }
        
        // set the maximum number of pillars
        int xMax = Convert.ToInt32(_mazeWidth / _mazeUnitWidth + 1f);
        int zMax = Convert.ToInt32(_mazeHeight / _mazeUnitHeight + 1f);

        GameObject newPillar = null;

        // generate the pillars for the maze 
        for (int x = 0; x < xMax; x++)
        {
            for (int z = 0; z < zMax; z++)
            {
                // pick a random value for the pillar
                int randomNumber = (int)UnityEngine.Random.Range(0, 3);
                
                switch(randomNumber)
                {
                    case 0:
                        newPillar = _highPolyPillar1;
                        break;
                    case 1:
                        newPillar = _highPolyPillar2;
                        break;
                    case 2:
                        newPillar = _highPolyPillar3;
                        break;
                    default:
                        throw new RandomizerNumberIsTooHighException("Random Number is to high: Random number for choosing a high-poly pillar exceeds the options for them");
                        break;
                }

                // create the pillar
                Instantiate(newPillar, new Vector3((x - 0.5f) * _mazeUnitWidth, 0.375f, (z - 0.5f) * _mazeUnitHeight), Quaternion.Euler(new Vector3(90, 0, 0)));

                // put it on a list for static batching later on 
                highpolymaze.Add(newPillar);
            }
        }

        // statically batch the new assets
        GameObject[] highpolygos = highpolymaze.ToArray<GameObject>();
        if (highpolygos != null)
        {
            UnityEngine.StaticBatchingUtility.Combine(highpolygos, _staticBatch);
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
        if (_mazeHeight < 5f)
        {
            _mazeHeight = 5f;
        }
        if (_mazeWidth < 5f)
        {
            _mazeWidth = 5f;
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
        if (Convert.ToInt32(lastMazeUnitPosition.x + 2 * _mazeUnitWidth) <= Convert.ToInt32(_mazeWidth) && !hasNeighborRight)
        {
            positionValues.Add(Direction.right);
        }
        if (lastMazeUnitPosition.x > 0f && !hasNeighborLeft)
        {
            positionValues.Add(Direction.left);
        }
        if (Convert.ToInt32(lastMazeUnitPosition.z + 2 * _mazeUnitHeight) <= Convert.ToInt32(_mazeHeight) && !hasNeighborUp)
        {
            positionValues.Add(Direction.up);
        }
        if (lastMazeUnitPosition.z > 0f && !hasNeighborDown)
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
                    throw new CantInstantiateObjectException("Failed to instantiate GameObject: The new maze unit gameobject had the wrong parameters for instantiation.");
                }
            }
            else
            {
                throw new CantDestroyObjectException("Failed to destroy GameObject: Cannot find a wall to remove in the last maze tile.");
            }
        }
    }
}

public class RandomizerNumberIsTooHighException : Exception
{
    public RandomizerNumberIsTooHighException() : base() { }
    public RandomizerNumberIsTooHighException(string message) : base(message) { }
    public RandomizerNumberIsTooHighException(string message, Exception inner) : base(message, inner) { }
}

public class CantInstantiateObjectException : Exception
{
    public CantInstantiateObjectException() : base() { }
    public CantInstantiateObjectException(string message) : base(message) { }
    public CantInstantiateObjectException(string message, Exception inner) : base(message, inner) { }
}

public class CantDestroyObjectException : Exception
{
    public CantDestroyObjectException() : base() { }
    public CantDestroyObjectException(string message) : base(message) { }
    public CantDestroyObjectException(string message, Exception inner) : base(message, inner) { }
}