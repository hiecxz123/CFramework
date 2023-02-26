using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Camera targetCamera;

    public float movementSpeed = 2;
    public float upAngle = 85f;
    public float downAngle = -85f;

    private float rotateX = 0f;
    private float rotateY = 0f;

    private void Start()
    {
        rotateX = transform.eulerAngles.x;
        rotateY = transform.eulerAngles.y;
    }
    private void Update()
    {
        var X = Input.GetAxis("Mouse X");
        var Y = -Input.GetAxis("Mouse Y");

        RotateCamera(X, Y);
    }
    void RotateCamera(float x, float y)
    {
        rotateX += x * movementSpeed;
        rotateY += y * movementSpeed;

        rotateY = ClampAngle(rotateY, downAngle, upAngle);
        rotateX = ClampAngle(rotateX, -360, 360);

        transform.rotation 
            = Quaternion.Euler(transform.eulerAngles.x, rotateX, transform.eulerAngles.z);
        targetCamera.transform.rotation 
            = Quaternion.Euler(rotateY, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    public float ClampAngle(float angle, float min, float max)
    {
        do
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
        } while (angle < -360 || angle > 360);

        return Mathf.Clamp(angle, min, max);
    }

}
