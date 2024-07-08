//Checks for the XR environment to enable the XR keyboard
//Created by James Vanderhyde, 3 July 2024

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

public class KeyboardCheck : MonoBehaviour
{
    void Awake()
    {
        //Check for VR
        if (XRSettings.enabled)
        {
            //Enable the XR keyboard
            XRKeyboardDisplay kd = this.GetComponent<XRKeyboardDisplay>();
            kd.enabled = true;
        }
    }
}
