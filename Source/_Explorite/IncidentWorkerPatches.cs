/**
 * 对IncidentWorker_WandererJoin.CanFireNowSub的函数补丁。
 * 禁用半人马阵营生成流浪者加入事件。
 * --siiftun1857
 */
using System;
using RimWorld;
using HarmonyLib;
using Verse;
using static Explorite.ExploriteCore;
using SaveOurShip2;

namespace Explorite
{
    /**
     * <summary>
     * 禁用半人马阵营生成流浪者加入事件。
     * </summary>
     */
    [StaticConstructorOnStartup]
    internal static class IncidentWorkerPatches
    {
        // NoWandererJoinForCentaurs
        // IncidentWorker_WandererJoinTransportPod
        // ReSharper disable once InconsistentNaming
        private static readonly Type patchType = typeof(IncidentWorkerPatches);

        static IncidentWorkerPatches()
        {
            //Harmony harmonyInstance = new Harmony(id: "Explorite.rimworld.mod.IncidentWorkerPatches");

            harmonyInstance.Patch(AccessTools.Method(typeof(IncidentWorker_WandererJoin), "CanFireNowSub", new[] { typeof(IncidentParms) }),
                postfix: new HarmonyMethod(patchType, nameof(WandererJoin_CanFireNowPostfix)));
        }

        [HarmonyPostfix]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(null, "IDE0060")]
        public static void WandererJoin_CanFireNowPostfix(IncidentParms parms, ref bool __result)
        {
            if (Faction.OfPlayer.def.defName == "CentaurPlayerColony")
                __result = false;
        }

    }
    /**
     * <summary>
     * 使半人马可以在太空中靠动力装甲存活。
     * </summary>
     */
    [StaticConstructorOnStartup]
    internal static class HasSpaceSuitSlowPatches
    {
        // NoWandererJoinForCentaurs
        // IncidentWorker_WandererJoinTransportPod
        // ReSharper disable once InconsistentNaming
        private static readonly Type patchType = typeof(HasSpaceSuitSlowPatches);

        static HasSpaceSuitSlowPatches()
        {
            if (!InstelledMods.SoS2)
                return;
            harmonyInstance.Patch(AccessTools.Method(typeof(ShipInteriorMod2), "HasSpaceSuitSlow", new[] { typeof(Pawn) }),
                postfix: new HarmonyMethod(patchType, nameof(HasSpaceSuitSlowPostfix)));
        }

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

    }
}
