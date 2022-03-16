//Replaces items with a duplicate when the item is dragged away
//Created by James Vanderhyde, 3 December 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleryShelf : DragParent
{
    private GalleryCover cover;

    void Start()
    {
        this.cover = this.transform.parent.GetComponentInChildren<GalleryCover>();
        this.cover.gameObject.SetActive(false);
    }

    public override bool DragDuplicate()
    {
        return true;
    }

    public override void ObjectDragged(Draggable d)
    {
        //Bring up the gallery cover
        this.cover.gameObject.SetActive(true);
    }
}
