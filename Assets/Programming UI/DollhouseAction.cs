//Dollhouse action classes
//Created by James Vanderhyde, 18 November 2021

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

namespace Dollhouse
{
    public class OrderControl
    {
        private bool done;
        private string name; //For debugging purposes, like a thread name

        private OrderControl(bool done, string name)
        {
            this.done = done;
            this.name = name;
        }

        public static OrderControl Running(bool done, string name)
        {
            //Debug.Log(name+" "+(done?"done":"not done"));
            return new OrderControl(done, name);
        }

        public bool IsDone()
        {
            return done;
        }
    }

    public class DollhouseActionState : types.MalAtom
    {
        private IEnumerator<OrderControl> coroutine;
        public readonly types.MalObjectReference worldObject;
        private DollhouseAction action;
        private types.MalList arguments;

        private DollhouseActionState(IEnumerator<OrderControl> coroutine, types.MalObjectReference worldObject, DollhouseAction action, types.MalList arguments)
        {
            this.coroutine = coroutine;
            this.worldObject = worldObject;
            this.action = action;
            this.arguments = arguments;
        }

        public bool IsDone()
        {
            return this.coroutine.Current.IsDone();
        }

        public static DollhouseActionState StartUnityCoroutine(IEnumerator<OrderControl> coroutine, DollhouseActionState stateWithObject, DollhouseAction action, types.MalList arguments)
        {
            types.MalObjectReference mor = null;
            if (stateWithObject != null)
                mor = stateWithObject.worldObject;
            return StartUnityCoroutine(coroutine, mor, action, arguments);
        }

        public static DollhouseActionState StartUnityCoroutine(IEnumerator<OrderControl> coroutine, types.MalObjectReference worldObject, DollhouseAction action, types.MalList arguments)
        {
            //Get a MonoBehaviour to run the coroutine
            MonoBehaviour component = null;
            if (worldObject != null && worldObject.value is MonoBehaviour)
                component = (MonoBehaviour)worldObject.value;

            //Start the coroutine
            if (component != null)
                component.StartCoroutine(coroutine);
            else
            {
                //In this case there were no actions involving objects, so no time needs to be taken.
                //The coroutine should already be done.
                coroutine.MoveNext();
                Debug.Assert(coroutine.Current.IsDone(), "A Dollhouse action tried to start with no associated world object.");
            }

            //Return information about the coroutine so control structures can wait for it
            return new DollhouseActionState(coroutine, worldObject, action, arguments);
        }
    }

    public abstract class DollhouseAction : types.MalFunc
    {
        //A DollhouseAction is a function that, when evaluated,
        //  starts a coroutine and returns a DollhouseActionState.

        protected abstract types.MalObjectReference getWorldObjectFromArguments(types.MalList arguments); 

        protected abstract IEnumerator<OrderControl> implementation(types.MalList arguments);

        public override types.MalVal apply(types.MalList arguments)
        {
            //Get an object involved in this action. This is needed to provide a MonoBehaviour to call StartCoroutine.
            types.MalObjectReference mor = this.getWorldObjectFromArguments(arguments);

            //Start the implementation
            IEnumerator<OrderControl> coroutine = this.implementation(arguments);

            //Return information about the coroutine so control structures can wait for it
            return DollhouseActionState.StartUnityCoroutine(coroutine, mor, this, arguments);
        }
    }

}
