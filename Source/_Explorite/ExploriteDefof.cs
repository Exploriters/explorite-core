/**
* 常量文件。
* --siiftun1857
*/
using HarmonyLib;
using RimWorld;
using Verse;
namespace Explorite
{
    public static partial class ExploriteCore
    {
        public static readonly Harmony harmonyInstance = new Harmony(id: "Explorite.rimworld.mod.HarmonyPatches");

        public static readonly ThingDef AlienCentaurDef = !InstelledMods.RimCentaurs ? null : DefDatabase<ThingDef>.GetNamed("Alien_Centaur", errorOnFail: false);
        public static readonly HediffDef HyperManipulatorHediffDef = !InstelledMods.RimCentaurs ? null : DefDatabase<HediffDef>.GetNamed("HyperManipulator", errorOnFail: false);
        public static readonly BodyPartDef CentaurScapularDef = !InstelledMods.RimCentaurs ? null : DefDatabase<BodyPartDef>.GetNamed("CentaurScapular", errorOnFail: false);
        public static readonly PawnKindDef CentaurColonistDef = !InstelledMods.RimCentaurs ? null : DefDatabase<PawnKindDef>.GetNamed("CentaurColonist", errorOnFail: false);
        public static readonly FactionDef CentaurPlayerColonyDef = !InstelledMods.RimCentaurs ? null : DefDatabase<FactionDef>.GetNamed("CentaurPlayerColony", errorOnFail: false);

        public static readonly ThingDef AlienSayersDef = !InstelledMods.Sayers ? null : DefDatabase<ThingDef>.GetNamed("Alien_Sayers", errorOnFail: false);
        public static readonly ThingDef AlienFlowerBorhAnimalDef = !InstelledMods.Sayers ? null : DefDatabase<ThingDef>.GetNamed("Alien_FlowerBorhAnimal", errorOnFail: false);

        //public static readonly Def CentaursScenarioRetroCruiseDef = !(InstelledMods.SoS2 && InstelledMods.RimCentaurs) ? null : DefDatabase<EnemyShipDef>.GetNamed("CentaursScenarioRetroCruise");

        //public static readonly AbilityDef AbilityTrishot_TrishotDef = DefDatabase<AbilityDef>.GetNamed("AbilityTrishot_Trishot");
        //public static readonly AbilityDef AbilityTrishot_IcoshotDef = DefDatabase<AbilityDef>.GetNamed("AbilityTrishot_Icoshot");
        //public static readonly AbilityDef AbilityTrishot_OneshotDef = DefDatabase<AbilityDef>.GetNamed("AbilityTrishot_Oneshot");

        public static readonly Backstory CentaurCivilRetro = BackstoryDatabase.allBackstories.TryGetValue("CentaurCivil_Retro");
        public static readonly Backstory CentaurCivilMayinas = BackstoryDatabase.allBackstories.TryGetValue("Backstory_Mayinas_Exploriter");

    }

}
