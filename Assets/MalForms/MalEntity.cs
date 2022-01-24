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
    public Entity value = null;

    private Camera objectCam;
    private Texture2D tex;
    private int oldLayer;

    void Awake()
    {
        if (value == null)
            value = GameObject.Find("World").GetComponent<Entity>();
    }

    void Start()
    {
        if (value != GameObject.Find("World").GetComponent<Entity>())
            this.TakePicture();
    }

    public override types.MalVal read_form()
    {
        //Creates an s-expression to refer to the entity.
        //It would be more efficient for running to simply wrap the Entity in a MalObjectReference,
        // but the s-expression is more useful for converting to code.
        types.MalList ml = new types.MalList();
        types.MalMap mm = new types.MalMap();

        mm.assoc(types.MalKeyword.keyword(":guid"), new types.MalString(value.guid));

        ml.cons(mm);
        ml.cons(new types.MalSymbol("entity"));
        return ml;
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
        this.oldLayer = this.value.gameObject.layer;
        Renderer[] rends = this.value.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rends) 
            r.gameObject.layer = objectCamLayer;
        Bounds b;
        if (rends.Length > 1)
        {
            //In the case of a group object, we have to combine all the children into one picture.
            Vector3 max = rends[0].bounds.max;
            Vector3 min = rends[0].bounds.min;
            foreach (Renderer r in rends)
            {
                max = Vector3.Max(r.bounds.max, max);
                min = Vector3.Min(r.bounds.min, min);
            }
            b = new Bounds((max+min)/2, max-min);
        }
        else
        {
            b = rends[0].bounds;
            //If there is no renderer, such as for a marker or dummy object, we'll need a different way to draw it.
        }
        float radius = b.extents.magnitude;
        this.objectCam.transform.position = Camera.main.transform.position;
        this.objectCam.transform.LookAt(b.center);
        this.objectCam.orthographicSize = radius;

        //Tell the camera to render to the sprite next frame
        StartCoroutine(CopyTextureToSprite());
    }

    private IEnumerator CopyTextureToSprite()
    {
        //Wait for next frame
        yield return null;

        bool useHardwareTextureCopy = false;
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
        Renderer[] rends = this.value.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rends)
            r.gameObject.layer = this.oldLayer;
    }
}
