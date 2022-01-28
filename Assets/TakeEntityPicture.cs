//Takes pictures of entities for the UI forms
//Created by James Vanderhyde, 27 January 2022

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakeEntityPicture : MonoBehaviour
{
    private Queue<MalEntity> needPicture = new Queue<MalEntity>();
    private Camera cam;
    private int objectCamLayer;

    void Start()
    {
        this.cam = this.GetComponent<Camera>();
        this.objectCamLayer = 0;
        int mask = cam.cullingMask;
        while (mask > 1)
        {
            mask >>= 1;
            this.objectCamLayer++;
        }

        StartCoroutine(CopyTextureToSprite());
    }

    public void TakePicture(MalEntity entityForm)
    {
        needPicture.Enqueue(entityForm);
    }

    private IEnumerator CopyTextureToSprite()
    {
        while (true)
        {
            //Wait for something to enter the queue
            while (needPicture.Count == 0)
                yield return null;

            //Get the next thing in the queue
            MalEntity entityForm = needPicture.Dequeue();

            //Create the sprite for the form
            RenderTexture rtex = this.cam.targetTexture;
            Texture2D tex = new Texture2D(rtex.width, rtex.height, TextureFormat.RGBA32, false);
            Image im = entityForm.transform.GetChild(0).GetComponent<Image>();
            im.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0f, 1f), 100.0f);

            //Focus the camera on the 3D object
            int oldLayer = entityForm.value.gameObject.layer;
            Renderer[] rends = entityForm.value.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in rends)
                r.gameObject.layer = this.objectCamLayer;
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
                b = new Bounds((max + min) / 2, max - min);
            }
            else
            {
                b = rends[0].bounds;
                //If there is no renderer, such as for a marker or dummy object, we'll need a different way to draw it.
            }
            float radius = b.extents.magnitude;
            this.cam.transform.position = Camera.main.transform.position;
            this.cam.transform.LookAt(b.center);
            this.cam.orthographicSize = radius;

            //Wait for next frame
            yield return null;

            //Copy the camera result into the sprite
            bool useHardwareTextureCopy = false;
            if (useHardwareTextureCopy)
            {
                //Hardware texture copy
                Graphics.CopyTexture(rtex, tex);
            }
            else
            {
                //Software texture copy
                RenderTexture old_rt = RenderTexture.active;
                RenderTexture.active = rtex;
                tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
                tex.Apply();
                RenderTexture.active = old_rt;
            }

            //Switch the object back to its normal layer
            foreach (Renderer r in rends)
                r.gameObject.layer = oldLayer;
        }
    }
}
