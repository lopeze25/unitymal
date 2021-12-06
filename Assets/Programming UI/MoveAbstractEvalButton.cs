//Makes requests for the specified eval button to move
//Created by James Vanderhyde, 3 December 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class MoveAbstractEvalButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private EvalButtonMover evalButton;

    void Awake()
    {
        this.evalButton = this.GetButtonMover();
    }

    protected abstract EvalButtonMover GetButtonMover();

    private bool partOfGalleryPlane()
    {
        return this.gameObject.GetComponentInParent<GalleryShelf>() != null;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (!this.partOfGalleryPlane())
        {
            if (!pointerEventData.dragging)
            {
                //Debug.Log(gameObject.name + " Expression enter: " + pointerEventData.position + ", " + this.transform.position);
                evalButton.Request(this.gameObject);
            }
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (!this.partOfGalleryPlane())
        {
            //Debug.Log(gameObject.name + " Expression exit: " + pointerEventData.position + ", " + this.transform.position);
            evalButton.Relinquish(this.gameObject, pointerEventData.position);
        }
    }
}
