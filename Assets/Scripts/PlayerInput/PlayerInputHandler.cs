using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public float xInput { get; private set; }
    public float yInput { get; private set; }

    [SerializeField] 
    private float inputHoldTime = 0.2f;
    
    public Vector2 PlayerMoveInput { get; private set; }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        PlayerMoveInput = context.ReadValue<Vector2>();
        
        xInput = PlayerMoveInput.x;
        yInput = PlayerMoveInput.y;
    }
}
