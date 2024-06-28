//Updates the namespace panel when define is evaluated
//Created by James Vanderhyde, 28 June 2024

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NamespaceUpdater : MonoBehaviour
{
    private Button evalButton;
    private TMP_InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        this.evalButton = this.GetComponent<MoveEvalActionButton>().GetButtonMover().GetComponent<Button>();
        this.inputField = this.GetComponentInChildren<TMP_InputField>();
        this.evalButton.onClick.AddListener(AddToNamespace);
    }

    void AddToNamespace()
    {
        if (evalButton.GetComponent<EvalButtonMover>().GetActiveForm()==this.GetComponent<MalDefForm>())
        {
            Debug.Log("Add to namespace: "+inputField.text);
        }
    }
}
