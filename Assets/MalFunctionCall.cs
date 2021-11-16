//Any function call that appears in the UI
//Created by James Vanderhyde, 16 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;
using TMPro;

public class MalFunctionCall : MalForm
{
    public override types.MalVal read_form()
    {
        types.MalList ml = new types.MalList();
        string functionName = transform.GetChild(0).GetComponent<TMP_Text>().text;
        types.MalVal arg1 = transform.GetChild(1).GetComponentInChildren<MalForm>().read_form();
        types.MalVal arg2 = transform.GetChild(2).GetComponentInChildren<MalForm>().read_form();
        ml.cons(arg2);
        ml.cons(arg1);
        ml.cons(new types.MalSymbol(functionName));

        return ml;
    }
}
