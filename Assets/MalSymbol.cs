//The symbol form that appears in the UI
//Created by James Vanderhyde, 8 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mal;

public class MalSymbol : MalForm
{
    [SerializeField]
    private string symbolName;

    void Start()
    {
        //Use gray for the color, since the value could be anything.
        Image im = GetComponent<Image>();
        if (im)
        {
            im.color = Color.HSVToRGB(0f,0f,0.8f);
        }

        //Update the text
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = symbolName;
    }

    public void SetSymbolName(string symbolName)
    {
        this.symbolName = symbolName;
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = symbolName;

        //Tell the block to resize itself
        ContentSizeFitter fitter = text.GetComponent<ContentSizeFitter>();
        if (fitter)
        {
            fitter.SetLayoutHorizontal();
            fitter.SetLayoutVertical();
        }
        ListManagement lm = GetComponentInParent<ListManagement>();
        if (lm)
            lm.AddToList(this.gameObject);
    }

    public override types.MalVal read_form()
    {
        return new types.MalSymbol(symbolName);
    }

}
