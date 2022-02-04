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
        private readonly bool done;
        private readonly string name; //For debugging purposes, like a thread name

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
        private readonly IEnumerator<OrderControl> coroutine;
        public readonly DollhouseProgram worldObject;
        private readonly DollhouseAction action;
        private readonly types.MalList arguments;

        private DollhouseActionState(IEnumerator<OrderControl> coroutine, DollhouseProgram worldObject, DollhouseAction action, types.MalList arguments)
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

        public static DollhouseActionState StartUnityCoroutine(IEnumerator<OrderControl> coroutine, DollhouseProgram worldObject, DollhouseAction action, types.MalList arguments)
        {
            //Start the coroutine
            worldObject.StartCoroutine(coroutine);

            //Return information about the coroutine so control structures can wait for it
            return new DollhouseActionState(coroutine, worldObject, action, arguments);
        }
    }

    public abstract class DollhouseAction : types.MalFunc
    {
        //A DollhouseAction is a function that, when evaluated,
        //  starts a coroutine and returns a DollhouseActionState.

        private readonly DollhouseProgram outerProgram;

        public DollhouseAction(DollhouseProgram dp)
        {
            this.outerProgram = dp;
        }

        protected abstract IEnumerator<OrderControl> implementation(types.MalList arguments);

        public override types.MalVal apply(types.MalList arguments)
        {
            //Start the implementation
            IEnumerator<OrderControl> coroutine = this.implementation(arguments);

            //Return information about the coroutine so control structures can wait for it
            return DollhouseActionState.StartUnityCoroutine(coroutine, this.outerProgram, this, arguments);
        }
    }

}
