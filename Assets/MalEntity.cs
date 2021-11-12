//The entity (game object) form that appears in the UI
//Created by James Vanderhyde, 11 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mal;

public class MalEntity : MalForm
{
    [SerializeField]
    public GameObject value;

    void Awake()
    {
    }

    void Start()
    {
        Image im = GetComponent<Image>();
        if (im)
        {
            im.color = Color.HSVToRGB(0.95f, 0.2f, 0.8f);
        }
    }

    public override types.MalVal read_form()
    {
        return new types.MalObjectReference(value);
    }

}
