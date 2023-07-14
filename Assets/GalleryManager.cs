//Creates and deletes gallery objects in the world
//Created by James Vanderhyde, 14 July 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GalleryManager : MonoBehaviour
{
    public SaveLoad gallery;

    public void OnCreateObject(InputAction.CallbackContext context)
    {
        if (context.phase==InputActionPhase.Performed)
            gallery.Load("(create-entity {:gallery-name \"Cylinder\"})");
    }
}
