using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefiningForm : MonoBehaviour
{
    private List<TieToDefine> listOfForms = new List<TieToDefine>();

    public void AddToListOfSymbolForms(TieToDefine symbolForm)
    {
        listOfForms.Add(symbolForm);
    }

    public void ChangeSymbolNames(string symbolName)
    {
        foreach (TieToDefine ttd in listOfForms)
        {
            ttd.GetComponent<MalSymbol>().SetSymbolName(symbolName);
        }
    }
}
