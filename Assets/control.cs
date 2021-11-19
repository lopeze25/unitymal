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
        private MalForm form;
        private DollhouseAction action;
        private types.MalList arguments;

        public DollhouseActionState(IEnumerator<OrderControl> coroutine, MalForm form, DollhouseAction action, types.MalList arguments)
        {
            this.coroutine = coroutine;
            this.form = form;
            this.action = action;
            this.arguments = arguments;
        }

        public bool IsDone()
        {
            return this.coroutine.Current.IsDone();
        }
    }

    public abstract class DollhouseAction : types.MalFunc
    {
        //We cannot store any state in the DollhouseAction, because each command is a singleton.

        protected abstract IEnumerator<OrderControl> implementation(types.MalList arguments);

        public override types.MalVal apply(types.MalList arguments)
        {
            types.MalObjectReference mor = (types.MalObjectReference)arguments.first();
            GameObject obj = (GameObject)mor.value;
            MalForm component = obj.GetComponent<MalForm>();

            IEnumerator<OrderControl> coroutine = this.implementation(arguments.rest());
            component.StartCoroutine(coroutine);

            return new DollhouseActionState(coroutine, component, this, arguments.rest());
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
            protected override IEnumerator<OrderControl> implementation(types.MalList arguments)
            {
                //Start one action at a time and wait for it to finish
                foreach (types.MalVal argument in arguments)
                {
                    types.MalVal actionVal = argument;
                    if (actionVal is types.DelayCall)
                    {
                        //Start the action
                        actionVal = (actionVal as types.DelayCall).Deref();
                    }
                    if (actionVal is DollhouseActionState)
                    {
                        //Wait for it to finish
                        DollhouseActionState action = actionVal as DollhouseActionState;
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
            protected override IEnumerator<OrderControl> implementation(types.MalList arguments)
            {
                //Start all the actions
                List<DollhouseActionState> actualActions = new List<DollhouseActionState>();
                foreach (types.MalVal argument in arguments)
                {
                    types.MalVal actionVal = argument;
                    if (actionVal is types.DelayCall)
                    {
                        //Start the action
                        actionVal = (actionVal as types.DelayCall).Deref();
                    }
                    if (actionVal is DollhouseActionState)
                        actualActions.Add(actionVal as DollhouseActionState);
                    //If the argument was not actually an action, then we just skip it.
                }

                //Wait for each action to finish.
                // If they are different lengths, it will always yield on a longer one.
                foreach (DollhouseActionState action in actualActions)
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
            protected override IEnumerator<OrderControl> implementation(types.MalList arguments)
            {
                //Start one action at a time to find one that is not immediately done
                foreach (types.MalVal argument in arguments)
                {
                    types.MalVal actionVal = argument;
                    if (actionVal is types.DelayCall)
                    {
                        //Start the action
                        actionVal = (actionVal as types.DelayCall).Deref();
                    }
                    if (actionVal is DollhouseActionState)
                    {
                        //Check if the action is not already done
                        DollhouseActionState action = actionVal as DollhouseActionState;
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
