
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;

namespace W
{
    public class Lua
    {
        public static Script Env { get; private set; }

        public static void Run(string code) {
            if (Env == null) Env = new Script();

            Env.Globals["card"] = DynValue.NewNil();
            Env.Globals["button"] = DynValue.NewNumber(0); // 0 为双击
        }

    }
}
