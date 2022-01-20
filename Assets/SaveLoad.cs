//Load and save the Dollhouse world
//Created by James Vanderhyde, 12 January 2022

using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using Mal;

public class SaveLoad : MonoBehaviour
{
    public List<GameObject> galleryPrefabs = new List<GameObject>();

    private Dictionary<string,GameObject> galleryMap = new Dictionary<string,GameObject>();

    void Awake()
    {
        foreach (GameObject go in galleryPrefabs)
            galleryMap.Add(go.name, go);
    }

    void Start()
    {
        Debug.Log(this.Save());
        this.Load(this.Save());
    }

    public string Save()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("(list ");
        foreach (Transform child in this.transform)
        {
            Entity item = child.GetComponent<Entity>();
            if (item != null)
                sb.Append(printer.pr_str(item.read_form()));
        }
        sb.Append(")");
        return sb.ToString();
    }

    public void Load(string worldString)
    {
        types.MalVal worldTree = reader.read_str(worldString);
        foreach (types.MalVal child in worldTree as types.MalList)
            this.LoadWorldObject(child, this.transform);
    }

    private void LoadWorldObject(types.MalVal obj, Transform parent)
    {
        if (obj is types.MalList)
        {
            types.MalList o = obj as types.MalList;
            if (o.first() is types.MalSymbol)
            {
                types.MalSymbol objType = o.first() as types.MalSymbol;
                if (objType.name.Equals("entity"))
                    this.LoadEntity(o, parent);
            }
        }
    }

    private void LoadEntity(types.MalList item, Transform parent)
    {
        types.MalMap itemData = (types.MalMap)item.rest().first();

        //If there is a gallery name, we use the gallery to instantiate the object.
        //Other possibilities will have to be handled another way.
        types.MalString galleryName = (types.MalString)itemData.get(types.MalKeyword.keyword(":gallery-name"));
        string prefabName = galleryName.value;
        GameObject go = GameObject.Instantiate(this.galleryMap[prefabName], parent);

        types.MalString itemName = (types.MalString)itemData.get(types.MalKeyword.keyword(":name"));
        go.name = itemName.value;
        types.MalString itemGuid = (types.MalString)itemData.get(types.MalKeyword.keyword(":guid"));
        Entity e = go.GetComponent<Entity>();
        e.guid = itemGuid.value;

        types.MalVector transformData = (types.MalVector)itemData.get(types.MalKeyword.keyword(":transform"));
        float posx = ((types.MalNumber)transformData.nth(0)).value;
        float posy = ((types.MalNumber)transformData.nth(1)).value;
        float posz = ((types.MalNumber)transformData.nth(2)).value;
        float rotx = ((types.MalNumber)transformData.nth(3)).value;
        float roty = ((types.MalNumber)transformData.nth(4)).value;
        float rotz = ((types.MalNumber)transformData.nth(5)).value;
        float scax = ((types.MalNumber)transformData.nth(6)).value;
        float scay = ((types.MalNumber)transformData.nth(7)).value;
        float scaz = ((types.MalNumber)transformData.nth(8)).value;
        go.transform.position = new Vector3(posx, posy, posz);
        go.transform.eulerAngles = new Vector3(rotx, roty, rotz);
        go.transform.localScale = new Vector3(scax, scay, scaz);

        types.MalList childData = (types.MalList)itemData.get(types.MalKeyword.keyword(":children"));
        foreach (types.MalVal child in childData)
            this.LoadWorldObject(child, go.transform);
    }
}
