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
        GameObject newPanel = GameObject.Instantiate(symbolPanel, this.transform.parent);
        TMPro.TMP_InputField field = newPanel.GetComponentInChildren<TMPro.TMP_InputField>();
        SymbolTracker st = newPanel.GetComponent<SymbolTracker>();
        field.onEndEdit.AddListener(st.CreateSymbolFromField);
    }
}
