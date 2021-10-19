//The user can drag UI objects around the canvas
//This kind of drop target has a default place holder.
//Created by James Vanderhyde, 15 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReplacingDropTarget : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Color emptyColor;
    public MalForm defaultValue;

    public void OnDrop(PointerEventData data)
    {
        if (data.pointerDrag != null)
        {
            FuncManagement fm = GetComponentInParent<FuncManagement>();
            if (fm)
                fm.AddToList(data.pointerDrag);
            else
                Debug.LogError("The drop target must be the child of a function.");

            Image image = GetComponent<Image>();
            image.color = emptyColor;

            Transform replaced = this.transform.GetChild(0);
            Object.Destroy(replaced.gameObject);
            data.pointerDrag.transform.SetParent(this.transform);
            data.pointerDrag.transform.localPosition = new Vector3(2, -2, 0);
        }
    }

    public void ReplaceWithDefault()
    {
        Object.Instantiate(defaultValue, this.transform);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.dragging)
        {
            Image image = GetComponent<Image>();
            emptyColor = image.color;
            image.color = Color.yellow;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.dragging)
        {
            Image image = GetComponent<Image>();
            image.color = emptyColor;
        }
    }
}
