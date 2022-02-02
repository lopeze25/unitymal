//The form that appears in the UI to represent an action executing
//Created by James Vanderhyde, 2 February 2022

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class MalActionState : MalForm
{
    public Dollhouse.DollhouseActionState value;

    IEnumerator Start()
    {
        //Highlight the form until it's done executing
        UnityEngine.UI.Image im = this.GetComponent<UnityEngine.UI.Image>();
        if (im != null && !this.value.IsDone())
        {
            im.color = new Color32(33, 156, 204, 255);
            while (!this.value.IsDone())
                yield return null;
            im.color = new Color32(24, 67, 84, 255);
        }
    }

    public override types.MalVal read_form()
    {
        return value;
    }
}
