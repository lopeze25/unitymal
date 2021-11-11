//Controls how the evaluate button appears and disappears
//Created by James Vanderhyde, 7 October 2021

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using Mal;

public class EvalButtonMover : MonoBehaviour, IPointerExitHandler
{
    private MalForm form;
    private List<Transform> forms = new List<Transform>();

    void Awake()
    {
        this.form = null;
    }

    public void Request(GameObject hoverForm)
    {
        this.forms.Add(hoverForm.GetComponent<Transform>());
        this.gameObject.SetActive(true);
        this.form = findDeepestInHierarchy();
        this.transform.position = this.form.transform.position + new Vector3(0, 0, 0);
    }

    public void Relinquish(GameObject hoverForm, Vector2 mousePos)
    {
        this.forms.Remove(hoverForm.GetComponent<Transform>());
        if (this.rectContainsPoint(mousePos))
        {
            //The mouse has entered this button; do not move the button.
        }
        else if (forms.Count == 0)
        {
            //The mouse has moved out of the outermost form; hide the button.
            this.gameObject.SetActive(false);
            this.form = null;
        }
        else
        {
            //The mouse has moved out of an inner form; move the button.
            this.form = findDeepestInHierarchy();
            this.transform.position = this.form.transform.position + new Vector3(0, 0, 0);
        }
    }

    private bool rectContainsPoint(Vector2 p)
    {
        float localX = -(this.transform.position.x - p.x);
        float localY = this.transform.position.y - p.y;
        return ((localX <= (this.transform as RectTransform).rect.width) &&
                (localY <= (this.transform as RectTransform).rect.height));

    }

    private MalForm findDeepestInHierarchy()
    {
        if (forms.Count == 0)
            return null;
        Transform deepest = forms[0];
        for (int i = 0; i < forms.Count; i++)
            if (forms[i].IsChildOf(deepest))
                deepest = forms[i];
        return deepest.GetComponent<MalForm>();
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (forms.Count == 0)
            this.gameObject.SetActive(false);
    }

    public MalForm GetActiveForm()
    {
        return this.form;
    }
}
