using UnityEngine;
using UnityEngine.UI;

public class DirectionalUI : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public RectTransform cubeIndicator;  // UI element for the cube indicator
    public RectTransform sphereIndicator;  // UI element for the sphere indicator
    public Transform cube;  // Reference to the cube
    public Transform sphere;  // Reference to the sphere

    [Header("Settings")]
    public float offScreenThreshold = 0.1f;  // The minimum distance from the screen edge to show the indicator
    public Vector2 indicatorOffset = new Vector2(50, 50);  // Offset for positioning the indicator around the edge

    private void Update()
    {
        UpdateIndicator(cube, cubeIndicator);
        UpdateIndicator(sphere, sphereIndicator);
    }

    private void UpdateIndicator(Transform obj, RectTransform indicator)
    {
        Vector3 screenPos = mainCamera.WorldToViewportPoint(obj.position);

        // Check if the object is off-screen
        if (screenPos.x < -offScreenThreshold || screenPos.x > 1 + offScreenThreshold || screenPos.y < -offScreenThreshold || screenPos.y > 1 + offScreenThreshold)
        {
            // Calculate direction to the object in screen space
            Vector3 directionToObject = obj.position - mainCamera.transform.position;

            // Project this direction to screen space
            Vector3 screenDir = mainCamera.WorldToScreenPoint(obj.position);

            // Normalize direction and map it to the edges of the screen
            Vector3 normalizedDir = new Vector3(
                Mathf.Clamp(screenDir.x / Screen.width, 0, 1),
                Mathf.Clamp(screenDir.y / Screen.height, 0, 1),
                0
            );

            // Calculate the position of the indicator on the screen based on direction
            Vector2 screenPosition = new Vector2(
                Mathf.Clamp(screenPos.x, offScreenThreshold, 1 - offScreenThreshold) * Screen.width,
                Mathf.Clamp(screenPos.y, offScreenThreshold, 1 - offScreenThreshold) * Screen.height
            );

            indicator.anchoredPosition = new Vector2(
                screenPosition.x - Screen.width / 2f,  // Offset for the screen center
                screenPosition.y - Screen.height / 2f  // Offset for the screen center
            );

            // Rotate the indicator to face the direction of the object off-screen
            float angle = Mathf.Atan2(directionToObject.y, directionToObject.x) * Mathf.Rad2Deg;
            indicator.rotation = Quaternion.Euler(0f, 0f, angle);

            indicator.gameObject.SetActive(true);  // Show the indicator
        }
        else
        {
            indicator.gameObject.SetActive(false);  // Hide the indicator if the object is on-screen
        }
    }
}
