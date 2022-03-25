//The recur form form that appears in the UI
//Created by James Vanderhyde, 5 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class MalRecurForm : MalForm
{
    [SerializeField]
    public DragParentReplace parameterTarget;

    private RecurPoint recurPoint = null;

    public override types.MalVal read_form()
    {
        Transform contents = this.transform.GetChild(1);

        types.MalList ml = new types.MalList();
        for (int i = contents.childCount-1; i >= 0; i--)
        {
            MalForm child = contents.GetChild(i).GetChild(0).GetComponent<MalForm>();
            types.MalVal value = child.read_form();
            ml.cons(value);
        }

        ml.cons(new types.MalSymbol("recur"));
        
        return ml;
    }

    public override void setChildForms(List<MalForm> children)
    {
        Transform contents = this.transform.GetChild(1);
        foreach (MalForm f in children)
        {
            DragParentReplace pTarget = GameObject.Instantiate(this.parameterTarget, contents);
            f.transform.SetParent(pTarget.transform);
        }
    }

    public RecurPoint GetRecurPoint()
    {
        return this.recurPoint;
    }

    public void SetRecurPoint(RecurPoint form)
    {
        this.recurPoint = form;

        Transform contents = this.transform.GetChild(1);
        if (contents.childCount == 0)
        {
            GameObject addedChild = null;
            foreach (Transform child in form.transform.GetChild(0))
            {
                SymbolTracker p = child.GetComponentInChildren<SymbolTracker>();
                if (p)
                {
                    DragParentReplace pTarget = GameObject.Instantiate(this.parameterTarget, contents);
                    pTarget.defaultValue = p.transform.GetChild(1).GetChild(0).GetComponent<MalForm>();
                    pTarget.ReplaceWithDefault();
                    addedChild = p.gameObject;
                }
            }

            //Tell the block to resize itself (not working)
            if (addedChild != null)
            {
                ListManagement lm = this.GetComponentInParent<ListManagement>();
                if (lm)
                    lm.AddToList(addedChild);
            }
        }
    }
}
