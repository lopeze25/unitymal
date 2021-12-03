//Move the plane on or off the screen in response to button click
//Created by James Vanderhyde, 2 December 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFormPlane : MonoBehaviour
{
    private bool canRollOut = true;
    private bool canRollUp = false;

    public void SlideButtonClick()
    {
        if (canRollUp && (this.transform.position.x < 800))
            this.StartRollUp();
        if (canRollOut && (this.transform.position.x > 800))
            this.StartRollOut();
    }

    public void StartRollUp()
    {
        StartCoroutine(RollUpAnimation());
    }

    private IEnumerator RollUpAnimation()
    {
        if (canRollUp)
        {
            canRollUp = false;
            float xtarget = this.transform.position.x + 900;
            while (this.transform.position.x < xtarget)
            {
                this.transform.Translate(2400f * Time.deltaTime, 0, 0);
                yield return null;
            }
            this.transform.Translate(xtarget - this.transform.position.x, 0, 0);
            canRollOut = true;
        }
    }

    public void StartRollOut()
    {
        StartCoroutine(RollOutAnimation());
    }

    private IEnumerator RollOutAnimation()
    {
        if (canRollOut)
        {
            canRollOut = false;
            float xtarget = this.transform.position.x - 900;
            while (this.transform.position.x > xtarget)
            {
                this.transform.Translate(-2400f * Time.deltaTime, 0, 0);
                yield return null;
            }
            this.transform.Translate(xtarget - this.transform.position.x, 0, 0);
            canRollUp = true;
        }
    }

}
