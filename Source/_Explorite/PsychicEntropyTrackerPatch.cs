﻿/**
 * 移除半人马的疼痛带来心灵熵消散增益。
 * TODO: 修复补丁
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
     * <summary>移除半人马的疼痛带来心灵熵消散增益。</summary>
     */
    [StaticConstructorOnStartup]
    public static class PsychicEntropyTrackerPatch
    {
        private static readonly Type patchType = typeof(PsychicEntropyTrackerPatch);

        static PsychicEntropyTrackerPatch()
        {
            Harmony harmonyInstance = new Harmony(id: "Explorite.rimworld.mod.PsychicEntropyTrackerPatch");

            harmonyInstance.Patch(AccessTools.Method(typeof(Pawn_PsychicEntropyTracker), nameof(Pawn_PsychicEntropyTracker.PainMultiplier), null),
                postfix: new HarmonyMethod(patchType, nameof(NoPainBounsForCentaursPostfix)));
        }

        [HarmonyPostfix]
        public static void NoPainBounsForCentaursPostfix(Pawn_PsychicEntropyTracker __instance, ref float __result)
        {
            if (__instance.Pawn.def == AlienCentaurDef)
                __result = -__result;
        }

    }
}
