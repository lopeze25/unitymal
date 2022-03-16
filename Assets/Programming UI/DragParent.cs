//Defines behavior for what happens when an item is dragged out of something
//Created by James Vanderhyde, 16 March 2022

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DragParent : MonoBehaviour
{
    public abstract bool DragDuplicate();
    public abstract void ObjectDragged(Draggable d);
}
