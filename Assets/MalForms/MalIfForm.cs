//The if form form that appears in the UI
//Created by James Vanderhyde, 6 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class MalIfForm : MalForm
{
    public override types.MalVal read_form()
    {
        types.MalList ml = new types.MalList();
        types.MalVal elseValue = transform.GetChild(2).GetComponentInChildren<MalForm>().read_form();
        ml.cons(elseValue);
        types.MalVal thenValue = transform.GetChild(1).GetComponentInChildren<MalForm>().read_form();
        ml.cons(thenValue);
        types.MalVal conditionValue = transform.GetChild(0).GetComponentInChildren<MalForm>().read_form();
        ml.cons(conditionValue);
        ml.cons(new types.MalSymbol("if"));
        
        return ml;
    }

    public override void setChildForms(List<MalForm> children)
    {
        this.Replace(transform.GetChild(2).GetComponentInChildren<MalForm>(), children[2]);
        this.Replace(transform.GetChild(1).GetComponentInChildren<MalForm>(), children[1]);
        this.Replace(transform.GetChild(0).GetComponentInChildren<MalForm>(), children[0]);
    }

}
