//The user can drag UI objects around the canvas
//Created by James Vanderhyde, 28 September 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropTarget : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Color emptyColor;
    private bool pointerInMe = false;

    public virtual void OnDrop(PointerEventData data)
    {
        if (data.pointerDrag != null)
        {
            //Change the highlight back
            SetHighlight(false);

            //Add the new contents
            data.pointerDrag.transform.SetParent(this.transform.parent.GetChild(this.transform.GetSiblingIndex()+1));
            data.pointerDrag.transform.localPosition = new Vector3(2, -2, 0);

            //Tell the block to resize itself
            ListManagement lm = GetComponentInParent<ListManagement>();
            if (lm)
                lm.AddToList(data.pointerDrag);
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
            DropTarget dt = transform.parent.gameObject.GetComponentInParent<DropTarget>();
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
            DropTarget dt = transform.parent.gameObject.GetComponentInParent<DropTarget>();
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
