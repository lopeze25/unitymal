//Activates the action button when the mouse enters an object
//Created by James Vanderhyde, 6 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEvalActionButton : MoveAbstractEvalButton
{
    public override EvalButtonMover GetButtonMover()
    {
        Canvas canvas = this.gameObject.GetComponentInParent<Canvas>();
        return canvas.GetComponentInChildren<EvalButton>(true).GetComponent<EvalButtonMover>();
    }
}
