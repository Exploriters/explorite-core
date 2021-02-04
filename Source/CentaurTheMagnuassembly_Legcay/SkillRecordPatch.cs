using System;
using System.Reflection;
//using Harmony;
using HarmonyLib;
using RimWorld;
using Verse;
using static CentaurTheMagnuassembly.RimCentaurCore;

namespace CentaurTheMagnuassembly
{
    [StaticConstructorOnStartup]
    public static class SkillRecordPatch
    {
        private static readonly Type patchType = typeof(SkillRecordPatch);
        static SkillRecordPatch()
        {
            Harmony harmonyInstance = new Harmony("CentaurTheMagnuassembly.rimworld.mod.SkillRecordPatch");

            harmonyInstance.Patch(AccessTools.Method(typeof(SkillRecord), nameof(SkillRecord.Learn), new[] { typeof(float), typeof(bool) }),
                prefix: new HarmonyMethod(patchType, nameof(SkillLearnPrefix)));
            harmonyInstance.Patch(AccessTools.Method(typeof(SkillRecord), nameof(SkillRecord.Interval), new Type[] { }),
                postfix: new HarmonyMethod(patchType, nameof(SkillIntervalPostfix)));
        }
        public static void SkillLearnPrefix(SkillRecord __instance, ref float xp)
        {
            if (__instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn)
            {
                if (pawn.def == AlienCentaurDef)
                {
                    xp = Math.Max(0, xp);
                }
            }
        }
        public static void SkillIntervalPostfix(SkillRecord __instance)
        {
            __instance.xpSinceMidnight = 0f;
        }
    }
}
