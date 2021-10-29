//The let form form that appears in the UI
//Created by James Vanderhyde, 29 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class MalLetForm : MalForm
{
    public override types.MalVal read_form()
    {
        types.MalList ml = new types.MalList();
        string childName = transform.GetChild(0).GetComponentInChildren<TMPro.TMP_InputField>().text;
        types.MalVal childValue = transform.GetChild(0).GetChild(3).GetComponentInChildren<MalForm>().read_form();
        types.MalVal expressionValue = transform.GetChild(1).GetComponentInChildren<MalForm>().read_form();
        ml.cons(expressionValue);
        types.MalList bindingList = new types.MalList();
        bindingList.cons(childValue);
        bindingList.cons(new types.MalSymbol(childName));
        ml.cons(bindingList);
        ml.cons(new types.MalSymbol("let*"));
        
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
