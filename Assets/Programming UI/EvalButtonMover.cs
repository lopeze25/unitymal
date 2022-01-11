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
        this.Show();
    }

    public void Relinquish(GameObject hoverForm, Vector2 mousePos, Camera cam)
    {
        this.forms.Remove(hoverForm.GetComponent<Transform>());
        if (RectTransformUtility.RectangleContainsScreenPoint(this.transform as RectTransform, mousePos, cam))
        {
            //The mouse has entered this button; do not move the button.
        }
        else if (this.forms.Count == 0)
        {
            //The mouse has moved out of the outermost form; hide the button.
            this.Hide();
        }
        else
        {
            //The mouse has moved out of an inner form; move the button.
            this.form = findDeepestInHierarchy();
            this.transform.position = this.form.transform.position + new Vector3(0, 0, 0);
        }
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
        this.form = null;
        //this.transform.position = new Vector3(0, 0, 0);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        this.form = findDeepestInHierarchy();
        this.transform.position = this.form.transform.position + new Vector3(0, 0, 0);
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
        {
            this.Hide();
        }
    }

    public MalForm GetActiveForm()
    {
        return this.form;
    }
}
