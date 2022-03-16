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

    private Draggable draggedObject = null;
    public Draggable DraggedObject => this.draggedObject;

    void Awake()
    {
        DollhouseProgramUI c = this.GetComponentInParent<DollhouseProgramUI>();
        if (c != null)
        {
            this.draggingPlane = c.transform.Find("drag plane") as RectTransform;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(this.draggingPlane, eventData.position, eventData.pressEventCamera, out globalMousePos);
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

        //Start drag cases:
        // 1. Pulling off of a shelf. Drag a clone, leaving the original.
        // 2. Pulling off of a defining form. Drag a clone, leaving original.
        // 3. Pulling out of a replacing drop target, leaving something else.
        // 4. Pulling out of a list, leaving nothing, parent must resize.
        // 5. Dragging around within the plane.

        Transform oldParent = this.transform.parent;
        DragParent dParent = this.transform.GetComponentInParent<DragParent>();
        if (dParent == null)
            throw new MissingComponentException("Cannot find DragParent component.");
        if (dParent.DragDuplicate())
        {
            //Clone the dragged object
            this.draggedObject = GameObject.Instantiate(this, draggingPlane);
        }
        else
        {
            //Switch the parent
            this.transform.SetParent(draggingPlane);
            this.draggedObject = this;
        }
        dParent.ObjectDragged(this);

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

        //Make sure drop targets can see the mouse through the dragged object
        CanvasGroup g = this.draggedObject.GetComponent<CanvasGroup>();
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
        CanvasGroup g = this.GetComponent<CanvasGroup>();
        if (g)
            g.blocksRaycasts = true;

        //Disable all the drop targets
        DropTarget[] targets = this.GetComponentInParent<Canvas>().GetComponentsInChildren<DropTarget>(true);
        foreach (DropTarget t in targets)
            t.enabled = false;

        this.draggedObject = null;
    }

    protected virtual void SetDraggedPosition(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, eventData.position, eventData.pressEventCamera, out globalMousePos);
        RectTransform rt = this.draggedObject.GetComponent<RectTransform>();
        rt.position = globalMousePos - this.pressPositionOffset;
    }
}
