//The fn form form that appears in the UI
//Created by James Vanderhyde, 3 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class MalFnForm : MalForm
{
    public RectTransform unboundSymbolPanel;

    public override types.MalVal read_form()
    {
        types.MalList ml = new types.MalList();
        types.MalVal bodyForm = transform.GetChild(1).GetComponentInChildren<MalForm>().read_form();
        ml.cons(bodyForm);
        types.MalVector bindingVector = new types.MalVector();
        TMPro.TMP_InputField[] children = transform.GetChild(0).GetComponentsInChildren<TMPro.TMP_InputField>();
        foreach (TMPro.TMP_InputField field in children)
        {
            bindingVector.conj(new types.MalSymbol(field.text));
        }
        ml.cons(bindingVector);
        ml.cons(new types.MalSymbol("fn*"));
        
        return ml;
    }

    public override void setChildForms(List<MalForm> children)
    {
        Transform bindingPanel = this.transform.GetChild(0);

        List<string> symbolNames = new List<string>();
        Transform vecContents = children[0].transform.GetChild(0);
        for (int i = 0; i < vecContents.childCount; i++)
        {
            symbolNames.Add(vecContents.GetChild(i).GetComponent<MalSymbol>().GetSymbolName());
        }
        for (int i = 0; i < symbolNames.Count; i++)
        {
            RectTransform rt = GameObject.Instantiate(unboundSymbolPanel, bindingPanel);
            rt.GetChild(0).GetComponent<TMPro.TMP_InputField>().text = symbolNames[i];
            rt.GetChild(1).GetComponentInChildren<MalSymbol>().SetSymbolName(symbolNames[i]);
        }
        children[0].transform.SetParent(null);
        GameObject.Destroy(children[0]);

        this.Replace(this.transform.GetChild(1).GetComponentInChildren<MalForm>(), children[1]);
    }
}
