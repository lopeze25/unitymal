//The entity (game object) form that appears in the UI
//Created by James Vanderhyde, 11 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mal;

public class MalEntity : MalForm
{
    [SerializeField]
    public Entity value = null;

    private string guid = null;

    void Start()
    {
        if (this.value == null)
        {
            if (this.guid == null)
                this.value = GameObject.Find("World").GetComponent<Entity>();
            else
            {
                this.value = Dollhouse.entities.GetEntityByGuid(this.guid);
                this.guid = null;
                this.TakePicture();
            }
        }
        else
            this.TakePicture();
    }

    public override types.MalVal read_form()
    {
        if (this.value == GameObject.Find("World").GetComponent<Entity>())
            return new types.MalSymbol("world");
 
        //Creates an s-expression to refer to the entity.
        //It would be more efficient for running to simply wrap the Entity in a MalObjectReference,
        // but the s-expression is more useful for converting to code.
        types.MalList ml = new types.MalList();
        types.MalMap mm = new types.MalMap();

        string g = this.guid;
        if (this.value != null)
            g = this.value.guid;
        mm.assoc(types.MalKeyword.keyword(":guid"), new types.MalString(g));

        ml.cons(mm);
        ml.cons(new types.MalSymbol("entity"));
        return ml;
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

        MalForm guidForm = d[types.MalKeyword.keyword(":guid")];
        this.guid = ((MalString)guidForm).value;
        
        children[0].transform.SetParent(null);
        GameObject.Destroy(children[0].gameObject);
    }

    private void TakePicture()
    {
        GameObject.Find("Close-up camera").GetComponent<TakeEntityPicture>().TakePicture(this);
    }

}
