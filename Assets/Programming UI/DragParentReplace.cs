//The user can drag UI objects around the canvas
//This kind of drag parent fills in with a default place holder.
//Created by James Vanderhyde, 15 October 2021
//Split from ReplacingDropTarget, 16 March 2022

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragParentReplace : DragParent
{
    public MalForm defaultValue;

    public override bool DragDuplicate()
    {
        return false;
    }

    public override void ObjectDragged(Draggable d)
    {
        this.ReplaceWithDefault();
    }

    public void ReplaceWithDefault()
    {
        RectTransform rt = (RectTransform)Object.Instantiate(this.defaultValue, this.transform).transform;
        rt.anchoredPosition = new Vector3(2, -2, 0);
    }
}
