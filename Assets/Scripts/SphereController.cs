using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereController : MonoBehaviour
{
    private PlayerControls controls;
    private Vector2 moveInput;

    public float moveSpeed = 5f; // Speed of lateral and vertical movement
    public float forwardSpeed = 10f; // Base forward movement speed
    public float forwardSpeedVariation = 1f; // Variation in forward speed
    public float smoothTime = 0.1f; // Time for smoothing input
    public Camera mainCamera; // Reference to the camera

    private Vector3 velocity = Vector3.zero; // For SmoothDamp

    public Transform cube; // Reference to the cube

    private float currentForwardSpeed; // Variable forward speed for the sphere
    private bool isMoving = false; // To track if controls are pressed

    void Awake()
    {
        controls = new PlayerControls();

        controls.Sphere.Move.performed += ctx => {
            moveInput = ctx.ReadValue<Vector2>();
            isMoving = true;
        };
        controls.Sphere.Move.canceled += ctx => {
            moveInput = Vector2.zero;
            isMoving = false;
        };

        mainCamera = Camera.main; // Get the main camera
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        // Log to confirm positions are set correctly
        Debug.Log("Sphere position: " + transform.position);
        Debug.Log("Cube position: " + cube.position);

        // Start the sphere and cube at separate positions
        transform.position = new Vector3(-5f, 0, 0);  // Sphere starts to the left
        cube.position = new Vector3(5f, 0, 0);        // Cube starts to the right

        // Apply a slight variation in the forward speed
        currentForwardSpeed = forwardSpeed + Random.Range(-forwardSpeedVariation, forwardSpeedVariation);
    }

    void Update()
    {
        // This will only handle input and forward speed adjustment based on the cube's position
        AdjustForwardSpeed();
    }

    void LateUpdate()
    {
        // Smooth lateral and vertical movement based on input
        Vector3 inputMovement = new Vector3(moveInput.x * moveSpeed, moveInput.y * moveSpeed, 0f);
        Vector3 smoothedMovement = Vector3.SmoothDamp(transform.position, transform.position + inputMovement, ref velocity, smoothTime);

        // If no input, move them apart
        if (!isMoving)
        {
            Vector3 directionToMove = (transform.position.x < cube.position.x) ? Vector3.right : Vector3.left;
            smoothedMovement += directionToMove * moveSpeed * Time.deltaTime;
        }

        // Apply movement
        transform.position = new Vector3(smoothedMovement.x, smoothedMovement.y, transform.position.z);

        // Apply forward movement
        transform.position += Vector3.forward * currentForwardSpeed * Time.deltaTime;

        //// Keep the sphere within the screen bounds vertically
        //Vector3 position = transform.position;
        //position.y = Mathf.Clamp(position.y, -mainCamera.orthographicSize, mainCamera.orthographicSize);
        //transform.position = position;
    }

    void AdjustForwardSpeed()
    {
        // Calculate the distance to the cube
        float distance = Vector3.Distance(transform.position, cube.position);

        // Adjust forward speed based on the distance
        if (distance > 4f) // Too far apart
        {
            currentForwardSpeed += 0.1f; // Speed up
        }
        else if (distance < 2f) // Too close
        {
            currentForwardSpeed -= 0.1f; // Slow down
        }

        // Clamp the forward speed to avoid extreme values
        currentForwardSpeed = Mathf.Clamp(currentForwardSpeed, forwardSpeed - forwardSpeedVariation, forwardSpeed + forwardSpeedVariation);
    }
}
