//The let form form that appears in the UI
//Created by James Vanderhyde, 29 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class MalLetForm : MalForm
{
    public override types.MalVal read_form()
    {
        types.MalList ml = new types.MalList();
        types.MalVal expressionValue = transform.GetChild(1).GetComponentInChildren<MalForm>().read_form();
        ml.cons(expressionValue);

        types.MalVector bindingVector = new types.MalVector();
        TMPro.TMP_InputField[] children = transform.GetChild(0).GetComponentsInChildren<TMPro.TMP_InputField>();
        foreach (TMPro.TMP_InputField field in children)
        {
            bindingVector.conj(new types.MalSymbol(field.text));
            types.MalVal childValue = field.transform.parent.GetChild(2).GetComponentInChildren<MalForm>().read_form();
            bindingVector.conj(childValue);
        }
        ml.cons(bindingVector);

        ml.cons(new types.MalSymbol("let*"));
        
        return ml;
    }
}
