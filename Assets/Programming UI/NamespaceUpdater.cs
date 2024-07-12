//Updates the namespace panel when define is evaluated
//Created by James Vanderhyde, 28 June 2024

using System.Collections;
using System.Collections.Generic;
using Mal;
using UnityEngine;
using UnityEngine.UI;

public class NamespaceUpdater : MonoBehaviour
{
    private Button evalButton;
    private NameShelf nameShelf;

    // Start is called before the first frame update
    void Start()
    {
        this.evalButton = this.GetComponent<MoveEvalActionButton>().GetButtonMover().GetComponent<Button>();
        this.nameShelf = this.GetComponentInParent<DollhouseProgramUI>().GetComponentInChildren<NameShelf>();
        this.evalButton.onClick.AddListener(AddToNamespace);
    }

    void AddToNamespace()
    {
        //This method is called every time the evaluate button is clicked.
        //So we need to check if the user is actually evaluating this define form.
        if (evalButton.GetComponent<EvalButtonMover>().GetActiveForm()==this.GetComponent<MalDefForm>())
        {
            //Get the MAL code from the def form
            MalDefForm activeForm = this.GetComponent<MalDefForm>();
            types.MalVal expression = activeForm.read_form();
            string codeText = printer.pr_str(Highlight.removeHighlights(expression));

            //Save it in the shelf
            this.nameShelf.AddToShelf(activeForm.GetSymbolName(), codeText);
        }
    }
}
