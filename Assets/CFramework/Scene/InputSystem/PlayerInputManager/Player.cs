using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    PlayerInput input;
    Vector3 dir;
    void Start()
    {
        input = transform.GetComponent<PlayerInput>();

        input.onActionTriggered += OnActionTriggered;
    }
    public void Update()
    {
        transform.Translate(dir * 10 * Time.deltaTime);
    }
    private void OnActionTriggered(InputAction.CallbackContext obj)
    {
        //任意输入触发都会触发此事件
        //Debug.Log(name);
        switch (obj.action.name)
        {
            case "Move":
                Debug.Log(name+":"+obj.ReadValue<Vector2>());
                dir = obj.ReadValue<Vector2>();
                break;
            case "Jump":
                Debug.Log(name + ":" +"Fire");
                break;
        }
    }
}
