//The def form form that appears in the UI
//Created by James Vanderhyde, 25 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class MalDefForm : MalForm
{
    public override types.MalVal read_form()
    {
        types.MalList ml = new types.MalList();
        string childName = transform.GetChild(0).GetComponentInChildren<TMPro.TMP_InputField>().text;
        types.MalVal childValue = transform.GetChild(1).GetComponentInChildren<MalForm>().read_form();
        ml.cons(childValue);
        ml.cons(new types.MalSymbol(childName));
        ml.cons(new types.MalSymbol("def!"));
        
        return ml;
    }
}
