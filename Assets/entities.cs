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
        }

        private static Dictionary<string, Entity> entityMap = new Dictionary<string, Entity>();
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
            private Dictionary<string, GameObject> galleryMap;

            public create_entity(Dictionary<string, GameObject> galleryMap)
            {
                this.galleryMap = galleryMap;
            }

            public override types.MalVal apply(types.MalList arguments)
            {
                types.MalMap itemData = (types.MalMap)arguments.first();

                //If there is a gallery name, we use the gallery to instantiate the object.
                //Other possibilities will have to be handled another way.
                types.MalString galleryName = (types.MalString)itemData.get(types.MalKeyword.keyword(":gallery-name"));
                string prefabName = galleryName.value;
                GameObject go = GameObject.Instantiate(this.galleryMap[prefabName]);

                types.MalString itemName = (types.MalString)itemData.get(types.MalKeyword.keyword(":name"));
                go.name = itemName.value;
                types.MalString itemGuid = (types.MalString)itemData.get(types.MalKeyword.keyword(":guid"));
                Entity e = go.GetComponent<Entity>();
                e.guid = itemGuid.value;
                entityMap[e.guid] = e;

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
                    ((Entity)((types.MalObjectReference)child).value).GetComponent<Transform>().parent = e.GetComponent<Transform>();

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
        }
    }
}
