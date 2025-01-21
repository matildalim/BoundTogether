using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public Transform cube;
    public Transform sphere;

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 5, -3);
    public float smoothSpeed = 0.15f; // Lower values = smoother but slower, higher values = more responsive
    public float forwardOffset = -1f; // How far behind the characters the camera should be (smaller absolute value = closer)

    private Camera mainCamera;
    private Vector3 velocity = Vector3.zero;

    void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera.orthographic)
        {
            mainCamera.orthographic = false;
        }
    }

    void LateUpdate() // Using LateUpdate for smoother camera following
    {
        if (cube == null || sphere == null) return;

        // Calculate midpoint between characters
        Vector3 midpoint = (cube.position + sphere.position) / 2f;

        // Calculate the average forward movement of both characters
        float averageZPosition = midpoint.z;

        // Create desired position with adjusted forward offset
        Vector3 desiredPosition = new Vector3(
            midpoint.x + offset.x,
            midpoint.y + offset.y,
            averageZPosition + forwardOffset // Keep camera closer to characters
        );

        // Smoothly move camera to desired position
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            smoothSpeed
        );

        // Look at the midpoint slightly ahead of the characters
        Vector3 lookAtPoint = new Vector3(
            midpoint.x,
            midpoint.y,
            midpoint.z + 2f // Look slightly ahead
        );
        transform.LookAt(lookAtPoint);
    }

    // Optional: Visualize camera target in Scene view
    void OnDrawGizmos()
    {
        if (cube == null || sphere == null) return;

        Vector3 midpoint = (cube.position + sphere.position) / 2f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(midpoint, 0.5f);
    }
}