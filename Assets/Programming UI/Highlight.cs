//MAL command to highlight a line of code while it executes
//Created by James Vanderhyde, 18 January 2021

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class Highlight : types.MalFunc
{
    private static IEnumerator highlight(Dollhouse.DollhouseActionState actionState, MalForm component)
    {
        UnityEngine.UI.Image im = component.GetComponent<UnityEngine.UI.Image>();
        if (im != null)
        {
            if (actionState.IsValid())
            {
                im.color = new Color32(253, 229, 154, 255);
                while (!actionState.IsDone())
                    yield return null;
                im.color = new Color32(185, 185, 185, 255);
            }
            else
            {
                //error state, color red
                im.color = new Color32(253, 165, 154, 255);
            }
        }
        //This will have a problem when multiple coroutines are highlighting the same code.
        //Suggested fix: Add Highlight/Dehighlight methods to MalForm, and handle multiple requests.
    }

    public override types.MalVal apply(types.MalList arguments)
    {
        //Parse the arguments
        if (arguments.isEmpty() || arguments.rest().isEmpty())
            throw new ArgumentException("highlight is missing a value.");
        if (!(arguments.first() is types.MalObjectReference))
            throw new ArgumentException("First argument to highlight must be an instruction in the programming UI.");
        types.MalObjectReference mor = arguments.first() as types.MalObjectReference;
        GameObject obj = (GameObject)mor.value;
        MalForm component = obj.GetComponent<MalForm>();
        if (component == null)
            throw new ArgumentException("First argument to highlight must be an instruction in the programming UI.");
        if (!(arguments.rest().first() is Dollhouse.DollhouseActionState))
            return arguments.rest().first(); //just return the value if it's not a Dollhouse action
        Dollhouse.DollhouseActionState state = arguments.rest().first() as Dollhouse.DollhouseActionState;

        //Start the coroutine to highlight, wait for the instruction to finish, and dehighlight
        component.StartCoroutine(highlight(state, component));
        return state;
    }

    public static types.MalVal removeHighlights(types.MalVal tree)
    {
        if (tree is types.MalList)
        {
            types.MalList list = tree as types.MalList;
            if (list.isEmpty())
                return list;
            if (list.first() is Highlight)
                return removeHighlights(list.rest().rest().first());
            types.MalList r = removeHighlights(list.rest()) as types.MalList;
            r.cons(removeHighlights(list.first()));
            return r;
        }
        //For now we're assuming highlights can only be buried in lists, not in other collections.
        return tree;
    }
}
