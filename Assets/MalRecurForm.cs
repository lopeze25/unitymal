//The recur form form that appears in the UI
//Created by James Vanderhyde, 5 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class MalRecurForm : MalForm
{
    [SerializeField]
    public ReplacingDropTarget parameterTarget;

    public override types.MalVal read_form()
    {
        Transform contents = this.transform.GetChild(1);

        types.MalList ml = new types.MalList();
        for (int i = contents.childCount-1; i >= 0; i--)
        {
            MalForm child = contents.GetChild(i).GetComponent<MalForm>();
            types.MalVal value = child.read_form();
            ml.cons(value);
        }

        ml.cons(new types.MalSymbol("recur"));
        
        return ml;
    }

    public void SetRecurPoint(RecurPoint form)
    {
        Transform contents = this.transform.GetChild(1);
        if (contents.childCount == 0)
        {
            TieToTracker[] parameters = form.transform.GetChild(0).GetComponentsInChildren<TieToTracker>();
            GameObject addedChild = null;
            foreach (TieToTracker p in parameters)
            {
                ReplacingDropTarget pTarget = GameObject.Instantiate(parameterTarget, contents);
                pTarget.defaultValue = p.GetComponent<MalForm>();
                pTarget.ReplaceWithDefault();
                addedChild = p.gameObject;
            }
            GameObject.Destroy(this.GetComponent<Draggable>());
            this.gameObject.AddComponent<DraggableRestricted>();

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
