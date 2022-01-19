//Control structures for Dollhouse actions
//Created by James Vanderhyde, 18 November 2021

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mal;

namespace Dollhouse
{
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

                //Evaluate the first argument
                env.Environment doEnv = new env.Environment(environment, false);
                types.MalVal actionArg = evaluator.eval_ast(arguments.first(), doEnv);

                //Get the second argument to evaluate later
                types.MalVal doLater = types.MalNil.malNil;
                if (!arguments.rest().isEmpty())
                    doLater = arguments.rest().first();

                //Check if the first argument is an action, and if it has anything to do
                if (actionArg is DollhouseActionState)
                {
                    DollhouseActionState actionState = actionArg as DollhouseActionState;
                    if (!actionState.IsDone())
                    {
                        //Create a delay call for the second action
                        env.Environment doEnvTail = new env.Environment(environment, true);
                        types.DelayCall doLaterDelay = new types.DelayCall(doLater, doEnvTail);

                        //Start the coroutine method to wait through both actions
                        IEnumerator<OrderControl> coroutine = waitForBoth(actionState, doLaterDelay);

                        //Return information about the coroutine so control structures can wait for it
                        return DollhouseActionState.StartUnityCoroutine(coroutine, actionState, null, types.MalList.empty);
                    }
                }

                //Otherwise, we skip the first argument, and evaluate and return the second argument immediately
                return evaluator.eval_ast(doLater, doEnv);
            }

            private IEnumerator<OrderControl> waitForBoth(DollhouseActionState actionState, types.DelayCall doLaterDelay)
            {
                //Wait for the first action to finish
                while (!actionState.IsDone())
                {
                    yield return OrderControl.Running(false, "do-wait");
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
                //Look through the action arguments to find the first that does something
                DollhouseActionState action = findFirstThatDoesSomething(arguments, environment);

                //Start the coroutine method to wait for the action that was found
                IEnumerator<OrderControl> coroutine = doAction(action);
                
                //Return information about the coroutine so control structures can wait for it
                return DollhouseActionState.StartUnityCoroutine(coroutine, action, null, types.MalList.empty);
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
