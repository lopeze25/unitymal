//The list form that appears in the UI
//Created by James Vanderhyde, 8 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class MalList : MalForm
{
    public override types.MalVal read_form()
    {
        Transform contents = transform.GetChild(1);

        types.MalList ml = new types.MalList();
        for (int i=0; i<contents.childCount; i++)
        {
            MalForm child = contents.GetChild(i).GetComponent<MalForm>();
            types.MalVal value = child.read_form();
            ml.cons(value);
        }

        types.MalList highlight = new types.MalList();
        highlight.cons(ml);
        highlight.cons(new types.MalObjectReference(this.gameObject)); //line of code to highlight
        highlight.cons(new Highlight());

        return highlight;
    }

    public override void setChildForms(List<MalForm> children)
    {
        Transform contents = transform.GetChild(1);

        foreach (MalForm item in children)
        {
            item.transform.SetParent(contents, false);
        }
    }
}
