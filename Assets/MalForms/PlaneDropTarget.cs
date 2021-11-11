//The user can drag UI objects around the canvas and drop onto the canvas
//Created by James Vanderhyde, 4 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaneDropTarget : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData data)
    {
        if (data.pointerDrag != null)
        {
            data.pointerDrag.transform.SetParent(this.transform);
        }
    }
}
