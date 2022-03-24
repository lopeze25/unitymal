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
            Draggable draggedObject = data.pointerDrag.GetComponent<Draggable>();
            if (draggedObject != null)
            {
                Draggable droppedObject = draggedObject.MovingObject;
                droppedObject.transform.SetParent(this.transform);

                //Delete symbols that are out of scope when dropped.
                TieToTracker ttt = droppedObject.GetComponent<TieToTracker>();
                if (ttt != null)
                    GameObject.Destroy(ttt.gameObject);
            }
        }
    }
}
