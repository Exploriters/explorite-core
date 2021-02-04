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
        static HelloWorldLog()
        {
            Log.Message("[SayersMOD]Is it working? Did you see me? OuO");
        }
    }
}
