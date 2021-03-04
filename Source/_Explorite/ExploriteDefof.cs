/********************
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
        public static GameComponent_CentaurStory GameComponentCentaurStory => Current.Game.GetComponent<GameComponent_CentaurStory>();

        public static readonly Harmony harmonyInstance = new Harmony(id: "Explorite.rimworld.mod.HarmonyPatches");

        public static readonly StatDef PawnShieldEnergyMaxDef = DefDatabase<StatDef>.GetNamed("PawnShieldEnergyMax");
        public static readonly StatDef PawnShieldRechargeRateDef = DefDatabase<StatDef>.GetNamed("PawnShieldRechargeRate");


        public static readonly ThingDef AlienCentaurDef = !InstelledMods.RimCentaurs ? null : DefDatabase<ThingDef>.GetNamed("Alien_Centaur");
        public static readonly ThingDef CentaurHeaddressDef = !InstelledMods.RimCentaurs ? null : DefDatabase<ThingDef>.GetNamed("Apparel_CentaurHeaddress");
        public static readonly ThingDef TrishotThing1Def = !InstelledMods.RimCentaurs ? null : DefDatabase<ThingDef>.GetNamed("Bow_Trishot_1Stage");
        public static readonly ThingDef TrishotThing2Def = !InstelledMods.RimCentaurs ? null : DefDatabase<ThingDef>.GetNamed("Bow_Trishot_2Stage");
        public static readonly ThingDef TrishotThingDef = !InstelledMods.RimCentaurs ? null : DefDatabase<ThingDef>.GetNamed("Bow_Trishot");
        public static readonly ThingDef ComponentArchotechDef = !InstelledMods.RimCentaurs ? null : DefDatabase<ThingDef>.GetNamed("ComponentArchotech");
        public static readonly ThingDef ThermoDamperDef = !InstelledMods.RimCentaurs ? null : DefDatabase<ThingDef>.GetNamed("ThermoDamper");
        public static readonly ThingDef OrangiceDef = !InstelledMods.RimCentaurs ? null : DefDatabase<ThingDef>.GetNamed("Orangice");
        public static readonly HediffDef HyperManipulatorHediffDef = !InstelledMods.RimCentaurs ? null : DefDatabase<HediffDef>.GetNamed("HyperManipulator");
        public static readonly HediffDef PsychicDeafHediffDef = !InstelledMods.RimCentaurs ? null : DefDatabase<HediffDef>.GetNamed("PsychicDeafCentaur");
        public static readonly HediffDef SubsystemBlankHediffDef = !InstelledMods.RimCentaurs ? null : DefDatabase<HediffDef>.GetNamed("CentaurSubsystem_BlankBlank");
        public static readonly BodyPartDef CentaurScapularDef = !InstelledMods.RimCentaurs ? null : DefDatabase<BodyPartDef>.GetNamed("CentaurScapular");
        public static readonly BodyPartDef CentaurSubsystemBodyPartDef = !InstelledMods.RimCentaurs ? null : DefDatabase<BodyPartDef>.GetNamed("CentaurSubsystem");
        public static readonly BodyPartGroupDef CentaurCorePartGroupDef = !InstelledMods.RimCentaurs ? null : DefDatabase<BodyPartGroupDef>.GetNamed("CentaurCorePart");
        public static readonly BodyPartGroupDef CentaurSubsystemGroup0Def = !InstelledMods.RimCentaurs ? null : DefDatabase<BodyPartGroupDef>.GetNamed("CentaurSubsystem");
        public static readonly BodyPartGroupDef CentaurSubsystemGroup1Def = !InstelledMods.RimCentaurs ? null : DefDatabase<BodyPartGroupDef>.GetNamed("CentaurSubsystemPrimary");
        public static readonly BodyPartGroupDef CentaurSubsystemGroup2Def = !InstelledMods.RimCentaurs ? null : DefDatabase<BodyPartGroupDef>.GetNamed("CentaurSubsystemSecondary");
        public static readonly BodyPartGroupDef CentaurSubsystemGroup3Def = !InstelledMods.RimCentaurs ? null : DefDatabase<BodyPartGroupDef>.GetNamed("CentaurSubsystemTertiary");
        public static readonly PawnKindDef CentaurColonistDef = !InstelledMods.RimCentaurs ? null : DefDatabase<PawnKindDef>.GetNamed("CentaurColonist");
        public static readonly FactionDef CentaurPlayerColonyDef = !InstelledMods.RimCentaurs ? null : DefDatabase<FactionDef>.GetNamed("CentaurPlayerColony");
        public static readonly FactionDef CentaurDummyFactionDef = !InstelledMods.RimCentaurs ? null : DefDatabase<FactionDef>.GetNamed("CentaurFactionDummy");
        public static readonly StuffCategoryDef OrangiceStuffDef = !InstelledMods.RimCentaurs ? null : DefDatabase<StuffCategoryDef>.GetNamed("Orangice");
        public static readonly ResearchProjectDef CentaurStoryProjectStep1Def = !InstelledMods.RimCentaurs ? null : DefDatabase<ResearchProjectDef>.GetNamed("CentaurStoryStep1st");
        public static readonly ResearchProjectDef CentaurStoryProjectStep2Def = !InstelledMods.RimCentaurs ? null : DefDatabase<ResearchProjectDef>.GetNamed("CentaurStoryStep2nd");
        public static readonly ResearchProjectDef CentaurStoryProjectStep3Def = !InstelledMods.RimCentaurs ? null : DefDatabase<ResearchProjectDef>.GetNamed("CentaurStoryStep3rd");
        public static readonly BodyDef CentaurBodyDef = !InstelledMods.RimCentaurs ? null : DefDatabase<BodyDef>.GetNamed("Centaur");

        public static readonly HediffDef HediffCentaurSubsystem_NeedsCapacitor_Def = !InstelledMods.RimCentaurs ? null : DefDatabase<HediffDef>.GetNamed("CentaurSubsystem_NeedsCapacitor");
        public static readonly HediffDef HediffCentaurSubsystem_HazardAdaptation_Def = !InstelledMods.RimCentaurs ? null : DefDatabase<HediffDef>.GetNamed("CentaurSubsystem_HazardAdaptation");
        public static readonly HediffDef HediffCentaurSubsystem_AntiMass_Def = !InstelledMods.RimCentaurs ? null : DefDatabase<HediffDef>.GetNamed("CentaurSubsystem_AntiMass");

        public static readonly ThingDef AlienSayersDef = !InstelledMods.Sayers ? null : DefDatabase<ThingDef>.GetNamed("Alien_Sayers");
        public static readonly ThingDef AlienFlowerBorhAnimalDef = !InstelledMods.Sayers ? null : DefDatabase<ThingDef>.GetNamed("Alien_FlowerBorhAnimal");
        public static readonly ThingDef FleshTreeDef = !InstelledMods.Sayers ? null : DefDatabase<ThingDef>.GetNamed("Plant_BloodyTree");
        public static readonly ThingDef BloodyTreeMeatDef = !InstelledMods.Sayers ? null : DefDatabase<ThingDef>.GetNamed("BloodyTree_Meat");
        public static readonly ThingDef PlantReactionComputerDef = !InstelledMods.Sayers ? null : DefDatabase<ThingDef>.GetNamed("PlantReactionComputer");
        public static readonly DamageDef InjectionDamageDef = !InstelledMods.Sayers ? null : DefDatabase<DamageDef>.GetNamed("Injection");
        public static readonly HediffDef InjectionHediffDef = !InstelledMods.Sayers ? null : DefDatabase<HediffDef>.GetNamed("Injection");
        public static readonly FactionDef SayersPlayerColonyDef = !InstelledMods.Sayers ? null : DefDatabase<FactionDef>.GetNamed("SayersClan");
        public static readonly FactionDef SayersPlayerColonySingleDef = !InstelledMods.Sayers ? null : DefDatabase<FactionDef>.GetNamed("SayersRefugees");
        public static readonly BodyDef SayersBodyDef = !InstelledMods.RimCentaurs ? null : DefDatabase<BodyDef>.GetNamed("Body_Sayers");

        public static readonly ThingDef AlienMichellesDef = !InstelledMods.Sayers ? null : DefDatabase<ThingDef>.GetNamed("Alien_Michelles");

        public static readonly ThingDef AlienGuoguoDef = !InstelledMods.GuoGuo ? null : DefDatabase<ThingDef>.GetNamed("Alien_Guoguo");
        public static readonly FactionDef GuoguoPlayerColonyDef = !InstelledMods.GuoGuo ? null : DefDatabase<FactionDef>.GetNamed("GuoguoPlayerColony");

        //public static readonly Def CentaursScenarioRetroCruiseDef = !(InstelledMods.SoS2 && InstelledMods.RimCentaurs) ? null : DefDatabase<EnemyShipDef>.GetNamed("CentaursScenarioRetroCruise");

        //public static readonly AbilityDef AbilityTrishot_TrishotDef = DefDatabase<AbilityDef>.GetNamed("AbilityTrishot_Trishot");
        //public static readonly AbilityDef AbilityTrishot_IcoshotDef = DefDatabase<AbilityDef>.GetNamed("AbilityTrishot_Icoshot");
        //public static readonly AbilityDef AbilityTrishot_OneshotDef = DefDatabase<AbilityDef>.GetNamed("AbilityTrishot_Oneshot");

        public static readonly Backstory CentaurCivilRetro = BackstoryDatabase.allBackstories.TryGetValue("CentaurCivil_Retro");
        public static readonly Backstory CentaurCivilMayinas = BackstoryDatabase.allBackstories.TryGetValue("Backstory_Mayinas_Exploriter");

    }

}
