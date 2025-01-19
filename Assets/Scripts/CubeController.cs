using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    private PlayerControls controls;
    private Vector2 moveInput;

    public float moveSpeed = 5f; // Speed of lateral movement
    public float smoothTime = 0.1f; // Time for smoothing input
    public Camera mainCamera; // Reference to the camera

    private Vector3 velocity = Vector3.zero; // For SmoothDamp

    public Transform sphere; // Reference to the sphere

    void Awake()
    {
        controls = new PlayerControls();

        controls.Cube.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Cube.Move.canceled += ctx => moveInput = Vector2.zero;

        mainCamera = Camera.main; // Get the main camera
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        // Start the cube and sphere apart
        transform.position = new Vector3(-5f, 0, 0); // Cube starts on the left
        sphere.position = new Vector3(5f, 0, 0); // Sphere starts on the right
    }

    void Update()
    {
        // Smooth movement based on input
        Vector3 targetPosition = new Vector3(moveInput.x * moveSpeed, moveInput.y * moveSpeed, 0f);
        Vector3 smoothedMovement = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Apply the movement
        transform.position = new Vector3(smoothedMovement.x, smoothedMovement.y, transform.position.z);

        // Move cube closer to the sphere when player inputs are detected
        MoveCloserToSphere();

        // Keep the cube within the screen bounds vertically
        Vector3 position = transform.position;
        position.y = Mathf.Clamp(position.y, -mainCamera.orthographicSize, mainCamera.orthographicSize);
        transform.position = position;
    }

    void MoveCloserToSphere()
    {
        // Move towards the sphere
        Vector3 direction = (sphere.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}
