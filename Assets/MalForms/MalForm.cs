//The parent class of all forms that appear in the UI
//Created by James Vanderhyde, 8 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public abstract class MalForm : MonoBehaviour
{
    public string galleryItemName;

    public abstract types.MalVal read_form();
}
