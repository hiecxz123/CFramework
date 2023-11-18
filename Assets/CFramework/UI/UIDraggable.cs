using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDraggable : MonoBehaviour,IDragHandler,IBeginDragHandler
{
    RectTransform rectTransform;
    public Camera uiCamera;
    public bool setLastSibling = true;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (uiCamera == null)
        {
            uiCamera = Camera.main;
        }
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (setLastSibling)
        {
            rectTransform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }
    
}
