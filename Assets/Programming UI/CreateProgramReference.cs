//Allows 3D objects in the world to be dragged into the UI
//Created by James Vanderhyde, 12 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using Mal;

public class CreateProgramReference : MonoBehaviour, IPointerDownHandler
{
    void OnMouseDown()
    {
        //Find the active programming UI
        DollhouseProgramUI programUI = DollhouseProgram.GetActiveProgramUI();
        if (programUI != null)
        {
            //Create the Mal form
            MalEntity result;
            result = (MalEntity)programUI.transform.GetComponentInChildren<MalPrinter>().pr_form(new types.MalObjectReference(this.GetComponent<Entity>()));
        }
        else
        {
            Entity e = this.GetComponent<Entity>();
            GetComponentInParent<SaveLoad>().Load("(remove-entity {:guid \""+e.guid+"\"})");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.OnMouseDown();
    }

    public void OnVRSelect(SelectEnterEventArgs eventData)
    {
        this.OnMouseDown();
    }

}
