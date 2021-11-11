//The anonymous function form that appears in the UI
//Created by James Vanderhyde, 14 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mal;

public class MalAnonFunc : MalForm
{
    [SerializeField]
    public types.MalFunc value;

    void Awake()
    {
    }

    void Start()
    {
        //Pick a color for this atom, based on the value
    }

    public override types.MalVal read_form()
    {
        return value;
    }

}
