//A world object with a unique ID
//Created by James Vanderhyde, 20 January 2022

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class Entity : MonoBehaviour
{
    public string guid = "";

    public types.MalVal read_form()
    {
        types.MalList ml = new types.MalList();
        types.MalMap mm = new types.MalMap();

        mm.assoc(types.MalKeyword.keyword(":name"), new types.MalString(this.gameObject.name));
        mm.assoc(types.MalKeyword.keyword(":guid"), new types.MalString(this.guid));

        var gi = this.GetComponent<GalleryItem>();
        if (gi != null)
            mm.assoc(types.MalKeyword.keyword(":gallery-name"), new types.MalString(gi.galleryItemName));

        types.MalVector transformData = new types.MalVector();
        transformData.conj(new types.MalNumber(this.transform.position.x));
        transformData.conj(new types.MalNumber(this.transform.position.y));
        transformData.conj(new types.MalNumber(this.transform.position.z));
        transformData.conj(new types.MalNumber(this.transform.eulerAngles.x));
        transformData.conj(new types.MalNumber(this.transform.eulerAngles.y));
        transformData.conj(new types.MalNumber(this.transform.eulerAngles.z));
        transformData.conj(new types.MalNumber(this.transform.localScale.x));
        transformData.conj(new types.MalNumber(this.transform.localScale.y));
        transformData.conj(new types.MalNumber(this.transform.localScale.z));
        mm.assoc(types.MalKeyword.keyword(":transform"), transformData);

        types.MalList childData = new types.MalList();
        foreach (Transform child in this.transform)
        {
            Entity item = child.GetComponent<Entity>();
            if (item != null)
                childData.cons(item.read_form());
        }
        mm.assoc(types.MalKeyword.keyword(":children"), childData);

        ml.cons(mm);
        ml.cons(new types.MalSymbol("entity"));
        return ml;
    }

}
