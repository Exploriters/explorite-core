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
using static CentaurTheMagnuassembly.RimCentaurCore;

namespace CentaurTheMagnuassembly
{
    [StaticConstructorOnStartup]
    public static class PsychicEntropyTrackerPatch
    {
        private static readonly Type patchType = typeof(PsychicEntropyTrackerPatch);

        static PsychicEntropyTrackerPatch()
        {
            Harmony harmonyInstance = new Harmony(id: "CentaurTheMagnuassembly.rimworld.mod.PsychicEntropyTrackerPatch");

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
