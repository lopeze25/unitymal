//Allows 3D objects in the world to be dragged into the UI
//Created by James Vanderhyde, 12 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class Draggable3D : MonoBehaviour
{
    private MalPrinter formPrinter;
    private Camera objectCam;
    private int objectCamLayer;
    private Texture2D tex;
    private int oldLayer;
    private Renderer rend;

    void Start()
    {
        Transform bp = GameObject.Find("Canvas").transform.Find("build plane");
        this.formPrinter = bp.GetComponent<MalPrinter>();
        this.objectCam = GameObject.Find("Close-up camera").GetComponent<Camera>();
        this.objectCamLayer = 0;
        int mask = this.objectCam.cullingMask;
        while (mask > 1)
        {
            mask >>= 1;
            this.objectCamLayer++;
        }
        this.rend = this.GetComponent<Renderer>();
    }

    void OnMouseDown()
    {
        //Create the Mal form
        MalEntity result;
        result = (MalEntity)formPrinter.pr_form(new types.MalObjectReference(this.gameObject));

        //Create the sprite for the form
        RenderTexture rtex = this.objectCam.targetTexture;
        this.tex = new Texture2D(rtex.width, rtex.height, TextureFormat.RGBA32, false);
        result.SetSprite(Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0f, 1f), 100.0f));

        //Focus the camera on the 3D object
        this.oldLayer = this.gameObject.layer;
        this.gameObject.layer = this.objectCamLayer;
        float radius = this.rend.bounds.extents.magnitude;
        this.objectCam.transform.position = Camera.main.transform.position;
        this.objectCam.transform.LookAt(this.transform);
        this.objectCam.orthographicSize = radius;

        //Tell the camera to render to the sprite next frame
        StartCoroutine(CopyTextureToSprite());
    }

    IEnumerator CopyTextureToSprite()
    {
        //Wait for next frame
        yield return null;

        //Software texture copy
        //RenderTexture old_rt = RenderTexture.active;
        //RenderTexture.active = this.objectCam.targetTexture;
        //this.tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        //this.tex.Apply();
        //RenderTexture.active = old_rt;

        //Hardware texture copy
        Graphics.CopyTexture(this.objectCam.targetTexture, this.tex);

        //Switch the object back to its normal
        this.gameObject.layer = oldLayer;
    }
}
