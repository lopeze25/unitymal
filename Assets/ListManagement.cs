//Utilities for manipulating a list
//Created by James Vanderhyde, 28 September 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListManagement : MonoBehaviour
{
    public void AddToList(GameObject obj)
    {
        //Add the item
        Transform contents = transform.GetChild(1);
        obj.transform.SetParent(contents);

        //Rebuild the list
        this.Rebuild();
    }

    public void RemoveFromList(GameObject obj)
    {
        //Rebuild the list
        this.Rebuild();
    }

    private void Rebuild()
    {
        //Force rebuild of myself
        RectTransform rt = this.GetComponent<RectTransform>();
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        Debug.Log("Rebuild: " + gameObject.name);

        Transform grandparent = this.transform.parent.parent;
        ListManagement list = grandparent.GetComponent<ListManagement>();
        if (list)
        {
            //Force rebuild of my parent container
            RectTransform rtp = this.transform.parent as RectTransform;
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rtp);

            //Continue up the tree of lists
            list.Rebuild();
        }
    }

    public void Enlarge(float w)
    {
        RectTransform rt = this.GetComponent<RectTransform>();
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w + 10 + rt.rect.width);

        Transform grandparent = this.transform.parent.parent;
        ListManagement list = grandparent.GetComponent<ListManagement>();
        if (list)
            list.Enlarge(w);
    }
}
