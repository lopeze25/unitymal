//The user can drag symbols around the canvas, inside a specific region
//Created by James Vanderhyde, 29 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableRestricted : Draggable
{
    private RectTransform region;

    void Start()
    {
        this.region = (RectTransform)this.GetComponentInParent<DefiningForm>().GetComponentInChildren<DragPanel>().transform;
    }

    protected override void SetDraggedPosition(PointerEventData eventData)
    {
        base.SetDraggedPosition(eventData);
        RectTransform rt = this.GetComponent<RectTransform>();
        Vector3[] rtCorners = new Vector3[4];
        rt.GetWorldCorners(rtCorners);
        Vector3[] regionCorners = new Vector3[4];
        this.region.GetWorldCorners(regionCorners);
        float leftOffset = rtCorners[0].x - regionCorners[0].x;
        float rightOffset = regionCorners[2].x - rtCorners[2].x;
        if (leftOffset < 0)
            rt.position += new Vector3(-leftOffset, 0, 0);
        else if (rightOffset < 0)
            rt.position += new Vector3(rightOffset, 0, 0);
        float bottomOffset = rtCorners[0].y - regionCorners[0].y;
        float topOffset = regionCorners[2].y - rtCorners[2].y;
        if (bottomOffset < 0)
            rt.position += new Vector3(0, -bottomOffset, 0);
        else if (topOffset < 0)
            rt.position += new Vector3(0, topOffset, 0);
    }
}
