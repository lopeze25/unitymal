//Transformation functions and actions
//Created by James Vanderhyde, 16 November 2021

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

namespace Dollhouse
{
    public class transformations
    {
        private abstract class TransformationAction : DollhouseAction
        {
            protected abstract IEnumerator<OrderControl> implementation(types.MalMap arguments);

            protected override IEnumerator<OrderControl> implementation(types.MalList arguments)
            {
                return this.implementation((types.MalMap)arguments.first());
            }
        }

        public static readonly Dictionary<string, types.MalVal> ns = new Dictionary<string, types.MalVal>();
        static transformations()
        {
            ns.Add("distance between", new distance_between());
            ns.Add("move", new move());
        }

        private class distance_between : types.MalFunc
        {
            public override types.MalVal apply(types.MalList arguments)
            {
                if (!(arguments.first() is types.MalObjectReference))
                    throw new ArgumentException("First argument must be an object with a transform.");
                if (!(arguments.rest().first() is types.MalObjectReference))
                    throw new ArgumentException("Second argument must be an object with a transform.");

                GameObject a = (GameObject)((types.MalObjectReference)arguments.first()).value;
                GameObject b = (GameObject)((types.MalObjectReference)arguments.rest().first()).value;

                return new types.MalNumber(Vector3.Distance(a.transform.position, b.transform.position));
            }
        }

        private class move : TransformationAction
        {
            protected override IEnumerator<OrderControl> implementation(types.MalMap arguments)
            {
                if (!(arguments.get(types.MalKeyword.keyword(":transform")) is types.MalObjectReference))
                    throw new ArgumentException("First argument must be an object with a transform.");
                if (!(arguments.get(types.MalKeyword.keyword(":distance")) is types.MalNumber))
                    throw new ArgumentException("Distance argument must be a number.");

                Transform objectTransform = ((GameObject)((types.MalObjectReference)arguments.get(types.MalKeyword.keyword(":transform"))).value).transform;
                Vector3 direction = Vector3.forward;
                float distance = ((types.MalNumber)arguments.get(types.MalKeyword.keyword(":distance"))).value;
                float time = 1f;

                float speed = distance / time;
                while (time > 0)
                {
                    objectTransform.Translate(speed * Time.deltaTime * direction);
                    time -= Time.deltaTime;
                    yield return OrderControl.Running(time <= 0, "Move:" + time);
                }
            }
        }
    }
}
