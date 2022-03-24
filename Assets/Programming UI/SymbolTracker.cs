//A component on a symbol creator (a parameter) in the scope of a defining form
//Created by James Vanderhyde, 4 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SymbolTracker : MonoBehaviour
{
    void Awake()
    {
        TMPro.TMP_InputField field = this.GetComponentInChildren<TMPro.TMP_InputField>();
        field.onEndEdit.AddListener(this.CreateSymbolFromField);
    }

    public string GetSymbolName()
    {
        return this.transform.GetChild(1).GetComponentInChildren<MalSymbol>().GetSymbolName();
    }

    private List<MalSymbol> GetSymbols()
    {
        //Get all the symbols
        Transform df = this.GetComponentInParent<DefiningForm>().transform;
        MalSymbol[] symbols = df.GetComponentsInChildren<MalSymbol>();

        //Find symbols that match the name
        List<MalSymbol> nameMatches = new List<MalSymbol>();
        foreach (MalSymbol s in symbols)
            if (s.GetSymbolName().Equals(this.GetSymbolName()))
                nameMatches.Add(s);

        return nameMatches;

        //Filter out the ones whose defining form is not this one's.
    }

    public void CreateSymbolFromField(string symbolName)
    {
        //Pull the new name from the input field, if not specified by the parameter
        if (symbolName.Equals(""))
            symbolName = this.transform.GetChild(0).GetComponentInChildren<TMPro.TMP_InputField>().text;

        //Get all the symbols in child forms that match the names
        List<MalSymbol> symbols = this.GetSymbols();

        //Rename all the symbols in child forms
        foreach (MalSymbol s in symbols)
            s.SetSymbolName(symbolName);
    }
}
