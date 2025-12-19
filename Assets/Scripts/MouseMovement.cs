using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;

public class MouseMovement : MonoBehaviour
{
    public float sensX;
    public float sensY;

    // Set clamping
    public float topClamp = -90f;
    public float bottomClamp = 90f;

    public Transform playerBody;

    float xRotation = 0f;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * sensX * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;

        // Clamp rotation
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

        // Rotate camera and orientation
        playerBody.Rotate(Vector3.up * mouseX);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
