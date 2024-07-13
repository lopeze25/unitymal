//Any function call UI for a user defined function
//Created by James Vanderhyde, 12 July 2024

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;
using TMPro;

public class MalUserFunctionCall : MalForm
{
    [SerializeField]
    private DragParentReplace parameterPrefab;
    [SerializeField]
    private MalForm parameterDefault;

    public void SetNameAndParameters(string name, types.MalCollection parameters)
    {
        TMP_Text nameText = this.transform.GetChild(0).GetComponent<TMP_Text>();
        nameText.text = name;

        foreach (types.MalVal p in parameters)
        {
            DragParentReplace param = GameObject.Instantiate(parameterPrefab,this.transform);
            param.defaultValue = GameObject.Instantiate(parameterDefault,param.transform);
        }
    }

    public override types.MalVal read_form()
    {
        types.MalList ml = new types.MalList();
        for (int i=transform.childCount-1; i>=1; i--)
        {
            types.MalVal arg = transform.GetChild(i).GetComponentInChildren<MalForm>().read_form();
            ml.cons(arg);
        }
        string functionName = this.transform.GetChild(0).GetComponent<TMP_Text>().text;
        ml.cons(new types.MalSymbol(functionName));

        return ml;
    }

    public override void setChildForms(List<MalForm> children)
    {
        for (int i=0; i<children.Count; i++)
            this.Replace(transform.GetChild(i+1).GetComponentInChildren<MalForm>(), children[i]);
    }
}
