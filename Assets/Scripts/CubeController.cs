using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CubeController : MonoBehaviour
{
    private PlayerControls controls;
    private Vector2 moveInput;

    public float moveSpeed = 5f; // Speed of movement for the sphere

    void Awake()
    {
        controls = new PlayerControls();

        controls.Cube.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Cube.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        // If the moveInput vector is non-zero, move the sphere
        if (moveInput.sqrMagnitude > 0)  // This check helps prevent unnecessary movement if no input
        {
            Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed * Time.deltaTime;
            transform.Translate(move); // Using Translate instead of modifying position directly
        }
    }
}
