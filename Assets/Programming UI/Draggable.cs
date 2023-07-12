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
    private RectTransform buildPlane = null;
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
            this.buildPlane = c.transform.Find("build plane") as RectTransform;
        }
    }

    private MalRecurForm findRecurForm(Transform start)
    {
        MalRecurForm recurForm = start.GetComponent<MalRecurForm>();
        if (recurForm != null)
            return recurForm;
        foreach (Transform child in start)
        {
            //Skipping recur points, look in each child for a recur form.
            RecurPoint rp = child.GetComponent<RecurPoint>();
            if (rp == null)
            {
                recurForm = findRecurForm(child);
                if (recurForm != null)
                    return recurForm;
            }
        }
        return null;
    }

    private void EnableCompatibleDropTargets()
    {
        DropTarget[] targets = this.buildPlane.GetComponentsInChildren<DropTarget>(true);

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
    }

    private void SetRestrictedRegion()
    {
        //Check for recur forms in children of this draggable.
        //  If the corresponding recur point is also a child, no problem.
        //  Otherwise, we have to restrict to the recur point's drag panel.
        MalRecurForm recurForm = this.findRecurForm(this.transform);
        DefiningForm recurPointDefiningForm = null;
        if (recurForm != null)
        {
            RecurPoint rp = recurForm.GetRecurPoint();
            if (rp)
                recurPointDefiningForm = rp.GetComponent<DefiningForm>();
        }

        //Look at all the symbols used in the body of this form
        HashSet<string> usedSymbols = new HashSet<string>();
        MalSymbol[] symbolForms = this.GetComponentsInChildren<MalSymbol>();
        foreach (MalSymbol ms in symbolForms)
            usedSymbols.Add(ms.GetSymbolName());

        //Walk up the tree looking at DefiningForms (stop at the recur point, if any).
        //For each DefiningForm, for each symbol defined by that form,
        //  check if that symbol is used in the body of this Draggable.
        //  If so, the current DefiningForm's DragPanel is the one to use.
        this.region = null;
        DefiningForm df = this.transform.parent.GetComponentInParent<DefiningForm>();
        while ((df != null) && (df != recurPointDefiningForm) && (this.region == null))
        {
            foreach (Transform child in df.transform.GetChild(0))
            {
                SymbolTracker st = child.GetComponent<SymbolTracker>();
                if (st)
                {
                    if (usedSymbols.Contains(st.GetSymbolName()))
                    {
                        DragPanel dp = df.GetComponentInChildren<DragPanel>();
                        this.region = (RectTransform)dp.transform;
                    }
                }
            }
            df = df.transform.parent.GetComponentInParent<DefiningForm>();
        }

        //If we didn't find a restricting drag panel,
        //  use the recur point, if any, otherwise use the full build plane.
        if (this.region == null)
        {
            if (recurPointDefiningForm != null)
            {
                DragPanel dp = recurPointDefiningForm.GetComponentInChildren<DragPanel>();
                this.region = (RectTransform)dp.transform;
            }
            else
            {
                this.region = this.buildPlane;
            }
        }
    }

    private void ClearDragPlane()
    {
        //Move orphaned objects in the drag plane back into the build plane
        foreach (Transform child in this.draggingPlane)
        {
            child.SetParent(this.buildPlane);
        }

        //Orphaned objects are easy to create by dropping them outside the build plane.
        //Since the build plane does not receive the drop message, it does not reparent the object.
        //The object that receives the OnDrop event is the one the mouse is over when released,
        //  not the one the object is over.
    }

    private void NegotiateDragWithParent()
    {
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
            this.movingObject = GameObject.Instantiate(this, this.draggingPlane);
        }
        else
        {
            //Switch the parent
            this.transform.SetParent(this.draggingPlane);
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
    }

    private void SetDraggedPosition(PointerEventData eventData)
    {
        //Move the dragged object
        RectTransformUtility.ScreenPointToWorldPointInRectangle(this.draggingPlane, eventData.position, eventData.pressEventCamera, out Vector3 globalMousePos);
        RectTransform rt = this.movingObject.GetComponent<RectTransform>();
        rt.position = globalMousePos - this.pressPositionOffset;

        //Adjust to restricted region
        RectTransform root = (RectTransform)this.GetComponentInParent<Canvas>().transform;
        Bounds rtBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(root, rt);
        Bounds regionBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(root, this.region);
        Vector3 minmax = new Vector3(rtBounds.min.x, rtBounds.max.y, rtBounds.center.z);
        Vector3 maxmin = new Vector3(rtBounds.max.x, rtBounds.min.y, rtBounds.center.z);
        float d1 = regionBounds.SqrDistance(rtBounds.min);
        float d2 = regionBounds.SqrDistance(minmax);
        float d3 = regionBounds.SqrDistance(rtBounds.max);
        float d4 = regionBounds.SqrDistance(maxmin);
        if ((d1 > d2) && (d1 > d4))
            rt.localPosition += regionBounds.ClosestPoint(rtBounds.min) - rtBounds.min;
        else if ((d2 > d3) && (d2 > d1))
            rt.localPosition += regionBounds.ClosestPoint(minmax) - minmax;
        else if ((d3 > d2) && (d3 > d4))
            rt.localPosition += regionBounds.ClosestPoint(rtBounds.max) - rtBounds.max;
        else if (((d4 > d1) && (d4 > d3)) || (d4 > d2))
            rt.localPosition += regionBounds.ClosestPoint(maxmin) - maxmin;
        else
            rt.localPosition += regionBounds.ClosestPoint(minmax) - minmax;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(this.draggingPlane, eventData.position, eventData.pressEventCamera, out Vector3 globalMousePos);
        RectTransform rt = this.GetComponent<RectTransform>();
        this.pressPositionOffset = globalMousePos - rt.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Enable only recur points for recur, all drop points for others
        this.EnableCompatibleDropTargets();

        //Restrict the plane of dragging for recur and local symbols
        this.SetRestrictedRegion();

        this.ClearDragPlane();

        this.NegotiateDragWithParent();

        //Make sure drop targets can see the mouse through the dragged object
        CanvasGroup g = this.movingObject.GetComponent<CanvasGroup>();
        if (g)
            g.blocksRaycasts = false;

        //Move
        this.SetDraggedPosition(eventData);
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

        //Disable all the drop targets in the build plane
        DropTarget[] targets = this.buildPlane.GetComponentsInChildren<DropTarget>(true);
        foreach (DropTarget t in targets)
            t.enabled = false;

        this.movingObject = null;
    }
}
