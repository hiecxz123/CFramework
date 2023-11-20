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
        //��ҡ��
        Debug.Log("leftStick:"+gamepad.leftStick.ReadValue());
        //��ҡ��
        Debug.Log("rightStick:" + gamepad.rightStick.ReadValue());
        if(gamepad.leftStickButton.wasPressedThisFrame)
        {
            Debug.Log("��ҡ�˰���");
        }
        if(gamepad.leftStickButton.isPressed)
        {
            Debug.Log("��ҡ�˰�ס");
        }
        if(gamepad.leftStickButton.wasReleasedThisFrame)
        {
            Debug.Log("��ҡ��̧��");
        }
        if(gamepad.dpad.left.wasPressedThisFrame)
        {
            Debug.Log("���������");
        }
        if (gamepad.dpad.right.wasPressedThisFrame)
        {
            Debug.Log("���������");
        }
        if (gamepad.dpad.up.wasPressedThisFrame)
        {
            Debug.Log("���������");
        }
        if (gamepad.dpad.down.wasPressedThisFrame)
        {
            Debug.Log("���������");
        }

        if(gamepad.buttonNorth.wasPressedThisFrame)
        {
            //gamepad.triangleButton.wasPressedThisFrame; ����
            //gamepad.yButton.wasPressedThisFrame; Y
            Debug.Log("�ϰ���������/Y");
        }
        if(gamepad.buttonSouth.wasPressedThisFrame)
        {
            //gamepad.crossButton.wasPressedThisFrame; ��
            //gamepad.aButton.wasPressedThisFrame; A
            Debug.Log("�°�������/A");
        }
        if(gamepad.buttonEast.wasPressedThisFrame)
        {
            //gamepad.circleButton.wasPressedThisFrame; Բ
            //gamepad.aButton.wasPressedThisFrame; B
            Debug.Log("�Ұ�����Բ/B");
        }
        if(gamepad.buttonWest.wasPressedThisFrame)
        {
            //gamepad.squareButton.wasPressedThisFrame; ����
            //gamepad.xButton.wasPressedThisFrame; x
            Debug.Log("�󰴼�������/X");
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
