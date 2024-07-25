using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Android.Types;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CallMenu : MonoBehaviour
{
    public GameObject menuGameObject;
    public GameObject currentObject;
    public TMP_Text positionText;
  //  public GameObject panel; 

  public void OnSelect()
   {
  Debug.Log("Called OnAwake Menu");
   OnAwake();
   Debug.Log(this.gameObject.name + " was selected");
   Debug.Log("Called Select Menu");
   UpdateTransformPanel();
    OnVRSelect();
   }


    private void OnAwake()
    {
        // Find the ComponentMenu GameObject if not already assigned
        if (menuGameObject == null)
        {
            menuGameObject = GameObject.Find("ComponentMenu");
            if (menuGameObject != null)
            {
                Debug.Log("ComponentMenu found.");
                // Find the Panel child GameObject within ComponentMenu
                GameObject panel = GameObject.Find("/ComponentMenu/Canvas/Panel");
                // Transform panel = menuGameObject.transform.Find("Canvas/Panel");
                if (panel != null)
                {
                    Debug.Log("Panel found under ComponentMenu.");
                    // Find the TMP_Text component within the Panel
                    positionText = panel.GetComponentInChildren<TMP_Text>(); 

                    
                    // positionText = panel.transform.Find("transformtext")?.GetComponent<TMP_Text>();
                    if (positionText != null)
                    {
                        Debug.Log("TMP_Text component found on transformtext.");
                    }
                    else
                    {
                        Debug.LogError("TMP_Text component not found on transformtext.");
                    }
                }
                else
                {
                    Debug.LogError("Panel not found under ComponentMenu.");
                }
            }
            else
            {
                Debug.LogError("ComponentMenu not found.");
            }
        }

        // Set the current object to this game object
        if (currentObject == null)
        {
            currentObject = this.gameObject;
            Debug.Log("Set currentObject to " + currentObject.name);
        }

        Debug.Log("Called Update");
    }
    private void UpdateTransformPanel()
    {
        Transform targetTransform = currentObject.transform;
        if (targetTransform != null)
        {
            if (currentObject != null)
            {
                if (positionText != null)
                {

                    positionText.text = "Position: " + menuGameObject.transform.position.ToString();
                    Debug.Log("Called UpdatedtransformPanel");

                }
                else
                {
                    Debug.LogError("positiontext object not found under ComponentMenu.");
                }
            } else
            {
                Debug.LogError("Current object not found under ComponentMenu.");
            }

        } else
        {
            Debug.LogError("targetTransform not found under ComponentMenu.");
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
        c.transform.forward = new Vector3(0, 0, 3);
        c.transform.localScale = new Vector3(0.0014f, 0.0024f, 0.036f); // Scale the canvas

    }
}

///public class CallMenu : MonoBehaviour
///{
  //  public GameObject menuGameObject;
 //   public GameObject currentObject;
  /// <summary>
  ///  public TMP_Text positionText;
  /// </summary>
  /////  public GameObject panel;

   // private void Update()
  //  {
    ///    menuGameObject = GameObject.Find("ComponentMenu");
    // /   if (menuGameObject != null)
     ///   {
     //       panel = menuGameObject.transform.Find("Panel").gameObject;
      ///      if (panel != null)
      ///      {
      //          positionText = panel.GetComponentInChildren<TMP_Text>();
     ////      }
     ///       else
     // //      {
          //      Debug.LogError("Panel not found under ComponentMenu.");
           // }
     ///   }
     //   else
     //   {
      ///      Debug.LogError("ComponentMenu not found.");
      //  }

      //  currentObject = this.gameObject;
      //  Debug.Log("Initialized CallMenu");
 //   }

 ////   private void UpdateTransformPanel()
  ///////  {
      //  if (positionText != null)
    ////////// ////  ///////     if (currentObject != null)
        //////////    {

        ////////////        Transform targetTransform = currentObject.transform;
         //////////// /////////////      positionText.text = $"Position: {targetTransform.position}";
           //    Debug.Log("Updated transform panel with position: " + targetTransform.position);
         ///   }
         //   else
          ///  {
          //      Debug.LogError("CurrentObject is null.");
          //  }
      //  }
      //  else
      //  {
          //  Debug.LogError("PositionText is null.");
       // }
  //  }


   // public void OnSelect(BaseEventData eventData)
   // {
       // Debug.Log(this.gameObject.name + " was selected");
     //  Debug.Log("Called Select Menu");
     //   UpdateTransformPanel();
 //       OnVRSelect();
 //   }
 //   public void OnVRSelect()
 //   {
//        Debug.Log("Called");
        // Put the canvas in front of the user
//        Canvas c = this.menuGameObject.GetComponent<Canvas>();
//        Transform head = Camera.main.transform;
//
 //       c.transform.position = head.position + head.forward * 0.2f;
//        c.transform.rotation = Quaternion.LookRotation(head.forward, Vector3.up);
 //       c.transform.localScale = new Vector3(0.0014f, 0.0024f, 0.036f); // Scale the canvas
//
 //   }
//
//}

























//   public void SelectMenu()
//{

//  Debug.Log("Was selected");
//   // Check if the ComponentMenu is on the same GameObject
//    ComponentMenu menu = GetComponent<ComponentMenu>();
//    if (menu == null)
//    {
//       Debug.LogWarning("ComponentMenu not found on this GameObject");
//
//       // Check if the ComponentMenu is on a child GameObject
//       menu = GetComponentInChildren<ComponentMenu>();
//       if (menu == null)
//     {
//       Debug.LogError("ComponentMenu not found");
//     return;
//}
//}
//
//  Debug.Log("ComponentMenu found");
//
// Ensure the menu UI is initialized
// GameObject menuUI = menu.GetMenuUI()?.gameObject;
// if (menuUI == null)
// {
//   Debug.LogError("Menu UI  null");
//    return;
//  }

//  Debug.Log("Menu UI found");

//  if (menuUI.activeSelf)
//  {
//      menuUI.SetActive(false);
///      menu.enabled = false;
///     Debug.Log("Menu UI and ComponentMenu disabled");
///  }
///  else
///  {
//     menuUI.SetActive(true);
///      menu.enabled = true;
//     Debug.Log("Menu UI and ComponentMenu enabled");
//  }
//}