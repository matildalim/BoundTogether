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

    public float colorChangeTimer = 10f;
    private float currentTime = 0f;
    private Color currentStartColor;
    private Color currentEndColor;

    void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = lineThickness;
        lineRenderer.endWidth = lineThickness;
        currentStartColor = startColor;
        currentEndColor = endColor;
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

        currentTime += Time.deltaTime;

        if (currentTime >= colorChangeTimer)
        {
            currentTime = 0f;
            currentStartColor = new Color(Random.value, Random.value, Random.value);  // Random color
            currentEndColor = new Color(Random.value, Random.value, Random.value);
        }

        float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        lineRenderer.startWidth = lineThickness + pulse * 0.2f;
        lineRenderer.endWidth = lineThickness + pulse * 0.2f;

        lineRenderer.startColor = Color.Lerp(lineRenderer.startColor, currentStartColor, pulse);
        lineRenderer.endColor = Color.Lerp(lineRenderer.endColor, currentEndColor, pulse);
    }
}
