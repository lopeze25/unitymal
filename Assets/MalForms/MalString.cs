//The string form that appears in the UI
//Created by James Vanderhyde, 12 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mal;

public class MalString : MalForm
{
    [SerializeField]
    public string value;

    void Awake()
    {
    }

    void Start()
    {
        //Pick a color for this atom, based on the value
        Image im = GetComponent<Image>();
        if (im)
        {
            int hue = 0;
            foreach (char c in value.ToCharArray())
                hue += (int)c;
            im.color = Color.HSVToRGB((hue % 36) / 36f, 0.4f, 0.7f);
        }

        //Update the text
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = value;
    }

    public override types.MalVal read_form()
    {
        return new types.MalString(value);
    }

}
