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
        RectTransform rt = this.GetComponent<RectTransform>();
        if (canRollUp && (rt.anchoredPosition.x < 800))
            this.StartRollUp();
        if (canRollOut && (rt.anchoredPosition.x > 800))
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
            RectTransform rt = this.GetComponent<RectTransform>();
            float xtarget = rt.anchoredPosition.x + 900;
            Vector2 pos = rt.anchoredPosition;
            while (rt.anchoredPosition.x < xtarget)
            {
                pos.x += 2400f * Time.deltaTime;
                rt.anchoredPosition = pos;
                yield return null;
            }
            pos.x = xtarget;
            rt.anchoredPosition = pos;
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
            RectTransform rt = this.GetComponent<RectTransform>();
            float xtarget = rt.anchoredPosition.x - 900;
            Vector2 pos = rt.anchoredPosition;
            while (rt.anchoredPosition.x > xtarget)
            {
                pos.x += -2400f * Time.deltaTime;
                rt.anchoredPosition = pos;
                yield return null;
            }
            pos.x = xtarget;
            rt.anchoredPosition = pos;
            canRollUp = true;
        }
    }

}
