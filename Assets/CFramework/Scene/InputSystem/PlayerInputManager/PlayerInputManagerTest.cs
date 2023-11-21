using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManagerTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
        PlayerInputManager.instance.onPlayerLeft += OnPlayerLeft;
    }

    private void OnPlayerLeft(PlayerInput obj)
    {
        Debug.Log("Player Left:" + obj.name);
    }

    private void OnPlayerJoined(PlayerInput obj)
    {
        Debug.Log("Player Join:"+obj.name);
    }
}
