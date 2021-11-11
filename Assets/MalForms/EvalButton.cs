//Reads and evaluates the attached form
//Created by James Vanderhyde, 29 October 2021

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using Mal;

public class EvalButton : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        ReadEval();
    }

    private void ReadEval()
    {
        types.MalVal expression = this.GetComponent<EvalButtonMover>().GetActiveForm().read_form();
        Debug.Log(printer.pr_str(expression));
        types.MalVal value = evaluator.eval_ast(expression, env.baseEnvironment);
        //Value is ignored.
    }
}
