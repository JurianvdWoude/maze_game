using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _mazeUnitSizeInput;
    [SerializeField]
    private GameObject _mazeUnitSizeSlider;
    [SerializeField]
    private GameObject _mazeSizeInput;
    [SerializeField]
    private GameObject _mazeSizeSlider;

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    private void Update()
    {
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
        else if (mazeUnitSliderField != null )
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


        TMPro.TMP_InputField mazeInputField = _mazeSizeInput.gameObject.GetComponent<TMPro.TMP_InputField>();
        Slider mazeSliderField = _mazeSizeSlider.gameObject.GetComponent<Slider>();
        if
        (
            mazeInputField.text != null &&
            int.TryParse(mazeInputField.text, out _) &&
            mazeInputField.text != GameManager.mazeSize.ToString()
        )
        {
            GameManager.mazeSize = float.Parse(mazeInputField.text);
            mazeSliderField.value = GameManager.mazeSize;
        }
        else if (mazeSliderField != null)
        {
            GameManager.mazeSize = mazeSliderField.value;
            mazeInputField.text = GameManager.mazeSize.ToString();
        }

        else
        {
            GameManager.mazeUnitSize = 10f;
            mazeUnitSliderField.value = GameManager.mazeUnitSize;
            mazeUnitInputField.text = GameManager.mazeUnitSize.ToString();
        }
    }
}
