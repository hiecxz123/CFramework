using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputCSharp : MonoBehaviour
{
    PlayerInput input;
    void Start()
    {
        input = transform.GetComponent<PlayerInput>();
        input.onDeviceRegained += OnDeviceRegained;
        input.onDeviceLost += OnDeveceLost;
        input.onControlsChanged += OnControlsChanged;

        input.onActionTriggered += OnActionTriggered;
    }
    private void OnActionTriggered(InputAction.CallbackContext obj)
    {
        //任意输入触发都会触发此事件
        Debug.Log("ActionTriggered");
        switch(obj.action.name)
        {
            case "Move":
                Debug.Log(obj.ReadValue<Vector2>());
                break;
            case "Jump":
                Debug.Log("Jump");
                break;
        }
    }
    private void OnControlsChanged(PlayerInput obj)
    {

    }
    private void OnDeveceLost(PlayerInput obj)
    {

    }
    private void OnDeviceRegained(PlayerInput obj)
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
