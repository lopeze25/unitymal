//The user can drag UI objects around the canvas
//Created by James Vanderhyde, 28 September 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform buildPlane = null;
    private RectTransform draggingPlane = null;
    private Vector3 pressPositionOffset;

    void Start()
    {
        Canvas c = this.GetComponentInParent<Canvas>();
        if (c != null)
        {
            this.buildPlane = c.transform.Find("build plane") as RectTransform;
            this.draggingPlane = c.transform.Find("drag plane") as RectTransform;
        }
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

        ListManagement lm = transform.parent.GetComponentInParent<ListManagement>();
        transform.SetParent(draggingPlane);
        if (lm)
            lm.RemoveFromList(eventData.pointerDrag);

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
