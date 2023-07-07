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

    void Awake()
    {
        Dictionary<string, GameObject> galleryMap = new Dictionary<string, GameObject>();
        foreach (GameObject go in galleryPrefabs)
            galleryMap.Add(go.name, go);
        Dollhouse.entities.ns.Add("create-entity", new Dollhouse.entities.create_entity(galleryMap));
        Dollhouse.entities.ns.Add("world", new types.MalObjectReference(this.GetComponent<Entity>()));
    }

    void Start()
    {
        this.LoadGame();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            this.SaveGame();
    }

    void OnApplicationQuit()
    {
        this.SaveGame();
    }

    private void LoadGame()
    {
        if (PlayerPrefs.HasKey("world"))
        {
            string worldString = PlayerPrefs.GetString("world");
            this.Load(worldString);
        }
    }

    private void SaveGame()
    {
        string worldString = this.Save();
        Debug.Log(worldString);
        PlayerPrefs.SetString("world", worldString);
    }

    public string Save()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("(list ");
        foreach (Transform child in this.transform)
        {
            Entity item = child.GetComponent<Entity>();
            if (item != null)
                sb.Append(printer.pr_str(item.read_create_form()));
        }
        sb.Append(")");
        return sb.ToString();
    }

    public void Load(string worldString)
    {
        //Define the evaluation environment
        Mal.env.Environment environment = new Mal.env.Environment(Mal.env.baseEnvironment, false);
        environment.setAll(Dollhouse.entities.ns);

        //Read and evaluate the string
        types.MalVal worldChildList = evaluator.eval_ast(reader.read_str(worldString), environment);

        //There may be a security flaw in executing arbirary MAL code here.
        //But the programming UI allows arbitrary input, anyway.
    }
}
