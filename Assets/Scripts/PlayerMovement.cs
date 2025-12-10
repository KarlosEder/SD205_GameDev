using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    public float speed = 12.0f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3.0f;

    public Transform groundCheck;
    public Transform Camera;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isGrounded;
    bool isMoving;

    private Vector3 lastPosition = new Vector3(0f,0f,0f);

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Reset default velocity
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Get inputs
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Create moving vector
        Vector3 move = Camera.transform.right * x + Camera.transform.forward * z;

        // Move player
        controller.Move(move * speed * Time.deltaTime);

        // Check if player can jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Going up
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Falling down
        velocity.y += gravity * Time.deltaTime;

        // Excecute jump
        controller.Move(velocity * Time.deltaTime);

        // Check if moving
        if (lastPosition != gameObject.transform.position && isGrounded == true)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;  
        }

        lastPosition = gameObject.transform.position;
    }
}
