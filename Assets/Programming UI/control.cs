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
        public readonly types.MalObjectReference worldObject;
        private DollhouseAction action;
        private types.MalList arguments;

        public DollhouseActionState(IEnumerator<OrderControl> coroutine, types.MalObjectReference worldObject, DollhouseAction action, types.MalList arguments)
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
            Draggable3D component = null;
            if (mor != null && mor.value != null)
                component = ((GameObject)mor.value).GetComponent<Draggable3D>();

            //Start the coroutine
            IEnumerator<OrderControl> coroutine = this.implementation(arguments);
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
            return new DollhouseActionState(coroutine, mor, this, arguments);
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
            protected override types.MalObjectReference getWorldObjectFromArguments(types.MalList arguments)
            {
                return null;
            }

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
                if (arguments.isEmpty())
                    throw new ArgumentException("do-wait is missing a value.");
                env.Environment doEnv = new env.Environment(environment, false);

                //Get the first argument, and a component for running the coroutine
                types.MalVal actionArg = evaluator.eval_ast(arguments.first(), doEnv);
                types.MalObjectReference mor = null;
                Draggable3D component = null;
                if (actionArg is DollhouseActionState)
                {
                    DollhouseActionState actionState = actionArg as DollhouseActionState;
                    mor = actionState.worldObject;
                    if (mor != null && mor.value != null)
                        component = ((GameObject)mor.value).GetComponent<Draggable3D>();
                }

                //Get the second argument to evaluate later
                env.Environment doEnvTail = new env.Environment(environment, true);
                types.MalVal doLater = types.MalNil.malNil;
                if (!arguments.rest().isEmpty())
                    doLater = arguments.rest().first();
                types.DelayCall doLaterDelay = new types.DelayCall(doLater, doEnvTail);

                //Start the coroutine to wait
                IEnumerator<OrderControl> coroutine = doAndWait(actionArg, doLaterDelay);
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
                return new DollhouseActionState(coroutine, mor, null, types.MalList.empty);
            }

            private IEnumerator<OrderControl> doAndWait(types.MalVal action, types.DelayCall doLaterDelay)
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
                types.MalList actions = arguments;

                //Null case: no actions, do nothing
                if (actions.isEmpty())
                {
                    types.MalList nop = new types.MalList();
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
                doi.cons(this);

                //Do the first action, wait to finish, and then evaluate the rest.
                types.MalList dw = new types.MalList();
                dw.cons(doi);
                dw.cons(actions.first());
                dw.cons(ns["do-wait"]);
                return dw;
            }
        }

        private class do_together : DollhouseAction
        {
            protected override types.MalObjectReference getWorldObjectFromArguments(types.MalList arguments)
            {
                //Check all the actions, which were already started when the function was evaluated
                List<DollhouseActionState> actualActions = new List<DollhouseActionState>();
                foreach (types.MalVal argument in arguments)
                {
                    if (argument is DollhouseActionState)
                        if ((argument as DollhouseActionState).worldObject != null)
                            return (argument as DollhouseActionState).worldObject;
                }

                //No world objects were found
                return null;
            }

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
                //Get an object involved in this action. This is needed to provide a MonoBehaviour to call StartCoroutine.
                DollhouseActionState action = findFirstThatDoesSomething(arguments, environment);
                types.MalObjectReference mor = null;
                if (action != null)
                    mor = action.worldObject;
                Draggable3D component = null;
                if (action != null && mor != null && mor.value != null)
                    component = ((GameObject)mor.value).GetComponent<Draggable3D>();

                //Start the coroutine to do something
                IEnumerator<OrderControl> coroutine = doAction(action);
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
                return new DollhouseActionState(coroutine, mor, null, types.MalList.empty);
            }

            private DollhouseActionState findFirstThatDoesSomething(types.MalList actions, env.Environment environment)
            {
                //Evaluate one action at a time to find one that is not immediately done
                foreach (types.MalVal argument in actions)
                {
                    types.MalVal evalArg = evaluator.eval_ast(argument, environment);
                    if (evalArg is DollhouseActionState)
                    {
                        //Check if the action is not already done
                        DollhouseActionState action = evalArg as DollhouseActionState;
                        if (!action.IsDone())
                        {
                            //Use this one, and skip all the rest
                            return action;
                        }
                    }
                }

                //No actions that did anything were found.
                return null;
            }

            private IEnumerator<OrderControl> doAction(DollhouseActionState action)
            {
                //Wait for it to finish
                while (!action.IsDone())
                {
                    yield return OrderControl.Running(false, "do only one");
                }

                //The action is done
                yield return OrderControl.Running(true, "do only one");
            }
        }
    }
}
