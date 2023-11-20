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
            //������ָ
        }
        TouchControl touchControl = touchScreen.touches[0];
        if(touchControl.press.wasPressedThisFrame)
        {
            //����
        }
        if(touchControl.press.isPressed)
        {
            //����
        }
        if(touchControl.press.wasReleasedThisFrame)
        {
            //̧��
        }
        if(touchControl.tap.isPressed)
        {
            //���
        }
        //�����������
        Debug.Log(touchControl.tapCount);

        //λ��
        Debug.Log(touchControl.position.ReadValue());
        //��һ�νӴ�ʱ��λ��
        Debug.Log(touchControl.startPosition.ReadValue());
        //�Ӵ�����Ĵ�С
        Debug.Log(touchControl.radius.ReadValue());
        //ÿ֮֡���λ��
        Debug.Log(touchControl.delta.ReadValue());

        //��ȡ��ָ״̬
        //None:��
        //Began:��ʼ�Ӵ�
        //Moved:�ƶ�
        //Ended:����
        //Canceled:ȡ��
        //Stationary:��ֹ
        Debug.Log(touchControl.phase.ReadValue());

#elif ENABLE_LEGACY_INPUT_MANAGER
        
#endif
    }
}
