//The map form that appears in the UI
//Created by James Vanderhyde, 27 January 2022

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class MalMap : MalForm
{
    public override types.MalVal read_form()
    {
        Transform keys = transform.GetChild(0);
        Transform vals = transform.GetChild(1);

        types.MalMap m = new types.MalMap();
        for (int i = 0; i < keys.childCount; i++)
        {
            MalForm childKey = keys.GetChild(i).GetComponent<MalForm>();
            MalForm childVal = vals.GetChild(i).GetComponent<MalForm>();
            types.MalVal k = childKey.read_form();
            types.MalVal v = childVal.read_form();
            m.assoc(k, v);
        }

        return m;
    }



}
