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
    private RectTransform region;

    private Draggable movingObject = null;
    public Draggable MovingObject => this.movingObject;

    void Awake()
    {
        DollhouseProgramUI c = this.GetComponentInParent<DollhouseProgramUI>();
        if (c != null)
        {
            this.draggingPlane = c.transform.Find("drag plane") as RectTransform;
        }
    }

    public void SetRestrictedRegion()
    {
        DefiningForm df = this.transform.parent.GetComponentInParent<DefiningForm>();
        if (df != null)
        {
            DragPanel dp = df.GetComponentInChildren<DragPanel>();
            this.region = (RectTransform)dp.transform;
        }
        else
        {
            Canvas c = this.GetComponentInParent<Canvas>();
            this.region = (RectTransform)c.transform;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(this.draggingPlane, eventData.position, eventData.pressEventCamera, out Vector3 globalMousePos);
        RectTransform rt = this.GetComponent<RectTransform>();
        this.pressPositionOffset = globalMousePos - rt.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        DropTarget[] targets = this.GetComponentInParent<Canvas>().GetComponentsInChildren<DropTarget>(true);

        MalRecurForm recurForm = this.GetComponent<MalRecurForm>();
        if (recurForm != null)
        {
            //Enable compatible drop targets for recur forms
            foreach (DropTarget t in targets)
            {
                //Three cases: t is fn/loop, t is let/if/do, or t is something else.
                TailPosition tp = t.GetComponent<TailPosition>();
                while (tp != null)
                {
                    MalForm parentForm = tp.transform.parent.GetComponentInParent<MalForm>();
                    RecurPoint rp = parentForm.GetComponent<RecurPoint>();
                    if (rp != null)
                        break;
                    tp = parentForm.transform.parent.GetComponent<TailPosition>();
                }
                if (tp == null)
                    t.enabled = false;
                else
                    t.enabled = true;
            }
        }
        else
        {
            //Enable all the drop targets
            foreach (DropTarget t in targets)
                t.enabled = true;
        }

        this.SetRestrictedRegion();

        //Start drag cases:
        // 1. Pulling off of a shelf. Drag a clone, leaving the original.
        // 2. Pulling off of a defining form. Drag a clone, leaving original.
        // 3. Pulling out of a replacing drop target, leaving something else.
        // 4. Pulling out of a list, leaving nothing.
        // 5. Dragging around within the plane.

        Transform oldParent = this.transform.parent;
        DragParent dParent = this.transform.GetComponentInParent<DragParent>();
        if (dParent == null)
            throw new MissingComponentException("Cannot find DragParent component.");
        if (dParent.DragDuplicate())
        {
            //Clone the dragged object
            this.movingObject = GameObject.Instantiate(this, draggingPlane);
        }
        else
        {
            //Switch the parent
            this.transform.SetParent(draggingPlane);
            this.movingObject = this;

            //Tell a list parent to resize
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
        dParent.ObjectDragged(this);

        //Make sure drop targets can see the mouse through the dragged object
        CanvasGroup g = this.movingObject.GetComponent<CanvasGroup>();
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
        CanvasGroup g = this.movingObject.GetComponent<CanvasGroup>();
        if (g)
            g.blocksRaycasts = true;

        //Disable all the drop targets
        DropTarget[] targets = this.GetComponentInParent<Canvas>().GetComponentsInChildren<DropTarget>(true);
        foreach (DropTarget t in targets)
            t.enabled = false;

        this.movingObject = null;
    }

    protected virtual void SetDraggedPosition(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, eventData.position, eventData.pressEventCamera, out globalMousePos);
        RectTransform rt = this.movingObject.GetComponent<RectTransform>();
        rt.position = globalMousePos - this.pressPositionOffset;

        //Adjust to restricted region
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
