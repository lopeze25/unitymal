//Control structures for Dollhouse actions
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

        //There is no real need to separate these, as they are called one after the other.
        //  However, logically they do different things:
		//  initialize converts from a MAL list to C# data,
        //  and implementation is a C# implementation of the action.
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

    public class control
    {
        public static readonly Dictionary<string, types.MalVal> ns = new Dictionary<string, types.MalVal>();
        static control()
        {
            ns.Add("do in order", new do_in_order());
            ns.Add("do together", new do_together());
            ns.Add("do only one", new do_only_one());
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

        private class do_together : DollhouseAction
        {
            private types.MalList actions;

            protected override void initialize(types.MalList arguments)
            {
                this.actions = arguments;
            }

            protected override IEnumerator<OrderControl> implementation()
            {
                //Start all the actions
                List<DollhouseAction> actualActions = new List<DollhouseAction>();
                foreach (types.MalVal argument in actions)
                {
                    types.MalVal actionVal = argument;
                    if (actionVal is types.DelayCall)
                    {
                        //Start the action
                        actionVal = (actionVal as types.DelayCall).Deref();
                    }
                    if (actionVal is DollhouseAction)
                        actualActions.Add(actionVal as DollhouseAction);
                    //If the argument was not actually an action, then we just skip it.
                }

                //Wait for each action to finish.
                // If they are different lengths, it will always yield on a longer one.
                foreach (DollhouseAction action in actualActions)
                {
                    //Wait for the action to finish
                    while (!action.IsDone())
                    {
                        yield return OrderControl.Running(false, "do together");
                    }
                }

                //All the actions are done
                yield return OrderControl.Running(true, "do together");
            }
        }

        private class do_only_one : DollhouseAction
        {
            private types.MalList actions;

            protected override void initialize(types.MalList arguments)
            {
                this.actions = arguments;
            }

            protected override IEnumerator<OrderControl> implementation()
            {
                //Start one action at a time to find one that is not immediately done
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
                        //Check if the action is not already done
                        DollhouseAction action = actionVal as DollhouseAction;
                        if (!action.IsDone())
                        {
                            //Wait for it to finish
                            while (!action.IsDone())
                            {
                                yield return OrderControl.Running(false, "do only one");
                            }

                            //Skip all the rest
                            break;
                        }
                    }
                }

                //The action is done, or there weren't any to do
                yield return OrderControl.Running(true, "do only one");
            }
        }
    }
}
