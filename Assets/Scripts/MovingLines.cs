using UnityEngine;


public class MovingLines : MonoBehaviour
{
    public LineRenderer lineRenderer;  // LineRenderer component
    public Transform cube;             // Reference to Cube
    public Transform sphere;           // Reference to Sphere

    public float lineThickness = 0.1f; // Line thickness

    void Start()
    {
        // Set the LineRenderer properties
        lineRenderer.positionCount = 2;  // Only two points (Cube and Sphere)
        lineRenderer.startWidth = lineThickness;
        lineRenderer.endWidth = lineThickness;
        lineRenderer.startColor = Color.blue;  // Start color of the line
        lineRenderer.endColor = Color.green;   // End color of the line
    }

    void Update()
    {
        // Update the positions of the line based on Cube and Sphere positions
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, cube.position); // Start point: Cube position
            lineRenderer.SetPosition(1, sphere.position); // End point: Sphere position
        }

        // Optional: Add pulse or animation to the line's width or color
        float pulse = Mathf.PingPong(Time.time, 1f);
        lineRenderer.startWidth = lineThickness + pulse * 0.1f;
        lineRenderer.endWidth = lineThickness + pulse * 0.1f;
    }
}
