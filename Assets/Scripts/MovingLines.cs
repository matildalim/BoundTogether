using UnityEngine;

public class MovingLines : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform cube;
    public Transform sphere;

    public float lineThickness = 0.1f;
    public Color startColor = Color.blue;
    public Color endColor = Color.green;
    public float pulseSpeed = 1f;

    void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = lineThickness;
        lineRenderer.endWidth = lineThickness;
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;
    }

    void Update()
    {
        if (lineRenderer != null && cube != null && sphere != null)
        {
            lineRenderer.SetPosition(0, cube.position);
            lineRenderer.SetPosition(1, sphere.position);
        }

        float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        lineRenderer.startWidth = lineThickness + pulse * 0.1f;
        lineRenderer.endWidth = lineThickness + pulse * 0.1f;

        lineRenderer.startColor = Color.Lerp(startColor, endColor, pulse);
        lineRenderer.endColor = Color.Lerp(endColor, startColor, pulse);
    }
}
