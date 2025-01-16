using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    private PlayerControls controls;
    private Vector2 moveInput;

    public float moveSpeed = 5f; // Speed of lateral movement
    public float forwardSpeed = 10f; // Constant forward movement speed
    public float smoothTime = 0.1f; // Time for smoothing input
    public Camera mainCamera; // Reference to the camera

    private Vector3 velocity = Vector3.zero; // For SmoothDamp

    public Transform sphere; // Reference to the sphere

    public float minDistance = 2f; // Minimum distance to maintain between cube and sphere
    public float maxDistance = 5f; // Maximum distance to maintain between cube and sphere

    void Awake()
    {
        controls = new PlayerControls();

        controls.Cube.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Cube.Move.canceled += ctx => moveInput = Vector2.zero;

        mainCamera = Camera.main; // Get the main camera
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        // Constant forward movement
        Vector3 forwardMove = Vector3.forward * forwardSpeed * Time.deltaTime;

        // Smooth the lateral movement based on input (side-to-side movement)
        Vector3 targetPosition = new Vector3(moveInput.x * moveSpeed, moveInput.y * moveSpeed, 0f);
        Vector3 smoothedMovement = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Apply both forward movement and smoothed lateral movement
        transform.position = new Vector3(smoothedMovement.x, smoothedMovement.y, transform.position.z) + forwardMove;

        // Ensure cube and sphere maintain a minimum distance apart
        AdjustPositionForProximity();

        // Keep the cube within the screen bounds vertically
        Vector3 position = transform.position;
        position.y = Mathf.Clamp(position.y, -mainCamera.orthographicSize, mainCamera.orthographicSize);
        transform.position = position;
    }

    void AdjustPositionForProximity()
    {
        // Calculate the distance between the cube and sphere
        float currentDistance = Vector3.Distance(transform.position, sphere.position);

        // If the distance is greater than the maximum distance, move the cube towards the sphere
        if (currentDistance > maxDistance)
        {
            Vector3 direction = (sphere.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, sphere.position, moveSpeed * Time.deltaTime);
        }
        // If the distance is less than the minimum distance, move the cube away from the sphere
        else if (currentDistance < minDistance)
        {
            Vector3 direction = (transform.position - sphere.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, sphere.position, -moveSpeed * Time.deltaTime);
        }
    }
}
