//The user can drag UI objects around the canvas
//This kind of drop target has a default place holder.
//Created by James Vanderhyde, 15 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReplacingDropTarget : DropTarget
{
    public MalForm defaultValue;

    public override void OnDrop(PointerEventData data)
    {
        if (data.pointerDrag != null)
        {
            //Change the highlight back
            SetHighlight(false);

            //Destroy the old contents
            Transform replaced = this.transform.GetChild(0);
            replaced.SetParent(null);
            Object.Destroy(replaced.gameObject);

            //Add the new contents
            data.pointerDrag.transform.SetParent(this.transform);
            data.pointerDrag.transform.localPosition = new Vector3(2, -2, 0);

            //Tell the block to resize itself
            ListManagement lm = GetComponentInParent<ListManagement>();
            if (lm)
                lm.AddToList(data.pointerDrag);
        }
    }

    public void ReplaceWithDefault()
    {
        Transform buildPlane = gameObject.GetComponentInParent<MalPrinter>().transform;
        //RectTransform rt = (RectTransform)Object.Instantiate(defaultValue, this.transform).transform;
        RectTransform rt = (RectTransform)Object.Instantiate(defaultValue, buildPlane).transform;
        rt.SetParent(this.transform);
        rt.anchoredPosition = new Vector3(2, -2, 0);
    }
}
