using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

public class KeyboardInputTest : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard currentKeyboard = Keyboard.current;
        //按下
        if (currentKeyboard.aKey.wasPressedThisFrame)
        {
            Debug.Log("A was pressed this frame");
        }
        //长按
        if (currentKeyboard.aKey.isPressed)
        {
            Debug.Log("A pressed");
        }
        //抬起
        if (currentKeyboard.aKey.wasReleasedThisFrame)
        {
            Debug.Log("A was released this frame");
        }

        Keyboard.current.onTextInput += (o) =>
        {
            Debug.Log("event:"+o.ToString());
        };
#elif ENABLE_LEGACY_INPUT_MANAGER
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Debug.Log(x + " " + y);
#endif
    }
}
