//Utilities for manipulating a function call list
//Created by James Vanderhyde, 15 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuncManagement : ListManagement
{
    public override void AddToList(GameObject obj)
    {
        //Add the item

        //Rebuild the list
        this.Rebuild();
    }

}
