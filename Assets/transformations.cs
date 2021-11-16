//Transformation functions and actions
//Created by James Vanderhyde, 16 November 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

public class transformations
{
    public static readonly Dictionary<string, types.MalVal> ns = new Dictionary<string, types.MalVal>();
    static transformations()
    {
        ns.Add("distance between", new distance_between());
    }

    private class distance_between : types.MalFunc
    {
        public override types.MalVal apply(types.MalList arguments)
        {
            GameObject a = (GameObject)((types.MalObjectReference)arguments.first()).value;
            GameObject b = (GameObject)((types.MalObjectReference)arguments.rest().first()).value;

            return new types.MalNumber(Vector3.Distance(a.transform.position, b.transform.position));
        }
    }
}
