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
    private MalPrinter printer;
    private MalForm result;
    private readonly Vector3 startPositionOffset = new Vector3(30, -15, 0);

    void Awake()
    {
        Canvas canvas = gameObject.GetComponentInParent<Canvas>();
        this.printer = canvas.GetComponentInChildren<MalPrinter>();
        this.result = null;
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        ReadEvalPrint();

        //Set position of new object based on mouse location
        Vector3 globalMousePos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(printer.GetComponent<RectTransform>(), pointerEventData.position, pointerEventData.pressEventCamera, out globalMousePos);
        RectTransform rt = result.GetComponent<RectTransform>();
        rt.position = globalMousePos - this.startPositionOffset;

        //Hide myself, the button
        this.gameObject.SetActive(false);
    }

    private void ReadEvalPrint()
    {
        types.MalVal expression = this.GetComponent<EvalButtonMover>().GetActiveForm().read_form();
        types.MalVal value = evaluator.eval_ast(expression, env.baseEnvironment);
        this.result = printer.pr_form(value);
    }
}
