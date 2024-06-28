//Activates the eval button when the mouse enters an object
//Created by James Vanderhyde, 6 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEvalPrintButton : MoveAbstractEvalButton
{
    public override EvalButtonMover GetButtonMover()
    {
        Canvas canvas = this.gameObject.GetComponentInParent<Canvas>();
        EvalPrintButton epb = canvas.GetComponentInChildren<EvalPrintButton>(true);
        return epb.GetComponent<EvalButtonMover>();
    }
}
