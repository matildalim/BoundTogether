using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalIndicator : MonoBehaviour
{
    public Camera mainCamera;
    public RectTransform cubeIndicator;
    public RectTransform sphereIndicator;
    public Transform cube;
    public Transform sphere;

    private void Update()
    {
        UpdateIndicator(cube, cubeIndicator);
    }

    private void UpdateIndicator(Transform target, RectTransform indicator)
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

        if (screenPos.z > 0 && (screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height))
        {
            indicator.gameObject.SetActive(true);

            Vector3 direction = (target.position - mainCamera.transform.position).normalized; // calculate direction vector and normalize it
            Vector3 indicatorPosition = mainCamera.transform.position + direction * 10;

            Vector3 clampedScreenPos = screenPos;
            clampedScreenPos.x = Mathf.Clamp(screenPos.x, 0, Screen.width);
            clampedScreenPos.y = Mathf.Clamp(screenPos.y, 0, Screen.height); // convert direction to screen coordinates

            indicator.position = clampedScreenPos;        

            //rotate indicator to point at the target 
            //<empty>//
        }
        else
        {
            indicator.gameObject.SetActive(false); // hide indicator if target on screen
        }
    }
}
