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
            public TransformationAction(DollhouseProgram dp) : base(dp) { }

            protected static float getNumberParameter(types.MalMap arguments, string keyword, string exceptionMessage)
            {
                types.MalVal arg = arguments.get(types.MalKeyword.keyword(keyword));
                if (!(arg is types.MalNumber))
                    throw new ArgumentException(exceptionMessage);
                return (arg as types.MalNumber).value;
            }

            protected static T getComponentParameter<T>(types.MalMap arguments, string keyword, string exceptionMessage) where T : Component
            {
                types.MalVal arg = arguments.get(types.MalKeyword.keyword(keyword));
                if (!(arg is types.MalObjectReference) || !((arg as types.MalObjectReference).value is MonoBehaviour))
                    throw new ArgumentException(exceptionMessage);
                return ((Component)(arg as types.MalObjectReference).value).GetComponent<T>();
            }

            protected enum Direction { Forward, Backward, Right, Left, Up, Down }
            protected static Dictionary<Direction, Vector3> directionVectors = new Dictionary<Direction, Vector3>();
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

        public static Dictionary<string, types.MalVal> CreateNamespace(DollhouseProgram dp)
        {
            Dictionary<string, types.MalVal> ns = new Dictionary<string, types.MalVal>
            {
                { "distance-between", new distance_between() },
                { "move", new move(dp) },
                { "turn", new turn(dp) },
                { "tip", new tip(dp) },
                { "turn-to-face", new turn_to_face(dp) }
            };
            return ns;
        }

        private class distance_between : types.MalFunc
        {
            public override types.MalVal apply(types.MalList arguments)
            {
                if (!(arguments.first() is types.MalObjectReference))
                    throw new ArgumentException("First argument must be an entity with a transform.");
                if (!(arguments.rest().first() is types.MalObjectReference))
                    throw new ArgumentException("Second argument must be an entity with a transform.");

                MonoBehaviour a = (MonoBehaviour)((types.MalObjectReference)arguments.first()).value;
                MonoBehaviour b = (MonoBehaviour)((types.MalObjectReference)arguments.rest().first()).value;

                return new types.MalNumber(Vector3.Distance(a.transform.position, b.transform.position));
            }
        }

        private class move : TransformationAction
        {
            public move(DollhouseProgram dp) : base(dp) { }

            protected override IEnumerator<OrderControl> implementation(types.MalMap arguments)
            {
                Transform objectTransform = getComponentParameter<Transform>(arguments, ":transform", "First argument must be an entity with a transform.");
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

        private class turn : TransformationAction
        {
            public turn(DollhouseProgram dp) : base(dp) { }

            protected override IEnumerator<OrderControl> implementation(types.MalMap arguments)
            {
                Transform objectTransform = getComponentParameter<Transform>(arguments, ":transform", "First argument must be an entity with a transform.");
                Direction direction = getDirectionParameter(arguments, ":direction", "Turn direction must be right or left.");
                float revolutions = getNumberParameter(arguments, ":revolutions", "Revolutions argument must be a number.");
                float time = 1f;

                float dir;
                if (direction == Direction.Right) dir = 1f;
                else if (direction == Direction.Left) dir = -1f;
                else throw new ArgumentException("Invalid Turn Direction: " + direction);

                float rotationSpeed = dir * revolutions * 360 / time;
                float t = time;
                while (t > 0)
                {
                    objectTransform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);
                    t -= Time.deltaTime;
                    yield return OrderControl.Running(t <= 0, "Turn:" + t);
                }
            }
        }

        private class tip : TransformationAction
        {
            public tip(DollhouseProgram dp) : base(dp) { }

            protected override IEnumerator<OrderControl> implementation(types.MalMap arguments)
            {
                Transform objectTransform = getComponentParameter<Transform>(arguments, ":transform", "First argument must be an entity with a transform.");
                Direction direction = getDirectionParameter(arguments, ":direction", "Tip direction must be forward, backward, right, or left.");
                float revolutions = getNumberParameter(arguments, ":revolutions", "Revolutions argument must be a number.");
                float time = 1f;

                Vector3 axis;
                if (direction == Direction.Right) axis = Vector3.forward;
                else if (direction == Direction.Left) axis = Vector3.back;
                else if (direction == Direction.Forward) axis = Vector3.right;
                else if (direction == Direction.Backward) axis = Vector3.left;
                else throw new ArgumentException("Invalid Tip Direction: " + direction);

                float rotationSpeed = revolutions * 360 / time;
                float t = time;
                while (t > 0)
                {
                    objectTransform.Rotate(rotationSpeed * Time.deltaTime * axis);
                    t -= Time.deltaTime;
                    yield return OrderControl.Running(t <= 0, "Tip:" + t);
                }
            }
        }

        private class turn_to_face : TransformationAction
        {
            public turn_to_face(DollhouseProgram dp) : base(dp) { }

            protected override IEnumerator<OrderControl> implementation(types.MalMap arguments)
            {
                Transform objectTransform = getComponentParameter<Transform>(arguments, ":transform", "First argument must be an entity with a transform.");
                Transform targetTransform = getComponentParameter<Transform>(arguments, ":target", "Target must be an entity with a transform.");
                float time = 1f;

                Vector3 vectorToTarget = targetTransform.position - objectTransform.position;
                float distance = vectorToTarget.magnitude;
                if (distance > 0.0001f)
                {
                    //Calculate amount and direction
                    Vector3 facingDirection = objectTransform.forward;
                    Vector3 targetDirection = vectorToTarget / distance;
                    float revolutions = Mathf.Acos(Vector3.Dot(facingDirection, targetDirection)) / (2 * Mathf.PI);
                    float dir = Vector3.Dot(objectTransform.up, Vector3.Cross(facingDirection, targetDirection));
                    if (dir < 0) dir = -1;
                    else dir = 1; //If it's close to 0, it doesn't matter which way you turn.

                    //Turn
                    float rotationSpeed = dir * revolutions * 360 / time;
                    float t = time;
                    while (t > 0)
                    {
                        objectTransform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);
                        t -= Time.deltaTime;
                        yield return OrderControl.Running(t <= 0, "TurnToFace:" + t);
                    }
                }
                else
                {
                    //Do nothing for the given time
                    float t = time;
                    while (t > 0)
                    {
                        t -= Time.deltaTime;
                        yield return OrderControl.Running(t <= 0, "TurnToFace");
                    }
                }
            }
        }
    }
}
