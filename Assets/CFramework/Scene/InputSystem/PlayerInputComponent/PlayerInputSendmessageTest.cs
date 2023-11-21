using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSendmessageTest : MonoBehaviour
{
    public void OnDeviceLost(PlayerInput input)
    {
        Debug.Log("DeviceLost");
    }

    public void OnDeviceRegained(PlayerInput input)
    {
        Debug.Log("DeviceRegained");
    }

    public void OnControlsChanged(PlayerInput input)
    {
        Debug.Log("ControlsChanged");
    }

    public void OnMove(InputValue inputValue)
    {
        Debug.Log("Move:"+inputValue.Get<Vector2>());
    }
    public void OnJump(InputValue inputValue)
    {
        Debug.Log("Jump");
    }
}
