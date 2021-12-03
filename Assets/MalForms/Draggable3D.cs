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
        this.formPrinter = GameObject.Find("Canvas").transform.GetComponentInChildren<MalPrinter>();
    }

    void OnMouseDown()
    {
        //Create the Mal form
        MalEntity result;
        result = (MalEntity)formPrinter.pr_form(new types.MalObjectReference(this.gameObject));
    }

}
