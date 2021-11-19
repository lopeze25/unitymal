//Activates the action button when the mouse enters an object
//Created by James Vanderhyde, 6 October 2021

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveEvalActionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Canvas canvas;
    private EvalButtonMover evalButton;

    void Awake()
    {
        canvas = gameObject.GetComponentInParent<Canvas>();
        evalButton = canvas.GetComponentInChildren<EvalButton>().GetComponent<EvalButtonMover>();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (!pointerEventData.dragging)
        {
            //Debug.Log(gameObject.name + " Expression enter: " + pointerEventData.position+", "+this.transform.position);
            evalButton.Request(this.gameObject);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //Debug.Log(gameObject.name+" Expression exit");
        evalButton.Relinquish(this.gameObject, pointerEventData.position);
    }
}
