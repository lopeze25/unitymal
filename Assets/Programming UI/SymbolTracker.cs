//A component on a symbol creator (a parameter) in the scope of a defining form
//Created by James Vanderhyde, 4 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SymbolTracker : MonoBehaviour
{
    private List<TieToTracker> listOfForms = new List<TieToTracker>();

    public string GetSymbolName()
    {
        return this.transform.GetChild(1).GetComponentInChildren<MalSymbol>().GetSymbolName();
    }

    public void AddToListOfSymbolForms(TieToTracker symbolForm)
    {
        listOfForms.Add(symbolForm);
    }

    public void CreateSymbolFromField(string symbolName)
    {
        MalSymbol symbol = this.GetComponentInChildren<MalSymbol>();
        if (symbolName.Equals(""))
            symbolName = transform.GetChild(0).GetComponentInChildren<TMPro.TMP_InputField>().text;
        symbol.SetSymbolName(symbolName);
        foreach (TieToTracker ttd in listOfForms)
        {
            ttd.GetComponent<MalSymbol>().SetSymbolName(symbolName);
        }
    }
}
