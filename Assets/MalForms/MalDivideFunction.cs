//The / function form that appears in the UI
//Created by James Vanderhyde, 21 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class MalDivideFunction : MalForm
{
    public override types.MalVal read_form()
    {
        types.MalList ml = new types.MalList();
        types.MalVal childNumerator = transform.GetChild(0).GetComponentInChildren<MalForm>().read_form();
        types.MalVal childDenominator = transform.GetChild(2).GetComponentInChildren<MalForm>().read_form();
        ml.cons(childDenominator);
        ml.cons(childNumerator);
        ml.cons(new types.MalSymbol("/"));

        return ml;
    }

    public override void setChildForms(List<MalForm> children)
    {
        this.Replace(transform.GetChild(0).GetComponentInChildren<MalForm>(), children[0]);
        this.Replace(transform.GetChild(2).GetComponentInChildren<MalForm>(), children[1]);
    }
}
