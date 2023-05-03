using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIBaseBehavior : MonoBehaviour
{
    protected void SliderBehavior
    (
        GameObject _mazeHeightInput,
        GameObject _mazeWidthInput,
        GameObject _mazeUnitSizeInput,
        GameObject _mazeHeightSlider,
        GameObject _mazeWidthSlider,
        GameObject _mazeUnitSizeSlider
    )
    {
        // Get the Input Field for the size of the maze's tiles
        TMPro.TMP_InputField mazeUnitInputField = _mazeUnitSizeInput.gameObject.GetComponent<TMPro.TMP_InputField>();
        // Get the Slider for the size of the maze's tiles
        Slider mazeUnitSliderField = _mazeUnitSizeSlider.gameObject.GetComponent<Slider>();
        // if there's input and it's a number and it is different from what's stored in the Gamemanager
        if
        (
            mazeUnitInputField.text != null &&
            int.TryParse(mazeUnitInputField.text, out _) &&
            mazeUnitInputField.text != GameManager.mazeUnitSize.ToString()
        )
        {
            // Set the stored maze tile size to the one in the Input Field
            GameManager.mazeUnitSize = float.Parse(mazeUnitInputField.text);
            // Set the Slider value equal to the value in the Input Field
            mazeUnitSliderField.value = GameManager.mazeUnitSize;
        }
        // otherwise, if the Slider's used instead of the input field
        else if (mazeUnitSliderField != null)
        {
            // Set the stored maze tile size to the one in the Slider
            GameManager.mazeUnitSize = mazeUnitSliderField.value;
            // Set the Input Field value equal to the value in the Slider
            mazeUnitInputField.text = GameManager.mazeUnitSize.ToString();
        }
        else
        {
            // If Neither the Input Field or Slider are used, set the store tile size to 1
            GameManager.mazeUnitSize = 1f;
            // Then set the Slider's value equal to 1
            mazeUnitSliderField.value = GameManager.mazeUnitSize;
            // And the Input Field equal to 1
            mazeUnitInputField.text = GameManager.mazeUnitSize.ToString();
        }

        // Similar as above, but with the Maze Width Slider and Input Field
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

        // Similar as above, but with the Maze Height Slider and Input Field
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
