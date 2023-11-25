using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public enum ScreenJoyStick_Type
{
    //��̬λ�ã��������ʼλ��ΪdPad��ʾλ��
    DynamicPosition,
    //dPadλ�ò��ɶ�
    StaticPosition
}
public enum JoyStickButton_ShowType
{
    AlwaysShow,
    GeneralHideClickShow
}

public class ScreenJoyStick : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    //ҡ�˿ɵ������
    public Image m_joyStickArea;
    //ҡ�˰�ť
    public Image m_joyStickButtonImage;
    //ҡ�˰�ť�ƶ�����
    public Image m_dPadImage;
    //���ݰ�ť��תҡ��dPad
    public bool m_rotateDPad = true;

    //ҡ�˰�ť�ƶ��뾶
    public float m_buttonMoveRadious = 400;
    //����뾶
    public float m_clickRadious = 800;
    //ҡ�˰�ťչʾ��ʽ
    public JoyStickButton_ShowType m_stickButtonShowType = JoyStickButton_ShowType.AlwaysShow;
    //��Ļҡ������
    public ScreenJoyStick_Type m_screenJoyStickType = ScreenJoyStick_Type.DynamicPosition;
    //�����ʼλ��
    Vector2 m_pointerDownPosition = Vector3.zero;
    //���豸��������������ΪVector2,��Ҳ����ҡ��һ��
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
        //ͼƬ���͸�������ƣ���Ҫ����ͼƬRead
        m_joyStickArea.alphaHitTestMinimumThreshold = 0.5f;
        //ƽʱ���أ������ʾҡ��dPad��ҡ�˰�ť
        if (m_stickButtonShowType == JoyStickButton_ShowType.GeneralHideClickShow)
        {
            m_joyStickButtonImage.enabled = false;
            m_dPadImage.enabled = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //����Ļ����ת��ΪanchoredPosition���꣬��Ҫ�����壬��Ļ���꣬����������صĵ��λ��
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)transform, eventData.position, eventData.pressEventCamera, out m_pointerDownPosition);
        //�����ҡ��λ�ò��ı�������ÿ�ε����ʼλ�ö�Ϊ0
        if (m_screenJoyStickType == ScreenJoyStick_Type.StaticPosition)
        {
            m_pointerDownPosition = Vector3.zero;
        }
        //�����Ǻ���ҡ�˰�ť��ʾ��ʽ������һ������ʾ
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
        //��ȡ��ǰ�����Ļ��������ڸ������anchoredPosition
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, pointerPosition, uiCamera, out var position);
        //��ȡƫ��
        var delta = position - m_pointerDownPosition;
        //�����ƶ��뾶
        delta = Vector2.ClampMagnitude(delta, m_buttonMoveRadious);
        //����ҡ�˰�ťλ��
        ((RectTransform)m_joyStickButtonImage.transform).anchoredPosition = m_pointerDownPosition + delta;
        //����ҡ��dPadλ��
        ((RectTransform)m_dPadImage.transform).anchoredPosition = m_pointerDownPosition;
        //��תҡ��dPad
        Vector2 v1 = Vector2.up;
        Vector2 v2 = delta;
        float angle = Vector3.Angle(v2,v1);
        if(Vector3.Cross(v1, v2).z<0)
        {
            angle *= -1;
        }
        Quaternion rotate = Quaternion.Euler(0,0,angle);
        ((RectTransform)m_dPadImage.transform).rotation = rotate;
        //��һ��
        var newPos = new Vector2(delta.x / m_buttonMoveRadious, delta.y / m_buttonMoveRadious);
        //�����豸����
        SendValueToControl(newPos);
    }

    private void OnDrawGizmosSelected()
    {
        //��Ҫ��������ĸ�����ı�������ת����������ľ���
        Gizmos.matrix = ((RectTransform)transform).localToWorldMatrix;
        //���õ�ǰҡ����������λ��
        var joyStickPos = ((RectTransform)transform).anchoredPosition;

        Gizmos.color = new Color32(255, 0, 0, 255);
        //�༭��״̬�£����λ�þ���ҡ�˰�ť����λ��
        var pointerDownPos = ((RectTransform)m_joyStickButtonImage.transform).anchoredPosition;
        if (Application.isPlaying)
            pointerDownPos = m_pointerDownPosition;
        //���ư�ť�ƶ�����
        DrawGizmoCircle(pointerDownPos, m_buttonMoveRadious);

        Gizmos.matrix = ((RectTransform)transform.parent).localToWorldMatrix;
        Gizmos.color = new Color32(0, 255, 0, 255);
        //���ư�ť�������
        DrawGizmoCircle(joyStickPos, m_clickRadious);
        //����������Dped�������Ԥ��ֵͬ��
        UpdateDynamicOriginClickableArea();
        UpdateDPadImage();
    }
    //����Բ��
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



