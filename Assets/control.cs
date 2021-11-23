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
            //Get the UI form this action came from. This is useful for
            // (1) providing a MonoBehaviour to call StartCoroutine
            // (2) future goal of code highlighting during execution
            types.MalObjectReference mor = (types.MalObjectReference)arguments.first();
            GameObject obj = (GameObject)mor.value;
            MalForm component = obj.GetComponent<MalForm>();

            //Start the coroutine
            IEnumerator<OrderControl> coroutine = this.implementation(arguments.rest());
            component.StartCoroutine(coroutine);

            //Return information about the coroutine so control structures can wait for it
            return new DollhouseActionState(coroutine, component, this, arguments.rest());
        }
    }

    public class control
    {
        public static readonly Dictionary<string, types.MalVal> ns = new Dictionary<string, types.MalVal>();
        static control()
        {
            ns.Add("no-op", new no_op());
            ns.Add("do-wait", new do_wait());
            ns.Add("do in order", new do_in_order());
            ns.Add("do together", new do_together());
            ns.Add("do only one", new do_only_one());
        }

        private class no_op : DollhouseAction
        {
            protected override IEnumerator<OrderControl> implementation(types.MalList arguments)
            {
                yield return OrderControl.Running(true, "no-op");
            }
        }

        private class do_wait : types.MalMacro
        {
            public override types.MalVal apply(types.MalList arguments, env.Environment environment)
            {
                //Parse the arguments
                if (arguments.isEmpty() || arguments.rest().isEmpty())
                    throw new ArgumentException("do-wait is missing a value.");
                env.Environment doEnv = new env.Environment(environment, false);
                types.MalVal component = evaluator.eval_ast(arguments.first(), doEnv);
                types.MalObjectReference mor = (types.MalObjectReference)component;
                UnityEngine.GameObject obj = (UnityEngine.GameObject)mor.value;
                MalForm componentForm = obj.GetComponent<MalForm>();
                types.MalVal action = evaluator.eval_ast(arguments.rest().first(), doEnv);
                env.Environment doEnvTail = new env.Environment(environment, true);
                types.MalVal doLater = types.MalNil.malNil;
                if (!arguments.rest().rest().isEmpty())
                    doLater = arguments.rest().rest().first();
                types.DelayCall doLaterDelay = new types.DelayCall(doLater, doEnvTail);

                //Start the coroutine to wait
                IEnumerator<OrderControl> coroutine = doAndWait(componentForm, action, doLaterDelay);
                componentForm.StartCoroutine(coroutine);

                //Return information about the coroutine so control structures can wait for it
                return new DollhouseActionState(coroutine, componentForm, null, types.MalList.empty);
            }

            private IEnumerator<OrderControl> doAndWait(MalForm component, types.MalVal action, types.DelayCall doLaterDelay)
            {
                if (action is DollhouseActionState)
                {
                    DollhouseActionState actionState = action as DollhouseActionState;

                    //Wait for the action to finish
                    while (!actionState.IsDone())
                    {
                        yield return OrderControl.Running(false, "do-wait");
                    }
                }

                //Evaluate the next action
                types.MalVal result = doLaterDelay.Deref();
                //Cases for the body of doLaterDelay:
                //  "do-wait" or an action, which returns a DollhouseActionState.
                //    Either way, coroutines are continuing to be started. Just keep waiting.
                //  nil or any other value; the coroutines are done.
                //  "recur", which calls the function or loop again. Either of the above may be the result.
                if (result is DollhouseActionState)
                {
                    DollhouseActionState resultState = result as DollhouseActionState;

                    //Wait for the action to finish
                    while (!resultState.IsDone())
                    {
                        yield return OrderControl.Running(false, "do-wait");
                    }
                    yield return OrderControl.Running(true, "do-wait");
                }
                else
                    yield return OrderControl.Running(true, "do-wait "+result.GetType());
            }
        }

        private class do_in_order : types.MalMacro
        {
            public override types.MalVal apply(types.MalList arguments, env.Environment environment)
            {
                return evaluator.eval_ast(expand(arguments, environment), environment);
            }

            private types.MalVal expand(types.MalList arguments, env.Environment environment)
            {
                if (arguments.isEmpty())
                    throw new ArgumentException("do in order is missing a component.");

                types.MalVal component = arguments.first();
                types.MalList actions = arguments.rest();

                //Null case: no actions, do nothing
                if (actions.isEmpty())
                {
                    types.MalList nop = new types.MalList();
                    nop.cons(component); //inject the component
                    nop.cons(ns["no-op"]);
                    return nop;
                }

                //Base case: one action, return the action
                if (actions.rest().isEmpty())
                {
                    return actions.first();
                }

                //Recursive do in order on the rest of the actions
                types.MalList doi = actions.rest();
                doi.cons(component); //inject the component
                doi.cons(this);

                //Do the first action, wait to finish, and then evaluate the rest.
                types.MalList dw = new types.MalList();
                dw.cons(doi);
                dw.cons(actions.first());
                dw.cons(component); //inject the component
                dw.cons(ns["do-wait"]);
                return dw;
            }
        }

        private class do_together : DollhouseAction
        {
            protected override IEnumerator<OrderControl> implementation(types.MalList arguments)
            {
                //Check all the actions, which were already started when the function was evaluated
                List<DollhouseActionState> actualActions = new List<DollhouseActionState>();
                foreach (types.MalVal argument in arguments)
                {
                    if (argument is DollhouseActionState)
                        actualActions.Add(argument as DollhouseActionState);
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

        private class do_only_one : types.MalMacro
        {
            public override types.MalVal apply(types.MalList arguments, env.Environment environment)
            {
                return evaluator.eval_ast(expand(arguments, environment), environment);
            }

            private types.MalVal expand(types.MalList arguments, env.Environment environment)
            {
                return types.MalNil.malNil;
            }

            protected IEnumerator<OrderControl> implementation(types.MalList arguments)
            {
                //Start one action at a time to find one that is not immediately done
                foreach (types.MalVal argument in arguments)
                {
                    if (argument is DollhouseActionState)
                    {
                        //Check if the action is not already done
                        DollhouseActionState action = argument as DollhouseActionState;
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
