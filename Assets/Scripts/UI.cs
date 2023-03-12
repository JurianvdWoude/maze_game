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

    public void Start()
    {
        _finishedText.SetActive(false);
    }
    // Update is called once per frame
    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    private void Update()
    {
        if (GameManager.finishedGeneratingMaze)
        {
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
