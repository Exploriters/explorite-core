/********************
 * 包含多个补丁的合集文件。
 * --siiftun1857
 */
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using static Explorite.ExploriteCore;
using static Verse.DamageInfo;

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
        private static string App(this string str, ref string target)
        {
            return target = str;
        }
        static ExploritePatches()
        {
            string last_patch = "";
            string last_patch_method = "";
            try
            {
                harmonyInstance.Patch(AccessTools.Method(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn).App(ref last_patch_method), new[] { typeof(PawnGenerationRequest) }),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(GeneratePawnPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(SkillRecord), nameof(SkillRecord.Learn).App(ref last_patch_method)),
                    prefix: new HarmonyMethod(patchType, last_patch = nameof(SkillLearnPrefix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(SkillRecord), nameof(SkillRecord.Interval).App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(SkillIntervalPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(Pawn_PsychicEntropyTracker), "get_PainMultiplier".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(NoPainBounsForCentaursPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(IncidentWorker_WandererJoin), "CanFireNowSub".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(WandererJoinCannotFirePostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(Need_Outdoors), "get_Disabled".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(NeedOutdoors_DisabledPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(Need_Mood), nameof(Need_Mood.GetTipString).App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(NeedMood_GetTipStringPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(Need), nameof(Need.DrawOnGUI).App(ref last_patch_method)),
                    prefix: new HarmonyMethod(patchType, last_patch = nameof(Need_DrawOnGUIPrefix)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(Need_DrawOnGUIPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(NeedsCardUtility), "DrawThoughtListing".App(ref last_patch_method)),
                    prefix: new HarmonyMethod(patchType, last_patch = nameof(NeedsCardUtilityDrawThoughtListingPrefix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(Need_Seeker), nameof(Need_Seeker.NeedInterval).App(ref last_patch_method)),
                    prefix: new HarmonyMethod(patchType, last_patch = nameof(Need_NeedIntervalPrefix)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(Need_NeedIntervalPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(Need_Food), "get_MaxLevel".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(NeedMaxLevelPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(ThoughtWorker_NeedComfort), "CurrentStateInternal".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(ThoughtWorker_NeedComfort_CurrentStateInternalPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(MeditationFocusDef), nameof(MeditationFocusDef.CanPawnUse).App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(MeditationFocusCanPawnUsePostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(MeditationFocusDef), nameof(MeditationFocusDef.EnablingThingsExplanation).App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(MeditationFocusExplanationPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(RecipeDef), "get_AvailableNow".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(RecipeDefAvailableNowPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(MentalBreaker), "get_BreakThresholdMinor".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(MentalBreaker_BreakThresholdMinorPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(MentalBreaker), "get_BreakThresholdMajor".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(MentalBreaker_BreakThresholdMajorPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(MentalBreaker), "get_BreakThresholdExtreme".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(MentalBreaker_BreakThresholdExtremePostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(CompAssignableToPawn), "get_AssigningCandidates".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(AssignToPawnCandidatesPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(CompAssignableToPawn), "get_HasFreeSlot".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(AssignBedToPawnHasFreeSlotPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(CompAssignableToPawn_Bed), nameof(CompAssignableToPawn_Bed.TryAssignPawn).App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(AssignBedToPawnTryAssignPawnPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(RestUtility), nameof(RestUtility.CanUseBedEver).App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(RestUtilityCanUseBedEverPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(Plant), "get_GrowthRateFactor_Temperature".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(PlantGrowthRateFactorNoTemperaturePostfix)));
                //harmonyInstance.Patch(AccessTools.Method(typeof(Plant), "get_Resting".App(ref last_patch_method)),
                //    postfix: new HarmonyMethod(patchType, last_patch = nameof(PlantNoRestingPostfix)));
                //harmonyInstance.Patch(AccessTools.Method(typeof(Plant), "get_GrowthRate".App(ref last_patch_method)),
                //    postfix: new HarmonyMethod(patchType, last_patch = nameof(PlantGrowthRateFactorEnsurePostfix)));
                //harmonyInstance.Patch(AccessTools.Method(typeof(Plant), "get_LeaflessNow".App(ref last_patch_method)),
                //    postfix: new HarmonyMethod(patchType, last_patch = nameof(PlantLeaflessNowPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(MinifiedThing), nameof(MinifiedThing.Destroy).App(ref last_patch_method)),
                    prefix: new HarmonyMethod(patchType, last_patch = nameof(MinifiedThingDestroyPrefix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(Alert_NeedBatteries), "NeedBatteries".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(AlertNeedBatteriesPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(JobGiver_GetFood), "TryGiveJob".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(GetFoodTryGiveJobPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), "get_InPainShock".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(PawnHealthTrackerInPainShockPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(MassUtility), nameof(MassUtility.Capacity).App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(MassUtilityCapacityPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(HediffComp_GetsPermanent), "set_IsPermanent".App(ref last_patch_method)),
                    prefix: new HarmonyMethod(patchType, last_patch = nameof(HediffComp_GetsPermanentIsPermanentPrefix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(HediffComp_TendDuration), "get_AllowTend".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(HediffComp_TendDurationAllowTendPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(HediffComp_ReactOnDamage), nameof(HediffComp_ReactOnDamage.Notify_PawnPostApplyDamage).App(ref last_patch_method)),
                    prefix: new HarmonyMethod(patchType, last_patch = nameof(HediffComp_ReactOnDamageNotify_PawnPostApplyDamagePrefix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(StatPart_ApparelStatOffset), nameof(StatPart.TransformValue).App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(PsychicSensitivityPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(PawnGraphicSet), nameof(PawnGraphicSet.ResolveAllGraphics).App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(PawnGraphicSetResolveAllGraphicsPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(CompAffectedByFacilities), nameof(CompAffectedByFacilities.CanPotentiallyLinkTo_Static).App(ref last_patch_method), new Type[] { typeof(ThingDef), typeof(IntVec3), typeof(Rot4), typeof(ThingDef), typeof(IntVec3), typeof(Rot4) }),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(CompAffectedByFacilitiesCanPotentiallyLinkToStaticPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(PawnGraphicSet), nameof(PawnGraphicSet.ResolveApparelGraphics).App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(PawnGraphicSetResolveApparelGraphicsPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(TraitSet), nameof(TraitSet.DegreeOfTrait).App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(TraitSetDegreeOfTraitPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(TraitSet), nameof(TraitSet.HasTrait).App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(TraitSetHasTraitPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(TraitSet), nameof(TraitSet.GetTrait).App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(TraitSetGetTraitPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(RaceProperties), nameof(RaceProperties.SpecialDisplayStats).App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(RacePropertiesSpecialDisplayStatsPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(StatWorker), nameof(StatWorker.ShouldShowFor).App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(StatWorkerShouldShowForPostfix)));

                harmonyInstance.Patch(AccessTools.Method(AccessTools.TypeByName("RimWorld.Recipe_RemoveBodyPart"), "GetPartsToApplyOn".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(RecipeRemoveBodyPartGetPartsToApplyOnPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(OutfitDatabase), "GenerateStartingOutfits".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(OutfitDatabaseGenerateStartingOutfitsPostfix)));

                //harmonyInstance.Patch(AccessTools.Method(typeof(GenHostility), nameof(GenHostility.HostileTo), new Type[] { typeof(Thing), typeof(Thing) }),
                //    postfix: new HarmonyMethod(patchType, last_patch = nameof(GenHostilityHostileToPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(Pawn), nameof(Pawn.ThreatDisabled).App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(PawnThreatDisabledPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(ApparelProperties), nameof(ApparelProperties.GetCoveredOuterPartsString).App(ref last_patch_method)),
                    prefix: new HarmonyMethod(patchType, last_patch = nameof(ApparelPropertiesGetCoveredOuterPartsStringPostfix)));
                
                harmonyInstance.Patch(AccessTools.Method(typeof(ThingMaker), nameof(ThingMaker.MakeThing).App(ref last_patch_method)),
                    prefix: new HarmonyMethod(patchType, last_patch = nameof(ThingMakerMakeThingPrefix)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(ThingMakerMakeThingPostfix)));

                //harmonyInstance.Patch(AccessTools.Method(typeof(GenRecipe), nameof(GenRecipe.MakeRecipeProducts).App(ref last_patch_method)),
                //    prefix: new HarmonyMethod(patchType, last_patch = nameof(GenRecipeMakeRecipeProductsPrefix)),
                //    postfix: new HarmonyMethod(patchType, last_patch = nameof(GenRecipeMakeRecipeProductsPostfix)));
                harmonyInstance.Patch(AccessTools.Method(typeof(GenRecipe), "PostProcessProduct".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(GenRecipePostProcessProductPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(RimWorld.Planet.FactionGiftUtility), "GetBaseGoodwillChange".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(FactionGiftUtilityGetBaseGoodwillChangePostfix)));
                
                harmonyInstance.Patch(AccessTools.Method(typeof(InspirationHandler), nameof(InspirationHandler.GetRandomAvailableInspirationDef).App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(InspirationHandlerGetRandomAvailableInspirationDefPostfix)));
                
                nameof(DamageInfo).App(ref last_patch_method);
                harmonyInstance.Patch(AccessTools.Constructor(typeof(DamageInfo),
                    parameters: new Type[] { typeof(DamageDef), typeof(float), typeof(float), typeof(float), typeof(Thing), typeof(BodyPartRecord), typeof(ThingDef), typeof(SourceCategory), typeof(Thing) }),
                    prefix: new HarmonyMethod(patchType, last_patch = nameof(DamageInfoCtorPrefix)));

                nameof(BattleLogEntry_ExplosionImpact).App(ref last_patch_method);
                harmonyInstance.Patch(AccessTools.Constructor(typeof(BattleLogEntry_ExplosionImpact),
                    parameters: new Type[] { typeof(Thing), typeof(Thing), typeof(ThingDef), typeof(ThingDef), typeof(DamageDef)}),
                    prefix: new HarmonyMethod(patchType, last_patch = nameof(BattleLogEntry_ExplosionImpactCtorPrefix)));
                
                //harmonyInstance.Patch(AccessTools.Method(typeof(Projectile), nameof(Projectile.Launch).App(ref last_patch_method),
                //    parameters: new Type[] { typeof(Thing), typeof(Vector3), typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(ProjectileHitFlags), typeof(Thing), typeof(ThingDef)}),
                //    postfix: new HarmonyMethod(patchType, last_patch = nameof(ProjectileLaunchPostfix)));
                
                harmonyInstance.Patch(AccessTools.Method(typeof(HealthCardUtility), "GetListPriority".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(HealthCardUtilityGetListPriorityPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(TendUtility), nameof(TendUtility.CalculateBaseTendQuality).App(ref last_patch_method), new Type[] { typeof(Pawn), typeof(Pawn), typeof(float), typeof(float) }),
                    prefix: new HarmonyMethod(patchType, last_patch = nameof(TendUtilityCalculateBaseTendQualityPrefix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(QuestPart_DropPods), "set_Things".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(QuestPartDropPodsSetThingsPostfix)));

                harmonyInstance.Patch(AccessTools.Method(typeof(ResearchProjectDef), "get_CanStartNow".App(ref last_patch_method)),
                    postfix: new HarmonyMethod(patchType, last_patch = nameof(ResearchProjectDefCanStartNowPostfix)));

                if (InstelledMods.SoS2)
                {
                    // 依赖 类 SaveOurShip2.ShipInteriorMod2
                    harmonyInstance.Patch(AccessTools.Method(AccessTools.TypeByName("SaveOurShip2.ShipInteriorMod2"), "HasSpaceSuitSlow".App(ref last_patch_method), new[] { typeof(Pawn) }),
                        postfix: new HarmonyMethod(patchType, last_patch = nameof(HasSpaceSuitSlowPostfix)));

                    // 依赖 类 RimWorld.ThoughtWorker_SpaceThoughts
                    harmonyInstance.Patch(AccessTools.Method(AccessTools.TypeByName("RimWorld.ThoughtWorker_SpaceThoughts"), "CurrentStateInternal".App(ref last_patch_method)),
                        postfix: new HarmonyMethod(patchType, last_patch = nameof(ThoughtWorker_SpaceThoughts_CurrentStateInternalPostfix)));
                }
            }
            catch (Exception e)
            {
                Log.Error(string.Concat(
                    "[Explorite]Patch sequence failare at ",
                    $"{last_patch}, {last_patch_method}, ",
                    $"an exception ({e.GetType().Name}) occurred.\n",
                    $"Message:\n   {e.Message}\n",
                    $"Stack Trace:\n{e.StackTrace}\n"
                    ));
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
                __instance.xpSinceMidnight = 0f;
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
                    if (app.def.apparel.tags.Contains("CentaurEVA"))
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

        ///<summary>更改渲染想法预补丁。</summary>
        [HarmonyPrefix]public static void NeedsCardUtilityDrawThoughtListingPrefix(ref Rect listingRect, Pawn pawn, ref Vector2 thoughtScrollPosition)
        {
            //改变半人马的想法显示渲染高度
            if (pawn.def == AlienCentaurDef)
            {
                if (pawn.needs.TryGetNeed<Need_CentaurCreativityInspiration>()?.ShouldShow == true)
                {
                    listingRect.y += 24f;
                    thoughtScrollPosition.y += 24f;
                }
            }
        }
        ///<summary>更改渲染模式预补丁。</summary>
        [HarmonyPrefix]public static void Need_DrawOnGUIPrefix(Need __instance, ref Rect rect, ref int maxThresholdMarkers, ref float customMargin, ref bool drawArrows, ref bool doTooltip)
        {
            if (
                __instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
                // && __instance.GetType().GetField("threshPercents", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is List<float> threshPercents
                )
            {
                //改变半人马的心情需求显示渲染高度
                if (__instance is Need_Mood && pawn.def == AlienCentaurDef && pawn.needs.TryGetNeed<Need_CentaurCreativityInspiration>()?.ShouldShow == true)
                {
                    //rect.height = Mathf.Max(rect.height * 0.666f, 30f);
                }
            }
        }
        ///<summary>更改渲染模式后期处理补丁。</summary>
        [HarmonyPostfix]public static void Need_DrawOnGUIPostfix(Need __instance, Rect rect, int maxThresholdMarkers, float customMargin, bool drawArrows, bool doTooltip)
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

                    Need_CentaurCreativityInspiration need = pawn.needs.TryGetNeed<Need_CentaurCreativityInspiration>();
                    if (need?.ShouldShow == true)
                    {
                        Rect rect2 = new Rect(rect);
                        //rect2.height *= 0.666f;
                        rect2.height *= 0.5f;
                        rect2.y += rect.height;
                        need.DrawOnGUI(rect2, maxThresholdMarkers, customMargin, drawArrows, doTooltip);
                    }
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
        ///<summary>增强半人马食物需求储备量。</summary>
        [HarmonyPostfix] public static void NeedMaxLevelPostfix(Need_Food __instance, ref float __result)
        {
            if (
                __instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
                )
            {
                if ((pawn?.health?.hediffSet?.GetFirstHediffOfDef(HediffCentaurSubsystem_NeedsCapacitor_Def)).SubsystemEnabled())
                {
                    __result *= 10f / 3f;
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

        ///<summary>更改特定配方的显示可用性。</summary>
        [HarmonyPostfix]public static void RecipeDefAvailableNowPostfix(RecipeDef __instance, ref bool __result)
        {
            /*if (__instance.defName == "InstallHyperManipulatorSurgery")
            {
                __result = false;
            }*/
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
            if (__instance is CompAssignableToPawn_Bed comp
             && !(comp.props is CompProperties_AssignableToPawn_NoPostLoadSpecial)
             && __instance?.Props?.maxAssignedPawnsCount < 2)
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
        ///<summary>使被收纳的秘密三射弓物体掉落故障三射弓。</summary>
        [HarmonyPrefix]public static void MinifiedThingDestroyPrefix(MinifiedThing __instance)
        {
            if (__instance.InnerThing is ISecretTrishot trishotOwner)
            {
                trishotOwner.LeaveTrishot(__instance.Position, __instance.Map);
            }
        }

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
                &&( pawn.def == AlienCentaurDef || pawn.def == AlienGuoguoDef || pawn.def == AlienMichellesDef))
            {
                __result = false;
            }
        }

        ///<summary>阻止半人马和Michelles得到任何永久性疤痕。</summary>
        [HarmonyPrefix]public static void HediffComp_GetsPermanentIsPermanentPrefix(HediffComp_GetsPermanent __instance, ref bool value)
        {
            if (__instance.Pawn.def == AlienCentaurDef
             || __instance.Pawn.def == AlienMichellesDef)
            {
                value = false;
            }
        }
        ///<summary>设置不允许<see cref = "HediffComp_TendDuration_CantTend" />症状被治疗。</summary>
        [HarmonyPostfix]public static void HediffComp_TendDurationAllowTendPostfix(HediffComp_TendDuration __instance, ref bool __result)
        {
            if (false&&
                __instance is HediffComp_TendDuration_CantTend
                && __instance.Pawn.def == AlienCentaurDef)
            {
                __result = false;
            }
        }
        ///<summary>阻止半人马大脑休克。</summary>
        [HarmonyPrefix]public static void HediffComp_ReactOnDamageNotify_PawnPostApplyDamagePrefix(HediffComp_ReactOnDamage __instance, ref DamageInfo dinfo, float totalDamageDealt)
        {
            if (__instance.Pawn.def == AlienCentaurDef
             && __instance.Props.damageDefIncoming == DamageDefOf.EMP && dinfo.Def == DamageDefOf.EMP)
            {
                dinfo.Def = null;
            }
        }

        ///<summary>增强半人马负重能力。</summary>
        [HarmonyPostfix]public static void MassUtilityCapacityPostfix(ref float __result, Pawn p, ref StringBuilder explanation)
        {
            if (p.def == AlienCentaurDef || p.def == AlienSayersDef)
            {
                string strPreProcess = "  - " + p.LabelShortCap + ": " + __result.ToStringMassOffset();
                __result = Math.Max(__result, p.def == AlienCentaurDef ? 1000f : 35f);
                if ((p?.health?.hediffSet?.GetFirstHediffOfDef(HediffCentaurSubsystem_AntiMass_Def)).SubsystemEnabled())
                {
                    __result *= 3;
                }
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
        [HarmonyPostfix]public static void PawnGraphicSetResolveApparelGraphicsPostfix(PawnGraphicSet __instance)
        {
            if (__instance.pawn.apparel.WornApparel.Any(ap => ap.def == CentaurHeaddressDef))
            {
                __instance.apparelGraphics.RemoveAll(ag => ag.sourceApparel.def.apparel.layers.Contains(ApparelLayerDefOf.Overhead));
            }
            if (__instance.pawn.def == AlienCentaurDef)
            {
                __instance.apparelGraphics.RemoveAll(ag => 
                    ag.sourceApparel.def.apparel.bodyPartGroups.Contains(DefDatabase<BodyPartGroupDef>.GetNamed("Waist"))
                 //&& ag.sourceApparel.def.apparel.layers.Contains(ApparelLayerDefOf.Belt)
                 && !ag.sourceApparel.def.apparel.bodyPartGroups.Where(bpgd => bpgd != DefDatabase<BodyPartGroupDef>.GetNamed("Waist")).Any()
                    );
            }
        }

        ///<summary>使半人马始终被视为具有特征等级。</summary>
        [HarmonyPostfix]public static void TraitSetDegreeOfTraitPostfix(TraitSet __instance, ref int __result, TraitDef tDef)
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
        [HarmonyPostfix]public static void TraitSetHasTraitPostfix(TraitSet __instance, ref bool __result, TraitDef tDef)
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
        ///<summary>为半人马特征制作样本。</summary>
        [HarmonyPostfix]public static void TraitSetGetTraitPostfix(TraitSet __instance, ref Trait __result, TraitDef tDef)
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
        ///<summary>在信息页移除半人马的寿命显示。</summary>
        [HarmonyPostfix]public static void RacePropertiesSpecialDisplayStatsPostfix(RaceProperties __instance, ref IEnumerable<StatDrawEntry> __result, ThingDef parentDef, StatRequest req)
        {
            /*_ = new StatDrawEntry(
                StatCategoryDefOf.BasicsPawn, 
                "StatsReport_LifeExpectancy".Translate(),
                float.PositiveInfinity.ToStringByStyle(ToStringStyle.Integer), 
                "Stat_Race_LifeExpectancy_Desc".Translate(), 
                2000);*/
            if (parentDef == AlienCentaurDef)
            {
                List<StatDrawEntry> result = __result.ToList();
                result.RemoveAll(
                    stat => stat.LabelCap == "StatsReport_LifeExpectancy".Translate().CapitalizeFirst()
                    && stat.ValueString == "-2147483648" //.Contains('-') // == float.PositiveInfinity.ToStringByStyle(ToStringStyle.Integer)
                    //&& stat.DisplayPriorityWithinCategory == 2000
                    );
                __result = result;
            }
        }
        ///<summary>在信息页移除半人马的疼痛休克阈值和精神崩溃阈值显示。</summary>
        [HarmonyPostfix]public static void StatWorkerShouldShowForPostfix(StatWorker __instance, ref bool __result, StatRequest req)
        {
            if (__result &&
                req.HasThing && req.Thing is Pawn pawn && pawn.def == AlienCentaurDef &&
                __instance.GetType().GetField("stat", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is StatDef stat
                && (stat == StatDefOf.PainShockThreshold || stat == StatDefOf.MentalBreakThreshold))
            {
                __result = false;
            }
        }
        ///<summary>从截肢手术清单中移除半人马的锁骨和子系统。</summary>
        [HarmonyPostfix]public static void RecipeRemoveBodyPartGetPartsToApplyOnPostfix(Recipe_Surgery __instance, ref IEnumerable<BodyPartRecord> __result, Pawn pawn, RecipeDef recipe)
        {
            if (pawn.def == AlienCentaurDef)
            {
                List<BodyPartRecord> result = __result.ToList();
                result.RemoveAll(bpr => 
                    bpr.def == CentaurScapularDef
                 || bpr.def == CentaurSubsystemBodyPartDef
                    );
                __result = result;
            }
        }
        ///<summary>添加默认服装方案。</summary>
        [HarmonyPostfix]public static void OutfitDatabaseGenerateStartingOutfitsPostfix(OutfitDatabase __instance)
        {
            if (InstelledMods.RimCentaurs)
            {
                Outfit outfit3 = __instance.MakeNewOutfit();
                outfit3.label = "OutfitCentaur".Translate();
                outfit3.filter.SetDisallowAll();
                outfit3.filter.SetAllow(SpecialThingFilterDefOf.AllowDeadmansApparel, allow: false);
                foreach (ThingDef allDef2 in DefDatabase<ThingDef>.AllDefs)
                {
                    if (allDef2.apparel != null && allDef2.apparel.defaultOutfitTags != null && allDef2.apparel.defaultOutfitTags.Contains("CentaurOutfit"))
                    {
                        outfit3.filter.SetAllow(allDef2, allow: true);
                    }
                }
            }
        }
        /*
        //static int ExLogLimit = 0;
        //static readonly MethodInfo IsPredatorHostileToMethod = AccessTools.Method(typeof(GenHostility), "IsPredatorHostileTo", new Type[] { typeof(Pawn), typeof(Pawn) });
        public static bool ExLog(this bool boolen, string msg, ref string str, bool tarfet = true)
        {
            if (boolen == tarfet)
            {
                str += msg;
            }
            return boolen;
        }
        ///<summary>检测敌对性状态，输出日志。</summary>
        [HarmonyPostfix] public static void GenHostilityHostileToPostfix(Thing a, Thing b, bool __result)
        {
            try
            {
                if (!__result)
                {
                    return;
                }
                if (a.def == AlienCentaurDef || b.def == AlienCentaurDef)
                {

                    static bool IsPredatorHostileTo(Pawn predator, Pawn toPawn)
                    {
                        try
                        {
                            return false;// (bool)IsPredatorHostileToMethod.Invoke(null, new object[] { predator, toPawn });
                        }
                        catch
                        {
                            return false;
                        }
                    }

                    if (a.Destroyed || b.Destroyed || a == b)
                    {
                        return;
                    }
                    Pawn pawn = a as Pawn;
                    Pawn pawn2 = b as Pawn;
                    string an = pawn.Name.ToStringShort;
                    string bn = pawn2.Name.ToStringShort;
                    string msg = $"[Explorite]Testing hostile with {an} and {bn}.\n";
                    _ =
                        (pawn != null
                            && pawn.MentalState != null
                            && pawn.MentalState.ForceHostileTo(b)).ExLog($"Hostile to {an} mental state.\n", ref msg)
                        || (pawn2 != null
                            && pawn2.MentalState != null
                            && pawn2.MentalState.ForceHostileTo(a)).ExLog($"Hostile to {bn} mental state.\n", ref msg)
                        || (pawn != null
                            && pawn2 != null
                            && (IsPredatorHostileTo(pawn, pawn2)
                                || IsPredatorHostileTo(pawn2, pawn))).ExLog($"Hostile to predator.\n", ref msg)
                        || ((a.Faction != null
                                && pawn2 != null
                                && pawn2.HostFaction == a.Faction
                                && (pawn == null
                                    || pawn.HostFaction == null)
                                && PrisonBreakUtility.IsPrisonBreaking(pawn2))
                                || (b.Faction != null
                                    && pawn != null
                                    && pawn.HostFaction == b.Faction
                                    && (pawn2 == null
                                        || pawn2.HostFaction == null)
                                    && PrisonBreakUtility.IsPrisonBreaking(pawn))).ExLog($"Hostile to prison breaking.\n", ref msg)
                        || ((a.Faction == null || pawn2 == null || pawn2.HostFaction != a.Faction)
                                && (b.Faction == null || pawn == null || pawn.HostFaction != b.Faction)
                                && (pawn == null
                                    || !pawn.IsPrisoner
                                    || pawn2 == null
                                    || !pawn2.IsPrisoner)
                                && (pawn == null
                                    || pawn2 == null
                                    || ((!pawn.IsPrisoner
                                        || pawn.HostFaction != pawn2.HostFaction
                                        || PrisonBreakUtility.IsPrisonBreaking(pawn))
                                    && (!pawn2.IsPrisoner
                                        || pawn2.HostFaction != pawn.HostFaction
                                        || PrisonBreakUtility.IsPrisonBreaking(pawn2))))
                            && (pawn == null
                                || pawn2 == null
                                || ((pawn.HostFaction == null
                                        || pawn2.Faction == null
                                        || pawn.HostFaction.HostileTo(pawn2.Faction)
                                        || PrisonBreakUtility.IsPrisonBreaking(pawn))
                                    && (pawn2.HostFaction == null
                                        || pawn.Faction == null
                                        || pawn2.HostFaction.HostileTo(pawn.Faction)
                                        || PrisonBreakUtility.IsPrisonBreaking(pawn2))))
                            && (a.Faction == null
                                || !a.Faction.IsPlayer
                                || pawn2 == null
                                || !pawn2.mindState.WillJoinColonyIfRescued)
                            && (b.Faction == null
                                || !b.Faction.IsPlayer
                                || pawn == null
                                || !pawn.mindState.WillJoinColonyIfRescued)
                            && a.Faction != null
                            && b.Faction != null
                            && a.Faction.HostileTo(b.Faction)
                        ).ExLog($"Hostile to faction relation.\n", ref msg);

                    Log.Message(msg);
                }
            }
            catch
            { }
        }
        */


        ///<summary>使半人马即使携带了物品也会被视为威胁。</summary>
        [HarmonyPostfix]public static void PawnThreatDisabledPostfix(Pawn __instance, ref bool __result, IAttackTargetSearcher disabledFor)
        {
            if (__instance.def == AlienCentaurDef)
            {
                if (__result)
                {
                    if (!__instance.Spawned)
                    {
                        __result = true;
                        return;
                    }
                    /* if (!__instance.InMentalState && __instance.GetTraderCaravanRole() == TraderCaravanRole.Carrier && !(__instance.jobs.curDriver is JobDriver_AttackMelee))
                    {
                        __result = true;
                        return;
                    } */
                    if (__instance.mindState.duty != null && __instance.mindState.duty.def.threatDisabled)
                    {
                        __result = true;
                        return;
                    }
                    if (!__instance.mindState.Active)
                    {
                        __result = true;
                        return;
                    }
                    if (__instance.Downed)
                    {
                        if (disabledFor == null)
                        {
                            __result = true;
                            return;
                        }
                        if (!(disabledFor.Thing is Pawn pawn) || pawn.mindState == null || pawn.mindState.duty == null || !pawn.mindState.duty.attackDownedIfStarving || !pawn.Starving())
                        {
                            __result = true;
                            return;
                        }
                    }
                    __result = false;
                    return;
                }
            }
        }
        ///<summary>使半人马服装正确显示覆盖的部位。</summary>
        [HarmonyPrefix]public static void ApparelPropertiesGetCoveredOuterPartsStringPostfix(ApparelProperties __instance, ref BodyDef body)
        {
            if (__instance.tags.Contains("CentaurBodyFit"))
            {
                body = CentaurBodyDef;
            }
            else if (__instance.tags.Contains("SayersBodyFit"))
            {
                body = SayersBodyDef;
            }
        }
        ///<summary>显示MakeThing的调用堆栈。</summary>
        [HarmonyPrefix]public static void ThingMakerMakeThingPrefix(ThingDef def)
        {
            if (def == TrishotThing2Def)
            {
                Log.Message("[Explorite]Making TriShot Prototype, with stack trace...");
            }
        }
        ///<summary>显示MakeThing的调用堆栈。</summary>
        [HarmonyPostfix]public static void ThingMakerMakeThingPostfix(Thing __result)
        {
            if (__result.def == OrangiceDef)
            {
                Log.Message($"[Explorite]Making Orangice thing {__result.ThingID} detected. Stack trace below.");
            }
        }
        /*
        ///<summary>将被作为合成原材料的三射弓从追踪中移除。</summary>
        [HarmonyPrefix]public static void GenRecipeMakeRecipeProductsPrefix(RecipeDef recipeDef, Pawn worker, List<Thing> ingredients, Thing dominantIngredient, IBillGiver billGiver)
        {
            foreach (Thing thing in ingredients)
            {
                try
                {
                    GameComponentCentaurStory.TryRemove(thing);
                }
                catch { }
            }
        }
        ///<summary>将被制作出来的三射弓添加到追踪中。</summary>
        [HarmonyPostfix]public static void GenRecipeMakeRecipeProductsPostfix(IEnumerable<Thing> __result, RecipeDef recipeDef, Pawn worker, List<Thing> ingredients, Thing dominantIngredient, IBillGiver billGiver)
        {
            foreach (Thing thing in __result)
            {
                try
                {
                    GameComponentCentaurStory.TryAdd(thing);
                }
                catch { }
            }
        }
        */
        ///<summary>将被制作出来的三射弓添加到追踪中。</summary>
        [HarmonyPostfix]public static void GenRecipePostProcessProductPostfix(Thing __result, Thing product, RecipeDef recipeDef, Pawn worker)
        {
            try
            {
                GameComponentCentaurStory.TryAdd(__result);
            }
            catch { }
        }
        ///<summary>降低故障三射弓的好感度加成。</summary>
        [HarmonyPostfix]public static void FactionGiftUtilityGetBaseGoodwillChangePostfix(ref float __result, Thing anyThing, int count, float singlePrice, Faction theirFaction)
        {
            if (anyThing?.def?.weaponTags?.Contains("CentaurTracedTrishot") == true)
            {
                float factor =  singlePrice / anyThing.MarketValue;
                //__result = 0f;//-= TrishotThing1Def.BaseMarketValue * count / 40f;
                __result -= TrishotThing1Def.BaseMarketValue * factor * count / 40f;
            }
        }
        ///<summary>移除半人马随机好心情灵感。</summary>
        [HarmonyPostfix]public static void InspirationHandlerGetRandomAvailableInspirationDefPostfix(InspirationHandler __instance, ref InspirationDef __result)
        {
            if (__instance.pawn.def == AlienCentaurDef)
            {
                __result = null;
            }
        }
        ///<summary>设置三射弓伤害源为满级三射弓。</summary>
        [HarmonyPrefix]public static void DamageInfoCtorPrefix(ref ThingDef weapon)
        {
            if (weapon?.weaponTags?.Contains("CentaurTracedTrishot") == true)
            {
                weapon = TrishotThingDef;
            }
        }
        ///<summary>设置日志中三射弓伤害源为满级三射弓。</summary>
        [HarmonyPrefix]public static void BattleLogEntry_ExplosionImpactCtorPrefix(ref ThingDef weaponDef)
        {
            if (weaponDef?.weaponTags?.Contains("CentaurTracedTrishot") == true)
            {
                weaponDef = TrishotThingDef;
            }
        }
        /*
        ///<summary>设置三射弓弹射物伤害来源为满级三射弓。</summary>
        [HarmonyPostfix]public static void ProjectileLaunchPostfix(Projectile __instance)
        {
            if (__instance.GetType().GetField("equipmentDef", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is ThingDef equipmentDef
             && equipmentDef?.weaponTags?.Contains("CentaurTracedTrishot") == true)
            {
                equipmentDef = TrishotThingDef;
            }
        }
        */
        ///<summary>修正健康面板中的身体部位排序。</summary>
        [HarmonyPostfix]public static void HealthCardUtilityGetListPriorityPostfix(ref float __result, BodyPartRecord rec)
        {
            if (rec?.groups?.Contains(CentaurCorePartGroupDef) ?? false)
            {
                __result += 40002f;
            }
            if (rec?.groups?.Contains(CentaurSubsystemGroup0Def) ?? false)
            {
                __result += 100000f;
                if (rec.groups.Contains(CentaurSubsystemGroup1Def))
                {
                    __result += 3f;
                }
                if (rec.groups.Contains(CentaurSubsystemGroup2Def))
                {
                    __result += 2f;
                }
                if (rec.groups.Contains(CentaurSubsystemGroup3Def))
                {
                    __result += 1f;
                }
            }
        }
        ///<summary>移除半人马自我治疗的70%效果惩罚。</summary>
        [HarmonyPrefix]public static void TendUtilityCalculateBaseTendQualityPrefix(Pawn doctor, Pawn patient, ref float medicinePotency, ref float medicineQualityMax)
        {
            if (doctor == patient && doctor?.def == AlienCentaurDef)
            {
                medicinePotency /= 0.7f;
            }
        }
        ///<summary>检测任务奖励中的橙冰。</summary>
        [HarmonyPostfix]public static void QuestPartDropPodsSetThingsPostfix(IEnumerable<Thing> value)
        {
            IEnumerable<Thing> orangices = value.Where(t => t.def == OrangiceDef);
            foreach (Thing thing in orangices)
            {
                Log.Warning($"[Explorite]Warning, Orangice thing {thing.ThingID} detected in quest part drop pods. Stack trace below.");
            }
        }
        ///<summary>阻止特定项目被研究。</summary>
        [HarmonyPostfix]public static void ResearchProjectDefCanStartNowPostfix(ResearchProjectDef __instance, ref bool __result)
        {
            if (__result && __instance?.tags?.Any(t => t.defName == "ExploriteNeverResearchable") == true)
            {
                __result = false;
            }
        }
    }
}
