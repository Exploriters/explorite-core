using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace Sayers
{
    public static class SayersCore
    {
        public static readonly ThingDef AlienSayersDef = DefDatabase<ThingDef>.GetNamed("Alien_Sayers");
        public static readonly ThingDef AlienFlowerBorhAnimalDef = DefDatabase<ThingDef>.GetNamed("Alien_FlowerBorhAnimal");
    }
}
