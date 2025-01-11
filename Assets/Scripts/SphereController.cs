using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SphereController : MonoBehaviour
{
    private PlayerControls controls;
    private Vector2 moveInput;

    public float moveSpeed = 5f; // Speed of movement for the sphere

    void Awake()
    {
        controls = new PlayerControls();

        // Make sure you map to the correct input action and action map
        controls.Sphere.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Sphere.Move.canceled += ctx => moveInput = Vector2.zero;
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
