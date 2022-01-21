//The * function form that appears in the UI
//Created by James Vanderhyde, 21 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class MalMultiplyFunction : MalForm
{
    public override types.MalVal read_form()
    {
        types.MalList ml = new types.MalList();
        types.MalVal childLHS = transform.GetChild(0).GetComponentInChildren<MalForm>().read_form();
        types.MalVal childRHS = transform.GetChild(2).GetComponentInChildren<MalForm>().read_form();
        ml.cons(childRHS);
        ml.cons(childLHS);
        ml.cons(new types.MalSymbol("*"));

        return ml;
    }
}
