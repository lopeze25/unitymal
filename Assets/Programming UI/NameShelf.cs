//Part of the programming UI gallery that can add names
//Created by James Vanderhyde, 3 July 2024

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameShelf : MonoBehaviour
{
    [SerializeField]
    private GameObject symbolForm;
    private Dictionary<string,GameObject> nameList = new();
    private Vector3 formPosition = new Vector3(12,-200,0);

    public void AddToShelf(String name)
    {
        if (!this.nameList.ContainsKey(name))
        {
            GameObject nameForm = GameObject.Instantiate(symbolForm,this.transform);
            nameForm.transform.localPosition = formPosition;
            formPosition.y -= 55;
            if (formPosition.y < -750)
            {
                formPosition.y += 750-165;
                formPosition.x += 200;
            }
            nameForm.GetComponent<MalSymbol>().SetSymbolName(name);
            this.nameList.Add(name,nameForm);
        }
    }

}
