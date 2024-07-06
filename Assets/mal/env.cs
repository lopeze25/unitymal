//Environment functions for mal
//Created by James Vanderhyde, 11 October 2021

using System;
using System.Collections.Generic;
using Mal;

namespace Mal
{
    public class env
    {
        public static readonly Environment baseEnvironment = new Environment(null, false);
        static env()
        {
            baseEnvironment.setAll(core.ns);
        }

        public class Environment
        {
            public readonly Environment outer;
            private Dictionary<string, types.MalVal> dict;
            public readonly types.FuncClosure recurPoint;

            public Environment(Environment outer, bool tailPosition, types.FuncClosure recurPoint = null)
            {
                this.outer = outer;
                this.dict = new Dictionary<string, types.MalVal>();
                if (!tailPosition)
                    this.recurPoint = null;
                else if (recurPoint != null)
                    this.recurPoint = recurPoint;
                else
                    this.recurPoint = outer.recurPoint;
            }

            public void setAll(Dictionary<string, types.MalVal> ns)
            {
                foreach (KeyValuePair<string, types.MalVal> item in ns)
                    this.set(item.Key, item.Value);
            }

            public void set(string key, types.MalVal value)
            {
                this.dict.Remove(key);
                this.dict.Add(key, value);
            }

            public Environment find(string key)
            {
                if (this.dict.ContainsKey(key))
                    return this;
                else
                    return this.outer?.find(key);
            }

            public types.MalVal get(string key)
            {
                Environment e = this.find(key);
                if (e != null) return e.dict[key];
                else throw new ArgumentException("Symbol " + key + " not found.");
            }

            public types.MalVector getBoundSymbols()
            {
                types.MalVector v = new types.MalVector();
                foreach ((string symbol, types.MalVal value) in this.dict)
                {
                    v.conj(new types.MalSymbol(symbol));
                    v.conj(value);
                }
                return v;
            }
        }
    }
}