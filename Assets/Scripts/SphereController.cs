using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SphereController : BaseCharacter
{   
    protected override void SetupControls()
    {
        controls.Sphere.Move.performed += ctx => {
            moveInput = ctx.ReadValue<Vector2>();
            isMoving = true;
        };
        controls.Sphere.Move.canceled += ctx => {
            moveInput = Vector2.zero;
            isMoving = false;
        };
    }

    protected override Vector3 GetDefaultPosition()
    {
        return new Vector3(-5f, 0f, 0f);
    }

    protected override void AdjustTrailEffect()
    {
        Debug.Log("AdjustTrailEffect running on " + gameObject.name);

        if (trailRenderer == null || otherPlayer == null)
        {
            Debug.LogWarning("TrailRenderer or OtherPlayer is missing on " + gameObject.name);
            return;
        }

        float distance = Vector3.Distance(transform.position, otherPlayer.position);
        float newWidth = Mathf.Lerp(0.1f, 0.5f, 1 - Mathf.InverseLerp(minDistance, maxDistance, distance));
        float opacity = Mathf.Lerp(1f, 0.2f, 1 - Mathf.InverseLerp(minDistance, maxDistance, distance));

        Color baseColor = Color.blue;
        Color fadedColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(baseColor, 0.0f), new GradientColorKey(fadedColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(opacity, 0.0f), new GradientAlphaKey(0f, 1.0f) }
        );

        trailRenderer.startWidth = newWidth;
        trailRenderer.endWidth = newWidth * 0.6f;
        trailRenderer.colorGradient = gradient;
        trailRenderer.time = 1.5f; //set longer trail time for a more fluid effect
    }
}
