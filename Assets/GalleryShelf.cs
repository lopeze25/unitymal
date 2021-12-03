//Replaces items with a duplicate when the item is dragged away
//Created by James Vanderhyde, 3 December 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleryShelf : MonoBehaviour
{
    private GalleryCover cover;

    void Start()
    {
        this.cover = this.transform.parent.GetComponentInChildren<GalleryCover>();
        this.cover.gameObject.SetActive(false);
    }

    public void Replace(GameObject original)
    {
        //Duplicate the chosen object
        Object.Instantiate(original, this.transform);

        //Bring up the gallery cover
        this.cover.gameObject.SetActive(true);
    }
}
