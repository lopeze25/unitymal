//The parent class of all forms that appear in the UI
//Created by James Vanderhyde, 8 October 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public abstract class MalForm : MonoBehaviour
{
    public string galleryItemName;

    private void Awake()
    {
        //Disable all the drop targets. They are enabled when a drag starts.
        DropTarget[] targets = this.GetComponentsInChildren<DropTarget>(true);
        foreach (DropTarget t in targets)
            t.enabled = false;
    }

    public abstract types.MalVal read_form();

    public virtual void setChildForms(List<MalForm> children)
    {
        throw new System.ArgumentException("setChildForms not implemented for "+this.GetType());
    }

    public void Replace(MalForm oldChild, MalForm newChild)
    {
        newChild.transform.SetParent(oldChild.transform.parent, false);
        oldChild.transform.SetParent(null);
        GameObject.Destroy(oldChild.gameObject);
    }
}
