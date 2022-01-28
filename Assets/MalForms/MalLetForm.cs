//The let form form that appears in the UI
//Created by James Vanderhyde, 29 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class MalLetForm : MalForm
{
    public RectTransform boundSymbolPanel;

    public override types.MalVal read_form()
    {
        types.MalList ml = new types.MalList();
        types.MalVal expressionValue = transform.GetChild(1).GetComponentInChildren<MalForm>().read_form();
        ml.cons(expressionValue);

        types.MalVector bindingVector = new types.MalVector();
        TMPro.TMP_InputField[] children = transform.GetChild(0).GetComponentsInChildren<TMPro.TMP_InputField>();
        foreach (TMPro.TMP_InputField field in children)
        {
            bindingVector.conj(new types.MalSymbol(field.text));
            types.MalVal childValue = field.transform.parent.GetChild(2).GetComponentInChildren<MalForm>().read_form();
            bindingVector.conj(childValue);
        }
        ml.cons(bindingVector);

        ml.cons(new types.MalSymbol("let*"));
        
        return ml;
    }

    public override void setChildForms(List<MalForm> children)
    {
        Transform bindingPanel = this.transform.GetChild(0);

        List<string> symbolNames = new List<string>();
        List<MalForm> symbolValues = new List<MalForm>();
        Transform vecContents = children[0].transform.GetChild(0);
        for (int i=0; i+1 < vecContents.childCount; i += 2)
        {
            symbolNames.Add(vecContents.GetChild(i).GetComponent<MalSymbol>().GetSymbolName());
            symbolValues.Add(vecContents.GetChild(i+1).GetComponent<MalForm>());
        }
        for (int i=0; i<symbolNames.Count; i++)
        {
            RectTransform rt = GameObject.Instantiate(boundSymbolPanel, bindingPanel);
            rt.GetChild(0).GetComponent<TMPro.TMP_InputField>().text = symbolNames[i];
            rt.GetChild(1).GetComponentInChildren<MalSymbol>().SetSymbolName(symbolNames[i]);
            this.Replace(rt.GetChild(2).GetComponentInChildren<MalForm>(), symbolValues[i]);
        }
        children[0].transform.SetParent(null);
        GameObject.Destroy(children[0].gameObject);

        this.Replace(this.transform.GetChild(1).GetComponentInChildren<MalForm>(), children[1]);
    }
}
