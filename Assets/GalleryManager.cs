//Creates and deletes gallery objects in the world
//Created by James Vanderhyde, 14 July 2023

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class GalleryManager : MonoBehaviour
{
    public SaveLoad gallery;
    public GameObject menuGameObject;
    public Button cat;
    public Button cylinder;
    public Button pig;
    public Button program;
    // public void OnCreateObject(InputAction.CallbackContext context)
    // {
    //  if (context.phase==InputActionPhase.Performed )
    //     gallery.Load("(create-entity {:gallery-name \"Cylinder\"})");
    //  }

    public void ToggleMenu(InputAction.CallbackContext context)
    {
        Debug.Log("Toggled");
        if (context.phase == InputActionPhase.Performed)
        {
            if (menuGameObject.activeSelf == true)
                menuGameObject.SetActive(false);
            else
            {
                menuGameObject.SetActive(true);
                OnVRSelect();

            }
        }
    }


    public void OnVRSelect()
    {

        //Put the canvas in front of the user
        Canvas c = this.menuGameObject.transform.GetComponentInParent<Canvas>();
        Transform head = Camera.main.transform;

        c.transform.position = head.position;
        c.transform.forward = head.forward;
        c.transform.Translate(new Vector3(0, 0, 0.2f));
        Transform imageTransform = c.transform.Find("Menu");
        //Refers to the "Menu" object under Canvas to scale it infront of the user
            imageTransform.localScale = new Vector3(0.5f, 0.20f, 0.11f);
}


    private void Start()
    {
        cat.onClick.AddListener(CreateCat);
        cylinder.onClick.AddListener(CreateCylinder);
        pig.onClick.AddListener(CreatePig);
        program.onClick.AddListener(CreateProgram);
        Debug.Log("ListenersAdded");
    }

    public void CreateCylinder()
    {
        gallery.Load("(create-entity {:gallery-name \"Cylinder\"})");
    }
    public void CreateCat()
    {
        gallery.Load("(create-entity {:gallery-name \"Cat\"})");
    }
    public void CreatePig()
    {
        gallery.Load("(create-entity {:gallery-name \"Pig\"})");
    }

    public void CreateProgram()
    {
        Debug.Log("Program Called");
        gallery.Load("(create-entity {:gallery-name \"Program\"})");
    }

}