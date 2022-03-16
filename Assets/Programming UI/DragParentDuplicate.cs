//Duplicate when an item is dragged out of something
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
