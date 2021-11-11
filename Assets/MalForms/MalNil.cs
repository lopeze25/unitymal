//The nil atom form that appears in the UI
//Created by James Vanderhyde, 3 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mal;

public class MalNil : MalForm
{
    public override types.MalVal read_form()
    {
        return types.MalNil.malNil;
    }

}
