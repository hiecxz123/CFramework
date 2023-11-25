using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public enum ScreenJoyStick_Type
{
    //动态位置，点击的起始位置为dPad显示位置
    DynamicPosition,
    //dPad位置不可动
    StaticPosition
}
public enum JoyStickButton_ShowType
{
    AlwaysShow,
    GeneralHideClickShow
}

public class ScreenJoyStick : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    //摇杆可点击区域
    public Image m_joyStickArea;
    //摇杆按钮
    public Image m_joyStickButtonImage;
    //摇杆按钮移动区域
    public Image m_dPadImage;
    //根据按钮旋转摇杆dPad
    public bool m_rotateDPad = true;

    //摇杆按钮移动半径
    public float m_buttonMoveRadious = 400;
    //点击半径
    public float m_clickRadious = 800;
    //摇杆按钮展示方式
    public JoyStickButton_ShowType m_stickButtonShowType = JoyStickButton_ShowType.AlwaysShow;
    //屏幕摇杆类型
    public ScreenJoyStick_Type m_screenJoyStickType = ScreenJoyStick_Type.DynamicPosition;
    //点击初始位置
    Vector2 m_pointerDownPosition = Vector3.zero;
    //绑定设备按键，限制类型为Vector2,，也就是摇杆一类
    [InputControl(layout = "Vector2")]
    [SerializeField]
    private string m_ControlPath;
    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }

    private void Start()
    {
        if (m_joyStickArea == null)
        {
            m_joyStickArea = GetComponent<Image>();
        }
        //图片点击透明度限制，需要开启图片Read
        m_joyStickArea.alphaHitTestMinimumThreshold = 0.5f;
        //平时隐藏，点击显示摇杆dPad和摇杆按钮
        if (m_stickButtonShowType == JoyStickButton_ShowType.GeneralHideClickShow)
        {
            m_joyStickButtonImage.enabled = false;
            m_dPadImage.enabled = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //将屏幕坐标转换为anchoredPosition坐标，需要父物体，屏幕坐标，摄像机，返回的点击位置
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)transform, eventData.position, eventData.pressEventCamera, out m_pointerDownPosition);
        //如果是摇杆位置不改变的情况，每次点击初始位置都为0
        if (m_screenJoyStickType == ScreenJoyStick_Type.StaticPosition)
        {
            m_pointerDownPosition = Vector3.zero;
        }
        //不管是何种摇杆按钮显示方式，按下一定会显示
        m_joyStickButtonImage.enabled = true;
        m_dPadImage.enabled = true;
        
        MoveStick(eventData.position, eventData.pressEventCamera);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_pointerDownPosition = Vector2.zero;
        ((RectTransform)m_joyStickButtonImage.transform).anchoredPosition
            = Vector3.zero;
        ((RectTransform)m_dPadImage.transform).anchoredPosition 
            = Vector3.zero;

        if (m_stickButtonShowType == JoyStickButton_ShowType.GeneralHideClickShow)
        {
            m_joyStickButtonImage.enabled = false;
            m_dPadImage.enabled = false;
        }

        SendValueToControl(Vector2.zero);
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveStick(eventData.position, eventData.pressEventCamera);
    }

    public void MoveStick(Vector2 pointerPosition, Camera uiCamera)
    {
        //获取当前点击屏幕坐标相对于父物体的anchoredPosition
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, pointerPosition, uiCamera, out var position);
        //获取偏移
        var delta = position - m_pointerDownPosition;
        //限制移动半径
        delta = Vector2.ClampMagnitude(delta, m_buttonMoveRadious);
        //设置摇杆按钮位置
        ((RectTransform)m_joyStickButtonImage.transform).anchoredPosition = m_pointerDownPosition + delta;
        //设置摇杆dPad位置
        ((RectTransform)m_dPadImage.transform).anchoredPosition = m_pointerDownPosition;
        //旋转摇杆dPad
        Vector2 v1 = Vector2.up;
        Vector2 v2 = delta;
        float angle = Vector3.Angle(v2,v1);
        if(Vector3.Cross(v1, v2).z<0)
        {
            angle *= -1;
        }
        Quaternion rotate = Quaternion.Euler(0,0,angle);
        ((RectTransform)m_dPadImage.transform).rotation = rotate;
        //归一化
        var newPos = new Vector2(delta.x / m_buttonMoveRadious, delta.y / m_buttonMoveRadious);
        //传出设备数据
        SendValueToControl(newPos);
    }

    private void OnDrawGizmosSelected()
    {
        //需要绘制物体的父物体的本地坐标转换世界坐标的矩阵
        Gizmos.matrix = ((RectTransform)transform).localToWorldMatrix;
        //设置当前摇杆整体所在位置
        var joyStickPos = ((RectTransform)transform).anchoredPosition;

        Gizmos.color = new Color32(255, 0, 0, 255);
        //编辑器状态下，点击位置就是摇杆按钮所在位置
        var pointerDownPos = ((RectTransform)m_joyStickButtonImage.transform).anchoredPosition;
        if (Application.isPlaying)
            pointerDownPos = m_pointerDownPosition;
        //绘制按钮移动区域
        DrawGizmoCircle(pointerDownPos, m_buttonMoveRadious);

        Gizmos.matrix = ((RectTransform)transform.parent).localToWorldMatrix;
        Gizmos.color = new Color32(0, 255, 0, 255);
        //绘制按钮点击区域
        DrawGizmoCircle(joyStickPos, m_clickRadious);
        //将点击区域和Dped的区域和预设值同步
        UpdateDynamicOriginClickableArea();
        UpdateDPadImage();
    }
    //绘制圆形
    private void DrawGizmoCircle(Vector2 center, float radius)
    {
        for (var i = 0; i < 32; i++)
        {
            var radians = i / 32f * Mathf.PI * 2;
            var nextRadian = (i + 1) / 32f * Mathf.PI * 2;
            Gizmos.DrawLine(
                new Vector3(center.x + Mathf.Cos(radians) * radius, center.y + Mathf.Sin(radians) * radius, 0),
                new Vector3(center.x + Mathf.Cos(nextRadian) * radius, center.y + Mathf.Sin(nextRadian) * radius, 0));
        }
    }

    private void UpdateDynamicOriginClickableArea()
    {
        if (m_joyStickArea)
        {
            ((RectTransform)transform).sizeDelta = new Vector2(m_clickRadious * 2, m_clickRadious * 2);
        }
    }

    private void UpdateDPadImage()
    {
        if(m_dPadImage)
        {
            ((RectTransform)m_dPadImage.transform).sizeDelta = new Vector2(m_buttonMoveRadious * 2, m_buttonMoveRadious * 2);
        }
    }

}



