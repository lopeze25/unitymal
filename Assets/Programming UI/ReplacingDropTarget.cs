//The user can drag UI objects around the canvas
//This kind of drop target has a default place holder.
//Created by James Vanderhyde, 15 October 2021
//Split from ReplacingDropTarget, 16 March 2022

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplacingDropTarget : DropTarget
{
    private TrashDropTarget trash = null;

    void Awake()
    {
        DollhouseProgramUI c = this.GetComponentInParent<DollhouseProgramUI>();
        if (c != null)
        {
            this.trash = c.transform.Find("trash").GetComponent<TrashDropTarget>();
        }
    }

    public override void HandleDrop(Transform droppedObject)
    {
        //Trash or destroy the old contents
        Transform replaced = this.transform.GetChild(0);
        if (this.trash)
            this.trash.PutInTrash(replaced);
        else
        {
            replaced.SetParent(null);
            Object.Destroy(replaced.gameObject);
        }

        //Add the new contents
        droppedObject.SetParent(this.transform);
        droppedObject.localPosition = new Vector3(2, -2, 0);
    }

}
