using System;
//using Harmony;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CentaurTheMagnuassembly
{
    public static class SkillRecordPatch
    {
        /*
        private static readonly Type patchType = typeof(AdditionalVerbPatch);
        static SkillRecordPatch()
        {
            HarmonyInstance harmonyInstance = HarmonyInstance.Create("CentaurTheMagnuassembly.rimworld.mod.SkillRecordPatch");
            if (!harmonyInstance.HasAnyPatches("CentaurTheMagnuassembly.rimworld.mod.SkillRecordPatch"))
            {
                harmonyInstance.Patch(
                    AccessTools.Method(
                        typeof(Pawn_EquipmentTracker), 
                        "GetGizmos", null, null), 
                    null, 
                    new HarmonyMethod(
                        patchType, 
                        "GetGizmosPostfix",
                        null)
                    );

                harmonyInstance.Patch(
                    AccessTools.Property(
                        typeof(VerbTracker),
                        "PrimaryVerb").GetGetMethod(), 
                    new HarmonyMethod(
                        patchType, 
                        "PrimaryVerbPrefix",
                        null)
                    );
            }
        }
        public static void Interval(SkillRecord __instance)
        {
            float num = (!__instance.pawn.story.traits.HasTrait(TraitDefOf.GreatMemory)) ? 1f : 0.5f;
            switch (__instance.levelInt)
            {
                case 10:
                    __instance.Learn(-0.1f * num, false);
                    break;
                case 11:
                    __instance.Learn(-0.2f * num, false);
                    break;
                case 12:
                    __instance.Learn(-0.4f * num, false);
                    break;
                case 13:
                    __instance.Learn(-0.6f * num, false);
                    break;
                case 14:
                    __instance.Learn(-1f * num, false);
                    break;
                case 15:
                    __instance.Learn(-1.8f * num, false);
                    break;
                case 16:
                    __instance.Learn(-2.8f * num, false);
                    break;
                case 17:
                    __instance.Learn(-4f * num, false);
                    break;
                case 18:
                    __instance.Learn(-6f * num, false);
                    break;
                case 19:
                    __instance.Learn(-8f * num, false);
                    break;
                case 20:
                    __instance.Learn(-12f * num, false);
                    break;
            }
        }
        */
    }
}
