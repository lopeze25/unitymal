//The entity (game object) form that appears in the UI
//Created by James Vanderhyde, 11 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mal;

public class MalEntity : MalForm
{
    [SerializeField]
    public GameObject value = null;

    private Camera objectCam;
    private Texture2D tex;
    private int oldLayer;

    void Awake()
    {
        if (value == null)
            value = GameObject.Find("World");
    }

    void Start()
    {
        if (value != GameObject.Find("World"))
            this.TakePicture();
    }

    public override types.MalVal read_form()
    {
        return new types.MalObjectReference(value);
    }

    private void TakePicture()
    {
        //Set up camera for close-up
        this.objectCam = GameObject.Find("Close-up camera").GetComponent<Camera>();
        int objectCamLayer = 0;
        int mask = this.objectCam.cullingMask;
        while (mask > 1)
        {
            mask >>= 1;
            objectCamLayer++;
        }

        //Create the sprite for the form
        RenderTexture rtex = this.objectCam.targetTexture;
        this.tex = new Texture2D(rtex.width, rtex.height, TextureFormat.RGBA32, false);
        Image im = this.transform.GetChild(0).GetComponent<Image>();
        im.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0f, 1f), 100.0f);

        //Focus the camera on the 3D object
        this.oldLayer = this.value.layer;
        this.value.layer = objectCamLayer;
        Renderer rend = this.value.GetComponent<Renderer>();
        //If there is no renderer, it may be a group object. In that case, we'll have to combine all the children into one picture.
        //We should do that in any case, in case some child objects stick out from the parent object.
        float radius = rend.bounds.extents.magnitude;
        this.objectCam.transform.position = Camera.main.transform.position;
        this.objectCam.transform.LookAt(this.value.transform);
        this.objectCam.orthographicSize = radius;

        //Tell the camera to render to the sprite next frame
        StartCoroutine(CopyTextureToSprite());
    }

    private IEnumerator CopyTextureToSprite()
    {
        //Wait for next frame
        yield return null;

        bool useHardwareTextureCopy = true;
        if (useHardwareTextureCopy)
        {
            //Hardware texture copy
            Graphics.CopyTexture(this.objectCam.targetTexture, this.tex);
        }
        else
        {
            //Software texture copy
            RenderTexture old_rt = RenderTexture.active;
            RenderTexture.active = this.objectCam.targetTexture;
            this.tex.ReadPixels(new Rect(0, 0, this.tex.width, this.tex.height), 0, 0);
            this.tex.Apply();
            RenderTexture.active = old_rt;
        }

        //Switch the object back to its normal layer
        this.value.layer = this.oldLayer;
    }
}
