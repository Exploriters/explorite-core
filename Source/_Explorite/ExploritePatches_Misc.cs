/**
 * 包含多个补丁的合集文件。
 * --siiftun1857
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
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
        static ExploritePatches()
        {
            try
            {
                //Log.Message("[Explorite]Patching Verse.PawnGenerator.GeneratePawn with postfix GeneratePawnPostfix");
                harmonyInstance.Patch(AccessTools.Method(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new[] { typeof(PawnGenerationRequest) }),
                    postfix: new HarmonyMethod(patchType, nameof(GeneratePawnPostfix)));

                //Log.Message("[Explorite]Patching RimWorld.SkillRecord.Learn with prefix SkillLearnPrefix");
                harmonyInstance.Patch(AccessTools.Method(typeof(SkillRecord), nameof(SkillRecord.Learn), new[] { typeof(float), typeof(bool) }),
                    prefix: new HarmonyMethod(patchType, nameof(SkillLearnPrefix)));
                //Log.Message("[Explorite]Patching RimWorld.SkillRecord.Interval with postfix SkillIntervalPostfix");
                harmonyInstance.Patch(AccessTools.Method(typeof(SkillRecord), nameof(SkillRecord.Interval), new Type[] { }),
                    postfix: new HarmonyMethod(patchType, nameof(SkillIntervalPostfix)));

                //Log.Message("[Explorite]Patching RimWorld.Pawn_PsychicEntropyTracker.get_PainMultiplier with postfix NoPainBounsForCentaursPostfix");
                harmonyInstance.Patch(AccessTools.Method(typeof(Pawn_PsychicEntropyTracker), "get_PainMultiplier", null),
                    postfix: new HarmonyMethod(patchType, nameof(NoPainBounsForCentaursPostfix)));

                //Log.Message("[Explorite]Patching RimWorld.IncidentWorker_WandererJoin.CanFireNowSub with postfix WandererJoin_CanFireNowPostfix");
                harmonyInstance.Patch(AccessTools.Method(typeof(IncidentWorker_WandererJoin), "CanFireNowSub", new[] { typeof(IncidentParms) }),
                    postfix: new HarmonyMethod(patchType, nameof(WandererJoinCannotFirePostfix)));

                //Log.Message("[Explorite]Patching RimWorld.Need_Outdoors.get_Disabled with postfix NeedOutdoors_DisabledPostfix");
                harmonyInstance.Patch(AccessTools.Method(typeof(Need_Outdoors), "get_Disabled", new Type[] { }),
                    postfix: new HarmonyMethod(patchType, nameof(NeedOutdoors_DisabledPostfix)));

                //Log.Message("[Explorite]Patching RimWorld.MeditationFocusDef.CanPawnUse with postfix MeditationFocusCanPawnUsePostfix");
                harmonyInstance.Patch(AccessTools.Method(typeof(MeditationFocusDef), nameof(MeditationFocusDef.CanPawnUse), new Type[] { typeof(Pawn) }),
                    postfix: new HarmonyMethod(patchType, nameof(MeditationFocusCanPawnUsePostfix)));

                //Log.Message("[Explorite]Patching RimWorld.CompAssignableToPawn_Throne.AssignedAnything with postfix AssignableThroneToPawnAssignedAnythingPostfix");
                //harmonyInstance.Patch(AccessTools.Method(typeof(CompAssignableToPawn_Throne), nameof(CompAssignableToPawn_Throne.AssignedAnything), new Type[] { typeof(Pawn) }),
                //    postfix: new HarmonyMethod(patchType, nameof(AssignThroneToPawnCandidatesPostfix)));

                //Log.Message("[Explorite]Patching RimWorld.CompAssignableToPawn_Bed.get_AssigningCandidates with postfix AssignBedToPawnCandidatesPostfix");
                harmonyInstance.Patch(AccessTools.Method(typeof(CompAssignableToPawn), "get_AssigningCandidates"),
                    postfix: new HarmonyMethod(patchType, nameof(AssignBedToPawnCandidatesPostfix)));

                //Log.Message("[Explorite]Patching RimWorld.CompAssignableToPawn_Bed.get_HasFreeSlot with postfix AssignBedToPawnHasFreeSlotPostfix");
                harmonyInstance.Patch(AccessTools.Method(typeof(CompAssignableToPawn), "get_HasFreeSlot"),
                    postfix: new HarmonyMethod(patchType, nameof(AssignBedToPawnHasFreeSlotPostfix)));

                //Log.Message("[Explorite]Patching RimWorld.CompAssignableToPawn_Bed.TryAssignPawn with postfix AssignBedToPawnTryAssignPawnPostfix");
                harmonyInstance.Patch(AccessTools.Method(typeof(CompAssignableToPawn_Bed), nameof(CompAssignableToPawn_Bed.TryAssignPawn)),
                    postfix: new HarmonyMethod(patchType, nameof(AssignBedToPawnTryAssignPawnPostfix)));

                //Log.Message("[Explorite]Patching RimWorld.Plant.get_GrowthRateFactor_Temperature with postfix AssignBedToPawnTryAssignPawnPostfix");
                harmonyInstance.Patch(AccessTools.Method(typeof(Plant), "get_GrowthRateFactor_Temperature"),
                    postfix: new HarmonyMethod(patchType, nameof(PlantGrowthRateFactorNoTemperaturePostfix)));

                //Log.Message("[Explorite]Patching RimWorld.Plant.get_GrowthRate with postfix PlantGrowthRateFactorEnsurePostfix");
                harmonyInstance.Patch(AccessTools.Method(typeof(Plant), "get_GrowthRate"),
                    postfix: new HarmonyMethod(patchType, nameof(PlantGrowthRateFactorEnsurePostfix)));

                //Log.Message("[Explorite]Patching RimWorld.Alert_NeedBatteries.NeedBatteries with postfix AlertNeedBatteriesPostfix");
                harmonyInstance.Patch(AccessTools.Method(typeof(Alert_NeedBatteries), "NeedBatteries", new Type[] { typeof(Map) }),
                    postfix: new HarmonyMethod(patchType, nameof(AlertNeedBatteriesPostfix)));

                //Log.Message("[Explorite]Patching RimWorld.JobGiver_GetFood.TryGiveJob with postfix GetFoodTryGiveJobPostfix");
                harmonyInstance.Patch(AccessTools.Method(typeof(JobGiver_GetFood), "TryGiveJob"),
                    postfix: new HarmonyMethod(patchType, nameof(GetFoodTryGiveJobPostfix)));

                //Log.Message("[Explorite]Patching Verse.Pawn_HealthTracker.get_InPainShock with postfix PawnHealthTrackerInPainShockPostfix");
                harmonyInstance.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), "get_InPainShock"),
                    postfix: new HarmonyMethod(patchType, nameof(PawnHealthTrackerInPainShockPostfix)));

                if (InstelledMods.RimCentaurs)
                {
                    // 依赖 HediffDef PsychicDeafCentaur
                    //Log.Message("[Explorite]Patching RimWorld.StatPart_ApparelStatOffset.TransformValue with postfix PsychicSensitivityPostfix");
                    harmonyInstance.Patch(AccessTools.Method(typeof(StatPart_ApparelStatOffset), nameof(StatPart_ApparelStatOffset.TransformValue)),
                        postfix: new HarmonyMethod(patchType, nameof(PsychicSensitivityPostfix)));

                    // 依赖 StuffCategoryDef Orangice
                    //Log.Message("[Explorite]Patching Verse.StuffProperties.CanMake with postfix StuffCanMakePostfix");
                    //harmonyInstance.Patch(AccessTools.Method(typeof(StuffProperties), nameof(StuffProperties.CanMake), new Type[] { typeof(BuildableDef) }),
                    //    postfix: new HarmonyMethod(patchType, nameof(StuffCanMakePostfix)));
                }
                if (InstelledMods.Sayers)
                { }
                if (InstelledMods.GuoGuo)
                { }
                if (InstelledMods.SoS2)
                {
                    // 依赖 类 SaveOurShip2.ShipInteriorMod2
                    //Log.Message("[Explorite]Patching SaveOurShip2.ShipInteriorMod2.HasSpaceSuitSlow with postfix HasSpaceSuitSlowPostfix");
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

        ///<summary>使半人马可以使用额外类型的冥想媒介。</summary>
        [HarmonyPostfix]public static void MeditationFocusCanPawnUsePostfix(MeditationFocusDef __instance, ref bool __result, Pawn p)
        {
            if (p.def == AlienCentaurDef&& (
                    __instance == MeditationFocusDefOf.Natural 
                    || __instance == DefDatabase<MeditationFocusDef>.GetNamed("Natural")    //自然
                    || __instance == DefDatabase<MeditationFocusDef>.GetNamed("Artistic")   //艺术
                    || __instance == DefDatabase<MeditationFocusDef>.GetNamed("Dignified")  //庄严
                    )
                )
            {
                __result = true;
            }
        }

        /*
        ///<summary>使半人马始终可以使用王座。</summary>
        [HarmonyPostfix]public static void AssignThroneToPawnCandidatesPostfix(CompAssignableToPawn_Throne __instance, ref IEnumerable<Pawn> __result)
        {
            if (!parent.Spawned)
            {
                return Enumerable.Empty<Pawn>();
            }
            return from p in parent.Map.mapPawns.FreeColonists
                   where p.royalty != null && p.royalty.AllTitlesForReading.Any()
                   orderby CanAssignTo(p).Accepted descending
                   select p;
        }*/

        
        ///<summary>使半人马只能使用双人床。</summary>
        [HarmonyPostfix]public static void AssignBedToPawnCandidatesPostfix(CompAssignableToPawn __instance, ref IEnumerable<Pawn> __result)
        {
            if (__instance is CompAssignableToPawn_Bed &&
                __instance?.Props?.maxAssignedPawnsCount < 2)
            {
                __result =  __result?.Where(pawn => pawn?.def != AlienCentaurDef);
            }
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

        ///<summary>使半人马不能与他人共用一个床。</summary>
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

        ///<summary>使血肉树的生长无视环境温度。</summary>
        [HarmonyPostfix]public static void PlantGrowthRateFactorNoTemperaturePostfix(Plant __instance, ref float __result)
        {
            if (__instance.def == FleshTreeDef)
            {
                __result = 1f;
            }
        }

        ///<summary>使血肉树的生长至少具有100%速率。</summary>
        [HarmonyPostfix]public static void PlantGrowthRateFactorEnsurePostfix(Plant __instance, ref float __result)
        {
            if (__instance.def == FleshTreeDef
                && __result < 1f)
            {
                __result = 1f;
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
                    if (!(t is Corpse))
                    {
                        return false;
                    }
                    if (t.IsForbidden(pawn))
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
        [HarmonyPostfix]
        public static void PawnHealthTrackerInPainShockPostfix(Pawn_HealthTracker __instance, ref bool __result)
        {
            if (
                __instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
                && pawn.def == AlienCentaurDef)
            {
                __result = false;
            }
        }

        /*
        ///<summary></summary>
        [HarmonyPostfix]public static void StuffCanMakePostfix(BuildableDef t, StuffProperties __instance, ref bool __result)
        {
            if(__instance?.parent?.stuffCategories?.Contains(OrangiceStuffDef) == true)
            {
                if (t?.MadeFromStuff == true ||
                    DefDatabase<BuildableDef>.GetNamed($"Blueprint_{t.defName}", false) != null
                    )
                {
                    __result = false;
                }
            }
        }
        */

    }
}
