using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
public class InputActionTest : MonoBehaviour
{
    public InputAction m_move;
    // Start is called before the first frame update

    void Start()
    {
#if ENABLE_INPUT_SYSTEM
        //启用
        m_move.Enable();
        //绑定事件
        m_move.started += StartedMove;
        m_move.performed += PerformedMove;
        m_move.performed += CancelMove;
#elif ENABLE_LEGACY_INPUT_MANAGER
        
#endif
    }

    private void CancelMove(InputAction.CallbackContext obj)
    {
        
    }

    private void PerformedMove(InputAction.CallbackContext obj)
    {
        //获取状态
        Debug.Log("状态:" + obj.phase);
        //获取Action名称
        Debug.Log("Action名称:" + obj.action.name);
        //获取设备信息
        Debug.Log("设备信息:" + obj.control.name);
        //获取值
        Debug.Log("值:" + obj.ReadValue<Vector2>());
        //获取持续时间
        Debug.Log("持续时间:" + obj.duration);
        //获取开始时间
        Debug.Log("开始时间:" + obj.startTime);
    }

    private void StartedMove(InputAction.CallbackContext obj)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
#if ENABLE_INPUT_SYSTEM

#elif ENABLE_LEGACY_INPUT_MANAGER

#endif
    }
}
