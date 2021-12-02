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
            protected static float getNumberParameter(types.MalMap arguments, string keyword, string exceptionMessage)
            {
                types.MalVal arg = arguments.get(types.MalKeyword.keyword(keyword));
                if (!(arg is types.MalNumber))
                    throw new ArgumentException(exceptionMessage);
                return (arg as types.MalNumber).value;
            }

            protected static T getComponentParameter<T>(types.MalMap arguments, string keyword, string exceptionMessage)
            {
                types.MalVal arg = arguments.get(types.MalKeyword.keyword(keyword));
                if (!(arg is types.MalObjectReference))
                    throw new ArgumentException(exceptionMessage);
                return ((GameObject)(arg as types.MalObjectReference).value).GetComponent<T>();
            }

            protected enum Direction { Forward, Backward, Right, Left, Up, Down }
            protected static Dictionary<Direction,Vector3> directionVectors = new Dictionary<Direction, Vector3>();
            static TransformationAction()
            {
                directionVectors.Add(Direction.Forward, Vector3.forward);
                directionVectors.Add(Direction.Backward, Vector3.back);
                directionVectors.Add(Direction.Right, Vector3.right);
                directionVectors.Add(Direction.Left, Vector3.left);
                directionVectors.Add(Direction.Up, Vector3.up);
                directionVectors.Add(Direction.Down, Vector3.down);
            }

            protected static Direction getDirectionParameter(types.MalMap arguments, string keyword, string exceptionMessage)
            {
                types.MalVal arg = arguments.get(types.MalKeyword.keyword(keyword));
                if (!(arg is types.MalKeyword))
                    throw new ArgumentException(exceptionMessage);
                types.MalKeyword key = arg as types.MalKeyword;
                if (key.Equals(types.MalKeyword.keyword(":forward")))
                    return Direction.Forward;
                if (key.Equals(types.MalKeyword.keyword(":backward")))
                    return Direction.Backward;
                if (key.Equals(types.MalKeyword.keyword(":right")))
                    return Direction.Right;
                if (key.Equals(types.MalKeyword.keyword(":left")))
                    return Direction.Left;
                if (key.Equals(types.MalKeyword.keyword(":up")))
                    return Direction.Up;
                if (key.Equals(types.MalKeyword.keyword(":down")))
                    return Direction.Down;
                throw new ArgumentException(exceptionMessage);
            }

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
                Transform objectTransform = getComponentParameter<Transform>(arguments, ":transform", "First argument must be an object with a transform.");
                Direction dir = getDirectionParameter(arguments, ":direction", "Move direction must be forward, backward, right, left, up, or down.");
                Vector3 direction = directionVectors[dir];
                float distance = getNumberParameter(arguments, ":distance", "Distance argument must be a number.");
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
