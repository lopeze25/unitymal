//The user can drag UI objects around the canvas
//This kind of drag parent fills in with items that were previously dropped.
//Created by James Vanderhyde, 28 March 2022

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragParentStack : DragParentReplace
{
    public override void ObjectDragged(Draggable d)
    {
        this.ReplaceWithTopOfStack();
    }

    public void ReplaceWithTopOfStack()
    {
        int stackSize = this.transform.GetChild(0).childCount;
        if (stackSize > 0)
        {
            RectTransform top = (RectTransform)this.transform.GetChild(0).GetChild(stackSize - 1);
            top.SetParent(this.transform);
            top.anchoredPosition = new Vector3(2, -2, 0);
        }
        else
        {
            this.ReplaceWithDefault();
        }
    }

}
