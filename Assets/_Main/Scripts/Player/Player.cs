using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // components
    CharacterController controller;

    // referecnes
    Transform cameraRef;

    // walking
    [SerializeField] float speed;

    // turning
    [SerializeField] float turnSpeed;
    float cameraRotation = 0;

    // input
    Vector2 moveInput;
    Vector2 turnInput;


    public void Init()
    {
        // get components
        controller = GetComponent<CharacterController>();

        // get references
        cameraRef = transform.Find("Camera");
    }

    void Update()
    {
        HandleInput();
        HandleMovement();
    }

    void HandleInput()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        turnInput.x = Input.GetAxis("Mouse X");
        turnInput.y = Input.GetAxis("Mouse Y");
    }

    void HandleMovement()
    {
        // movement
        controller.Move((transform.right * moveInput.x + transform.forward * moveInput.y + Vector3.down) * speed * Time.deltaTime);

        // rotation
        transform.eulerAngles += Vector3.up * turnInput.x * turnSpeed;
        cameraRef.localRotation = Quaternion.Euler(cameraRotation = Mathf.Clamp(cameraRotation -= turnInput.y * turnSpeed, -80, 80), 0, 0);
    }
}
