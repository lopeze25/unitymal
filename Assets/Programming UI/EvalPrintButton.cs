//Reads, evaluates, and prints the attached form
//Created by James Vanderhyde, 7 October 2021

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using Mal;

public class EvalPrintButton : MonoBehaviour, IPointerDownHandler
{
    private MalPrinter formPrinter;
    private MalForm result;
    private readonly Vector3 startPositionOffset = new Vector3(30, -15, 0);

    void Awake()
    {
        Canvas canvas = gameObject.GetComponentInParent<Canvas>();
        this.formPrinter = canvas.GetComponentInChildren<MalPrinter>();
        this.result = null;
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        ReadEvalPrint();

        //Set position of new object based on mouse location
        Vector3 globalMousePos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(formPrinter.GetComponent<RectTransform>(), pointerEventData.position, pointerEventData.pressEventCamera, out globalMousePos);
        RectTransform rt = result.GetComponent<RectTransform>();
        rt.position = globalMousePos - this.startPositionOffset;

        //Hide myself, the button
        this.GetComponent<EvalButtonMover>().Hide();
    }

    private void ReadEvalPrint()
    {
        MalForm activeForm = this.GetComponent<EvalButtonMover>().GetActiveForm();
        types.MalVal expression = activeForm.read_form();
        Debug.Log(printer.pr_str(expression));
        SymbolEnvironment environmentComponent = activeForm.transform.GetComponentInParent<SymbolEnvironment>();
        env.Environment environment = env.baseEnvironment;
        if (environmentComponent != null)
            environment = environmentComponent.environment;
        types.MalVal value = evaluator.eval_ast(expression, environment);
        this.result = formPrinter.pr_form(value);
    }
}
