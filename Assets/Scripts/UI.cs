using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
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
        gameObject.SetActive(true);
        _playButton.SetActive(false);
        _finishedText.SetActive(false);
        TMPro.TMP_Text mazeSizeInfoText = _mazeSizeInfoText.gameObject.GetComponent<TMPro.TMP_Text>();
        mazeSizeInfoText.text = "Maze Size: " + GameManager.mazeWidth.ToString() + " x " + GameManager.mazeHeight.ToString();
        TMPro.TMP_Text mazeUnitSizeInfoText = _mazeUnitSizeInfoText.gameObject.GetComponent<TMPro.TMP_Text>();
        mazeUnitSizeInfoText.text = "Corridor Tile Size: " + GameManager.mazeUnitSize.ToString() + " x " + GameManager.mazeUnitSize.ToString();
    }

    public void RestartGame()
    {
        GameManager.wonTheGameBefore = false;
        GameManager.LoadGame();
    }

    public void StartGame()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Transform childObject = transform.GetChild(i);
            childObject.gameObject.SetActive(false);
        }
        GameManager.startGame = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameManager.finishedGame)
        {
            // if the game is finished, turn on the UI again
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform childObject = transform.GetChild(i);
                childObject.gameObject.SetActive(true);
            }
            // state that you won via text
            GameManager.wonTheGameBefore = true;
            // turn on the main camera again
            GameManager.firstPersonMode = false;
            // set the play game state to false 
            GameManager.playGame = false;
            GameManager.finishedGame = false;
        } 
        if (GameManager.playGame)
        {
            _playButton.SetActive(false);
            _finishedText.SetActive(false);
            return;
        }
        else if (GameManager.finishedGeneratingMaze)
        {
            _playButton.SetActive(true);
            if (GameManager.wonTheGameBefore)
            {
                _finishedText.GetComponent<TMPro.TMP_Text>().text = "You've won the game!";
            } else
            {
                _finishedText.GetComponent<TMPro.TMP_Text>().text = "Finished Generating the Maze!";
            }
            _finishedText.SetActive(true);
        }

        TMPro.TMP_InputField mazeUnitInputField = _mazeUnitSizeInput.gameObject.GetComponent<TMPro.TMP_InputField>();
        Slider mazeUnitSliderField = _mazeUnitSizeSlider.gameObject.GetComponent<Slider>();
        if
        (
            mazeUnitInputField.text != null &&
            int.TryParse(mazeUnitInputField.text, out _) &&
            mazeUnitInputField.text != GameManager.mazeUnitSize.ToString()
        )
        {
            GameManager.mazeUnitSize = float.Parse(mazeUnitInputField.text);
            mazeUnitSliderField.value = GameManager.mazeUnitSize;
        }
        else if (mazeUnitSliderField != null)
        {
            GameManager.mazeUnitSize = mazeUnitSliderField.value;
            mazeUnitInputField.text = GameManager.mazeUnitSize.ToString();
        }
        else
        {
            GameManager.mazeUnitSize = 1f;
            mazeUnitSliderField.value = GameManager.mazeUnitSize;
            mazeUnitInputField.text = GameManager.mazeUnitSize.ToString();
        }


        TMPro.TMP_InputField mazeWidthInputField = _mazeWidthInput.gameObject.GetComponent<TMPro.TMP_InputField>();
        Slider mazeWidthSliderField = _mazeWidthSlider.gameObject.GetComponent<Slider>();
        if
        (
            mazeWidthInputField.text != null &&
            int.TryParse(mazeWidthInputField.text, out _) &&
            mazeWidthInputField.text != GameManager.mazeWidth.ToString()
        )
        {
            GameManager.mazeWidth = float.Parse(mazeWidthInputField.text);
            mazeWidthSliderField.value = GameManager.mazeWidth;
        }
        else if (mazeWidthSliderField != null)
        {
            GameManager.mazeWidth = mazeWidthSliderField.value;
            mazeWidthInputField.text = GameManager.mazeWidth.ToString();
        }

        else
        {
            GameManager.mazeWidth = 10f;
            mazeWidthSliderField.value = GameManager.mazeWidth;
            mazeWidthInputField.text = GameManager.mazeWidth.ToString();
        }


        TMPro.TMP_InputField mazeHeightInputField = _mazeHeightInput.gameObject.GetComponent<TMPro.TMP_InputField>();
        Slider mazeHeightSliderField = _mazeHeightSlider.gameObject.GetComponent<Slider>();
        if
        (
            mazeHeightInputField.text != null &&
            int.TryParse(mazeHeightInputField.text, out _) &&
            mazeHeightInputField.text != GameManager.mazeHeight.ToString()
        )
        {
            GameManager.mazeHeight = float.Parse(mazeHeightInputField.text);
            mazeHeightSliderField.value = GameManager.mazeHeight;
        }
        else if (mazeHeightSliderField != null)
        {
            GameManager.mazeHeight = mazeHeightSliderField.value;
            mazeHeightInputField.text = GameManager.mazeHeight.ToString();
        }

        else
        {
            GameManager.mazeHeight = 10f;
            mazeHeightSliderField.value = GameManager.mazeHeight;
            mazeHeightInputField.text = GameManager.mazeHeight.ToString();
        }
    }
}
