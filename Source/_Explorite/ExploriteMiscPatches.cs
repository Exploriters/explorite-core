/**
 * 包含多个补丁的合集文件。
 * --siiftun1857
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using SaveOurShip2;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
    /**
     * <summary>
     * 包含多个补丁的合集类。
     * </summary>
     */
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, "IDE0055")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, "IDE0058")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, "IDE0060")]
    [StaticConstructorOnStartup]
    internal static partial class ExploriteMiscPatches
    {
        internal static readonly Type patchType = typeof(ExploriteMiscPatches);

        /* TypeInitializationException 未得到解决
        private static MethodInfo Patch(
            Type type,
            string name,
            Type[] parameters = null,
            Type[] generics = null,
            string prefix = null,
            string postfix = null,
            string transpiler = null,
            string finalizer = null,
            bool incase = true
            )
        {
            return incase?harmonyInstance.Patch(AccessTools.Method(type, name, parameters, generics),
                prefix: postfix.NullOrEmpty()?null:new HarmonyMethod(patchType, prefix),
                postfix: postfix.NullOrEmpty()?null:new HarmonyMethod(patchType, postfix),
                transpiler: postfix.NullOrEmpty()?null:new HarmonyMethod(patchType, transpiler),
                finalizer: postfix.NullOrEmpty()?null:new HarmonyMethod(patchType, finalizer))
                :null;
        }*/
        static ExploriteMiscPatches()
        {
            /*
            Patch(typeof(PawnGenerator),                nameof(PawnGenerator.GeneratePawn),     new[] { typeof(PawnGenerationRequest) }, 
                postfix:nameof(GeneratePawnPostfix));
            Patch(typeof(SkillRecord),                  nameof(SkillRecord.Learn),              new[] { typeof(float), typeof(bool) }, 
                prefix: nameof(SkillLearnPrefix));
            Patch(typeof(SkillRecord),                  nameof(SkillRecord.Interval),           null, 
                postfix:nameof(SkillIntervalPostfix));
            Patch(typeof(Pawn_PsychicEntropyTracker),   "get_PainMultiplier",                   null, 
                postfix:nameof(NoPainBounsForCentaursPostfix));
            Patch(typeof(IncidentWorker_WandererJoin),  "CanFireNowSub",                        new[] { typeof(IncidentParms) }, 
                postfix:nameof(WandererJoin_CanFireNowPostfix));
            Patch(typeof(Need_Outdoors),                "get_Disabled",                         null, 
                postfix:nameof(NeedOutdoors_DisabledPostfix));
            Patch(typeof(ShipInteriorMod2),             "HasSpaceSuitSlow",                     new[] { typeof(Pawn) },
                incase: InstelledMods.SoS2, 
                postfix:nameof(HasSpaceSuitSlowPostfix));
            */

            harmonyInstance.Patch(AccessTools.Method(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new[] { typeof(PawnGenerationRequest) }),
                postfix: new HarmonyMethod(patchType, nameof(GeneratePawnPostfix)));

            harmonyInstance.Patch(AccessTools.Method(typeof(SkillRecord), nameof(SkillRecord.Learn), new[] { typeof(float), typeof(bool) }),
                prefix: new HarmonyMethod(patchType, nameof(SkillLearnPrefix)));
            harmonyInstance.Patch(AccessTools.Method(typeof(SkillRecord), nameof(SkillRecord.Interval), new Type[] { }),
                postfix: new HarmonyMethod(patchType, nameof(SkillIntervalPostfix)));

            harmonyInstance.Patch(AccessTools.Method(typeof(Pawn_PsychicEntropyTracker), "get_PainMultiplier", null),
                postfix: new HarmonyMethod(patchType, nameof(NoPainBounsForCentaursPostfix)));

            harmonyInstance.Patch(AccessTools.Method(typeof(IncidentWorker_WandererJoin), "CanFireNowSub", new[] { typeof(IncidentParms) }),
                postfix: new HarmonyMethod(patchType, nameof(WandererJoin_CanFireNowPostfix)));

            harmonyInstance.Patch(AccessTools.Method(typeof(Need_Outdoors), "get_Disabled", new Type[] { }),
                postfix: new HarmonyMethod(patchType, nameof(NeedOutdoors_DisabledPostfix)));

            if (InstelledMods.RimCentaurs)
            { }
            if (InstelledMods.Sayers)
            { }
            if (InstelledMods.GuoGuo)
            { }
            if (InstelledMods.SoS2)
            {
                harmonyInstance.Patch(AccessTools.Method(typeof(ShipInteriorMod2), "HasSpaceSuitSlow", new[] { typeof(Pawn) }),
                    postfix: new HarmonyMethod(patchType, nameof(HasSpaceSuitSlowPostfix)));
            }
        }

        ///<summary>阻止半人马的技能衰退。</summary>
        [HarmonyPrefix]
        public static void SkillLearnPrefix(SkillRecord __instance, ref float xp)
        {
            if (
                __instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
                && pawn.def == AlienCentaurDef
                )
            {
                xp = Math.Max(0, xp);
            }
        }

        ///<summary>移除半人马每日技能训练上限。</summary>
        [HarmonyPostfix]
        public static void SkillIntervalPostfix(SkillRecord __instance)
        {
            if (
                __instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
                && pawn.def == AlienCentaurDef
                )
            {
                __instance.xpSinceMidnight = 0f;
            }
        }

        ///<summary>移除半人马的疼痛带来心灵熵消散增益。</summary>
        [HarmonyPostfix]
        public static void NoPainBounsForCentaursPostfix(Pawn_PsychicEntropyTracker __instance, ref float __result)
        {
            if (__instance.Pawn.def == AlienCentaurDef)
                __result = 1f;
        }

        ///<summary>禁用半人马阵营生成流浪者加入事件。</summary>
        [HarmonyPostfix]
        public static void WandererJoin_CanFireNowPostfix(IncidentParms parms, ref bool __result)
        {
            if (Faction.OfPlayer.def == CentaurPlayerColonyDef)
                __result = false;
        }

        ///<summary>使半人马可以在太空中靠动力装甲存活。</summary>
        [HarmonyPostfix]
        public static void HasSpaceSuitSlowPostfix(Pawn pawn, ref bool __result)
        {
            if (pawn.def == AlienCentaurDef)
            {
                foreach (Apparel app in pawn.apparel.WornApparel)
                {
                    if (app.def.apparel.tags.Contains("EVA"))
                    {
                        __result = true;
                        break;
                    }
                }
            }
        }

        ///<summary>移除半人马的户外需求。</summary>
        [HarmonyPostfix]
        public static void NeedOutdoors_DisabledPostfix(Need_Outdoors __instance, ref bool __result)
        {
            if (
                __instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
                && pawn.def == AlienCentaurDef
                )
            {
                __result = true;
            }
        }

    }
}
