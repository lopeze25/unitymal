//A world object that came from a prefab in the gallery
//Created by James Vanderhyde, 12 January 2022

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class GalleryItem : MonoBehaviour
{
    public string galleryItemName;

    public types.MalVal read_form()
    {
        types.MalList ml = new types.MalList();
        types.MalMap mm = new types.MalMap();

        mm.assoc(types.MalKeyword.keyword(":name"), new types.MalString(this.gameObject.name));

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
        mm.assoc(types.MalKeyword.keyword(":transform"),transformData);

        types.MalList childData = new types.MalList();
        foreach (Transform child in this.transform)
        {
            GalleryItem item = child.GetComponent<GalleryItem>();
            if (item != null)
                childData.cons(item.read_form());
        }
        mm.assoc(types.MalKeyword.keyword(":children"), childData);

        ml.cons(mm);
        ml.cons(new types.MalString(this.galleryItemName));
        ml.cons(new types.MalSymbol("gallery-item"));
        return ml;
    }

}
