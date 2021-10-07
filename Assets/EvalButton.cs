//Controls how the evaluate button appears and disappears
//Created by James Vanderhyde, 7 October 2021

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class EvalButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        this.gameObject.SetActive(false);
    }
}
