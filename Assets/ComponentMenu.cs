using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
public class ComponentMenu : MonoBehaviour, IPointerDownHandler
{
    private static ComponentMenuUI activeUI = null;

    public static ComponentMenuUI GetActiveMenuUI()
    {
        return activeUI;
    }

    private ComponentMenuUI menuUI;

    void Awake()
    {
        foreach (Transform child in this.transform)
        {
            Canvas c = child.GetComponent<Canvas>();
            if (c != null)
            {
                menuUI = c.GetComponentsInChildren<ComponentMenuUI>(true)[0];

                //Check for VR
                if (XRSettings.enabled)
                {
                    //Change Canvas to world space
                    c.renderMode = RenderMode.WorldSpace;
                    c.transform.position = new Vector3(0, 0, 0);
                    c.transform.forward = new Vector3(0, 0, 3);
                    c.transform.localScale = new Vector3(0.006f, 0.006f, 0.006f);

                    //Switch to the correct GraphicRaycaster component
                    TrackedDeviceGraphicRaycaster trackedRC = c.GetComponent<TrackedDeviceGraphicRaycaster>();
                    GraphicRaycaster canvasRC = c.GetComponent<GraphicRaycaster>();
                    if ((trackedRC != null) && (canvasRC != null))
                    {
                        trackedRC.enabled = true;
                        canvasRC.enabled = false;
                    }
                }
            }
        }
    }

    public ComponentMenuUI GetMenuUI()
        {
            return this.menuUI;
        }

        void OnMouseDown()
        {
            //Toggle the active state of the corresponding UI.
            GameObject o = this.menuUI.gameObject;
            if (o.activeSelf)
            {
                o.SetActive(false);
                ComponentMenu.activeUI = null;
            }
            else
            {
                if (ComponentMenu.activeUI != null)
                    ComponentMenu.activeUI.gameObject.SetActive(false);
                o.SetActive(true);
                ComponentMenu.activeUI = this.menuUI;
            }
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            if ((eventData.pointerEnter != null) && (eventData.pointerEnter.GetComponent<ComponentMenu>() != null))
                this.OnMouseDown();
        }

        public void OnVRSelect(SelectEnterEventArgs eventData)
        {
            this.OnMouseDown();

            //Put the canvas in front of the user
            Canvas c = this.menuUI.transform.GetComponentInParent<Canvas>();
            Transform head = Camera.main.transform;
            c.transform.position = head.position;
            c.transform.forward = head.forward;
            c.transform.Translate(new Vector3(0, 0, 4f));
        }
    }

