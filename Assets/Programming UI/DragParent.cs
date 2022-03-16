//Defines behavior for what happens when an item is dragged out of something
//Created by James Vanderhyde, 16 March 2022

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragParent : MonoBehaviour
{
    public virtual bool DragDuplicate()
    {
        return false;
    }

    public virtual void ObjectDragged(Draggable d)
    {
        //Do nothing
    }
}
