/**
 * 阻止半人马的技能衰退，移除半人马每日技能训练上限。
 * --siiftun1857
 */
using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
    /**
     * <summary>
     * 阻止半人马的技能衰退，移除半人马每日技能训练上限。
     * </summary>
     */
    [StaticConstructorOnStartup]
    internal static class SkillRecordPatch
    {
        private static readonly Type patchType = typeof(SkillRecordPatch);
        static SkillRecordPatch()
        {
            //Harmony harmonyInstance = new Harmony("Explorite.rimworld.mod.SkillRecordPatch");

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
