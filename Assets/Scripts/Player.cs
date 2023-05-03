using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 mousePosition;
    private float playerSpeed = 2.0f;
    private float playerRotationSpeed = 0.25f;

    [SerializeField]
    private GameObject playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.AddComponent<CharacterController>();
        playerSpeed = 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // get the mouse position
        Vector3 newMousePosition = Input.mousePosition;
        // check in what direction the position of the mouse has changed
        Vector3 mousePositionDifference = newMousePosition - mousePosition;
        mousePosition = newMousePosition;

        Cursor.lockState = CursorLockMode.Locked;

        // get the change in the mouse's position in the x-axis and add it to the player's rotation
        Vector3 rotationalChange = gameObject.transform.rotation.eulerAngles + new Vector3(0, mousePositionDifference.x * playerRotationSpeed, 0); ;
        // set the player's rotation to this changed rotation
        gameObject.transform.rotation = Quaternion.Euler(rotationalChange);

        // set the player's movement based on the vertical input and horizontal input values
        Vector3 playerMovement = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");
        // move the player with the player controller
        // deltaTime is needed to account for any changes in the time between frames 
        controller.Move(playerMovement * Time.deltaTime * playerSpeed);

    }
}
