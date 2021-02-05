/**
 * 包含StaticConstructorOnStartup的初始化内容。
 * --siiftun1857
 */
using Verse;

namespace Explorite
{
    [StaticConstructorOnStartup]
    public static partial class ExploriteCore
    {
        static ExploriteCore()
        {
            Log.Message("[SayersMOD]Is it working? Did you see me? OuO");

            //LoadedModManager
            //InstelledMods.UpdateStatus();
        }
    }
}
