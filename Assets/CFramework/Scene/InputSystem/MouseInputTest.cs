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
        //通过屏幕中的一点进行射线检查
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
        //鼠标左键
        if(mouse.leftButton.wasPressedThisFrame)
        {
            Debug.Log("left button");
        }
        //鼠标右键
        if(mouse.rightButton.wasPressedThisFrame)
        {
            Debug.Log("right button");
        }
        //鼠标中键
        if(mouse.middleButton.wasPressedThisFrame)
        {
            Debug.Log("middle button");
        }
        //鼠标前侧键
        if(mouse.forwardButton.wasPressedThisFrame)
        {
            Debug.Log("forward button");
        }
        //鼠标后侧键
        if(mouse.backButton.wasPressedThisFrame)
        {
            Debug.Log("back button");
        }
        ////鼠标位置
        Debug.Log(Mouse.current.position.ReadValue());
        //鼠标每帧位移
        Debug.Log(Mouse.current.delta.ReadValue());
        //鼠标滚轮
        Debug.Log(Mouse.current.scroll.ReadValue());
#elif ENABLE_LEGACY_INPUT_MANAGER
        
#endif
    }

    
}
