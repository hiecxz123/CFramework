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
        //����
        m_move.Enable();
        //���¼�
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
        //��ȡ״̬
        Debug.Log("״̬:" + obj.phase);
        //��ȡAction����
        Debug.Log("Action����:" + obj.action.name);
        //��ȡ�豸��Ϣ
        Debug.Log("�豸��Ϣ:" + obj.control.name);
        //��ȡֵ
        Debug.Log("ֵ:" + obj.ReadValue<Vector2>());
        //��ȡ����ʱ��
        Debug.Log("����ʱ��:" + obj.duration);
        //��ȡ��ʼʱ��
        Debug.Log("��ʼʱ��:" + obj.startTime);
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
