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
    public string symbolName;

    void Awake()
    {
        if (symbolName.Equals(""))
            symbolName = gameObject.name;
    }

    void Start()
    {
        //Pick a color for this atom, based on the name
        Image im = GetComponent<Image>();
        if (im)
        {
            int hue = 0;
            foreach (char c in symbolName.ToCharArray())
                hue += (int)c;
            im.color = Color.HSVToRGB((hue%36)/36f,0.7f,0.8f);
        }

        //Update the text
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = symbolName;
    }

    public override types.MalVal read_form()
    {
        return new types.MalSymbol(symbolName);
    }

}
