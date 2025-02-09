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

}
