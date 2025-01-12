using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cube;
    public Transform sphere;

    public float smoothSpeed = 0.125f;
    public Vector3 offset; //offset for camera position
    public float lookSmoothSpeed = 5f;

    void LateUpdate()
    {
        Vector3 midpoint = (sphere.position + cube.position) / 2f;

        Vector3 desiredPosition = midpoint + offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

    }
}
