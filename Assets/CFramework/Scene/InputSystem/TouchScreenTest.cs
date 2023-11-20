using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

public class TouchScreenTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        Touchscreen touchScreen = Touchscreen.current;
        if(touchScreen==null)
        {
            return;
        }
        Debug.Log(touchScreen.touches.Count);
        foreach (var item in touchScreen.touches)
        {
            //遍历手指
        }
        TouchControl touchControl = touchScreen.touches[0];
        if(touchControl.press.wasPressedThisFrame)
        {
            //按下
        }
        if(touchControl.press.isPressed)
        {
            //长按
        }
        if(touchControl.press.wasReleasedThisFrame)
        {
            //抬起
        }
        if(touchControl.tap.isPressed)
        {
            //点击
        }
        //连续点击次数
        Debug.Log(touchControl.tapCount);

        //位置
        Debug.Log(touchControl.position.ReadValue());
        //第一次接触时的位置
        Debug.Log(touchControl.startPosition.ReadValue());
        //接触区域的大小
        Debug.Log(touchControl.radius.ReadValue());
        //每帧之间的位移
        Debug.Log(touchControl.delta.ReadValue());

        //获取手指状态
        //None:无
        //Began:开始接触
        //Moved:移动
        //Ended:结束
        //Canceled:取消
        //Stationary:静止
        Debug.Log(touchControl.phase.ReadValue());

#elif ENABLE_LEGACY_INPUT_MANAGER
        
#endif
    }
}
