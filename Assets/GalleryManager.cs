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
   // public Button program;
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
            if (menuGameObject.activeSelf)
            {
                menuGameObject.SetActive(false);
                Debug.Log("Menu set to inactive");
            }
            else
            {
                menuGameObject.SetActive(true);
                Debug.Log("Menu set to active");
                OnVRSelect();
            }
        }
    }

    public void OnVRSelect()
    {
        Debug.Log("Called");
        // Put the canvas in front of the user
        Canvas c = this.menuGameObject.GetComponent<Canvas>();
        Transform head = Camera.main.transform;

        c.transform.position = head.position + head.forward * 0.2f;
        c.transform.rotation = Quaternion.LookRotation(head.forward, Vector3.up);
        c.transform.localScale = new Vector3(0.0014f, 0.0024f, 0.036f); // Scale the canvas

        }
    private void Start()
    {
        cat.onClick.AddListener(CreateCat);
        cylinder.onClick.AddListener(CreateCylinder);
        pig.onClick.AddListener(CreatePig);
      //  program.onClick.AddListener(CreateProgram);
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


 
}