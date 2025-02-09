using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : BaseCharacter
{
    protected override void SetupControls()
    {
        controls.Cube.Move.performed += ctx => {
            moveInput = ctx.ReadValue<Vector2>();
            isMoving = true;
        };
        controls.Cube.Move.canceled += ctx => {
            moveInput = Vector2.zero;
            isMoving = false;
        };
    }

    protected override Vector3 GetDefaultPosition()
    {
        return new Vector3(5f, 0f, 0f);
    }
}

