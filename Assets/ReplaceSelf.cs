//The user can drag UI objects around the canvas
//This object replaces itself with a copy.
//Created by James Vanderhyde, 15 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReplaceSelf : MonoBehaviour
{
    public void Replace(Transform parent)
    {
        //Duplicate this object
        RectTransform rt = (RectTransform)Object.Instantiate(this, parent).transform;

        //Remove ReplaceSelf component
        Component.Destroy(this);
    }
}
