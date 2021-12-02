//The entity (game object) form that appears in the UI
//Created by James Vanderhyde, 11 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mal;

public class MalEntity : MalForm
{
    [SerializeField]
    public GameObject value = null;

    void Awake()
    {
        if (value == null)
            value = GameObject.Find("World");
    }

    public void SetSprite(Sprite s)
    {
        Image im = this.transform.GetChild(0).GetComponent<Image>();
        im.sprite = s;
    }

    public override types.MalVal read_form()
    {
        return new types.MalObjectReference(value);
    }

}
