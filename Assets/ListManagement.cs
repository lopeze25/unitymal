//Utility for rebuilding a layout when adding or removing
//Created by James Vanderhyde, 28 September 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListManagement : MonoBehaviour
{
    public void AddToList(GameObject obj)
    {
        Rebuild(obj.transform);
    }

    public void RemoveFromList(GameObject obj)
    {
        Rebuild(obj.transform);
    }

    private void Rebuild(Transform t)
    {
        //Rebuild the layout of the list
        LayoutGroup[] parentLayouts = t.GetComponentsInParent<LayoutGroup>();
        foreach (LayoutGroup layout in parentLayouts)
            RebuildLayout(layout);
        //Debug.Log("Rebuild: "+ parentLayouts.Length+ " parents");
    }

    private void RebuildLayout(LayoutGroup layout)
    {
        //LayoutGroup CalculateLayoutInput sets preferred width and height
        layout.CalculateLayoutInputHorizontal();
        layout.CalculateLayoutInputVertical();

        //ContentSizeFitter SetLayout sets width and height of the RectTransform
        ContentSizeFitter fitter = layout.GetComponent<ContentSizeFitter>();
        if (fitter)
        {
            fitter.SetLayoutHorizontal();
            fitter.SetLayoutVertical();
            //Debug.Log("Calculating layout " + layout.gameObject.name);
        }
    }
}
