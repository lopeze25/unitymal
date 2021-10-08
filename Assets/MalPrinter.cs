//Instantiates mal forms into the UI
//There is only one MalPrinter object, because otherwise we will have self-referential prefabs.
//Created by James Vanderhyde, 7 October 2021

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class MalPrinter : MonoBehaviour
{
    public MalAtom atomPrefab;
    public MalList listPrefab;

    public MalForm pr_form(types.MalVal tree)
    {
        return pr_form(tree, this.transform);
    }

    private MalForm pr_form(types.MalVal tree, Transform contents)
    {
        if (tree is types.MalList)
            return pr_list(tree as types.MalCollection, contents);
        else if (tree is types.MalAtom)
            return pr_atom(tree as types.MalAtom, contents);
        else
            throw new ArgumentException("Unknown Mal type in the tree");
    }

    private MalForm pr_atom(types.MalAtom tree, Transform contents)
    {
        MalAtom atom = Instantiate(atomPrefab, contents);
        atom.value = tree.value;
        return atom;
    }

    private MalForm pr_list(types.MalCollection tree, Transform contents)
    {
        MalList list = Instantiate(listPrefab, contents);
        foreach (types.MalVal child in tree)
        {
            pr_form(child, list.transform.GetChild(1));
        }
        return list;
    }
}
