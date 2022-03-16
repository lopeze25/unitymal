//The user can drag UI objects around the canvas
//This kind of drag parent duplicates the current content.
//Created by James Vanderhyde, 16 March 2022

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragParentDuplicate : DragParent
{
    public override bool DragDuplicate()
    {
        return true;
    }

    public override void ObjectDragged(Draggable d)
    {
        //Do nothing
    }
}
