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
        UpdateIndicator(cube, cubeIndicator); // Handle cube indicator
        UpdateIndicator(sphere, sphereIndicator); // Handle sphere indicator
    }

    private void UpdateIndicator(Transform target, RectTransform indicator)
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position); //transforms a position in world space into a screen space point 

        // Check if target is off-screen
        if (screenPos.z > 0 && (screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height))
        {
            indicator.gameObject.SetActive(true);
            Debug.Log($"Indicator for {target.name} is active and visible.");


            //Vector3 dirToTarget = target.position - mainCamera.transform.position; //getting the direction vector from the target to the camera 
            //dirToTarget.z = 0;
            //Vector2 direction = new Vector2(dirToTarget.x, dirToTarget.y); // normalizing the direction vector 

            //Vector3 clampedScreenPos = screenPos;
            //if (screenPos.x < 0)
            //{
            //    clampedScreenPos.x = 0;
            //}
            //else if (screenPos.x > Screen.width)
            //{
            //    clampedScreenPos.x = Screen.width;
            //}

            //if (screenPos.y < 0)
            //{
            //    clampedScreenPos.y = 0;
            //}
            //else if(screenPos.y > Screen.height)
            //{
            //    clampedScreenPos.y = Screen.height;
            //}

            Vector3 direction = target.position - mainCamera.transform.position;
            direction.z = 0;
            Vector2 screenDirection = new Vector2(direction.x, direction.y).normalized;

            Vector2 edgePosition = Vector2.zero;

            if (screenPos.x < 0)//left side
            {
                edgePosition.x = 0;
                edgePosition.y = Mathf.Clamp01(screenPos.y / Screen.height);
            }

            else if (screenPos.x < Screen.width)//right side
            {
                edgePosition.x = Screen.width;
                edgePosition.y = Mathf.Clamp01(screenPos.y / Screen.height);
            }

            else if (screenPos.y < 0)//bottom 
            {
                edgePosition.y = 0;
                edgePosition.x = Mathf.Clamp01(screenPos.x / Screen.width);
            }

            else if (screenPos.y > Screen.height)//top
            {
                edgePosition.y = Screen.height;
                edgePosition.x = Mathf.Clamp01(screenPos.x / Screen.width);
            }


            //now to convert this clamped screen position to canvas space 
            RectTransform canvasRect = indicator.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, edgePosition, mainCamera, out Vector2 localPos);

            indicator.anchoredPosition = localPos;
            indicator.up = direction;
        }
        else
        {
            indicator.gameObject.SetActive(false);
            Debug.Log($"Indicator for {target.name} is hidden.");
        }
    }

}
