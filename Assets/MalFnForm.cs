//The fn form form that appears in the UI
//Created by James Vanderhyde, 3 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class MalFnForm : MalForm
{
    public override types.MalVal read_form()
    {
        types.MalList ml = new types.MalList();
        string childName = transform.GetChild(0).GetComponentInChildren<TMPro.TMP_InputField>().text;
        types.MalVal bodyForm = transform.GetChild(1).GetComponentInChildren<MalForm>().read_form();
        ml.cons(bodyForm);
        types.MalList bindingList = new types.MalList();
        bindingList.cons(new types.MalSymbol(childName));
        ml.cons(bindingList);
        ml.cons(new types.MalSymbol("fn*"));
        
        return ml;
    }

    public void CreateSymbolFromField()
    {
        string symbolName = transform.GetChild(0).GetComponentInChildren<TMPro.TMP_InputField>().text;
        MalSymbol symbol = transform.GetChild(0).GetChild(2).GetComponentInChildren<MalSymbol>();
        symbol.SetSymbolName(symbolName);

        DefiningForm list = this.GetComponent<DefiningForm>();
        list.ChangeSymbolNames(symbolName);
    }
}
