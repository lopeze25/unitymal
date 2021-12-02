//Do one thing, wait, and do the rest control structure that appears in the UI
//Created by James Vanderhyde, 22 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;
using TMPro;

public class MalDoOneStructure : MalForm
{
    public override types.MalVal read_form()
    {
        MalForm child0 = this.transform.GetChild(0).GetComponentInChildren<MalForm>();
        MalForm child1 = this.transform.GetChild(1).GetComponentInChildren<MalForm>();

        types.MalList ml = new types.MalList();
        ml.cons(child1.read_form());
        ml.cons(child0.read_form());
        ml.cons(new types.MalObjectReference(this.gameObject)); //Inject the MonoBehaviour
        ml.cons(new types.MalSymbol("do-wait"));

        return ml;
    }
}
