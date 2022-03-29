//The user can drag UI objects around the canvas
//This kind of drop target holds items in the trash.
//Created by James Vanderhyde, 25 March 2022

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrashDropTarget : DropTarget
{
    public override void HandleDrop(Transform droppedObject)
    {
        this.PutInTrash(droppedObject);
    }

    public void PutInTrash(Transform trashedObject)
    {
        //Move the old contents to storage
        Transform replaced = this.transform.GetChild(1);
        replaced.SetParent(this.transform.GetChild(0));

        //Add the new contents
        trashedObject.SetParent(this.transform);
        trashedObject.localPosition = new Vector3(2, -2, 0);

        //Resize
        LayoutGroup layout = this.GetComponent<LayoutGroup>();
        layout.CalculateLayoutInputHorizontal();
        layout.CalculateLayoutInputVertical();
        ContentSizeFitter fitter = this.GetComponent<ContentSizeFitter>();
        fitter.SetLayoutHorizontal();
        fitter.SetLayoutVertical();
    }

}
