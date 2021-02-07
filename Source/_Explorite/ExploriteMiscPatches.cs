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

        //TypeInitializationException 未得到解决
        /* 
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
            Log.Message($"[Explorite]Patching {type.FullName} with {}");
            return incase?harmonyInstance.Patch(AccessTools.Method(type, name, parameters, generics),
                prefix: postfix.NullOrEmpty()?null:new HarmonyMethod(patchType, prefix),
                postfix: postfix.NullOrEmpty()?null:new HarmonyMethod(patchType, postfix),
                transpiler: postfix.NullOrEmpty()?null:new HarmonyMethod(patchType, transpiler),
                finalizer: postfix.NullOrEmpty()?null:new HarmonyMethod(patchType, finalizer))
                :null;
        }*/
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
        static ExploriteMiscPatches()
        {
            try
            {
                Log.Message("[Explorite]Patching Verse.PawnGenerator.GeneratePawn with postfix GeneratePawnPostfix");
                harmonyInstance.Patch(AccessTools.Method(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new[] { typeof(PawnGenerationRequest) }),
                    postfix: new HarmonyMethod(patchType, nameof(GeneratePawnPostfix)));

                Log.Message("[Explorite]Patching RimWorld.SkillRecord.Learn with prefix SkillLearnPrefix");
                harmonyInstance.Patch(AccessTools.Method(typeof(SkillRecord), nameof(SkillRecord.Learn), new[] { typeof(float), typeof(bool) }),
                    prefix: new HarmonyMethod(patchType, nameof(SkillLearnPrefix)));
                Log.Message("[Explorite]Patching RimWorld.SkillRecord.Interval with postfix SkillIntervalPostfix");
                harmonyInstance.Patch(AccessTools.Method(typeof(SkillRecord), nameof(SkillRecord.Interval), new Type[] { }),
                    postfix: new HarmonyMethod(patchType, nameof(SkillIntervalPostfix)));

                Log.Message("[Explorite]Patching RimWorld.Pawn_PsychicEntropyTracker.get_PainMultiplier with postfix NoPainBounsForCentaursPostfix");
                harmonyInstance.Patch(AccessTools.Method(typeof(Pawn_PsychicEntropyTracker), "get_PainMultiplier", null),
                    postfix: new HarmonyMethod(patchType, nameof(NoPainBounsForCentaursPostfix)));

                Log.Message("[Explorite]Patching RimWorld.IncidentWorker_WandererJoin.CanFireNowSub with postfix WandererJoin_CanFireNowPostfix");
                harmonyInstance.Patch(AccessTools.Method(typeof(IncidentWorker_WandererJoin), "CanFireNowSub", new[] { typeof(IncidentParms) }),
                    postfix: new HarmonyMethod(patchType, nameof(WandererJoinCannotFirePostfix)));

                Log.Message("[Explorite]Patching RimWorld.Need_Outdoors.get_Disabled with postfix NeedOutdoors_DisabledPostfix");
                harmonyInstance.Patch(AccessTools.Method(typeof(Need_Outdoors), "get_Disabled", new Type[] { }),
                    postfix: new HarmonyMethod(patchType, nameof(NeedOutdoors_DisabledPostfix)));

                Log.Message("[Explorite]Patching RimWorld.MeditationFocusDef.CanPawnUse with postfix MeditationFocusCanPawnUsePostfix");
                harmonyInstance.Patch(AccessTools.Method(typeof(MeditationFocusDef), nameof(MeditationFocusDef.CanPawnUse), new Type[] { typeof(Pawn) }),
                    postfix: new HarmonyMethod(patchType, nameof(MeditationFocusCanPawnUsePostfix)));

                if (InstelledMods.RimCentaurs)
                {
                    // 依赖 HediffDef PsychicDeafCentaur
                    Log.Message("[Explorite]Patching RimWorld.StatPart_ApparelStatOffset.TransformValue with postfix PsychicSensitivityPostfix");
                    harmonyInstance.Patch(AccessTools.Method(typeof(StatPart_ApparelStatOffset), nameof(StatPart_ApparelStatOffset.TransformValue)),
                        postfix: new HarmonyMethod(patchType, nameof(PsychicSensitivityPostfix)));
                }
                if (InstelledMods.Sayers)
                { }
                if (InstelledMods.GuoGuo)
                { }
                if (InstelledMods.SoS2)
                {
                    // 依赖 类 SaveOurShip2.ShipInteriorMod2
                    Log.Message("[Explorite]Patching SaveOurShip2.ShipInteriorMod2.HasSpaceSuitSlow with postfix HasSpaceSuitSlowPostfix");
                    harmonyInstance.Patch(AccessTools.Method(AccessTools.TypeByName("SaveOurShip2.ShipInteriorMod2"), "HasSpaceSuitSlow", new[] { typeof(Pawn) }),
                        postfix: new HarmonyMethod(patchType, nameof(HasSpaceSuitSlowPostfix)));
                }
            }
            catch(Exception e)
            {
                Log.Error(
                    "[Explorite]Patch sequence failare, " +
                    $"an exception ({e.GetType().Name}) occurred." +
                    $"Message:\n   {e.Message}\n" +
                    $"Stack Trace:\n   {e.StackTrace}\n"

                           );
            }
        }

        ///<summary>阻止半人马的技能衰退。</summary>
        [HarmonyPrefix]public static void SkillLearnPrefix(SkillRecord __instance, ref float xp)
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
        [HarmonyPostfix]public static void SkillIntervalPostfix(SkillRecord __instance)
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
        [HarmonyPostfix]public static void NoPainBounsForCentaursPostfix(Pawn_PsychicEntropyTracker __instance, ref float __result)
        {
            if (__instance.Pawn.def == AlienCentaurDef)
                __result = 1f;
        }

        ///<summary>禁用半人马阵营生成流浪者加入事件。</summary>
        [HarmonyPostfix]public static void WandererJoinCannotFirePostfix(IncidentParms parms, ref bool __result)
        {
            if (Faction.OfPlayer.def == CentaurPlayerColonyDef)
                __result = false;
        }

        ///<summary>使半人马可以在太空中靠动力装甲存活。</summary>
        [HarmonyPostfix]public static void HasSpaceSuitSlowPostfix(Pawn pawn, ref bool __result)
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
        [HarmonyPostfix]public static void NeedOutdoors_DisabledPostfix(Need_Outdoors __instance, ref bool __result)
        {
            if (
                __instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
                && pawn.def == AlienCentaurDef
                )
            {
                __result = true;
            }
        }

        ///<summary>使心灵敏感度属性受到心灵失聪hediff影响。</summary>
        [HarmonyPostfix]public static void PsychicSensitivityPostfix(StatPart_ApparelStatOffset __instance, StatRequest req, ref float val)
        {
            try
            {
                if (__instance.parentStat == StatDefOf.PsychicSensitivity &&
                    req.HasThing && (((Pawn)req.Thing)?.health?.hediffSet?.HasHediff(DefDatabase<HediffDef>.GetNamed("PsychicDeafCentaur"))) == true)
                {
                    val = 0f;
                }
            }
            catch (NullReferenceException)
            {
            }
            catch (InvalidCastException)
            {
            }
            catch
            {
            }
        }

        ///<summary>使半人马可以使用任何类型的冥想媒介。</summary>
        [HarmonyPostfix]public static void MeditationFocusCanPawnUsePostfix(Pawn p, ref bool __result)
        {
            if (p.def == AlienCentaurDef)
            {
                __result = true;
            }
        }
    }
}
