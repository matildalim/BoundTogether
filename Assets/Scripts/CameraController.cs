using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cube;   // Cube transform
    public Transform sphere; // Sphere transform

    public Vector3 offset = new Vector3(0, 5, -10);   // Adjusted offset (camera placed further back and higher)
    private Camera mainCamera; // Reference to the camera

    void Awake()
    {
        mainCamera = Camera.main; // Get the main camera reference
        if (mainCamera.orthographic)
        {
            mainCamera.orthographic = false; // Ensure the camera is in Perspective mode
        }
    }

    void Update()
    {
        // Find the midpoint between the cube and sphere
        Vector3 midpoint = (cube.position + sphere.position) / 2f;

        // Set the camera's position to be offset from the midpoint
        Vector3 desiredPosition = midpoint + offset;

        // Position the camera at the desired position
        transform.position = desiredPosition;

        // Make the camera look at the midpoint
        transform.LookAt(midpoint);
    }
}
