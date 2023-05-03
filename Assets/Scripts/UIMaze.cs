using UnityEngine;
using UnityEngine.UI;

public class UIMaze : UIBaseBehavior
{
    [SerializeField]
    private GameObject _mazeUnitSizeInput;
    [SerializeField]
    private GameObject _mazeUnitSizeSlider;
    [SerializeField]
    private GameObject _mazeWidthInput;
    [SerializeField]
    private GameObject _mazeWidthSlider;
    [SerializeField]
    private GameObject _mazeHeightInput;
    [SerializeField]
    private GameObject _mazeHeightSlider;
    [SerializeField]
    private GameObject _finishedText;
    [SerializeField]
    private GameObject _mazeSizeInfoText;
    [SerializeField]
    private GameObject _mazeUnitSizeInfoText;
    [SerializeField]
    private GameObject _playButton;


    public void Start()
    {
        GameManager.FinishGeneratingMazeEvent += ActivatePlayButtonAndInfoBox;

        GameManager.StartMazeGameEvent += ToggleUI;
        GameManager.StartMazeGameEvent += DisableCursor;
        GameManager.StopMazeGameEvent += ToggleUI;
        // when you exit from playing the game, you don't want there to be a message that you've won
        GameManager.StopMazeGameEvent += ShowRenderedMessage;
        GameManager.StopMazeGameEvent += EnableCursor;
        GameManager.FinishMazeGameEvent += ToggleUI;
        // enable the win message when reaching the goal
        GameManager.FinishMazeGameEvent += ShowWinMessage;
        GameManager.FinishMazeGameEvent += EnableCursor;

        DeactivatePlayButtonAndInfoBox();
        DisplayMazeSizeAsText();
    }

    // Update is called once per frame
    private void Update()
    {
        SliderBehavior(_mazeHeightInput,_mazeWidthInput,_mazeUnitSizeInput,
            _mazeHeightSlider,_mazeWidthSlider,_mazeUnitSizeSlider);
    }

    public void OnClickRestartButton()
    {
        // reload the scene when restarting
        GameManager.LoadGame();
    }

    public void OnClickStartButton()
    {
        GameManager.OnStartMazeGame();
    }

    public void OnClickQuitButton()
    {
        GameManager.QuitGame();
    }

    private void EnableCursor()
    {
        Cursor.visible = true;
        // Cursor.lockState = CursorLockMode.Confined;
    }

    private void DisableCursor()
    {
        Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.None;
    }

    private void DeactivatePlayButtonAndInfoBox()
    {
        // disable the play button
        _playButton.SetActive(false);
        // disable the text at the top of the ui canvas
        _finishedText.SetActive(false);
    }
    private void ActivatePlayButtonAndInfoBox()
    {
        // enable the play button
        _playButton.SetActive(true);
        // enable the text at the top of the ui canvas
        _finishedText.SetActive(true);
    }

    private void DisplayMazeSizeAsText()
    {
        // get the text component from the maze size ui element
        TMPro.TMP_Text mazeSizeInfoText = _mazeSizeInfoText.gameObject.GetComponent<TMPro.TMP_Text>();
        // change the ui element's textbox to show the maze size
        mazeSizeInfoText.text = "Maze Size: " + GameManager.mazeWidth.ToString() + " x " + GameManager.mazeHeight.ToString();
        // get the text component from the maze's tile size ui element 
        TMPro.TMP_Text mazeUnitSizeInfoText = _mazeUnitSizeInfoText.gameObject.GetComponent<TMPro.TMP_Text>();
        // change the ui element's textbox to show the maze's tile size
        mazeUnitSizeInfoText.text = "Maze Tile Size: " + GameManager.mazeUnitSize.ToString() + " x " + GameManager.mazeUnitSize.ToString();
    }

    private void ShowRenderedMessage()
    {
        // change the text at the top to indicate that the maze generation is finished 
        _finishedText.GetComponent<TMPro.TMP_Text>().text = "Finished Generating the Maze!";
    }

    private void ShowWinMessage()
    {
        // change the text at the top to a win message
        _finishedText.GetComponent<TMPro.TMP_Text>().text = "You've won the game!";

    }

    private void ToggleUI()
    {
        // for each child in this object
        for (int i = 0; i < transform.childCount; i++)
        {
            // get the transform
            Transform child = transform.GetChild(i);
            // toggle the child gameobject's active state
            child.gameObject.SetActive(!child.gameObject.activeSelf);
        }
    }
}
