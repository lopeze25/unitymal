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
        MalForm activeForm = this.GetComponent<EvalButtonMover>().GetActiveForm();
        types.MalVal expression = activeForm.read_form();
        Debug.Log(printer.pr_str(expression));
        SymbolEnvironment environmentComponent = activeForm.transform.GetComponentInParent<SymbolEnvironment>();
        env.Environment environment = env.baseEnvironment;
        if (environmentComponent != null)
            environment = environmentComponent.environment;
        types.MalVal value = evaluator.eval_ast(expression, environment);
        if (value is types.DelayCall)
            value = (value as types.DelayCall).Deref(); //Start the action
        //Value is ignored.
    }
}
