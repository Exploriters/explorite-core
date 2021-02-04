using HarmonyLib;
using RimWorld;
using System;
using Verse;
using static Sayers.SayersCore;

namespace Sayers
{
    /*
    
    // 故障：引发NullReferenceException

    [StaticConstructorOnStartup]
    public static class FoodPoisonImmuneForSayersPatch
    {
        private static readonly Type patchType = typeof(FoodPoisonImmuneForSayersPatch);

        static FoodPoisonImmuneForSayersPatch()
        {
            Harmony harmonyInstance = new Harmony(id: "Saysrs.rimworld.mod.FoodPoisonImmuneForSayersPatch");

            harmonyInstance.Patch(AccessTools.Method(typeof(FoodUtility), nameof(FoodUtility.AddFoodPoisoningHediff), new[] { typeof(Pawn), typeof(Thing), typeof(FoodPoisonCause) }),
                prefix: new HarmonyMethod(patchType, nameof(AddFoodPoisoningHediffPrefix)));
        }
        [HarmonyPrefix]
        public static void AddFoodPoisoningHediffPrefix(ref Pawn pawn, ref Thing ingestible, ref FoodPoisonCause cause)
        {
            if (pawn.def == AlienSayersDef)
                pawn = null;
        }
    }
    */
}
