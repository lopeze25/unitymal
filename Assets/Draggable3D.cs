//Allows 3D objects in the world to be dragged into the UI
//Created by James Vanderhyde, 12 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class Draggable3D : MonoBehaviour
{
    private MalPrinter formPrinter;

    void Start()
    {
        Transform bp = GameObject.Find("Canvas").transform.Find("build plane");
        this.formPrinter = bp.GetComponent<MalPrinter>();
    }

    void OnMouseDown()
    {
        MalForm result;
        result = formPrinter.pr_form(new types.MalObjectReference(this.gameObject));
        RectTransform rt = result.GetComponent<RectTransform>();

    }
}