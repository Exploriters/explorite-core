/********************
 * 常量文件。
 * 
 * 注意：有意不使用DefOf
 * --siiftun1857
 */
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Explorite
{
	public static partial class ExploriteCore
	{


		private static readonly Dictionary<Type, Def> malrefs = new Dictionary<Type, Def>();
		private static T GetModDef<T>(bool willAttempt, string defName, bool errorOnFail = true) where T : Def, new()
		{
			if (willAttempt)
			{
				return DefDatabase<T>.GetNamed(defName, errorOnFail);
			}
			else
			{
				return GetModDef<T>();
			}
		}
		private static T GetModDef<T>(bool _) where T : Def, new()
		{
			return GetModDef<T>();
		}
		private static T GetModDef<T>() where T : Def, new()
		{
			Type t = typeof(T);
			if (malrefs.TryGetValue(t, out Def value))
			{
				if ((value as T) != null)
				{
					return value as T;
				}
			}

			T def = new T
			{
				defName = "EXPLO_MALREF",
				label = "EXPLO_MALREF",
				description = $"Encountered bad reference of {nameof(T)}."
			};
			malrefs.Add(t, def);
			return def;
		}

		public static bool IsNonMal(this Def def)
		{
			if (def == null || malrefs.Any(kvp => kvp.Value == def))
				return false;
			else
				return true;
		}

		public static GameComponent_CentaurStory GameComponentCentaurStory => Current.Game.GetComponent<GameComponent_CentaurStory>();

		public static readonly Harmony harmonyInstance = new Harmony(id: "Explorite.rimworld.mod.HarmonyPatches");

		public static readonly StatDef PawnShieldEnergyMaxDef = GetModDef<StatDef>(true, "PawnShieldEnergyMax");
		public static readonly StatDef PawnShieldRechargeRateDef = GetModDef<StatDef>(true, "PawnShieldRechargeRate");

		public static readonly JobDef MaunalActionObjectAdjustingJob = GetModDef<JobDef>(true, "ExMaunalActionObjectAdjusting");
		public static readonly DesignationDef MaunalActionObjectAdjustDesignation = GetModDef<DesignationDef>(true, "ExMaunalActionObjectAdjust");
		public static readonly ConceptDef ThingdjustingDesignationConcept = GetModDef<ConceptDef>(true, "ExThingdjustingDesignation");
		public static readonly RecordDef ThingAdjustedRecord = GetModDef<RecordDef>(true, "ExThingAdjusted");

		public static readonly ThingDef AlienCentaurDef = GetModDef<ThingDef>(InstelledMods.RimCentaurs, "Alien_Centaur");
		public static readonly ThingDef AlienCentaurCorpseDef = GetModDef<ThingDef>(InstelledMods.RimCentaurs, "Corpse_Alien_Centaur");
		public static readonly ThingDef CentaurHeaddressDef = GetModDef<ThingDef>(InstelledMods.RimCentaurs, "Apparel_CentaurHeaddress");
		//public static readonly ThingDef TrishotThing1Def = GetModDef<ThingDef>(InstelledMods.RimCentaurs, "Bow_Trishot_1Stage");
		//public static readonly ThingDef TrishotThing2Def = GetModDef<ThingDef>(InstelledMods.RimCentaurs, "Bow_Trishot_2Stage");
		public static readonly ThingDef TrishotThingDef = GetModDef<ThingDef>(InstelledMods.RimCentaurs, "Bow_Trishot");
		public static readonly ThingDef ComponentArchotechDef = GetModDef<ThingDef>(InstelledMods.RimCentaurs, "ComponentArchotech");
		public static readonly ThingDef ThermoDamperDef = GetModDef<ThingDef>(InstelledMods.RimCentaurs, "ThermoDamper");
		public static readonly ThingDef OrangiceDef = GetModDef<ThingDef>(InstelledMods.RimCentaurs, "Orangice");
		public static readonly ThingDef HyperTrishotTurretBuildingDef = GetModDef<ThingDef>(InstelledMods.RimCentaurs, "Turret_HyperTrishot");
		public static readonly ThingDef CentaurBedDef = GetModDef<ThingDef>(InstelledMods.RimCentaurs, "CentaurBed");
		public static readonly HediffDef HyperManipulatorHediffDef = GetModDef<HediffDef>(InstelledMods.RimCentaurs, "HyperManipulator");
		public static readonly HediffDef SubsystemBlankHediffDef = GetModDef<HediffDef>(InstelledMods.RimCentaurs, "CentaurSubsystem_BlankBlank");
		public static readonly BodyDef CentaurBodyDef = GetModDef<BodyDef>(InstelledMods.RimCentaurs, "Centaur");
		public static readonly BodyPartDef CentaurScapularDef = GetModDef<BodyPartDef>(InstelledMods.RimCentaurs, "CentaurScapular");
		public static readonly BodyPartDef CentaurSubsystemBodyPartDef = GetModDef<BodyPartDef>(InstelledMods.RimCentaurs, "CentaurSubsystem");
		public static readonly BodyPartGroupDef CentaurTorsoGroupDef = GetModDef<BodyPartGroupDef>(InstelledMods.RimCentaurs, "CentaurTorso");
		public static readonly BodyPartGroupDef CentaurCorePartGroupDef = GetModDef<BodyPartGroupDef>(InstelledMods.RimCentaurs, "CentaurCorePart");
		public static readonly BodyPartGroupDef CentaurSubsystemGroup0Def = GetModDef<BodyPartGroupDef>(InstelledMods.RimCentaurs, "CentaurSubsystem");
		public static readonly BodyPartGroupDef CentaurSubsystemGroup1Def = GetModDef<BodyPartGroupDef>(InstelledMods.RimCentaurs, "CentaurSubsystemPrimary");
		public static readonly BodyPartGroupDef CentaurSubsystemGroup2Def = GetModDef<BodyPartGroupDef>(InstelledMods.RimCentaurs, "CentaurSubsystemSecondary");
		public static readonly BodyPartGroupDef CentaurSubsystemGroup3Def = GetModDef<BodyPartGroupDef>(InstelledMods.RimCentaurs, "CentaurSubsystemTertiary");
		public static readonly PawnKindDef CentaurColonistDef = GetModDef<PawnKindDef>(InstelledMods.RimCentaurs, "CentaurColonist");
		public static readonly FactionDef CentaurPlayerColonyDef = GetModDef<FactionDef>(InstelledMods.RimCentaurs, "CentaurPlayerColony");
		public static readonly FactionDef CentaurDummyFactionDef = GetModDef<FactionDef>(InstelledMods.RimCentaurs, "CentaurFactionDummy");
		public static readonly StuffCategoryDef OrangiceStuffDef = GetModDef<StuffCategoryDef>(InstelledMods.RimCentaurs, "Orangice");
		public static readonly ResearchProjectDef CentaurStoryProjectStep1Def = GetModDef<ResearchProjectDef>(InstelledMods.RimCentaurs, "CentaurStoryStep1st");
		public static readonly ResearchProjectDef CentaurStoryProjectStep2Def = GetModDef<ResearchProjectDef>(InstelledMods.RimCentaurs, "CentaurStoryStep2nd");
		public static readonly ResearchProjectDef CentaurStoryProjectStep3Def = GetModDef<ResearchProjectDef>(InstelledMods.RimCentaurs, "CentaurStoryStep3rd");

		public static readonly HediffDef PsychicDeafHediffDef = GetModDef<HediffDef>(InstelledMods.RimCentaurs && InstelledMods.Royalty, "PsychicDeafCentaur");
		//public static readonly MemeDef CentaurMemeDef = GetModDef<MemeDef>(InstelledMods.RimCentaurs && InstelledMods.Ideology, "Centaur");
		public static readonly MemeDef CentaurStructureMemeDef = GetModDef<MemeDef>(InstelledMods.RimCentaurs && InstelledMods.Ideology, "Structure_Centaur");
		public static readonly IdeoIconDef CentaurIdeoIconDef = GetModDef<IdeoIconDef>(InstelledMods.RimCentaurs && InstelledMods.Ideology, "Exploriter");
		//public static readonly ColorDef CentaurBlueDef = GetModDef<ColorDef>(InstelledMods.RimCentaurs, "CentaurBlue");
		//public static readonly ColorDef ExploriteBlueDef = GetModDef<ColorDef>(InstelledMods.RimCentaurs, "ExploriteBlue");
		//public static readonly ColorDef ExploriteOrangeDef = GetModDef<ColorDef>(InstelledMods.RimCentaurs, "ExploriteOrange");

		public static readonly HediffDef HediffCentaurSubsystem_NeedsCapacitor_Def = GetModDef<HediffDef>(InstelledMods.RimCentaurs, "CentaurSubsystem_NeedsCapacitor");
		public static readonly HediffDef HediffCentaurSubsystem_HazardAdaptation_Def = GetModDef<HediffDef>(InstelledMods.RimCentaurs, "CentaurSubsystem_HazardAdaptation");
		public static readonly HediffDef HediffCentaurSubsystem_AntiMass_Def = GetModDef<HediffDef>(InstelledMods.RimCentaurs, "CentaurSubsystem_AntiMass");

		public static readonly ThingDef AlienSayersDef = GetModDef<ThingDef>(InstelledMods.Sayers, "Alien_Sayers");
		public static readonly ThingDef AlienMichellesDef = GetModDef<ThingDef>(InstelledMods.Sayers);
		public static readonly ThingDef AlienFlowerBorhAnimalDef = GetModDef<ThingDef>(InstelledMods.Sayers);
		public static readonly ThingDef FleshTreeDef = GetModDef<ThingDef>(InstelledMods.Sayers, "Plant_BloodyTree");
		public static readonly ThingDef BloodyTreeMeatDef = GetModDef<ThingDef>(InstelledMods.Sayers, "BloodyTree_Meat");
		public static readonly ThingDef PlantReactionComputerDef = GetModDef<ThingDef>(InstelledMods.Sayers, "PlantReactionComputer");
		public static readonly DamageDef InjectionDamageDef = GetModDef<DamageDef>(InstelledMods.Sayers, "Injection");
		public static readonly HediffDef InjectionHediffDef = GetModDef<HediffDef>(InstelledMods.Sayers, "Injection");
		public static readonly FactionDef SayersPlayerColonyDef = GetModDef<FactionDef>(InstelledMods.Sayers, "SayersClan");
		public static readonly FactionDef SayersPlayerColonySingleDef = GetModDef<FactionDef>(InstelledMods.Sayers);
		public static readonly BodyDef SayersBodyDef = GetModDef<BodyDef>(InstelledMods.Sayers, "Body_Sayers");
		public static readonly ThoughtDef SayersAbandonedSadnessThoughtDef = GetModDef<ThoughtDef>(InstelledMods.Sayers, "SayersAbandonedSadness");
		public static readonly ThoughtDef SayersFeelLonelyThoughtDef = GetModDef<ThoughtDef>(InstelledMods.Sayers, "SayersWillFeelLonely");
		public static readonly MemeDef SayersMeme1Def = GetModDef<MemeDef>(InstelledMods.Sayers && InstelledMods.Ideology, "Experimental_SayersMeme");
		public static readonly MemeDef SayersMeme2Def = GetModDef<MemeDef>(InstelledMods.Sayers && InstelledMods.Ideology, "CarnivorousAnimal_SayersMeme");
		public static readonly MemeDef SayersStructureMemeDef = GetModDef<MemeDef>(InstelledMods.Sayers && InstelledMods.Ideology, "Structure_Sayers");

		public static readonly ThingDef AlienGuoguoDef = GetModDef<ThingDef>(InstelledMods.GuoGuo, "Alien_Guoguo");
		public static readonly FactionDef GuoguoPlayerColonyDef = GetModDef<FactionDef>(InstelledMods.GuoGuo, "GuoguoPlayerColony");

		public static readonly ThingDef AlienDeerFoxDef = GetModDef<ThingDef>(InstelledMods.DeerFox, "Alien_DeerFox");
		public static readonly FactionDef DeerFoxPlayerColonyDef = GetModDef<FactionDef>(InstelledMods.DeerFox, "PlayerColony_DeerFox");

		//public static readonly readonly Def CentaursScenarioRetroCruiseDef = GetModDef<EnemyShipDef>(InstelledMods.SoS2 && InstelledMods.RimCentaurs, "CentaursScenarioRetroCruise");

		//public static readonly readonly AbilityDef AbilityTrishot_TrishotDef = DefDatabase<AbilityDef>.GetNamed("AbilityTrishot_Trishot");
		//public static readonly readonly AbilityDef AbilityTrishot_IcoshotDef = DefDatabase<AbilityDef>.GetNamed("AbilityTrishot_Icoshot");
		//public static readonly readonly AbilityDef AbilityTrishot_OneshotDef = DefDatabase<AbilityDef>.GetNamed("AbilityTrishot_Oneshot");

		public static readonly Backstory CentaurCivilRetro = BackstoryDatabase.allBackstories.TryGetValue("CentaurCivil_Retro");
		public static readonly Backstory CentaurCivilMayinas = BackstoryDatabase.allBackstories.TryGetValue("Backstory_Mayinas_Exploriter");
		public static readonly Backstory DeerFoxStory = BackstoryDatabase.allBackstories.TryGetValue("DeerFox_Backstory");


	}

}
