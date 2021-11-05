//The fn form form that appears in the UI
//Created by James Vanderhyde, 3 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class MalFnForm : MalForm
{
    public override types.MalVal read_form()
    {
        types.MalList ml = new types.MalList();
        types.MalVal bodyForm = transform.GetChild(1).GetComponentInChildren<MalForm>().read_form();
        ml.cons(bodyForm);
        types.MalVector bindingVector = new types.MalVector();
        TMPro.TMP_InputField[] children = transform.GetChild(0).GetComponentsInChildren<TMPro.TMP_InputField>();
        foreach (TMPro.TMP_InputField field in children)
        {
            bindingVector.conj(new types.MalSymbol(field.text));
        }
        ml.cons(bindingVector);
        ml.cons(new types.MalSymbol("fn*"));
        
        return ml;
    }
}
