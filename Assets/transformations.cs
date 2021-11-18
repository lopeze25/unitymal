//Transformation functions and actions
//Created by James Vanderhyde, 16 November 2021

using System;
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
        ns.Add("move", new move());
        ns.Add("do in order", new do_in_order());
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
            return new OrderControl(done, name);
        }

        public bool IsDone()
        {
            return done;
        }
    }

    public abstract class DollhouseAction : types.MalFunc
    {
        private IEnumerator<OrderControl> coroutine;

        public DollhouseAction()
        {
            this.coroutine = null;
        }

        protected abstract void initialize(types.MalList arguments);
        protected abstract IEnumerator<OrderControl> implementation();

        public override types.MalVal apply(types.MalList arguments)
        {
            this.initialize(arguments.rest()); //first is the MonoBehaviour
            this.coroutine = this.implementation();

            types.MalObjectReference mor = (types.MalObjectReference)arguments.first();
            GameObject obj = (GameObject)mor.value;
            MalForm component = obj.GetComponent<MalForm>();
            component.StartCoroutine(this.coroutine);

            return this;
        }

        public bool IsDone()
        {
            return this.coroutine.Current.IsDone();
        }
    }

    private class move : DollhouseAction
    {
        private Transform objectTransform;
        private float distance;

        protected override void initialize(types.MalList arguments)
        {
            if (!(arguments.first() is types.MalObjectReference))
                throw new ArgumentException("First argument must be an object with a transform.");
            if (!(arguments.rest().first() is types.MalNumber))
                throw new ArgumentException("Distance argument must be a number.");

            this.objectTransform = ((GameObject)((types.MalObjectReference)arguments.first()).value).transform;
            this.distance = ((types.MalNumber)arguments.rest().first()).value;
        }

        protected override IEnumerator<OrderControl> implementation()
        {
            Vector3 direction = Vector3.forward;
            float distance = this.distance;
            float time = 1f;

            float speed = distance / time;
            while (time > 0)
            {
                this.objectTransform.Translate(speed * Time.deltaTime * direction);
                time -= Time.deltaTime;
                yield return OrderControl.Running(time <= 0, "Move:" + time);
            }
        }
    }

    private class do_in_order : DollhouseAction
    {
        private types.MalList actions;

        protected override void initialize(types.MalList arguments)
        {
            this.actions = arguments;
        }

        protected override IEnumerator<OrderControl> implementation()
        {
            //Start one action at a time and wait for it to finish
            foreach (types.MalVal argument in actions)
            {
                types.MalVal actionVal = argument;
                if (actionVal is types.DelayCall)
                {
                    //Start the action
                    actionVal = (actionVal as types.DelayCall).Deref();
                }
                if (actionVal is DollhouseAction)
                {
                    //Wait for it to finish
                    DollhouseAction action = actionVal as DollhouseAction;
                    while (!action.IsDone())
                    {
                        yield return OrderControl.Running(false, "do in order");
                    }
                }
                //If the argument was not actually an action, then we just skip it.
            }

            //All the actions are done
            yield return OrderControl.Running(true, "do in order");
        }
    }
}
