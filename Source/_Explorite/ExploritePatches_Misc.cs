/********************
 * 包含多个补丁的合集文件。
 * --siiftun1857
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
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
    internal static partial class ExploritePatches
    {
        internal static readonly Type patchType = typeof(ExploritePatches);

        static ExploritePatches()
        {
            string last_patch = "";
            try
            {
                harmonyInstance.Patch(AccessTools.Method(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new[] { typeof(PawnGenerationRequest) }),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(GeneratePawnPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(SkillRecord), nameof(SkillRecord.Learn)),
                    prefix: new HarmonyMethod(patchType, last_patch = nameof(SkillLearnPrefix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(SkillRecord), nameof(SkillRecord.Interval)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(SkillIntervalPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(Pawn_PsychicEntropyTracker), "get_PainMultiplier"),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(NoPainBounsForCentaursPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(IncidentWorker_WandererJoin), "CanFireNowSub"),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(WandererJoinCannotFirePostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(Need_Outdoors), "get_Disabled"),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(NeedOutdoors_DisabledPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(Need_Mood), nameof(Need_Mood.GetTipString)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(NeedMood_GetTipStringPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(Need), nameof(Need.DrawOnGUI)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(Need_DrawOnGUIPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(Need_Seeker), nameof(Need_Seeker.NeedInterval)),
                    prefix: new HarmonyMethod(patchType, last_patch = nameof(Need_NeedIntervalPrefix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(Need_Seeker), nameof(Need_Seeker.NeedInterval)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(Need_NeedIntervalPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(ThoughtWorker_NeedComfort), "CurrentStateInternal"),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(ThoughtWorker_NeedComfort_CurrentStateInternalPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(MeditationFocusDef), nameof(MeditationFocusDef.CanPawnUse)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(MeditationFocusCanPawnUsePostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(MeditationFocusDef), nameof(MeditationFocusDef.EnablingThingsExplanation)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(MeditationFocusExplanationPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(MentalBreaker), "get_BreakThresholdMinor"),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(MentalBreaker_BreakThresholdMinorPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(MentalBreaker), "get_BreakThresholdMajor"),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(MentalBreaker_BreakThresholdMajorPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(MentalBreaker), "get_BreakThresholdExtreme"),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(MentalBreaker_BreakThresholdExtremePostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(CompAssignableToPawn), "get_AssigningCandidates"),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(AssignToPawnCandidatesPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(CompAssignableToPawn), "get_HasFreeSlot"),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(AssignBedToPawnHasFreeSlotPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(CompAssignableToPawn_Bed), nameof(CompAssignableToPawn_Bed.TryAssignPawn)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(AssignBedToPawnTryAssignPawnPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(RestUtility), nameof(RestUtility.CanUseBedEver)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(RestUtilityCanUseBedEverPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(Plant), "get_GrowthRateFactor_Temperature"),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(PlantGrowthRateFactorNoTemperaturePostfix)));
                //harmonyInstance.Patch(AccessTools.Method(typeof(Plant), "get_Resting"),
                //    postfix: new HarmonyMethod(patchType, last_patch = nameof(PlantNoRestingPostfix)));
                //harmonyInstance.Patch(AccessTools.Method(typeof(Plant), "get_GrowthRate"),
                //    postfix: new HarmonyMethod(patchType, last_patch = nameof(PlantGrowthRateFactorEnsurePostfix)));
                //harmonyInstance.Patch(AccessTools.Method(typeof(Plant), "get_LeaflessNow"),
                //    postfix: new HarmonyMethod(patchType, last_patch = nameof(PlantLeaflessNowPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(Alert_NeedBatteries), "NeedBatteries"),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(AlertNeedBatteriesPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(JobGiver_GetFood), "TryGiveJob"),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(GetFoodTryGiveJobPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), "get_InPainShock"),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(PawnHealthTrackerInPainShockPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(MassUtility), nameof(MassUtility.Capacity)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(MassUtilityCapacityPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(HediffComp_GetsPermanent), "set_IsPermanent"),
                    prefix: new HarmonyMethod(patchType, last_patch = nameof(HediffComp_GetsPermanentIsPermanentPrefix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(HediffComp_TendDuration), "get_AllowTend"),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(HediffComp_TendDurationAllowTendPrefix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(StatPart_ApparelStatOffset), nameof(StatPart.TransformValue)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(PsychicSensitivityPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(PawnGraphicSet), nameof(PawnGraphicSet.ResolveAllGraphics)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(PawnGraphicSetResolveAllGraphicsPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(CompAffectedByFacilities), nameof(CompAffectedByFacilities.CanPotentiallyLinkTo_Static), new Type[] { typeof(ThingDef), typeof(IntVec3), typeof(Rot4), typeof(ThingDef), typeof(IntVec3), typeof(Rot4) }),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(CompAffectedByFacilitiesCanPotentiallyLinkToStaticPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(PawnGraphicSet), nameof(PawnGraphicSet.ResolveApparelGraphics)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(PawnGraphicSetResolveApparelGraphicsPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(TraitSet), nameof(TraitSet.DegreeOfTrait)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(TraitSetDegreeOfTraitPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(TraitSet), nameof(TraitSet.HasTrait)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(TraitSetHasTraitPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(TraitSet), nameof(TraitSet.GetTrait)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(TraitSetGetTraitPostfix)));


                if (InstelledMods.SoS2)
                {
                    // 依赖 类 SaveOurShip2.ShipInteriorMod2
                    harmonyInstance.Patch(AccessTools.Method(AccessTools.TypeByName("SaveOurShip2.ShipInteriorMod2"), "HasSpaceSuitSlow", new[] { typeof(Pawn) }),
                        postfix: new HarmonyMethod(patchType, last_patch = nameof(HasSpaceSuitSlowPostfix)));

                    // 依赖 类 RimWorld.ThoughtWorker_SpaceThoughts
                    harmonyInstance.Patch(AccessTools.Method(AccessTools.TypeByName("RimWorld.ThoughtWorker_SpaceThoughts"), "CurrentStateInternal"),
                        postfix: new HarmonyMethod(patchType, last_patch = nameof(ThoughtWorker_SpaceThoughts_CurrentStateInternalPostfix)));
                }
            }
            catch (Exception e)
            {
                Log.Error(
                    $"[Explorite]Patch sequence failare at {last_patch}, " +
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

        ///<summary>禁用阵营生成流浪者加入事件。</summary>
        [HarmonyPostfix]public static void WandererJoinCannotFirePostfix(IncidentParms parms, ref bool __result)
        {
            if (Faction.OfPlayer.def == CentaurPlayerColonyDef
             || Faction.OfPlayer.def == SayersPlayerColonyDef
             || Faction.OfPlayer.def == SayersPlayerColonySingleDef
             || Faction.OfPlayer.def == GuoguoPlayerColonyDef
                )
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

        ///<summary>移除半人马和Sayers的户外需求。</summary>
        [HarmonyPostfix]public static void NeedOutdoors_DisabledPostfix(Need_Outdoors __instance, ref bool __result)
        {
            if (
                __instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
                && (pawn.def == AlienCentaurDef || pawn.def == AlienSayersDef)
                )
            {
                __result = true;
            }
        }

        ///<summary>移除半人马的心情需求显示精神崩溃阈值描述。</summary>
        [HarmonyPostfix]public static void NeedMood_GetTipStringPostfix(Need_Mood __instance, ref string __result)
        {
            if (
                __instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
                && pawn.def == AlienCentaurDef
                )
            {
                __result = __instance.LabelCap + ": " + __instance.CurLevelPercentage.ToStringPercent() + "\n" + __instance.def.description;
            }
        }

        ///<summary>更改需求显示的分隔符。</summary>
        [HarmonyPostfix]public static void Need_DrawOnGUIPostfix(Need __instance)
        {
            if (
                __instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn &&
                __instance.GetType().GetField("threshPercents", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is List<float> threshPercents
                )
            {
                //移除半人马的心情需求显示精神崩溃阈值分隔符
                if (__instance is Need_Mood && pawn.def == AlienCentaurDef)
                {
                    threshPercents.Clear();
                }
                //移除半人马和Sayers的舒适需求显示第一个分隔符
                if (__instance is Need_Comfort && (pawn.def == AlienSayersDef/* || pawn.def == AlienCentaurDef*/))
                {
                    threshPercents.RemoveAll(num => num < 0.5f);
                }
                //移除Sayers的美观度需求显示第二个分隔符
                if (__instance is Need_Beauty && pawn.def == AlienSayersDef)
                {
                    threshPercents.RemoveAll(num => num > 0.3f && num < 0.4f);
                }
            }
        }

        ///<summary>记录在刻函数开始前的需求值。</summary>
        [HarmonyPrefix]public static void Need_NeedIntervalPrefix(Need_Seeker __instance, ref float __state)
        {
            __state = __instance.CurLevel;
        }

        ///<summary>对需求刻函数进行后期处理。</summary>
        [HarmonyPostfix]public static void Need_NeedIntervalPostfix(Need_Seeker __instance, float __state)
        {
            if (
                __instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
                )
            {
                __state -= __instance.CurLevel;

                if (__instance is Need_Mood && pawn.def == AlienCentaurDef)
                {
                    if (!(bool)AccessTools.Method(typeof(Need), "get_IsFrozen").Invoke(__instance, new object[] { }))
                    {
                        /*
                        float curInstantLevel = __instance.CurInstantLevel;
                        if (curInstantLevel > __instance.CurLevel)
                        {
                            //__instance.CurLevel += __instance.def.seekerRisePerHour * 0.06f;
                            __instance.CurLevel = ((__instance.CurInstantLevel * 20) + __instance.CurLevel)/21;
                            __instance.CurLevel = Mathf.Min(__instance.CurLevel, curInstantLevel);
                        }
                        if (curInstantLevel < __instance.CurLevel)
                        {
                            //__instance.CurLevel -= __instance.def.seekerFallPerHour * 0.06f;
                            __instance.CurLevel = ((__instance.CurInstantLevel * 20) + __instance.CurLevel) / 21;
                            __instance.CurLevel = Mathf.Max(__instance.CurLevel, curInstantLevel);
                        }
                        */
                        if (__instance.CurLevel == __instance.MaxLevel)
                        {
                            _ = (__instance.def.seekerRisePerHour * 0.06f) - __state;
                        }
                    }
                }
            }
        }

        ///<summary>移除半人马舒适度需求的负面情绪。</summary>
        [HarmonyPostfix]public static void ThoughtWorker_NeedComfort_CurrentStateInternalPostfix(ThoughtWorker_NeedComfort __instance, ref ThoughtState __result, Pawn p)
        {
            if (false &&
                p.def == AlienCentaurDef && __result.Active && __result.StageIndex == 0
                )
            {
                __result = ThoughtState.Inactive;
            }
        }

        ///<summary>移除半人马在SoS2太空中的负面情绪。</summary>
        [HarmonyPostfix]public static void ThoughtWorker_SpaceThoughts_CurrentStateInternalPostfix(ThoughtWorker __instance, ref ThoughtState __result, Pawn p)
        {
            if (
                p.def == AlienCentaurDef && __result.Active && 
                ( __result.StageIndex == 1 || __result.StageIndex == 3 )
                )
            {
                __result = ThoughtState.ActiveAtStage(__result.StageIndex - 1);
            }
        }

        ///<summary>移除半人马精神轻度崩溃阈值。</summary>
        [HarmonyPostfix]public static void MentalBreaker_BreakThresholdMinorPostfix(MentalBreaker __instance, ref float __result)
        {
            if (
                __instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
                && pawn.def == AlienCentaurDef
                )
            {
                __result = Math.Min(__result, -0.15f);
            }
        }
        ///<summary>移除半人马精神中度崩溃阈值。</summary>
        [HarmonyPostfix]public static void MentalBreaker_BreakThresholdMajorPostfix(MentalBreaker __instance, ref float __result)
        {
            if (
                __instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
                && pawn.def == AlienCentaurDef
                )
            {
                __result = Math.Min(__result, -0.15f);
            }
        }
        ///<summary>移除半人马精神重度崩溃阈值。</summary>
        [HarmonyPostfix]public static void MentalBreaker_BreakThresholdExtremePostfix(MentalBreaker __instance, ref float __result)
        {
            if (
                __instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
                && pawn.def == AlienCentaurDef
                )
            {
                __result = Math.Min(__result, -0.15f);
            }
        }

        ///<summary>使心灵敏感度属性受到心灵失聪hediff影响。</summary>
        [HarmonyPostfix]public static void PsychicSensitivityPostfix(StatPart __instance, StatRequest req, ref float val)
        {
            try
            {
                if (__instance.parentStat == StatDefOf.PsychicSensitivity &&
                    req.HasThing && (((Pawn)req.Thing)?.health?.hediffSet?.HasHediff(PsychicDeafHediffDef)) == true)
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

        ///<summary>使半人马和Sayers可以使用额外类型的冥想媒介。</summary>
        [HarmonyPostfix]public static void MeditationFocusCanPawnUsePostfix(MeditationFocusDef __instance, ref bool __result, Pawn p)
        {
            if ((p.def == AlienCentaurDef || p.def == AlienSayersDef || p.def == AlienGuoguoDef
                ) && (__instance == DefDatabase<MeditationFocusDef>.GetNamed("Natural")     //自然
                    || __instance == DefDatabase<MeditationFocusDef>.GetNamed("Artistic")   //艺术
                    || __instance == DefDatabase<MeditationFocusDef>.GetNamed("Dignified")  //庄严
                    || __instance == DefDatabase<MeditationFocusDef>.GetNamed("Morbid")     //病态
                    //|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Minimal")  //简约
                    //|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Flame")    //火焰
                    )
                )
            {
                __result = true;
            }
        }

        ///<summary>为半人马和Sayers补充启用冥想类型的原因。</summary>
        [HarmonyPostfix]public static void MeditationFocusExplanationPostfix(MeditationFocusDef __instance, ref string __result, Pawn pawn)
        {
            if ((pawn.def == AlienCentaurDef || pawn.def == AlienSayersDef || pawn.def == AlienGuoguoDef
                ) && (__instance == DefDatabase<MeditationFocusDef>.GetNamed("Natural")     //自然
                    || __instance == DefDatabase<MeditationFocusDef>.GetNamed("Artistic")   //艺术
                    || __instance == DefDatabase<MeditationFocusDef>.GetNamed("Dignified")  //庄严
                    || __instance == DefDatabase<MeditationFocusDef>.GetNamed("Morbid")     //病态
                    //|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Minimal")  //简约
                    //|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Flame")    //火焰
                    )
                )
            {
                //__result = $"  - {"MeditationFocusEnabledByExploriteRace".Translate(pawn.def.label)}"
                __result = $"  - {"Race".Translate()}: {pawn.def.label}"
                    //+ (__result.Length > 0 ? "\n" + __result : null)
                    ;
            }
        }

        ///<summary>对绑定选单进行补丁。</summary>
        [HarmonyPostfix]public static void AssignToPawnCandidatesPostfix(CompAssignableToPawn __instance, ref IEnumerable<Pawn> __result)
        {
            //从单人床选单中移除半人马。
            if (__instance is CompAssignableToPawn_Bed &&
                __instance?.Props?.maxAssignedPawnsCount < 2)
            {
                __result = __result?.Where(pawn => pawn?.def != AlienCentaurDef);
            }
            //向王座选单中加入半人马。
            /* if (__instance is CompAssignableToPawn_Throne
                && __instance.parent.Spawned)
            {
                List<Pawn> result = __result.ToList();
                foreach (Pawn pawn in __instance.parent.Map.mapPawns.FreeColonists.Where(pawn => pawn?.def == AlienCentaurDef))
                {
                    if (!result.Contains(pawn))
                        result.Add(pawn);
                }
                __result = result;
            } */
        }

        ///<summary>使半人马占满床位。</summary>
        [HarmonyPostfix]public static void AssignBedToPawnHasFreeSlotPostfix(CompAssignableToPawn __instance, ref bool __result)
        {
            if (__result == true &&
                __instance is CompAssignableToPawn_Bed)
            {
                //__result = __instance.AssignedPawns.Count() + __instance.AssignedPawns.Where(pawn => pawn.def == AlienCentaurDef).Count() < __instance.Props.maxAssignedPawnsCount;
                __result = __instance?.AssignedPawns?.Where(pawn => pawn.def == AlienCentaurDef)?.Count() >= 1;
            }
        }

        ///<summary>使半人马不能与他人同时被添加至同一个床。</summary>
        [HarmonyPostfix]public static void AssignBedToPawnTryAssignPawnPostfix(CompAssignableToPawn_Bed __instance, Pawn pawn)
        {
            List<Pawn> pawnsToRemove = new List<Pawn>();
            if (pawn?.def == AlienCentaurDef)
            {
                foreach (Pawn one_pawn in __instance.AssignedPawns)
                {
                    if (one_pawn != pawn)
                        pawnsToRemove.Add(one_pawn);
                }
            }
            else
            {
                foreach (Pawn one_pawn in __instance.AssignedPawns)
                {
                    if (one_pawn?.def == AlienCentaurDef)
                        pawnsToRemove.Add(one_pawn);
                }

            }

            foreach (Pawn one_pawn in pawnsToRemove)
            {
                __instance?.TryUnassignPawn(one_pawn);
            }

        }

        ///<summary>使半人马不能使用小尺寸床铺，使果果床铺不能被其他种族使用。</summary>
        [HarmonyPostfix]public static void RestUtilityCanUseBedEverPostfix(ref bool __result, Pawn p, ThingDef bedDef)
        {
            if (p.def == AlienCentaurDef && bedDef.Size.Area < 3)
            {
                __result = false;
            }
            if (p.def != AlienGuoguoDef && bedDef.HasComp(typeof(CompAssignableToPawn_Bed_Guoguo)))
            {
                __result = false;
            }
        }

        ///<summary>使指定植物的生长无视环境温度。</summary>
        [HarmonyPostfix]public static void PlantGrowthRateFactorNoTemperaturePostfix(Plant __instance, ref float __result)
        {
            if (__instance is Plant_FleshTree)
            {
                __result = 1f;
            }
        }
        /*
        ///<summary>使血肉树不会进入休眠状态。</summary>
        [HarmonyPostfix]public static void PlantNoRestingPostfix(Plant __instance, ref bool __result)
        {
            if (__instance is Plant_FleshTree)
            {
                __result = false;
            }
        }
        
        ///<summary>使血肉树的生长至少具有100%速率。</summary>
        [HarmonyPostfix]public static void PlantGrowthRateFactorEnsurePostfix(Plant __instance, ref float __result)
        {
            if (__instance is Plant_FleshTree
                && __result < 1f)
            {
                __result = 1f;
            }
        }

        ///<summary>使血肉树的叶子在成熟前不会出现。</summary>
        [HarmonyPostfix]public static void PlantLeaflessNowPostfix(Plant __instance, ref bool __result)
        {
            if (__instance is Plant_FleshTree)
            {
                __result = !__instance.CanYieldNow();
            }
        }
        */

        ///<summary>使三联电池同样被视为<see cref = "Alert_NeedBatteries" />可接受的电池类型。</summary>
        [HarmonyPostfix]public static void AlertNeedBatteriesPostfix(Alert_NeedBatteries __instance, ref bool __result, Map map)
        {
            if (__result == true &&
                map.listerBuildings.ColonistsHaveBuilding(thing => thing is Building_TriBattery))
            {
                __result = false;
            }
        }

        ///<summary>使Sayers优先选择尸体作为食物。</summary>
        [HarmonyPostfix]public static void GetFoodTryGiveJobPostfix(JobGiver_GetFood __instance, ref Job __result, Pawn pawn)
        {
            if (pawn?.def != AlienSayersDef)
            {
                return;
            }
            if (pawn.Downed)
            {
                return;
            }
            if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Malnutrition)?.Severity > 0.4f)
            {
                return;
            }
            Need_Food food = pawn.needs.food;
            if (
                __instance.GetType().GetField("minCategory", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is HungerCategory minCategory &&
                __instance.GetType().GetField("maxLevelPercentage", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is float maxLevelPercentage &&
                (food == null || (int)food.CurCategory < (int)minCategory || food.CurLevelPercentage > maxLevelPercentage))
            {
                return;
            }
            Thing thing = GenClosest.ClosestThingReachable(
                root: pawn.Position,
                map: pawn.Map,
                thingReq: ThingRequest.ForGroup(ThingRequestGroup.Corpse),
                peMode: PathEndMode.Touch,
                traverseParams: TraverseParms.For(pawn),
                maxDistance: 9999f,
                validator: delegate (Thing t)
                {
                    if (!(t is Corpse) || (t.def.ingestible.foodType & FoodTypeFlags.Meat) != 0 || t.def == BloodyTreeMeatDef)
                    {
                        return false;
                    }
                    if (t.IsForbidden(pawn))
                    {
                        return false;
                    }
                    if (!pawn.foodRestriction.CurrentFoodRestriction.Allows(t))
                    {
                        return false;
                    }
                    if (!t.IngestibleNow)
                    {
                        return false;
                    }
                    if (!pawn.RaceProps.CanEverEat(t))
                    {
                        return false;
                    }
                    return pawn.CanReserve(t);
                });
            if (thing == null)
            {
                return;
            }

            Job job = JobMaker.MakeJob(JobDefOf.Ingest, thing);
            if (job != null)
                __result = job;
        }

        ///<summary>阻止半人马疼痛休克。</summary>
        [HarmonyPostfix]public static void PawnHealthTrackerInPainShockPostfix(Pawn_HealthTracker __instance, ref bool __result)
        {
            if (
                __instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
                &&( pawn.def == AlienCentaurDef || pawn.def == AlienGuoguoDef ))
            {
                __result = false;
            }
        }

        ///<summary>阻止半人马得到任何永久性疤痕。</summary>
        [HarmonyPrefix]public static void HediffComp_GetsPermanentIsPermanentPrefix(HediffComp_GetsPermanent __instance, ref bool value)
        {
            if (
                __instance.Pawn.def == AlienCentaurDef)
            {
                value = false;
            }
        }
        ///<summary>设置不允许<see cref = "HediffComp_TendDuration_CantTend" />症状被治疗。</summary>
        [HarmonyPostfix]public static void HediffComp_TendDurationAllowTendPrefix(HediffComp_TendDuration __instance, ref bool __result)
        {
            if (false&&
                __instance is HediffComp_TendDuration_CantTend
                && __instance.Pawn.def == AlienCentaurDef)
            {
                __result = false;
            }
        }

        ///<summary>增强半人马负重能力。</summary>
        [HarmonyPostfix]public static void MassUtilityCapacityPostfix(ref float __result, Pawn p, ref StringBuilder explanation)
        {
            if (p.def == AlienCentaurDef || p.def == AlienSayersDef)
            {
                string strPreProcess = "  - " + p.LabelShortCap + ": " + __result.ToStringMassOffset();
                __result = Math.Max(__result, p.def == AlienCentaurDef?1000f:35f);
                if (explanation != null)
                {
                    explanation.Replace(strPreProcess, "  - " + p.LabelShortCap + ": " + __result.ToStringMassOffset());
                }
            }
        }

        ///<summary>使半人马的身体被渲染为其头发颜色。</summary>
        [HarmonyPostfix]public static void PawnGraphicSetResolveAllGraphicsPostfix(PawnGraphicSet __instance)
        {
            if (
                __instance.pawn.def == AlienCentaurDef)
            {
                //__instance.nakedGraphic.color = __instance.pawn.story.hairColor;
                __instance.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(
                    __instance.nakedGraphic.path,
                    __instance.nakedGraphic.Shader,
                    __instance.nakedGraphic.drawSize, 
                    __instance.pawn.story.hairColor);
            }
            if (
                __instance.pawn.def == AlienGuoguoDef)
            {
                Color.RGBToHSV(__instance.pawn.story.SkinColor, out _, out _, out float v);
                //__instance.nakedGraphic.color = __instance.pawn.story.hairColor;
                __instance.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(
                    __instance.nakedGraphic.path,
                    __instance.nakedGraphic.Shader,
                    __instance.nakedGraphic.drawSize, 
                    Color.HSVToRGB(0f, 0f, v));
            }
        }

        ///<summary>对设施连接性的后期处理。</summary>
        [HarmonyPostfix]public static void CompAffectedByFacilitiesCanPotentiallyLinkToStaticPostfix
            (ref bool __result, ThingDef facilityDef, IntVec3 facilityPos, Rot4 facilityRot, ThingDef myDef, IntVec3 myPos, Rot4 myRot)
        {
            if (__result == true && 
                facilityDef?.placeWorkers?.Contains(typeof(PlaceWorker_FacingPort)) == true && 
                !GenAdj.OccupiedRect(myPos, myRot, myDef.size).Cells.Contains(PlaceWorker_FacingPort.PortPosition(facilityDef, facilityPos, facilityRot)))
            {
                __result = false;
            }
        }

        ///<summary>对人物渲染器的服装选单的补丁。</summary>
        [HarmonyPostfix]
        public static void PawnGraphicSetResolveApparelGraphicsPostfix(PawnGraphicSet __instance)
        {
            if (__instance.pawn.apparel.WornApparel.Any(ap => ap.def == CentaurHeaddressDef))
            {
                __instance.apparelGraphics.RemoveAll(ag => ag.sourceApparel.def.apparel.layers.Contains(ApparelLayerDefOf.Overhead));
            }
            if (__instance.pawn.def == AlienCentaurDef)
            {
                __instance.apparelGraphics.RemoveAll(ag => ag.sourceApparel.def.apparel.bodyPartGroups.Contains(DefDatabase<BodyPartGroupDef>.GetNamed("Waist")));
            }
        }

        ///<summary>使半人马始终被视为具有特征等级。</summary>
        [HarmonyPostfix]
        public static void TraitSetDegreeOfTraitPostfix(TraitSet __instance, ref int __result, TraitDef tDef)
        {
            if (__instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
                && pawn.def == AlienCentaurDef)
            {
                if (tDef == TraitDefOf.Industriousness)
                    __result = Math.Max(2, __result);
                else if (tDef == TraitDefOf.DrugDesire)
                    __result = Math.Min(-1, __result);
            }
        }
        ///<summary>使半人马始终被视为具有特征。</summary>
        [HarmonyPostfix]
        public static void TraitSetHasTraitPostfix(TraitSet __instance, ref bool __result, TraitDef tDef)
        {
            if (__instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn)
            {
                if ( pawn.def == AlienCentaurDef && (
                    tDef == DefDatabase<TraitDef>.GetNamed("Masochist") ||
                    tDef == TraitDefOf.Industriousness ||
                    tDef == TraitDefOf.DrugDesire ||
                    tDef == TraitDefOf.Transhumanist ||
                    tDef == TraitDefOf.Kind ||
                    tDef == TraitDefOf.Asexual ))
                {
                    __result = true;
                }
                if ( pawn.def == AlienGuoguoDef && (
                    tDef == TraitDefOf.Kind ||
                    tDef == TraitDefOf.Asexual ))
                {
                    __result = true;
                }
            }
        }
        ///<summary>使半人马特征制作样本。</summary>
        [HarmonyPostfix]
        public static void TraitSetGetTraitPostfix(TraitSet __instance, ref Trait __result, TraitDef tDef)
        {
            if (__instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
                && pawn.def == AlienCentaurDef
                && __result == null)
            {
                if (tDef == TraitDefOf.DrugDesire)
                {
                    __result = new Trait(tDef, -1);
                }
            }
        }

    }
}
