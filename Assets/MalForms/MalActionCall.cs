//Any action call that appears in the UI
//Created by James Vanderhyde, 17 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;
using TMPro;

public class MalActionCall : MalForm
{
    public override types.MalVal read_form()
    {
        types.MalList ml = new types.MalList();
        string functionName = this.galleryItemName;
        types.MalMap mm = new types.MalMap();
        for (int i=1; i<transform.childCount; i++)
        {
            Parameter p = transform.GetChild(i).GetComponentInChildren<Parameter>();
            types.MalKeyword k = types.MalKeyword.keyword(p.keyword);
            mm.assoc(k, transform.GetChild(i).GetComponentInChildren<MalForm>().read_form());
        }
        ml.cons(mm);
        ml.cons(new types.MalSymbol(functionName));

        types.MalList highlight = new types.MalList();
        highlight.cons(ml);
        highlight.cons(new types.MalObjectReference(this.gameObject)); //line of code to highlight
        highlight.cons(new Highlight());

        return highlight;
    }

    public override void setChildForms(List<MalForm> children)
    {
        MalMap mm = (MalMap)children[0];
        Transform keys = mm.transform.GetChild(0);
        Transform vals = mm.transform.GetChild(1);
        Dictionary<types.MalKeyword, MalForm> d = new Dictionary<types.MalKeyword, MalForm>();
        for (int i = 0; i < keys.childCount; i++)
        {
            MalForm childKey = keys.GetChild(i).GetComponent<MalForm>();
            MalForm childVal = vals.GetChild(i).GetComponent<MalForm>();
            types.MalVal k = childKey.read_form();
            d[(types.MalKeyword)k] = childVal;
        }

        List<types.MalKeyword> keyList = new List<types.MalKeyword>();
        for (int i = 1; i < this.transform.childCount; i++)
        {
            Parameter p = this.transform.GetChild(i).GetComponentInChildren<Parameter>();
            keyList.Add(types.MalKeyword.keyword(p.keyword));
        }

        for (int i=0; i < keyList.Count; i++)
        { 
            this.Replace(this.transform.GetChild(i+1).GetComponentInChildren<MalForm>(),d[keyList[i]]);
        }
        children[0].transform.SetParent(null);
        GameObject.Destroy(children[0].gameObject);
    }
}
