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
    public MalSymbol symbolPrefab;
    public MalString stringPrefab;
    public MalNumber numberPrefab;
    public MalList listPrefab;
    public MalAnonFunc funcPrefab;

    public MalForm pr_form(types.MalVal tree)
    {
        return pr_form(tree, this.transform);
    }

    private MalForm pr_form(types.MalVal tree, Transform contents)
    {
        if (tree is types.MalList)
            return pr_list(tree as types.MalCollection, contents);
        else if (tree is types.MalString)
            return pr_string(tree as types.MalString, contents);
        else if (tree is types.MalNumber)
            return pr_number(tree as types.MalNumber, contents);
        else if (tree is types.MalBinaryOperator)
            return pr_func(tree as types.MalBinaryOperator, contents);
        else
            throw new ArgumentException("Unknown Mal type in the tree: "+tree.GetType());
    }

    private MalForm pr_string(types.MalString tree, Transform contents)
    {
        MalString atom = Instantiate(stringPrefab, contents);
        atom.value = tree.value;
        return atom;
    }

    private MalForm pr_number(types.MalNumber tree, Transform contents)
    {
        MalNumber atom = Instantiate(numberPrefab, contents);
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

    private MalForm pr_func(types.MalBinaryOperator tree, Transform contents)
    {
        MalAnonFunc atom = Instantiate(funcPrefab, contents);
        atom.value = tree;
        return atom;
    }
}
