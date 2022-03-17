//Instantiates mal forms into the UI
//The printing functionality is not spread out among the MalForm prefabs,
//  because in that case there would be self-referential prefabs.
//This component is what makes the build plane, the build plane.
//Created by James Vanderhyde, 7 October 2021

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class MalPrinter : MonoBehaviour
{
    public MalSymbol symbolPrefab;
    public MalKeyword keywordPrefab;
    public MalString stringPrefab;
    public MalNumber numberPrefab;
    public MalBoolean booleanTruePrefab;
    public MalBoolean booleanFalsePrefab;
    public MalList listPrefab;
    public MalVector vectorPrefab;
    public MalMap mapPrefab;
    public MalAnonFunc funcPrefab;
    public MalNil nilPrefab;
    public MalEntity entityPrefab;
    public MalActionState actionStatePrefab;
    public List<MalForm> galleryPrefabs = new List<MalForm>();
    private Dictionary<string, MalForm> galleryMap = null;

    private void buildGalleryMap()
    {
        if (galleryMap == null)
        {
            galleryMap = new Dictionary<string, MalForm>();
            foreach (MalForm f in galleryPrefabs)
                galleryMap.Add(f.galleryItemName, f);
        }
    }

    public MalForm pr_form(types.MalVal tree)
    {
        this.buildGalleryMap();
        try
        {
            return pr_form(tree, this.transform);
        }
        catch (Exception e)
        {
            //We catch all the exceptions to prevent a bug from messing up
            // the entire saved world. Obviously no exceptions should actually
            // occur; the bug causing the exception should be fixed.
            Debug.LogException(e);
            return pr_nil(types.MalNil.malNil, this.transform);
        }
    }

    private MalForm pr_form(types.MalVal tree, Transform contents)
    {
        if (tree is types.MalList)
            return pr_list(tree as types.MalList, contents);
        else if (tree is types.MalVector)
            return pr_vector(tree as types.MalVector, contents);
        else if (tree is types.MalMap)
            return pr_map(tree as types.MalMap, contents);
        else if (tree is types.MalSymbol)
            return pr_symbol(tree as types.MalSymbol, contents);
        else if (tree is types.MalKeyword)
            return pr_keyword(tree as types.MalKeyword, contents);
        else if (tree is types.MalString)
            return pr_string(tree as types.MalString, contents);
        else if (tree is types.MalNumber)
            return pr_number(tree as types.MalNumber, contents);
        else if (tree is types.MalBoolean)
            return pr_boolean(tree as types.MalBoolean, contents);
        else if (tree is types.MalFunc)
            return pr_func(tree as types.MalFunc, contents);
        else if (tree is types.MalNil)
            return pr_nil(tree as types.MalNil, contents);
        else if (tree is types.MalObjectReference)
            return pr_object(tree as types.MalObjectReference, contents);
        else if (tree is types.DelayCall)
            return pr_form((tree as types.DelayCall).Deref(), contents);
        else if (tree is Dollhouse.DollhouseActionState)
            return pr_actionState(tree as Dollhouse.DollhouseActionState, contents);
        else
            throw new ArgumentException("Unknown Mal type in the tree: "+tree.GetType());
    }

    private MalForm pr_symbol(types.MalSymbol tree, Transform contents)
    {
        MalSymbol atom = Instantiate(symbolPrefab, contents);
        atom.SetSymbolName(tree.name);
        return atom;
    }

    private MalForm pr_keyword(types.MalKeyword tree, Transform contents)
    {
        MalKeyword atom = Instantiate(keywordPrefab, contents);
        atom.SetKeywordName(":"+tree.name.Substring(1));
        return atom;
    }

    private MalForm pr_vector(types.MalVector tree, Transform contents)
    {
        MalVector v = Instantiate(vectorPrefab, contents);
        foreach (types.MalVal child in tree)
        {
            pr_form(child, v.transform.GetChild(0));
        }
        return v;
    }

    private MalForm pr_map(types.MalMap tree, Transform contents)
    {
        MalMap m = Instantiate(mapPrefab, contents);
        foreach (types.MalVal child in tree)
        {
            types.MalVal key = (child as types.MalVector).nth(0);
            types.MalVal val = (child as types.MalVector).nth(1);
            MalForm fk = pr_form(key, m.transform.GetChild(0));
            MalForm fv = pr_form(val, m.transform.GetChild(1));
            m.GetComponent<ListManagement>().AddToList(fk.gameObject); //force rebuild
            m.GetComponent<ListManagement>().AddToList(fv.gameObject); //force rebuild
        }
        return m;
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

    private MalForm pr_boolean(types.MalBoolean tree, Transform contents)
    {
        MalBoolean atom;
        if (tree.value)
            atom = Instantiate(booleanTruePrefab, contents);
        else
            atom = Instantiate(booleanFalsePrefab, contents);
        return atom;
    }

    private MalForm pr_list(types.MalList tree, Transform contents)
    {
        if (tree.first() is types.MalSymbol)
        {
            types.MalSymbol s = tree.first() as types.MalSymbol;
            if (this.galleryMap.ContainsKey(s.name))
            {
                MalForm f = Instantiate(this.galleryMap[s.name], contents);
                List<MalForm> childForms = new List<MalForm>();
                foreach (types.MalVal child in tree.rest())
                {
                    childForms.Add(pr_form(child, this.transform));
                }
                f.setChildForms(childForms);
                return f;
            }
        }

        MalList list = Instantiate(listPrefab, contents);
        List<types.MalVal> treeRev = new List<types.MalVal>();
        foreach (types.MalVal child in tree)
            treeRev.Add(child);
        treeRev.Reverse();
        foreach (types.MalVal child in treeRev)
        {
            pr_form(child, list.transform.GetChild(1));
        }
        return list;
    }

    private MalForm pr_func(types.MalFunc tree, Transform contents)
    {
        MalAnonFunc atom = Instantiate(funcPrefab, contents);
        atom.value = tree;
        return atom;
    }

    private MalForm pr_nil(types.MalNil tree, Transform contents)
    {
        MalNil atom = Instantiate(nilPrefab, contents);
        return atom;
    }

    private MalForm pr_object(types.MalObjectReference tree, Transform contents)
    {
        MalEntity atom = Instantiate(entityPrefab, contents);
        atom.value = (Entity)tree.value;
        return atom;
    }

    private MalForm pr_actionState(Dollhouse.DollhouseActionState tree, Transform contents)
    {
        MalActionState atom = Instantiate(actionStatePrefab, contents);
        atom.value = tree;
        return atom;
    }
}
