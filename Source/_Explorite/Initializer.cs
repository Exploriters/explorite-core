/**
 * 包含StaticConstructorOnStartup的初始化内容。
 * --siiftun1857
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Explorite
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
