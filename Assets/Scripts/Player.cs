using System.Collections;
using System.Collections.Generic;
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
        Vector3 newMousePosition = Input.mousePosition;
        Vector3 mousePositionDifference = newMousePosition - mousePosition;
        mousePosition = newMousePosition;

        Vector3 rotationalChange = gameObject.transform.rotation.eulerAngles + new Vector3(0, mousePositionDifference.x * playerRotationSpeed, 0); ;
        gameObject.transform.rotation = Quaternion.Euler(rotationalChange);

        Vector3 playerMovement = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");
        controller.Move(playerMovement * Time.deltaTime * playerSpeed);

    }
}
