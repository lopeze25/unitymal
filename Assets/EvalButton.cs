//Controls how the evaluate button appears and disappears
//Created by James Vanderhyde, 7 October 2021

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using Mal;

public class EvalButton : MonoBehaviour, IPointerExitHandler, IPointerDownHandler
{
    private MalForm form;
    private MalPrinter printer;
    private MalForm result;
    private readonly Vector3 startPositionOffset = new Vector3(30, -15, 0);

    void Awake()
    {
        Canvas canvas = gameObject.GetComponentInParent<Canvas>();
        this.printer = canvas.GetComponentInChildren<MalPrinter>();
        this.form = null;
        this.result = null;
    }

    public void Show(GameObject form)
    {
        this.gameObject.SetActive(true);
        this.form = form.GetComponent<MalForm>();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        this.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        ReadEvalPrint();

        //Set position
        Vector3 globalMousePos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(printer.GetComponent<RectTransform>(), pointerEventData.position, pointerEventData.pressEventCamera, out globalMousePos);
        RectTransform rt = result.GetComponent<RectTransform>();
        rt.position = globalMousePos - this.startPositionOffset;

        //Hide myself, the button
        this.gameObject.SetActive(false);
    }

    private void ReadEvalPrint()
    {
        types.MalVal expression = form.read_form();
        types.MalVal value = EVAL(expression, null);
        this.result = printer.pr_form(value);
    }

    // eval
    static types.MalVal EVAL(types.MalVal ast, string env)
    {
        return ast;
    }
}
