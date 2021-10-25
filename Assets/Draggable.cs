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

    void Awake()
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
        //Switch the parent and tell the old parent to resize itself
        Transform oldParent = this.transform.parent;
        this.transform.SetParent(draggingPlane);
        ReplacingDropTarget dropTarget = oldParent.GetComponent<ReplacingDropTarget>();
        if (dropTarget)
            dropTarget.ReplaceWithDefault();
        ReplaceSelf rs = this.GetComponent<ReplaceSelf>();
        if (rs)
            rs.Replace(oldParent);
        else
        {
            ListManagement lm = oldParent.GetComponentInParent<ListManagement>();
            if (lm)
            {
                //If it has any levels, go one level deeper to rebuild.
                if (oldParent.childCount > 0)
                    lm.RemoveFromList(oldParent.GetChild(0).gameObject);
                else
                    lm.RemoveFromList(oldParent.gameObject);
            }
        }

        //Make sure drop targets can see the mouse through the dragged object
        CanvasGroup g = GetComponent<CanvasGroup>();
        if (g)
            g.blocksRaycasts = false;

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
