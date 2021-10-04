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
        {
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
        //Make sure drop targets can see the mouse through the dragged object
        CanvasGroup g = GetComponent<CanvasGroup>();
        if (g)
            g.blocksRaycasts = false;

        //Switch the parent and tell the old parent to resize itself
        ListManagement lm = transform.parent.GetComponentInParent<ListManagement>();
        transform.SetParent(draggingPlane);
        if (lm)
            lm.RemoveFromList(eventData.pointerDrag);

        //Move
        SetDraggedPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Go back to catching the mouse
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
