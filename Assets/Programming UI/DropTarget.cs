//The user can drag UI objects around the canvas
//Base class for handling highlighting of drop targets
//Created by James Vanderhyde, 28 September 2021
//Extracted from DropTarget, 16 March 2022

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class DropTarget : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Color emptyColor;
    private bool pointerInMe = false;

    public abstract void HandleDrop(Transform droppedObject);

    public virtual void OnDrop(PointerEventData data)
    {
        if (data.pointerDrag != null)
        {
            Draggable draggable = data.pointerDrag.GetComponent<Draggable>();
            if (draggable != null)
            {
                this.HandleDrop(draggable.transform);

                //Change the highlight back
                SetHighlight(false);

                //Expand recur form if present
                MalRecurForm recurForm = draggable.GetComponent<MalRecurForm>();
                if (recurForm != null)
                {
                    RecurPoint rp = this.transform.GetComponentInParent<RecurPoint>();
                    recurForm.SetRecurPoint(rp);
                }

                //Tell the block to resize itself
                ListManagement lm = GetComponentInParent<ListManagement>();
                if (lm)
                    lm.AddToList(draggable.gameObject);
            }
        }
    }

    public void SetHighlight(bool on)
    {
        Image image = GetComponent<Image>();
        if (on)
        {
            emptyColor = image.color;
            image.color = Color.yellow;
        }
        else
        {
            image.color = emptyColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.dragging)
        {
            pointerInMe = true;
            SetHighlight(true);
            DropTarget dt = transform.parent.GetComponentInParent<DropTarget>();
            if (dt)
                dt.PointerEnteredChild();
        }
    }

    private void PointerEnteredChild()
    {
        SetHighlight(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.dragging)
        {
            pointerInMe = false;
            SetHighlight(false);
            DropTarget dt = transform.parent.GetComponentInParent<DropTarget>();
            if (dt)
                dt.PointerExitedChild();
        }
    }

    private void PointerExitedChild()
    {
        if (pointerInMe)
            SetHighlight(true);
    }
}
