//Activates the eval button when the mouse enters an object
//Created by James Vanderhyde, 6 October 2021

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveEvalButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Canvas canvas;
    private EvalButton evalButton;

    void Awake()
    {
        canvas = gameObject.GetComponentInParent<Canvas>();
        evalButton = canvas.GetComponentInChildren<EvalButton>();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //Debug.Log("Expression enter: "+ pointerEventData.position+", "+this.transform.position);
        evalButton.Show(this.gameObject);
        evalButton.transform.position = this.transform.position + new Vector3(0,0,0);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //Check if mouse is "exiting" by going into the eval button
        float localX = -(this.transform.position.x - pointerEventData.position.x);
        float localY = this.transform.position.y - pointerEventData.position.y;
        //Debug.Log("Expression leave: "+localX+" "+localY);
        if (!((localX <= (evalButton.transform as RectTransform).rect.width) &&
              (localY <= (evalButton.transform as RectTransform).rect.height)))
            evalButton.Hide();
    }
}
