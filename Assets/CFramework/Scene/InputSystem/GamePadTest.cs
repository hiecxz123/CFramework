using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

public class GamePadTest : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        Gamepad gamepad = Gamepad.current;
        if(gamepad==null)
        {
            return;
        }
        //左摇杆
        Debug.Log("leftStick:"+gamepad.leftStick.ReadValue());
        //右摇杆
        Debug.Log("rightStick:" + gamepad.rightStick.ReadValue());
        if(gamepad.leftStickButton.wasPressedThisFrame)
        {
            Debug.Log("左摇杆按下");
        }
        if(gamepad.leftStickButton.isPressed)
        {
            Debug.Log("左摇杆按住");
        }
        if(gamepad.leftStickButton.wasReleasedThisFrame)
        {
            Debug.Log("左摇杆抬起");
        }
        if(gamepad.dpad.left.wasPressedThisFrame)
        {
            Debug.Log("方向键：左");
        }
        if (gamepad.dpad.right.wasPressedThisFrame)
        {
            Debug.Log("方向键：右");
        }
        if (gamepad.dpad.up.wasPressedThisFrame)
        {
            Debug.Log("方向键：上");
        }
        if (gamepad.dpad.down.wasPressedThisFrame)
        {
            Debug.Log("方向键：下");
        }

        if(gamepad.buttonNorth.wasPressedThisFrame)
        {
            //gamepad.triangleButton.wasPressedThisFrame; 三角
            //gamepad.yButton.wasPressedThisFrame; Y
            Debug.Log("上按键：三角/Y");
        }
        if(gamepad.buttonSouth.wasPressedThisFrame)
        {
            //gamepad.crossButton.wasPressedThisFrame; 叉
            //gamepad.aButton.wasPressedThisFrame; A
            Debug.Log("下按键：叉/A");
        }
        if(gamepad.buttonEast.wasPressedThisFrame)
        {
            //gamepad.circleButton.wasPressedThisFrame; 圆
            //gamepad.aButton.wasPressedThisFrame; B
            Debug.Log("右按键：圆/B");
        }
        if(gamepad.buttonWest.wasPressedThisFrame)
        {
            //gamepad.squareButton.wasPressedThisFrame; 方框
            //gamepad.xButton.wasPressedThisFrame; x
            Debug.Log("左按键：方框/X");
        }
        if(gamepad.startButton.wasPressedThisFrame)
        {
            Debug.Log("StartButton");
        }
        if(gamepad.selectButton.wasPressedThisFrame)
        {
            Debug.Log("SelectButton");
        }
        if(gamepad.leftShoulder.wasPressedThisFrame)
        {
            Debug.Log("LeftShoulder");
        }
        if(gamepad.leftTrigger.wasPressedThisFrame)
        {
            Debug.Log("LeftTirgger");
        }
        

#elif ENABLE_LEGACY_INPUT_MANAGER
        
#endif
    }
}
