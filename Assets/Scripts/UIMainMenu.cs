using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : UIBaseBehavior
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


    private void Update()
    {
        SliderBehavior(_mazeHeightInput, _mazeWidthInput, _mazeUnitSizeInput,
            _mazeHeightSlider, _mazeWidthSlider, _mazeUnitSizeSlider);
    }

    public void OnClickStartButton()
    {
        GameManager.LoadGame();
    }
}
