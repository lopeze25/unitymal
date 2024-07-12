//The def form form that appears in the UI
//Created by James Vanderhyde, 25 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class MalDefForm : MalForm
{
    private TMPro.TMP_InputField inputField;

    void Awake()
    {
        this.inputField = this.transform.GetChild(0).GetComponentInChildren<TMPro.TMP_InputField>();
    }

    public override types.MalVal read_form()
    {
        types.MalList ml = new types.MalList();
        string childName = this.inputField.text;
        types.MalVal childValue = transform.GetChild(1).GetComponentInChildren<MalForm>().read_form();
        ml.cons(childValue);
        ml.cons(new types.MalSymbol(childName));
        ml.cons(new types.MalSymbol("def!"));
        
        return ml;
    }

    public override void setChildForms(List<MalForm> children)
    {
        this.Replace(this.transform.GetChild(0).GetComponentInChildren<MalForm>(), children[0]);
        this.inputField.text = (children[0] as MalSymbol).GetSymbolName();
        this.Replace(this.transform.GetChild(1).GetComponentInChildren<MalForm>(), children[1]);
    }

    public string GetSymbolName()
    {
        return this.inputField.text;
    }
}
