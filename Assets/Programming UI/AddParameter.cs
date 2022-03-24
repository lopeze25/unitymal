//Button event to create a new parameter
//Created by James Vanderhyde, 4 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddParameter : MonoBehaviour
{
    public GameObject symbolPanel;

    public void AddPanel()
    {
        GameObject.Instantiate(symbolPanel, this.transform.parent);
    }
}
