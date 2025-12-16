using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouseMovement : MonoBehaviour
{

    public float mouseSensitivity = 750f;

    float xRotation = 0f;
    float yRotation = 0f;

    public float topClamp = -90f;
    public float bottomClamp = 90f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Mouse inputs
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // X axis rotation 
        xRotation -= mouseY;

        // Clamp rotation
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

        // Y axis rotation 
        yRotation += mouseX;

        // Apply rotations to transform
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
