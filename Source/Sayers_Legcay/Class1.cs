using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Sayers
{
    [StaticConstructorOnStartup]
    public static class HelloWorldLog
    {
        public enum TickerType : byte
        {
            Never = 0,
            Normal = 1,
            Rage = 2,
            Long = 3
        }
        static HelloWorldLog()
        {
            Log.Message("My computer is too laggy QAQ I'm sad!DX");
        }
    }
    public class Class1
    {
    }
}
