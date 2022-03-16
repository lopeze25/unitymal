//The user can drag UI objects around the canvas
//This kind of drop target inserts into a specific target
//Created by James Vanderhyde, 28 September 2021
//Extracted from DropTarget, 16 March 2022

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsertingDropTarget : DropTarget
{
    public Transform target;

    public override void HandleDrop(Transform droppedObject)
    {
        //Add the new contents to the target
        droppedObject.SetParent(this.target);
        droppedObject.localPosition = new Vector3(2, -2, 0);
    }
}
