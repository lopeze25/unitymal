//The user can drag UI objects around the canvas
//Created by James Vanderhyde, 28 September 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform draggingPlane = null;
    private Vector3 pressPositionOffset;

    void Start()
    {
        Canvas c = this.GetComponentInParent<Canvas>();
        if (c != null)
            this.draggingPlane = c.transform as RectTransform;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, eventData.position, eventData.pressEventCamera, out globalMousePos);
        RectTransform rt = this.GetComponent<RectTransform>();
        this.pressPositionOffset = globalMousePos - rt.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CanvasGroup g = GetComponent<CanvasGroup>();
        if (g)
            g.blocksRaycasts = false;
        SetDraggedPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        CanvasGroup g = GetComponent<CanvasGroup>();
        if (g)
            g.blocksRaycasts = true;
    }

    private void SetDraggedPosition(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, eventData.position, eventData.pressEventCamera, out globalMousePos);
        RectTransform rt = this.GetComponent<RectTransform>();
        rt.position = globalMousePos - this.pressPositionOffset;
    }
}
