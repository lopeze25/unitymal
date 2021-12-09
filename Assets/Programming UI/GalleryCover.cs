//Blocks forms from being drop targets when a new form is dragged from a gallery
//Created by James Vanderhyde, 3 December 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GalleryCover : MonoBehaviour, IPointerEnterHandler, IDropHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        //When the mouse enters, open up the gallery
        if (!eventData.dragging)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void OnDrop(PointerEventData data)
    {
        //When the mouse drops something, destroy the object and open up the gallery
        if (data.pointerDrag != null)
        {
            data.pointerDrag.transform.SetParent(null);
            GameObject.Destroy(data.pointerDrag);
        }
        this.gameObject.SetActive(false);
    }

}
