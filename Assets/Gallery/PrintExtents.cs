//Print extents of all mesh renderers under this object
//Created by James Vanderhyde, 24 July 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintExtents : MonoBehaviour
{
    void Start()
    {
        var renderers = this.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            Debug.Log(r.gameObject.name+" "+r.bounds);
        }
    }

}
