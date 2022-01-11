//A component that stores a Dollhouse system (program) in a world object
//Created by James Vanderhyde, 17 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DollhouseProgram : MonoBehaviour, IPointerDownHandler
{
    private static DollhouseProgramUI activeUI = null;

    public static DollhouseProgramUI GetActiveProgramUI()
    {
        return activeUI;
    }

    private DollhouseProgramUI programUI;

    void Start()
    {
        programUI = this.GetComponentsInChildren<DollhouseProgramUI>(true)[0];
    }

    void OnMouseDown()
    {
        //Toggle the active state of the corresponding UI.
        GameObject o = this.programUI.gameObject;
        if (o.activeSelf)
        {
            o.SetActive(false);
            DollhouseProgram.activeUI = null;
        }
        else
        {
            if (DollhouseProgram.activeUI != null)
                DollhouseProgram.activeUI.gameObject.SetActive(false);
            o.SetActive(true);
            DollhouseProgram.activeUI = this.programUI;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.OnMouseDown();
    }


}
