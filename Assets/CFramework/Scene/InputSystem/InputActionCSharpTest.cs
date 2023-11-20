using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionCSharpTest : MonoBehaviour
{
    public DefaultInputAction m_defaultInputAction;
    void Start()
    {
        m_defaultInputAction = new DefaultInputAction();
        m_defaultInputAction.Enable();
        m_defaultInputAction.ThridPersonController.Move.performed +=
            movePerform;
    }
    private void movePerform(InputAction.CallbackContext obj)
    {
        Debug.Log(obj.ReadValue<Vector2>());
    }
}
