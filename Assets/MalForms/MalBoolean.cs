//The Boolean form that appears in the UI
//Created by James Vanderhyde, 5 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mal;

public class MalBoolean : MalForm
{
    [SerializeField]
    public bool value;

    void Awake()
    {
    }

    void Start()
    {
        //Pick a color for this atom, based on the value
        Image im = GetComponent<Image>();
        if (im)
        {
            im.color = Color.HSVToRGB(0.15f,value?0.4f:0.2f, value?0.8f:0.4f);
        }

        //Update the text
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = value?"true":"false";
    }

    public override types.MalVal read_form()
    {
        return new types.MalBoolean(value);
    }

}
