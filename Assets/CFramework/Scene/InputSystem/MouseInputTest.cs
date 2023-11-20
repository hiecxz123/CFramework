using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

public class MouseInputTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void FixedUpdate()
    {
        //ͨ����Ļ�е�һ��������߼��
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            RaycastHit info;
            if (Physics.Raycast
                (Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out info))
            {
                Debug.Log(info.collider.name);
            }
        }
        
    }
    // Update is called once per frame
    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        Mouse mouse = Mouse.current;
        //������
        if(mouse.leftButton.wasPressedThisFrame)
        {
            Debug.Log("left button");
        }
        //����Ҽ�
        if(mouse.rightButton.wasPressedThisFrame)
        {
            Debug.Log("right button");
        }
        //����м�
        if(mouse.middleButton.wasPressedThisFrame)
        {
            Debug.Log("middle button");
        }
        //���ǰ���
        if(mouse.forwardButton.wasPressedThisFrame)
        {
            Debug.Log("forward button");
        }
        //������
        if(mouse.backButton.wasPressedThisFrame)
        {
            Debug.Log("back button");
        }
        ////���λ��
        Debug.Log(Mouse.current.position.ReadValue());
        //���ÿ֡λ��
        Debug.Log(Mouse.current.delta.ReadValue());
        //������
        Debug.Log(Mouse.current.scroll.ReadValue());
#elif ENABLE_LEGACY_INPUT_MANAGER
        
#endif
    }

    
}
