//The number form that appears in the UI
//Created by James Vanderhyde, 12 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mal;

public class MalNumber : MalForm
{
    [SerializeField]
    public float value;

    void Awake()
    {
    }

    void Start()
    {
        //Pick a color for this atom, based on the value
        Image im = GetComponent<Image>();
        if (im)
        {
            float x = (value+Mathf.Floor(0.5f*Mathf.Log10(Mathf.Abs(value)+1)))*0.21f;
            float hue = x - Mathf.Floor(x);
            im.color = Color.HSVToRGB(hue,0.5f,0.8f);
        }

        //Update the text
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = value.ToString();
    }

    public override types.MalVal read_form()
    {
        return new types.MalNumber(value);
    }

}
