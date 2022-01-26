//The vector form that appears in the UI
//Created by James Vanderhyde, 26 January 2022

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class MalVector : MalForm
{
    public override types.MalVal read_form()
    {
        Transform contents = transform.GetChild(0);

        types.MalVector mv = new types.MalVector();
        for (int i=0; i<contents.childCount; i++)
        {
            MalForm child = contents.GetChild(i).GetComponent<MalForm>();
            types.MalVal value = child.read_form();
            mv.conj(value);
        }

        return mv;
    }
}
