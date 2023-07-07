//Entity functions for Dollhouse
//Created by James Vanderhyde, 21 January 2022

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

namespace Dollhouse
{
    public class entities
    {
        public static readonly Dictionary<string, types.MalVal> ns = new Dictionary<string, types.MalVal>();
        static entities()
        {
            ns.Add("entity", new entity());
            ns.Add("create-guid", new create_guid());
            //create-entity defined at runtime when the gallery is available
            //world defined at runtime when the world entity is available
            ns.Add("remove-entity", new remove_entity());
        }

        private static readonly Dictionary<string, Entity> entityMap = new Dictionary<string, Entity>();
        public static Entity GetEntityByGuid(string guid)
        {
            return entityMap[guid];
        }

        private class entity : types.MalFunc
        {
            public override types.MalVal apply(types.MalList arguments)
            {
                //Parse the arguments
                if (arguments.isEmpty())
                    throw new ArgumentException("entity is missing a map of look-up information.");
                if (!(arguments.first() is types.MalMap))
                    throw new ArgumentException("First argument to entity must be a map of look-up information.");

                //Pull the guid out of the map
                types.MalMap mm = arguments.first() as types.MalMap;
                types.MalVal guid = mm.get(types.MalKeyword.keyword(":guid"));
                if (!(guid is types.MalString))
                    throw new ArgumentException("The entity :guid is not a string.");
                return new types.MalObjectReference(entityMap[(guid as types.MalString).value]);
            }
        }

        private class create_guid : types.MalFunc
        {
            public override types.MalVal apply(types.MalList arguments)
            {
                return new types.MalString(System.Guid.NewGuid().ToString());
            }
        }

        public class create_entity : types.MalFunc
        {
            //map of gallery item names to prefabs
            private readonly Dictionary<string, GameObject> galleryMap;

            public create_entity(Dictionary<string, GameObject> galleryMap)
            {
                this.galleryMap = galleryMap;
            }

            public override types.MalVal apply(types.MalList arguments)
            {
                //Parse the arguments
                if (arguments.isEmpty())
                {
                    Debug.Log("create-entity is missing a map of look-up information.");
                    return types.MalNil.malNil;
                }
                if (!(arguments.first() is types.MalMap))
                {
                    Debug.Log("First argument to create-entity must be a map of look-up information.");
                    return types.MalNil.malNil;
                }
                types.MalMap itemData = (types.MalMap)arguments.first();

                //If there is a gallery name, we use the gallery to instantiate the object.
                //Other possibilities will have to be handled another way.
                if (itemData.containsKey(types.MalKeyword.keyword(":gallery-name")))
                {
                    //Instantiate the gallery item
                    types.MalString galleryName = (types.MalString)itemData.get(types.MalKeyword.keyword(":gallery-name"));
                    string prefabName = galleryName.value;
                    GameObject go = GameObject.Instantiate(this.galleryMap[prefabName]);

                    //Give the object a name in the scene
                    if (itemData.containsKey(types.MalKeyword.keyword(":name")))
                    {
                        types.MalString itemName = (types.MalString)itemData.get(types.MalKeyword.keyword(":name"));
                        go.name = itemName.value;
                    }
                    else go.name = galleryName.value;

                    //Get or create the GUID, and add the entity to the global entity map
                    Entity e = go.GetComponent<Entity>();
                    if (itemData.containsKey(types.MalKeyword.keyword(":guid")))
                    {
                        types.MalString itemGuid = (types.MalString)itemData.get(types.MalKeyword.keyword(":guid"));
                        e.guid = itemGuid.value;
                    }
                    else
                        e.guid = ((types.MalString)((types.MalFunc)ns["create-guid"]).apply(types.MalList.empty)).value;
                    entityMap[e.guid] = e;

                    //Read the transform of the object, if specified
                    if (itemData.containsKey(types.MalKeyword.keyword(":transform")))
                    {
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
                    }

                    //Make the world the parent of the new entity (it may get redefined later)
                    go.transform.parent = ((Entity)((types.MalObjectReference)ns["world"]).value).transform;

                    //If there are any children, set the parent of each to the new entity
                    if (itemData.containsKey(types.MalKeyword.keyword(":children")))
                    {
                        types.MalList childData = (types.MalList)itemData.get(types.MalKeyword.keyword(":children"));
                        foreach (types.MalVal child in childData)
                            ((Entity)((types.MalObjectReference)child).value).GetComponent<Transform>().parent = e.GetComponent<Transform>();
                    }

                    //If the new entity is a Program, get the program code out of it.
                    DollhouseProgram p = go.GetComponent<DollhouseProgram>();
                    if (p != null)
                    {
                        types.MalList programData = (types.MalList)itemData.get(types.MalKeyword.keyword(":program"));

                        MalPrinter mp = p.GetProgramUI().GetComponentsInChildren<MalPrinter>(true)[0];
                        foreach (types.MalVal codeChild in programData)
                        {
                            types.MalList codeChildData = (types.MalList)codeChild;
                            float x = ((types.MalNumber)codeChildData.first()).value;
                            float y = ((types.MalNumber)codeChildData.rest().first()).value;
                            MalForm item = mp.pr_form(codeChildData.rest().rest().first());
                            item.transform.localPosition = new Vector3(x, y, 0);
                        }
                        p.GetProgramUI().gameObject.SetActive(false);
                    }

                    return new types.MalObjectReference(e);
                }

                Debug.Log("Unknown gallery name in create-entity: " + printer.pr_str(itemData));
                return types.MalNil.malNil;
            }
        }

        private class remove_entity : types.MalFunc
        {
            public override types.MalVal apply(types.MalList arguments)
            {
                //Parse the arguments
                if (arguments.isEmpty())
                    throw new ArgumentException("remove-entity is missing a map of look-up information.");
                if (!(arguments.first() is types.MalMap))
                    throw new ArgumentException("First argument to remove-entity must be a map of look-up information.");

                //Pull the guid out of the map
                types.MalMap mm = arguments.first() as types.MalMap;
                types.MalVal guid = mm.get(types.MalKeyword.keyword(":guid"));
                if (!(guid is types.MalString))
                    throw new ArgumentException("The entity :guid is not a string.");
                Entity e = entityMap[(guid as types.MalString).value];

                //Remove the guid from the map
                entityMap.Remove((guid as types.MalString).value);

                //Reparent the children
                Transform p = e.transform.parent;
                foreach (Transform child in e.transform)
                    child.parent = p;

                //Destroy the object
                GameObject.Destroy(e.gameObject);

                return types.MalNil.malNil;
            }
        }

    }
}
