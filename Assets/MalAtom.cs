//The atom form that appears in the UI
//Created by James Vanderhyde, 8 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mal;

public class MalAtom : MalForm
{
    [SerializeField]
    public string value;

    void Awake()
    {
        if (value.Equals(""))
            value = gameObject.name;
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
            im.color = Color.HSVToRGB((hue%36)/36f,0.5f,0.8f);
        }
    }

    public override types.MalVal read_form()
    {
        return new types.MalAtom(value);
    }

}
