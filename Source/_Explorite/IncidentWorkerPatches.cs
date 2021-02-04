/**
 * 对IncidentWorker_WandererJoin.CanFireNowSub的函数补丁。
 * 禁用半人马阵营生成流浪者加入事件。
 * --siiftun1857
 */
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
//using Harmony;
using HarmonyLib;
using UnityEngine;
using Verse.AI;
using Verse;
using Verse.Sound;
using static Explorite.ExploriteCore;

namespace Explorite
{
    /**
     * <summary>
     * 禁用半人马阵营生成流浪者加入事件。
     * </summary>
     */
    [StaticConstructorOnStartup]
    public static class IncidentWorkerPatches
    {
        // NoWandererJoinForCentaurs
        // IncidentWorker_WandererJoinTransportPod
        // ReSharper disable once InconsistentNaming
        private static readonly Type patchType = typeof(IncidentWorkerPatches);

        static IncidentWorkerPatches()
        {
            Harmony harmonyInstance = new Harmony(id: "Explorite.rimworld.mod.IncidentWorkerPatches");

            harmonyInstance.Patch(AccessTools.Method(typeof(IncidentWorker_WandererJoin), "CanFireNowSub", new[] { typeof(IncidentParms) }),
                postfix: new HarmonyMethod(patchType, nameof(WandererJoin_CanFireNowPostfix)));
        }

        [HarmonyPostfix]
        public static void WandererJoin_CanFireNowPostfix(IncidentParms parms, ref bool __result)
        {
            if (Faction.OfPlayer.def.defName == "CentaurPlayerColony")
                __result = false;
        }

    }
}
