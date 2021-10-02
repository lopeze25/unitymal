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

    public void OnDrop(PointerEventData data)
    {
        if (data.pointerDrag != null)
        {
            ListManagement lm = GetComponentInParent<ListManagement>();
            if (lm)
                lm.AddToList(data.pointerDrag);
            Image image = GetComponent<Image>();
            image.color = emptyColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.dragging)
        {
            Image image = GetComponent<Image>();
            emptyColor = image.color;
            image.color = Color.yellow;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.dragging)
        {
            Image image = GetComponent<Image>();
            image.color = emptyColor;
        }
    }
}
