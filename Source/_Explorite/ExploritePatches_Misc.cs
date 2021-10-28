/********************
 * 包含多个补丁的合集文件。
 * --siiftun1857
 */
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using static Explorite.ExploriteCore;
using static Verse.DamageInfo;
using static Verse.PawnCapacityUtility;

namespace Explorite
{
	///<summary>包含多个补丁的合集类。</summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage(null, "IDE0055")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(null, "IDE0058")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(null, "IDE0060")]
	[StaticConstructorOnStartup]
	internal static partial class ExploritePatches
	{
		internal static readonly Type patchType = typeof(ExploritePatches);
		static ExploritePatches()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			try
			{
				Patch(AccessTools.Method(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new[] { typeof(PawnGenerationRequest) }),
					postfix: new HarmonyMethod(patchType, nameof(GeneratePawnPostfix)));

				//Patch(AccessTools.Method(typeof(IdeoGenerator), nameof(IdeoGenerator.GenerateIdeo)),
				//	postfix: new HarmonyMethod(patchType, nameof(GenerateIdeoPostfix)));
				Patch(AccessTools.Method(typeof(IdeoFoundation), nameof(IdeoFoundation.RandomizePrecepts)),
					postfix: new HarmonyMethod(patchType, nameof(IdeoFoundationRandomizePreceptsPostfix)));
				Patch(AccessTools.Method(typeof(IdeoFoundation_Deity), "FillDeity"),
					postfix: new HarmonyMethod(patchType, nameof(IdeoFoundationDeityFillDeityPostfix)));
				Patch(AccessTools.Method(typeof(Precept_Role), nameof(Precept_Role.GenerateNewApparelRequirements)),
					prefix: new HarmonyMethod(patchType, nameof(PreceptRoleGenerateNewApparelRequirementsPrefix)));

				Patch(AccessTools.Method(typeof(Designation), "Notify_Removing"),
					postfix: new HarmonyMethod(patchType, nameof(DesignationNotifyRemovingPostfix)));


				Patch(AccessTools.Method(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.GetGizmos)),
					postfix: new HarmonyMethod(patchType, nameof(GetGizmosPostfix)));

				Patch(AccessTools.Method(typeof(Targeter), nameof(Targeter.BeginTargeting), new Type[] { typeof(ITargetingSource), typeof(ITargetingSource) }),
					prefix: new HarmonyMethod(patchType, nameof(BeginTargetingPrefix)));
				Patch(AccessTools.Method(typeof(Targeter), nameof(Targeter.OrderPawnForceTarget)),
					postfix: new HarmonyMethod(patchType, nameof(OrderPawnForceTargetPostfix)));
				Patch(AccessTools.Method(typeof(Targeter), nameof(Targeter.StopTargeting)),
					prefix: new HarmonyMethod(patchType, nameof(StopTargetingPrefix)));

				Patch(AccessTools.Method(typeof(VerbTracker), "CreateVerbTargetCommand"),
					postfix: new HarmonyMethod(patchType, nameof(CreateVerbTargetCommandPostfix)));
				Patch(AccessTools.Property(typeof(CompEquippable), nameof(CompEquippable.PrimaryVerb)).GetGetMethod(),
					prefix: new HarmonyMethod(patchType, nameof(PrimaryVerbPrefix)));

				Patch(AccessTools.Method(typeof(VerbProperties), "AdjustedAccuracy"),
					postfix: new HarmonyMethod(patchType, nameof(AdjustedAccuracyPostfix)));

				Patch(AccessTools.Method(typeof(TooltipUtility), nameof(TooltipUtility.ShotCalculationTipString)),
					transpiler: new HarmonyMethod(patchType, nameof(ShotCalculationTipStringTranspiler)));
				//Patch(AccessTools.Method(typeof(ShotReport), nameof(ShotReport.HitReportFor)),
				//	prefix: new HarmonyMethod(patchType, nameof(ShotReportHitReportForPrefix)));
				//Patch(AccessTools.Method(typeof(ShotReport), nameof(ShotReport.GetTextReadout)),
				//	prefix: new HarmonyMethod(patchType, nameof(ShotReportGetTextReadoutPrefix)));

				Patch(AccessTools.Method(typeof(Verb_LaunchProjectile), nameof(Verb_LaunchProjectile.HighlightFieldRadiusAroundTarget)),
					postfix: new HarmonyMethod(patchType, nameof(VerbLaunchProjectileHighlightFieldRadiusAroundTargetPostfix)));


				Patch(AccessTools.Method(typeof(PawnRenderer), nameof(PawnRenderer.DrawEquipmentAiming)),
					prefix: new HarmonyMethod(patchType, nameof(DrawEquipmentAimingPrefix)));
				Patch(AccessTools.Method(typeof(Thing), "get_DefaultGraphic"),
					postfix: new HarmonyMethod(patchType, nameof(get_Graphic_PostFix)));

				//Patch(AccessTools.Method(typeof(SkillRecord), nameof(SkillRecord.Learn)),
				//	prefix: nameof(SkillLearnPrefix));
				//	postfix: nameof(SkillIntervalPostfix));
				//	prefix: nameof(SkillRecordIntervalPrefix),
				Patch(AccessTools.Method(typeof(SkillRecord), nameof(SkillRecord.Interval)),
					transpiler: new HarmonyMethod(patchType, nameof(SkillRecordIntervalTranspiler)));
				Patch(AccessTools.Method(typeof(SkillRecord), "get_LearningSaturatedToday"),
					postfix: new HarmonyMethod(patchType, nameof(SkillRecordLearningSaturatedTodayPostfix)));

				//MISSING PATCHING TARGET
				//Patch(AccessTools.Method(typeof(Pawn_PsychicEntropyTracker), "get_PainMultiplier"),
				//	postfix: nameof(NoPainBounsForCentaursPostfix));
				Patch(AccessTools.Method(typeof(StatPart_Pain), nameof(StatPart_Pain.PainFactor)),
					postfix: new HarmonyMethod(patchType, nameof(StatPartPainPainFactorPostfix)));
				Patch(AccessTools.Method(typeof(StatPart_Pain), nameof(StatPart_Pain.ExplanationPart)),
					postfix: new HarmonyMethod(patchType, nameof(StatPartPainExplanationPartPostfix)));
				Patch(AccessTools.Method(typeof(PsychicEntropyGizmo), "TryGetPainMultiplier"),
					postfix: new HarmonyMethod(patchType, nameof(PsychicEntropyGizmoTryGetPainMultiplierPostfix)));

				Patch(AccessTools.Method(typeof(IncidentWorker_WandererJoin), "CanFireNowSub"),
					postfix: new HarmonyMethod(patchType, nameof(WandererJoinCannotFirePostfix)));

				Patch(AccessTools.Method(typeof(Need_Outdoors), "get_Disabled"),
					postfix: new HarmonyMethod(patchType, nameof(NeedOutdoors_DisabledPostfix)));
				Patch(AccessTools.Method(typeof(Need_Mood), nameof(Need_Mood.GetTipString)),
					postfix: new HarmonyMethod(patchType, nameof(NeedMood_GetTipStringPostfix)));
				Patch(AccessTools.Method(typeof(Need), nameof(Need.DrawOnGUI)),
					prefix: new HarmonyMethod(patchType, nameof(Need_DrawOnGUIPrefix)),
					postfix: new HarmonyMethod(patchType, nameof(Need_DrawOnGUIPostfix)));
				Patch(AccessTools.Method(typeof(NeedsCardUtility), "DrawThoughtListing"),
					prefix: new HarmonyMethod(patchType, nameof(NeedsCardUtilityDrawThoughtListingPrefix)));
				Patch(AccessTools.Method(typeof(Need_Seeker), nameof(Need_Seeker.NeedInterval)),
					prefix: new HarmonyMethod(patchType, nameof(Need_NeedIntervalPrefix)),
					postfix: new HarmonyMethod(patchType, nameof(Need_NeedIntervalPostfix)));
				Patch(AccessTools.Method(typeof(Need_Food), "get_MaxLevel"),
					postfix: new HarmonyMethod(patchType, nameof(NeedMaxLevelPostfix)));

				Patch(AccessTools.Method(typeof(ThoughtWorker_NeedComfort), "CurrentStateInternal"),
					postfix: new HarmonyMethod(patchType, nameof(ThoughtWorker_NeedComfort_CurrentStateInternalPostfix)));

				Patch(AccessTools.Method(typeof(MeditationFocusDef), nameof(MeditationFocusDef.CanPawnUse)),
					postfix: new HarmonyMethod(patchType, nameof(MeditationFocusCanPawnUsePostfix)));
				Patch(AccessTools.Method(typeof(MeditationFocusDef), nameof(MeditationFocusDef.EnablingThingsExplanation)),
					postfix: new HarmonyMethod(patchType, nameof(MeditationFocusExplanationPostfix)));
				Patch(AccessTools.Method(typeof(RecipeDef), "get_AvailableNow"),
					postfix: new HarmonyMethod(patchType, nameof(RecipeDefAvailableNowPostfix)));

				Patch(AccessTools.Method(typeof(MentalBreaker), "get_BreakThresholdMinor"),
					postfix: new HarmonyMethod(patchType, nameof(MentalBreaker_BreakThresholdPostfix)));
				Patch(AccessTools.Method(typeof(MentalBreaker), "get_BreakThresholdMajor"),
					postfix: new HarmonyMethod(patchType, nameof(MentalBreaker_BreakThresholdPostfix)));
				Patch(AccessTools.Method(typeof(MentalBreaker), "get_BreakThresholdExtreme"),
					postfix: new HarmonyMethod(patchType, nameof(MentalBreaker_BreakThresholdPostfix)));

				//Patch(AccessTools.Method(typeof(CompAssignableToPawn), "get_AssigningCandidates"),
				//	postfix: nameof(AssignToPawnCandidatesPostfix));
				Patch(AccessTools.Method(typeof(CompAssignableToPawn), "get_HasFreeSlot"),
					//postfix: nameof(AssignBedToPawnHasFreeSlotPostfix),
					transpiler: new HarmonyMethod(patchType, nameof(AssignBedToPawnHasFreeSlotTranspiler)));
				Patch(AccessTools.Method(typeof(CompAssignableToPawn_Bed), nameof(CompAssignableToPawn_Bed.TryAssignPawn)),
					postfix: new HarmonyMethod(patchType, nameof(AssignBedToPawnTryAssignPawnPostfix)));
				Patch(AccessTools.Method(typeof(CompAssignableToPawn_Bed), nameof(CompAssignableToPawn_Bed.CanAssignTo)),
					postfix: new HarmonyMethod(patchType, nameof(AssignBedToPawnCanAssignToPostfix)));
				Patch(AccessTools.Method(typeof(RestUtility), nameof(RestUtility.CanUseBedEver)),
					postfix: new HarmonyMethod(patchType, nameof(RestUtilityCanUseBedEverPostfix)));
				Patch(AccessTools.Method(typeof(BedUtility), nameof(BedUtility.WillingToShareBed)),
					postfix: new HarmonyMethod(patchType, nameof(BedUtilityWillingToShareBedPostfix)));

				Patch(AccessTools.Method(typeof(Plant), "get_GrowthRateFactor_Temperature"),
					postfix: new HarmonyMethod(patchType, nameof(PlantGrowthRateFactorNoTemperaturePostfix)));
				//Patch(AccessTools.Method(typeof(Plant), "get_Resting"),
				//	postfix: nameof(PlantNoRestingPostfix));
				//Patch(AccessTools.Method(typeof(Plant), "get_GrowthRate"),
				//	postfix: nameof(PlantGrowthRateFactorEnsurePostfix));
				//Patch(AccessTools.Method(typeof(Plant), "get_LeaflessNow"),
				//	postfix: nameof(PlantLeaflessNowPostfix));

				Patch(AccessTools.Method(typeof(MinifiedThing), nameof(MinifiedThing.Destroy)),
					prefix: new HarmonyMethod(patchType, nameof(MinifiedThingDestroyPrefix)));

				//Patch(AccessTools.Method(typeof(Alert_NeedBatteries), "NeedBatteries"),
				//	postfix: new HarmonyMethod(patchType, nameof(AlertNeedBatteriesPostfix)));

				Patch(AccessTools.Method(typeof(JobGiver_GetFood), "TryGiveJob"),
					postfix: new HarmonyMethod(patchType, nameof(GetFoodTryGiveJobPostfix)));

				Patch(AccessTools.Method(typeof(Pawn_HealthTracker), "get_InPainShock"),
					postfix: new HarmonyMethod(patchType, nameof(PawnHealthTrackerInPainShockPostfix)));

				Patch(AccessTools.Method(typeof(MassUtility), nameof(MassUtility.Capacity)),
					postfix: new HarmonyMethod(patchType, nameof(MassUtilityCapacityPostfix)));

				Patch(AccessTools.Method(typeof(HediffComp_GetsPermanent), "set_IsPermanent"),
					prefix: new HarmonyMethod(patchType, nameof(HediffComp_GetsPermanentIsPermanentPrefix)));
				Patch(AccessTools.Method(typeof(HediffComp_TendDuration), "get_AllowTend"),
					postfix: new HarmonyMethod(patchType, nameof(HediffComp_TendDurationAllowTendPostfix)));
				Patch(AccessTools.Method(typeof(HediffComp_ReactOnDamage), nameof(HediffComp_ReactOnDamage.Notify_PawnPostApplyDamage)),
					prefix: new HarmonyMethod(patchType, nameof(HediffComp_ReactOnDamageNotify_PawnPostApplyDamagePrefix)));

				Patch(AccessTools.Method(typeof(StatPart_ApparelStatOffset), nameof(StatPart.TransformValue)),
					postfix: new HarmonyMethod(patchType, nameof(PsychicSensitivityPostfix)));

				Patch(AccessTools.Method(typeof(PawnGraphicSet), nameof(PawnGraphicSet.ResolveAllGraphics)),
					postfix: new HarmonyMethod(patchType, nameof(PawnGraphicSetResolveAllGraphicsPostfix)));

				Patch(AccessTools.Method(typeof(CompAffectedByFacilities), nameof(CompAffectedByFacilities.CanPotentiallyLinkTo_Static), new Type[] { typeof(ThingDef), typeof(IntVec3), typeof(Rot4), typeof(ThingDef), typeof(IntVec3), typeof(Rot4) }),
					postfix: new HarmonyMethod(patchType, nameof(CompAffectedByFacilitiesCanPotentiallyLinkToStaticPostfix)));

				Patch(AccessTools.Method(typeof(PawnGraphicSet), nameof(PawnGraphicSet.ResolveApparelGraphics)),
					postfix: new HarmonyMethod(patchType, nameof(PawnGraphicSetResolveApparelGraphicsPostfix)));

				Patch(AccessTools.Method(typeof(TraitSet), nameof(TraitSet.DegreeOfTrait)),
					postfix: new HarmonyMethod(patchType, nameof(TraitSetDegreeOfTraitPostfix)));
				Patch(AccessTools.Method(typeof(TraitSet), nameof(TraitSet.HasTrait), new Type[] { typeof(TraitDef) }),
					postfix: new HarmonyMethod(patchType, nameof(TraitSetHasTraitPostfix)));
				Patch(AccessTools.Method(typeof(TraitSet), nameof(TraitSet.HasTrait), new Type[] { typeof(TraitDef), typeof(int) }),
					postfix: new HarmonyMethod(patchType, nameof(TraitSetHasTraitDegreePostfix)));
				Patch(AccessTools.Method(typeof(TraitSet), nameof(TraitSet.GetTrait), new Type[] { typeof(TraitDef) }),
					postfix: new HarmonyMethod(patchType, nameof(TraitSetGetTraitPostfix)));
				Patch(AccessTools.Method(typeof(TraitSet), nameof(TraitSet.GetTrait), new Type[] { typeof(TraitDef), typeof(int) }),
					postfix: new HarmonyMethod(patchType, nameof(TraitSetGetTraitDegreePostfix)));

				Patch(AccessTools.Method(typeof(RaceProperties), nameof(RaceProperties.SpecialDisplayStats)),
					postfix: new HarmonyMethod(patchType, nameof(RacePropertiesSpecialDisplayStatsPostfix)));

				Patch(AccessTools.Method(typeof(StatWorker), nameof(StatWorker.ShouldShowFor)),
					postfix: new HarmonyMethod(patchType, nameof(StatWorkerShouldShowForPostfix)));

				Patch(AccessTools.Method(AccessTools.TypeByName("RimWorld.Recipe_RemoveBodyPart"), "GetPartsToApplyOn"),
					postfix: new HarmonyMethod(patchType, nameof(RecipeRemoveBodyPartGetPartsToApplyOnPostfix)));

				Patch(AccessTools.Method(typeof(OutfitDatabase), "GenerateStartingOutfits"),
					postfix: new HarmonyMethod(patchType, nameof(OutfitDatabaseGenerateStartingOutfitsPostfix)));
				// 依赖 类 AlienRace.RaceRestrictionSettings
				Patch(AccessTools.Method(AccessTools.TypeByName("AlienRace.RaceRestrictionSettings"), "CanWear"),
					willResolve: InstelledMods.HAR,
					postfix: new HarmonyMethod(patchType, nameof(RaceRestrictionSettingsCanWearPostfix))); ;
				Patch(AccessTools.Method(typeof(ApparelProperties), nameof(ApparelProperties.GetCoveredOuterPartsString)),
					prefix: new HarmonyMethod(patchType, nameof(ApparelPropertiesGetCoveredOuterPartsStringPostfix)));

				//Patch(AccessTools.Method(typeof(Pawn_OutfitTracker), "get_CurrentOutfit"),
				//	postfix: nameof(PawnOutfitTrackerGetCurrentOutfitPostfix));

				//Patch(AccessTools.Method(typeof(GenHostility), nameof(GenHostility.HostileTo), new Type[] { typeof(Thing), typeof(Thing) }),
				//	postfix: nameof(GenHostilityHostileToPostfix));

				Patch(AccessTools.Method(typeof(Pawn), nameof(Pawn.ThreatDisabled)),
					postfix: new HarmonyMethod(patchType, nameof(PawnThreatDisabledPostfix)));

				Patch(AccessTools.Method(typeof(ThingMaker), nameof(ThingMaker.MakeThing)),
					prefix: new HarmonyMethod(patchType, nameof(ThingMakerMakeThingPrefix)),
					postfix: new HarmonyMethod(patchType, nameof(ThingMakerMakeThingPostfix)));

				//Patch(AccessTools.Method(typeof(GenRecipe), nameof(GenRecipe.MakeRecipeProducts)),
				//	prefix: nameof(GenRecipeMakeRecipeProductsPrefix),
				//	postfix: nameof(GenRecipeMakeRecipeProductsPostfix));
				Patch(AccessTools.Method(typeof(GenRecipe), "PostProcessProduct"),
					postfix: new HarmonyMethod(patchType, nameof(GenRecipePostProcessProductPostfix)));

				Patch(AccessTools.Method(typeof(RimWorld.Planet.FactionGiftUtility), "GetBaseGoodwillChange"),
					postfix: new HarmonyMethod(patchType, nameof(FactionGiftUtilityGetBaseGoodwillChangePostfix)));

				Patch(AccessTools.Method(typeof(InspirationHandler), nameof(InspirationHandler.GetRandomAvailableInspirationDef)),
					postfix: new HarmonyMethod(patchType, nameof(InspirationHandlerGetRandomAvailableInspirationDefPostfix)));

				Patch(AccessTools.Constructor(typeof(DamageInfo),
					parameters: new Type[] { typeof(DamageDef), typeof(float), typeof(float), typeof(float), typeof(Thing), typeof(BodyPartRecord), typeof(ThingDef), typeof(SourceCategory), typeof(Thing), typeof(bool), typeof(bool) }),
					prefix: new HarmonyMethod(patchType, nameof(DamageInfoCtorPrefix)));

				Patch(AccessTools.Constructor(typeof(BattleLogEntry_ExplosionImpact),
					parameters: new Type[] { typeof(Thing), typeof(Thing), typeof(ThingDef), typeof(ThingDef), typeof(DamageDef) }),
					prefix: new HarmonyMethod(patchType, nameof(BattleLogEntry_ExplosionImpactCtorPrefix)));

				//Patch(AccessTools.Method(typeof(Projectile), nameof(Projectile.Launch),
				//	parameters: new Type[] { typeof(Thing), typeof(Vector3), typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(ProjectileHitFlags), typeof(Thing), typeof(ThingDef)}),
				//	postfix: nameof(ProjectileLaunchPostfix));

				Patch(AccessTools.Method(typeof(HealthCardUtility), "GetListPriority"),
					postfix: new HarmonyMethod(patchType, nameof(HealthCardUtilityGetListPriorityPostfix)));

				Patch(AccessTools.Method(typeof(TendUtility), nameof(TendUtility.CalculateBaseTendQuality), new Type[] { typeof(Pawn), typeof(Pawn), typeof(float), typeof(float) }),
					prefix: new HarmonyMethod(patchType, nameof(TendUtilityCalculateBaseTendQualityPrefix)));

				Patch(AccessTools.Method(typeof(QuestPart_DropPods), "set_Things"),
					postfix: new HarmonyMethod(patchType, nameof(QuestPartDropPodsSetThingsPostfix)));

				Patch(AccessTools.Method(typeof(ResearchProjectDef), "get_CanStartNow"),
					postfix: new HarmonyMethod(patchType, nameof(ResearchProjectDefCanStartNowPostfix)));

				Patch(AccessTools.Method(typeof(Building_TurretGun), "get_IsMortarOrProjectileFliesOverhead"),
					postfix: new HarmonyMethod(patchType, nameof(BuildingTurretGunIsMortarOrProjectileFliesOverheadPostfix)));
				Patch(AccessTools.Method(typeof(Building_TurretGun), "get_CanSetForcedTarget"),
					postfix: new HarmonyMethod(patchType, nameof(BuildingTurretGunCanSetForcedTargetPostfix)));
				Patch(AccessTools.Method(typeof(Building_TurretGun), "TryStartShootSomething"),
					transpiler: new HarmonyMethod(patchType, nameof(BuildingTurretGunTryStartShootSomethingTranspiler)));

				//Patch(AccessTools.Method(typeof(PawnCapacitiesHandler), nameof(PawnCapacitiesHandler.GetLevel)),
				//	transpiler: nameof(PawnCapacitiesHandlerGetLevelTranspiler));

				Patch(AccessTools.Method(typeof(PawnCapacityUtility), nameof(PawnCapacityUtility.CalculateCapacityLevel)),
					prefix: new HarmonyMethod(patchType, nameof(PawnCapacityUtilityCalculateCapacityLevelPrefix)),
					postfix: new HarmonyMethod(patchType, nameof(PawnCapacityUtilityCalculateCapacityLevelPostfix)));

				Patch(AccessTools.Method(typeof(Projectile), "ImpactSomething"),
					transpiler: new HarmonyMethod(patchType, nameof(ProjectileImpactSomethingTranspiler)));

				Patch(AccessTools.Method(typeof(DamageWorker), nameof(DamageWorker.ExplosionCellsToHit), new Type[] { typeof(IntVec3), typeof(Map), typeof(float), typeof(IntVec3?), typeof(IntVec3?) }),
					transpiler: new HarmonyMethod(patchType, nameof(DamageWorkerExplosionCellsToHitTranspiler)));

				Patch(AccessTools.Method(typeof(IdeoUtility), nameof(IdeoUtility.IsMemeAllowedFor)),
					postfix: new HarmonyMethod(patchType, nameof(IdeoUtilityIsMemeAllowedForPostfix)));
				Patch(AccessTools.Method(typeof(IdeoUtility), "CanAdd"),
					postfix: new HarmonyMethod(patchType, nameof(IdeoUtilityCanAddPostfix)));

				Patch(AccessTools.Method(typeof(PawnWoundDrawer), nameof(PawnWoundDrawer.RenderOverBody)),
					prefix: new HarmonyMethod(patchType, nameof(PawnWoundDrawerRenderOverBodyPrefix)));

				Patch(AccessTools.Method(typeof(IdeoSymbolPartDef), nameof(IdeoSymbolPartDef.CanBeChosenForIdeo)),
					postfix: new HarmonyMethod(patchType, nameof(IdeoSymbolPartDefCanBeChosenForIdeoPostfix)));

				Patch(AccessTools.Method(typeof(ThoughtWorker_WearingColor), "CurrentStateInternal"),
					transpiler: new HarmonyMethod(patchType, nameof(ThoughtWorkerWearingColorCurrentStateInternalTranspilerB)));
				Patch(AccessTools.Method(typeof(Dialog_StylingStation), "DrawApparelColor"),
					transpiler: new HarmonyMethod(patchType, nameof(DialogStylingStationDrawApparelColorTranspiler)));


				Patch(AccessTools.Method(typeof(RitualBehaviorWorker_Conversion), nameof(RitualBehaviorWorker_Conversion.CanStartRitualNow)),
					transpiler: new HarmonyMethod(patchType, nameof(RitualBehaviorWorkerCanStartRitualNowTranspiler_Conversion)));
				Patch(AccessTools.Method(typeof(RitualBehaviorWorker_Speech), nameof(RitualBehaviorWorker_Speech.CanStartRitualNow)),
					transpiler: new HarmonyMethod(patchType, nameof(RitualBehaviorWorkerCanStartRitualNowTranspiler_Speech)));

				Patch(AccessTools.Method(typeof(IdeoFoundation), nameof(IdeoFoundation.RandomizeCulture)),
					transpiler: new HarmonyMethod(patchType, nameof(IdeoFoundationRandomizeCultureTranspiler)));

				Patch(AccessTools.Method(typeof(Page_ConfigureIdeo), nameof(Page_ConfigureIdeo.PostOpen)),
					transpiler: new HarmonyMethod(patchType, nameof(PageConfigureIdeoPostOpenTranspiler)));
				Patch(AccessTools.Method(typeof(Page_ConfigureIdeo), "CanDoNext"),
					transpiler: new HarmonyMethod(patchType, nameof(PageConfigureIdeoCanDoNextTranspiler)));
				Patch(AccessTools.Constructor(typeof(Dialog_ChooseMemes), new Type[] { typeof(Ideo), typeof(MemeCategory), typeof(bool), typeof(Action), typeof(List<MemeDef>), typeof(bool) }),
					transpiler: new HarmonyMethod(patchType, nameof(DialogChooseMemesAnyMemeTranspiler)));
				Patch(AccessTools.Method(typeof(Dialog_ChooseMemes), "DoAcceptChanges"),
					transpiler: new HarmonyMethod(patchType, nameof(DialogChooseMemesAnyMemeTranspiler)));
				Patch(AccessTools.Method(typeof(Page_ConfigureIdeo), nameof(Page_ConfigureIdeo.Notify_ClosedChooseMemesDialog)),
					transpiler: new HarmonyMethod(patchType, nameof(DialogChooseMemesAnyMemeTranspiler)));
				Patch(AccessTools.Method(typeof(Dialog_ChooseMemes), "CanUseMeme"),
					transpiler: new HarmonyMethod(patchType, nameof(DialogChooseMemesPlayerNHiddenOffTranspiler)));
				Patch(AccessTools.Method(typeof(Dialog_ChooseMemes), "CanRemoveMeme"),
					transpiler: new HarmonyMethod(patchType, nameof(DialogChooseMemesPlayerNHiddenOffTranspiler)));
				//Patch(AccessTools.Method(typeof(Dialog_ChooseMemes), "TryAccept"),
				//	transpiler: new HarmonyMethod(patchType, nameof(DialogChooseMemesSpecStructCountinTranspiler)));
				//Patch(AccessTools.Method(typeof(Dialog_ChooseMemes), nameof(Dialog_ChooseMemes.DoWindowContents)),
				//	transpiler: new HarmonyMethod(patchType, nameof(DialogChooseMemesSpecStructCountinTranspiler)));
				Patch(AccessTools.Method(typeof(Dialog_ChooseMemes), "GetMemeCount"),
					postfix: new HarmonyMethod(patchType, nameof(DialogChooseMemesGetMemeCountPostfix)));

				Patch(AccessTools.Method(typeof(IdeoUIUtility), nameof(IdeoUIUtility.FactionForRandomization)),
					transpiler: new HarmonyMethod(patchType, nameof(IdeoUIUtilityFactionForRandomizationTranspiler)));

				//Patch(AccessTools.Method(typeof(IdeoUIUtility), "DoName"),
				//	transpiler: new HarmonyMethod(patchType, nameof(StrangeMethodFinderTranspiler)));
				//Patch(strangeMethod,
				//	transpiler: new HarmonyMethod(patchType, nameof(IdeoUIUtility_MT_LT_c__DisplayClass59_0__MT_DoName_LT_g____CultureAllowed_1_Transpiler)));

				//Patch(AccessTools.Method(AccessTools.TypeByName("RimWorld.IdeoUIUtility.<>c__DisplayClass59_0"), "<DoName>g__CultureAllowed|1"),
				//	transpiler: nameof(PrinterTranspiler));


				Patch(AccessTools.Method(typeof(StatWorker), nameof(StatWorker.GetValueUnfinalized)),
					transpiler: new HarmonyMethod(patchType, nameof(StatWorkerGetValueUnfinalizedTranspiler)));
				Patch(AccessTools.Method(typeof(StatWorker), nameof(StatWorker.GetExplanationUnfinalized)),
					transpiler: new HarmonyMethod(patchType, nameof(StatWorkerGetExplanationUnfinalizedTranspiler)));

				Patch(AccessTools.Method(typeof(ThoughtWorker_Precept_GroinUncovered), nameof(ThoughtWorker_Precept_GroinUncovered.HasUncoveredGroin)),
					prefix: new HarmonyMethod(patchType, nameof(ThoughtWorkerPreceptHasUncoveredPrefix)));
				Patch(AccessTools.Method(typeof(ThoughtWorker_Precept_GroinOrChestUncovered), nameof(ThoughtWorker_Precept_GroinOrChestUncovered.HasUncoveredGroinOrChest)),
					prefix: new HarmonyMethod(patchType, nameof(ThoughtWorkerPreceptHasUncoveredPrefix)));

				Patch(AccessTools.Method(typeof(RoyalTitleDef), nameof(RoyalTitleDef.GetBedroomRequirements)),
					postfix: new HarmonyMethod(patchType, nameof(RoyalTitleDefGetBedroomRequirementsPostfix)));
				Patch(AccessTools.Method(typeof(ApparelRequirement), nameof(ApparelRequirement.IsMet)),
					postfix: new HarmonyMethod(patchType, nameof(ApparelRequirementIsMetPostfix)));
				Patch(AccessTools.Method(typeof(Ideo), "get_ApparelColor"),
					prefix: new HarmonyMethod(patchType, nameof(IdeoApparelColorPrefix)));

				Patch(AccessTools.Method(typeof(Page_ChooseIdeoPreset), "PostOpen"),
					transpiler: new HarmonyMethod(patchType, nameof(PageChooseIdeoPresetPostOpenTranspiler)));

				Patch(AccessTools.Method(typeof(GenLabel), "NewThingLabel", new Type[] { typeof(Thing), typeof(int), typeof(bool) }),
					transpiler: new HarmonyMethod(patchType, nameof(GenLabelNewThingLabelTranspiler)));
				Patch(AccessTools.Method(typeof(GenLabel), "ThingLabel", new Type[] { typeof(Thing), typeof(int), typeof(bool) }),
					transpiler: new HarmonyMethod(patchType, nameof(GenLabelThingLabelTranspiler)));

				Patch(AccessTools.Method(typeof(WorkGiver_ExtractSkull), nameof(WorkGiver_ExtractSkull.CanExtractSkull)),
					postfix: new HarmonyMethod(patchType, nameof(WorkGiverExtractSkullCanExtractSkullPostfix)));

				Patch(AccessTools.Method(typeof(FoodUtility), nameof(FoodUtility.GetFoodPoisonChanceFactor)),
					prefix: new HarmonyMethod(patchType, nameof(FoodUtilityGetFoodPoisonChanceFactorPrefix)));

				Patch(AccessTools.Method(typeof(SkillUI), nameof(SkillUI.DrawSkill), new Type[] { typeof(SkillRecord), typeof(Rect), typeof(SkillUI.SkillDrawMode), typeof(string) }),
					transpiler: new HarmonyMethod(patchType, nameof(SkillUIDrawSkillTranspiler)));

				Patch(AccessTools.Method(typeof(CompNeuralSupercharger), nameof(CompNeuralSupercharger.CanAutoUse)),
					transpiler: new HarmonyMethod(patchType, nameof(CompNeuralSuperchargerCanAutoUseTranspiler)));

				Patch(AccessTools.Method(typeof(Hediff_Psylink), nameof(Hediff_Psylink.TryGiveAbilityOfLevel)),
					transpiler: new HarmonyMethod(patchType, nameof(HediffPsylinkTryGiveAbilityOfLevelTranspiler)));

				Patch(AccessTools.Method(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.SocialFightPossible)),
					prefix: new HarmonyMethod(patchType, nameof(PawnInteractionsTrackerSocialFightPossiblePrefix)));

				Patch(AccessTools.Method(typeof(StatPart_AddedBodyPartsMass), "GetAddedBodyPartsMass"),
					prefix: new HarmonyMethod(patchType, nameof(StatPartAddedBodyPartsMassGetAddedBodyPartsMassPrefix)));

				Patch(AccessTools.Method(typeof(QualityUtility), nameof(QualityUtility.GenerateQualityCreatedByPawn)),
					postfix: new HarmonyMethod(patchType, nameof(QualityUtilityGenerateQualityCreatedByPawnPostfix)));


				// 依赖 类 AlienRace.RaceRestrictionSettings
				//Patch(AccessTools.Method(AccessTools.TypeByName("AlienRace.HarmonyPatches"), "ChooseStyleItemPrefix"),
				//	willResolve: InstelledMods.HAR,
				//	transpiler: nameof(AlienRaceHarmonyPatchesChooseStyleItemPrefix));

				// 依赖 类 SaveOurShip2.ShipInteriorMod2
				Patch(AccessTools.Method(AccessTools.TypeByName("SaveOurShip2.ShipInteriorMod2"), "HasSpaceSuitSlow", new[] { typeof(Pawn) }),
					willResolve: InstelledMods.SoS2,
					postfix: new HarmonyMethod(patchType, nameof(HasSpaceSuitSlowPostfix)));

				// 依赖 类 RimWorld.ThoughtWorker_SpaceThoughts
				Patch(AccessTools.Method(AccessTools.TypeByName("RimWorld.ThoughtWorker_SpaceThoughts"), "CurrentStateInternal"),
					willResolve: InstelledMods.SoS2,
					postfix: new HarmonyMethod(patchType, nameof(ThoughtWorker_SpaceThoughts_CurrentStateInternalPostfix)));

			}
			catch (Exception e)
			{
				ExplortiePatchActionRecord last = records.Last();
				Log.Error(string.Concat(
					$"[Explorite]Patch sequence crashed, an exception ({e.GetType().Name}) occurred. Last patch was ",
					last,
					$"\n",
					$"Message:\n   {e.Message}\n",
					$"Stack Trace:\n{e.StackTrace}\n"
					));
			}
			stopwatch.Stop();
			Log.Message($"[Explorite]Patch sequence complete, solved total {records.Count()} patches, " +
				$"{records.Count(d => d.state == ExplortiePatchActionRecordState.Success)} success, " +
				$"{records.Count(d => d.state == ExplortiePatchActionRecordState.Failed)} failed, " +
				$"{records.Count(d => d.state == ExplortiePatchActionRecordState.Unresolved)} unresolved, " +
				$"in {stopwatch.ElapsedMilliseconds}ms.\n" +
				$"Printing patch records below...\n" +
				PrintPatches());
			Log.Message($"[Explorite]All patch targets:\n" +
				string.Join("\n", records.Where(s => s.original != null).OrderBy(s => s.SortValue).Select(r => r.original?.FullDescription()).Distinct())
				);
		}
		public static bool DisableFoodPoisoningPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisableFoodPoisoning") ?? false;
		}
		public static bool DisableSkillsDegreesPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisableSkillsDegrees") ?? false;
		}
		public static bool DisableMentalBreakPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisableMentalBreak") ?? false;
		}
		public static bool DisablePainShockPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisablePainShock") ?? false;
		}
		public static bool DisableScarPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisableScar") ?? false;
		}
		public static bool DisableBrainShockPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisableBrainShock") ?? false;
		}
		public static bool DisableOutdoorsNeedsPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisableOutdoorsNeeds") ?? false;
		}
		public static bool DisableDailySkillTrainLimitPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisableDailySkillTrainLimit") ?? false;
		}
		public static bool DisableEntropyPainBoostPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisableEntropyPainBoost") ?? false;
		}
		public static bool DisableSelfHealPenaltyPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisableSelfHealPenalty") ?? false;
		}
		public static bool DisableWoundRenderingPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisableWoundRendering") ?? false;
		}
		public static bool DisableSocialFightPossiblePredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisableSocialFightPossible") ?? false;
		}
		public static bool DisableAbilityGainFromPsylinkPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisableAbilityGainFromPsylink") ?? false;
		}
		public static bool DisablePassionDrawPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisablePassionDraw") ?? false;
		}
		public static bool DisableThoughtWorkerPreceptHasUncoveredPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisableThoughtWorkerPreceptHasUncovered") ?? false;
		}
		public static bool DisableWaistRenderingPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisableWaistRendering") ?? false;
		}
		public static bool OverrideAddedBodyPartsMassPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExOverrideAddedBodyPartsMass") ?? false;
		}
		public static bool QualityIncreasedPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExQualityIncreased") ?? false;
		}

		/*
		///<summary>阻止半人马的技能衰退。</summary>
		[HarmonyPrefix]public static void SkillLearnPrefix(SkillRecord __instance, ref float xp)
		{
			if (
				__instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
				&& pawn.def == AlienCentaurDef
				)
			{
				xp = Math.Max(0, xp);
				__instance.xpSinceMidnight = 0f;
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
		*/
		/*
		///<summary>阻止半人马的技能衰退。</summary>
		[HarmonyPrefix]public static bool SkillRecordIntervalPrefix(SkillRecord __instance)
		{
			if (
				__instance.GetType().GetField("pawn", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
				&& pawn.def == AlienCentaurDef
				)
			{
				return false;
			}
			return true;
		}
		*/
		///<summary>阻止半人马的技能衰退。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> SkillRecordIntervalTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			FieldInfo pawnInfo = typeof(SkillRecord).GetField("pawn", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			FieldInfo thingDefInfo = typeof(Thing).GetField("def", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			Label label1 = ilg.DefineLabel();
			Label label2 = ilg.DefineLabel();
			byte patchActionStage = 0;

			yield return new CodeInstruction(OpCodes.Ldarg_0);
			yield return new CodeInstruction(OpCodes.Ldfld, pawnInfo);
			yield return new CodeInstruction(OpCodes.Ldfld, thingDefInfo);
			yield return new CodeInstruction(OpCodes.Call, ((Predicate<ThingDef>)(DisableSkillsDegreesPredicate)).Method);
			yield return new CodeInstruction(OpCodes.Brfalse_S, label1);
			//yield return new CodeInstruction(OpCodes.Ldsfld, typeof(ExploriteCore).GetField(nameof(AlienCentaurDef), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static));
			//yield return new CodeInstruction(OpCodes.Bne_Un, label1);
			yield return new CodeInstruction(OpCodes.Ldc_R4, 0f);
			yield return new CodeInstruction(OpCodes.Br_S, label2);
			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0)
				{
					patchActionStage++;
					ins.labels.Add(label1);
					yield return ins;
				}
				else if (patchActionStage == 1 && ins.opcode == OpCodes.Stloc_0)
				{
					patchActionStage++;
					ins.labels.Add(label2);
					yield return ins;
				}
				else
				{
					yield return ins;
				}
				continue;
			}
			TranspilerStageCheckout(patchActionStage, 2);
			yield break;
		}
		///<summary>移除半人马每日技能训练上限。</summary>
		[HarmonyPostfix]public static void SkillRecordLearningSaturatedTodayPostfix(SkillRecord __instance, ref bool __result)
		{
			if (__result &&
				__instance.GetType().GetField("pawn", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
				&& DisableDailySkillTrainLimitPredicate(pawn.def)
				)
			{
				__result = false;
			}
		}

		/*
		///<summary>移除半人马的疼痛带来心灵熵消散增益。</summary>
		[HarmonyPostfix]public static void NoPainBounsForCentaursPostfix(Pawn_PsychicEntropyTracker __instance, ref float __result)
		{
			if (__instance.Pawn.def == AlienCentaurDef)
				__result = 1f;
		}
		*/
		///<summary>移除半人马的疼痛带来心灵熵消散增益。</summary>
		[HarmonyPostfix]public static void StatPartPainPainFactorPostfix(Pawn pawn, ref float __result)
		{
			if (DisableEntropyPainBoostPredicate(pawn.def))
				__result = 1f;
		}
		///<summary>移除半人马的疼痛带来心灵熵消散增益的描述文本。</summary>
		[HarmonyPostfix]public static void StatPartPainExplanationPartPostfix(StatRequest req, ref string __result)
		{
			if (req.HasThing && req.Thing is Pawn pawn && DisableEntropyPainBoostPredicate(pawn.def))
				__result = null;
		}
		///<summary>移除半人马的心灵熵Gizno的疼痛激励显示。</summary>
		[HarmonyPostfix]public static void PsychicEntropyGizmoTryGetPainMultiplierPostfix(PsychicEntropyGizmo __instance, ref float painMultiplier, ref bool __result)
		{
			if (__instance.GetType().GetField("tracker", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn_PsychicEntropyTracker tracker &&
				DisableEntropyPainBoostPredicate(tracker.Pawn.def))
			{
				painMultiplier = 1f;
				__result = false;
			}
		}

		///<summary>禁用阵营生成流浪者加入事件。</summary>
		[HarmonyPostfix]public static void WandererJoinCannotFirePostfix(IncidentParms parms, ref bool __result)
		{
			/*
			if (Faction.OfPlayer.def == CentaurPlayerColonyDef
			 || Faction.OfPlayer.def == SayersPlayerColonyDef
			 || Faction.OfPlayer.def == SayersPlayerColonySingleDef
			 || Faction.OfPlayer.def == GuoguoPlayerColonyDef
				)
				__result = false;
			*/
		}

		///<summary>使半人马可以在太空中靠动力装甲存活。</summary>
		[HarmonyPostfix]public static void HasSpaceSuitSlowPostfix(Pawn pawn, ref bool __result)
		{
			if (pawn.def == AlienCentaurDef)
			{
				foreach (Apparel app in pawn.apparel.WornApparel)
				{
					if (app.def.apparel.tags.Contains("CentaurEVA"))
					{
						__result = true;
						break;
					}
				}
			}
		}

		///<summary>移除半人马和Sayers的户外需求。</summary>
		[HarmonyPostfix]public static void NeedOutdoors_DisabledPostfix(Need_Outdoors __instance, ref bool __result)
		{
			if (
				__instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
				&& DisableOutdoorsNeedsPredicate(pawn.def)
				)
			{
				__result = true;
			}
		}

		///<summary>移除半人马的心情需求显示精神崩溃阈值描述。</summary>
		[HarmonyPostfix]public static void NeedMood_GetTipStringPostfix(Need_Mood __instance, ref string __result)
		{
			if (
				__instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
				&& DisableMentalBreakPredicate(pawn.def)
				)
			{
				__result = __instance.LabelCap + ": " + __instance.CurLevelPercentage.ToStringPercent() + "\n" + __instance.def.description;
			}
		}

		///<summary>更改渲染想法预补丁。</summary>
		[HarmonyPrefix]public static void NeedsCardUtilityDrawThoughtListingPrefix(ref Rect listingRect, Pawn pawn, ref Vector2 thoughtScrollPosition)
		{
			//改变半人马的想法显示渲染高度
			if (pawn.def == AlienCentaurDef)
			{
				if (pawn.needs.TryGetNeed<Need_CentaurCreativityInspiration>()?.ShouldShow == true)
				{
					listingRect.y += 24f;
					thoughtScrollPosition.y += 24f;
				}
			}
		}
		///<summary>更改渲染模式预补丁。</summary>
		[HarmonyPrefix]public static void Need_DrawOnGUIPrefix(Need __instance, ref Rect rect, ref int maxThresholdMarkers, ref float customMargin, ref bool drawArrows, ref bool doTooltip)
		{
			if (
				__instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
				// && __instance.GetType().GetField("threshPercents", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is List<float> threshPercents
				)
			{
				//改变半人马的心情需求显示渲染高度
				if (__instance is Need_Mood && pawn.def == AlienCentaurDef && pawn.needs.TryGetNeed<Need_CentaurCreativityInspiration>()?.ShouldShow == true)
				{
					//rect.height = Mathf.Max(rect.height * 0.666f, 30f);
				}
			}
		}
		///<summary>更改渲染模式后期处理补丁。</summary>
		[HarmonyPostfix]public static void Need_DrawOnGUIPostfix(Need __instance, Rect rect, int maxThresholdMarkers, float customMargin, bool drawArrows, bool doTooltip)
		{
			if (
				__instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn &&
				__instance.GetType().GetField("threshPercents", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is List<float> threshPercents
				)
			{
				//移除半人马的心情需求显示精神崩溃阈值分隔符
				if (__instance is Need_Mood && DisableMentalBreakPredicate(pawn.def))
				{
					threshPercents.Clear();

					Need_CentaurCreativityInspiration need = pawn.needs.TryGetNeed<Need_CentaurCreativityInspiration>();
					if (need?.ShouldShow == true)
					{
						Rect rect2 = new Rect(rect);
						//rect2.height *= 0.666f;
						rect2.height *= 0.5f;
						rect2.y += rect.height;
						need.DrawOnGUI(rect2, maxThresholdMarkers, customMargin, drawArrows, doTooltip);
					}
				}
				//移除半人马和Sayers的舒适需求显示第一个分隔符
				if (__instance is Need_Comfort && (pawn.def == AlienSayersDef/* || pawn.def == AlienCentaurDef*/))
				{
					threshPercents.RemoveAll(num => num < 0.5f);
				}
				//移除Sayers的美观度需求显示第二个分隔符
				if (__instance is Need_Beauty && pawn.def == AlienSayersDef)
				{
					threshPercents.RemoveAll(num => num > 0.3f && num < 0.4f);
				}
			}
		}

		///<summary>记录在刻函数开始前的需求值。</summary>
		[HarmonyPrefix]public static void Need_NeedIntervalPrefix(Need_Seeker __instance, ref float __state)
		{
			__state = __instance.CurLevel;
		}

		///<summary>对需求刻函数进行后期处理。</summary>
		[HarmonyPostfix]public static void Need_NeedIntervalPostfix(Need_Seeker __instance, float __state)
		{
			if (
				__instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
				)
			{
				__state -= __instance.CurLevel;

				if (__instance is Need_Mood && pawn.def == AlienCentaurDef)
				{
					if (!(bool)AccessTools.Method(typeof(Need), "get_IsFrozen").Invoke(__instance, new object[] { }))
					{
						/*
						float curInstantLevel = __instance.CurInstantLevel;
						if (curInstantLevel > __instance.CurLevel)
						{
							//__instance.CurLevel += __instance.def.seekerRisePerHour * 0.06f;
							__instance.CurLevel = ((__instance.CurInstantLevel * 20) + __instance.CurLevel)/21;
							__instance.CurLevel = Mathf.Min(__instance.CurLevel, curInstantLevel);
						}
						if (curInstantLevel < __instance.CurLevel)
						{
							//__instance.CurLevel -= __instance.def.seekerFallPerHour * 0.06f;
							__instance.CurLevel = ((__instance.CurInstantLevel * 20) + __instance.CurLevel) / 21;
							__instance.CurLevel = Mathf.Max(__instance.CurLevel, curInstantLevel);
						}
						*/
						if (__instance.CurLevel == __instance.MaxLevel)
						{
							_ = (__instance.def.seekerRisePerHour * 0.06f) - __state;
						}
					}
				}
			}
		}
		///<summary>增强半人马食物需求储备量。</summary>
		[HarmonyPostfix]public static void NeedMaxLevelPostfix(Need_Food __instance, ref float __result)
		{
			if (
				__instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
				)
			{
				if ((pawn?.health?.hediffSet?.GetFirstHediffOfDef(HediffCentaurSubsystem_NeedsCapacitor_Def)).SubsystemEnabled())
				{
					__result *= 10f / 3f;
				}
			}
		}

		///<summary>移除半人马舒适度需求的负面情绪。</summary>
		[HarmonyPostfix]public static void ThoughtWorker_NeedComfort_CurrentStateInternalPostfix(ThoughtWorker_NeedComfort __instance, ref ThoughtState __result, Pawn p)
		{
			if (false &&
				p.def == AlienCentaurDef && __result.Active && __result.StageIndex == 0
				)
			{
				__result = ThoughtState.Inactive;
			}
		}

		///<summary>移除半人马在SoS2太空中的负面情绪。</summary>
		[HarmonyPostfix]public static void ThoughtWorker_SpaceThoughts_CurrentStateInternalPostfix(ThoughtWorker __instance, ref ThoughtState __result, Pawn p)
		{
			if (
				p.def == AlienCentaurDef && __result.Active &&
				__instance.def.stages.ToArray()[__result.StageIndex].baseMoodEffect < 0f
				//(__result.StageIndex == 1 || __result.StageIndex == 3)
				)
			{
				//__result = ThoughtState.ActiveAtStage(__result.StageIndex - 1);
				__result = ThoughtState.Inactive;
			}
		}

		///<summary>移除半人马精神崩溃阈值。</summary>
		[HarmonyPostfix]public static void MentalBreaker_BreakThresholdPostfix(MentalBreaker __instance, ref float __result)
		{
			if (
				__instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
				&& DisableMentalBreakPredicate(pawn.def)
				)
			{
				__result = Math.Min(__result, -0.15f);
			}
		}

		///<summary>使心灵敏感度属性受到心灵失聪hediff影响。</summary>
		[HarmonyPostfix]public static void PsychicSensitivityPostfix(StatPart __instance, StatRequest req, ref float val)
		{
			try
			{
				if (__instance.parentStat == StatDefOf.PsychicSensitivity &&
					req.HasThing && (((Pawn)req.Thing)?.health?.hediffSet?.HasHediff(PsychicDeafHediffDef)) == true)
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

		///<summary>使半人马和Sayers可以使用额外类型的冥想媒介。</summary>
		[HarmonyPostfix]public static void MeditationFocusCanPawnUsePostfix(MeditationFocusDef __instance, ref bool __result, Pawn p)
		{
			if ((p.def == AlienCentaurDef || p.def == AlienSayersDef || p.def == AlienGuoguoDef
				) && (__instance == DefDatabase<MeditationFocusDef>.GetNamed("Natural")     //自然
					|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Artistic")   //艺术
					|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Dignified")  //庄严
					|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Morbid")     //病态
					//|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Minimal")  //简约
					//|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Flame")    //火焰
					)
				)
			{
				__result = true;
			}
		}

		///<summary>更改特定配方的显示可用性。</summary>
		[HarmonyPostfix]public static void RecipeDefAvailableNowPostfix(RecipeDef __instance, ref bool __result)
		{
			/*if (__instance.defName == "InstallHyperManipulatorSurgery")
			{
				__result = false;
			}*/
		}

		///<summary>为半人马和Sayers补充启用冥想类型的原因。</summary>
		[HarmonyPostfix]public static void MeditationFocusExplanationPostfix(MeditationFocusDef __instance, ref string __result, Pawn pawn)
		{
			if ((pawn.def == AlienCentaurDef || pawn.def == AlienSayersDef || pawn.def == AlienGuoguoDef
				) && (__instance == DefDatabase<MeditationFocusDef>.GetNamed("Natural")     //自然
					|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Artistic")   //艺术
					|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Dignified")  //庄严
					|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Morbid")     //病态
					//|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Minimal")  //简约
					//|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Flame")    //火焰
					)
				)
			{
				//__result = $"  - {"MeditationFocusEnabledByExploriteRace".Translate(pawn.def.label)}"
				__result = $"  - {"Race".Translate()}: {pawn.def.label}"
					//+ (__result.Length > 0 ? "\n" + __result : null)
					;
			}
		}
		/*
		///<summary>对绑定选单进行补丁。</summary>
		[HarmonyPostfix]public static void AssignToPawnCandidatesPostfix(CompAssignableToPawn __instance, ref IEnumerable<Pawn> __result)
		{
			//从单人床选单中移除半人马。
			if (__instance is CompAssignableToPawn_Bed comp
			 && !(comp.props is CompProperties_AssignableToPawn_NoPostLoadSpecial)
			 && __instance?.Props?.maxAssignedPawnsCount < 2)
			{
				__result = __result?.Where(pawn => pawn?.def != AlienCentaurDef);
			}
			//向王座选单中加入半人马。
			/ * if (__instance is CompAssignableToPawn_Throne
				&& __instance.parent.Spawned)
			{
				List<Pawn> result = __result.ToList();
				foreach (Pawn pawn in __instance.parent.Map.mapPawns.FreeColonists.Where(pawn => pawn?.def == AlienCentaurDef))
				{
					if (!result.Contains(pawn))
						result.Add(pawn);
				}
				__result = result;
			} * /
		}

		///<summary>使半人马占满床位。</summary>
		[HarmonyPostfix]public static void AssignBedToPawnHasFreeSlotPostfix(CompAssignableToPawn __instance, ref bool __result)
		{
			if (__result &&
				__instance is CompAssignableToPawn_Bed)
			{
				//__result = __instance.AssignedPawns.Count() + __instance.AssignedPawns.Where(pawn => pawn.def == AlienCentaurDef).Count() < __instance.Props.maxAssignedPawnsCount;
				__result = (!__instance?.AssignedPawns?.Any(pawn => pawn.def == AlienCentaurDef)) ?? true;
			}
		}
		*/
		public static int CentaursInList(CompAssignableToPawn instance)
		{
			if (instance is CompAssignableToPawn_Bed)
			{
				return instance.AssignedPawns.Where(p => p.def == AlienCentaurDef).Count();
			}
			return 0;
		}
		///<summary>使半人马占用更多床位。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> AssignBedToPawnHasFreeSlotTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			byte patchActionStage = 0;
			MethodInfo PawnsCountInfo = AccessTools.Method(typeof(List<Pawn>), "get_Count");
			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0 && ins.opcode == OpCodes.Callvirt && ins.operand == PawnsCountInfo as object)
				{
					patchActionStage++;
					yield return ins;
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, ((Func<CompAssignableToPawn, int>)CentaursInList).GetMethodInfo());
					yield return new CodeInstruction(OpCodes.Add);
					continue;
				}
				else
				{
					yield return ins;
					continue;
				}
			}
			TranspilerStageCheckout(patchActionStage, 1);
			yield break;
		}
		///<summary>使半人马与他人同时被添加至同一个床时被移除绑定。</summary>
		[HarmonyPostfix]public static void AssignBedToPawnTryAssignPawnPostfix(CompAssignableToPawn __instance, Pawn pawn)
		{
			if (pawn?.def == AlienCentaurDef)
			{
				foreach (Pawn one_pawn in __instance.AssignedPawns.Where(one_pawn => one_pawn != pawn).ToList())
				{
					__instance?.TryUnassignPawn(one_pawn);
				}
			}
			else
			{
				foreach (Pawn one_pawn in __instance.AssignedPawns.Where(one_pawn => one_pawn.def == AlienCentaurDef).ToList())
				{
					__instance?.TryUnassignPawn(one_pawn);
				}

			}
		}
		///<summary>使半人马不能使用小尺寸床铺。</summary>
		[HarmonyPostfix]public static void AssignBedToPawnCanAssignToPostfix(CompAssignableToPawn __instance, ref AcceptanceReport __result, Pawn pawn)
		{
			if (pawn.def == AlienCentaurDef && __instance.parent.def.Size.Area < 3)
			{
				__result = "TooLargeForBed".Translate();
			}
		}
		///<summary>使半人马不能使用小尺寸床铺。</summary>
		[HarmonyPostfix]public static void RestUtilityCanUseBedEverPostfix(ref bool __result, Pawn p, ThingDef bedDef)
		{
			if (p.def == AlienCentaurDef && bedDef.Size.Area < 3)
			{
				__result = false;
			}
		}
		///<summary>使半人马拒绝与他人共用床铺，使Sayers拒绝与非同族共用床铺。</summary>
		[HarmonyPostfix]public static void BedUtilityWillingToShareBedPostfix(ref bool __result, Pawn pawn1, Pawn pawn2)
		{
			if (pawn1 != pawn2 && (pawn1.def == AlienCentaurDef || pawn2.def == AlienCentaurDef))
			{
				__result = false;
			}
			if (pawn1.def == AlienSayersDef ^ pawn2.def == AlienSayersDef)
			{
				__result = false;
			}
		}

		///<summary>使指定植物的生长无视环境温度。</summary>
		[HarmonyPostfix]public static void PlantGrowthRateFactorNoTemperaturePostfix(Plant __instance, ref float __result)
		{
			if (__instance is Plant_FleshTree)
			{
				__result = 1f;
			}
		}
		/*
		///<summary>使血肉树不会进入休眠状态。</summary>
		[HarmonyPostfix]public static void PlantNoRestingPostfix(Plant __instance, ref bool __result)
		{
			if (__instance is Plant_FleshTree)
			{
				__result = false;
			}
		}
		
		///<summary>使血肉树的生长至少具有100%速率。</summary>
		[HarmonyPostfix]public static void PlantGrowthRateFactorEnsurePostfix(Plant __instance, ref float __result)
		{
			if (__instance is Plant_FleshTree
				&& __result < 1f)
			{
				__result = 1f;
			}
		}

		///<summary>使血肉树的叶子在成熟前不会出现。</summary>
		[HarmonyPostfix]public static void PlantLeaflessNowPostfix(Plant __instance, ref bool __result)
		{
			if (__instance is Plant_FleshTree)
			{
				__result = !__instance.CanYieldNow();
			}
		}
		*/
		///<summary>使被收纳的秘密三射弓物体掉落故障三射弓。</summary>
		[HarmonyPrefix]public static void MinifiedThingDestroyPrefix(MinifiedThing __instance)
		{
			if (__instance.InnerThing is ISecretTrishot trishotOwner)
			{
				trishotOwner.LeaveTrishot();
			}
		}
		/*
		///<summary>使三联电池同样被视为<see cref = "Alert_NeedBatteries" />可接受的电池类型。</summary>
		[HarmonyPostfix]public static void AlertNeedBatteriesPostfix(Alert_NeedBatteries __instance, ref bool __result, Map map)
		{
			if (__result == true &&
				map.listerBuildings.ColonistsHaveBuilding(thing => thing is Building_TriBattery))
			{
				__result = false;
			}
		}
		*/
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
					if (!(t is Corpse) || (t.def.ingestible.foodType & FoodTypeFlags.Meat) != 0 || t.def == BloodyTreeMeatDef)
					{
						return false;
					}
					if (t.IsForbidden(pawn))
					{
						return false;
					}
					if (!pawn.foodRestriction.CurrentFoodRestriction.Allows(t))
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
		[HarmonyPostfix]public static void PawnHealthTrackerInPainShockPostfix(Pawn_HealthTracker __instance, ref bool __result)
		{
			if (
				__instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
				&& DisablePainShockPredicate(pawn.def))
			{
				__result = false;
			}
		}

		///<summary>阻止半人马和Michelles得到任何永久性疤痕。</summary>
		[HarmonyPrefix]public static void HediffComp_GetsPermanentIsPermanentPrefix(HediffComp_GetsPermanent __instance, ref bool value)
		{
			if (DisableScarPredicate(__instance.Pawn.def))
			{
				value = false;
			}
		}
		///<summary>设置不允许<see cref = "HediffComp_TendDuration_CantTend" />症状被治疗。</summary>
		[HarmonyPostfix]public static void HediffComp_TendDurationAllowTendPostfix(HediffComp_TendDuration __instance, ref bool __result)
		{
			if (false &&
				__instance is HediffComp_TendDuration_CantTend
				&& __instance.Pawn.def == AlienCentaurDef)
			{
				__result = false;
			}
		}
		///<summary>阻止半人马大脑休克。</summary>
		[HarmonyPrefix]public static void HediffComp_ReactOnDamageNotify_PawnPostApplyDamagePrefix(HediffComp_ReactOnDamage __instance, ref DamageInfo dinfo, float totalDamageDealt)
		{
			if (DisableBrainShockPredicate(__instance.Pawn.def)
			 && __instance.Props.damageDefIncoming == DamageDefOf.EMP && dinfo.Def == DamageDefOf.EMP)
			{
				dinfo.Def = null;
			}
		}

		///<summary>增强半人马负重能力。</summary>
		[HarmonyPostfix]public static void MassUtilityCapacityPostfix(ref float __result, Pawn p, ref StringBuilder explanation)
		{
			if (p.def == AlienCentaurDef || p.def == AlienSayersDef)
			{
				string strPreProcess = "  - " + p.LabelShortCap + ": " + __result.ToStringMassOffset();
				__result = Math.Max(__result, p.def == AlienCentaurDef ? 350f : 35f);
				if ((p?.health?.hediffSet?.GetFirstHediffOfDef(HediffCentaurSubsystem_AntiMass_Def)).SubsystemEnabled())
				{
					__result *= 2.85714285714f;
				}
				if (explanation != null)
				{
					explanation.Replace(strPreProcess, "  - " + p.LabelShortCap + ": " + __result.ToStringMassOffset());
				}
			}
		}

		///<summary>使半人马的身体被渲染为其头发颜色。</summary>
		[HarmonyPostfix]public static void PawnGraphicSetResolveAllGraphicsPostfix(PawnGraphicSet __instance)
		{
			if (
				__instance.pawn.def == AlienCentaurDef)
			{
				//__instance.nakedGraphic.color = __instance.pawn.story.hairColor;
				__instance.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(
					__instance.nakedGraphic.path,
					__instance.nakedGraphic.Shader,
					__instance.nakedGraphic.drawSize,
					__instance.pawn.story.hairColor);
			}
			if (
				__instance.pawn.def == AlienGuoguoDef)
			{
				Color.RGBToHSV(__instance.pawn.story.SkinColor, out _, out _, out float v);
				//__instance.nakedGraphic.color = __instance.pawn.story.hairColor;
				__instance.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(
					__instance.nakedGraphic.path,
					__instance.nakedGraphic.Shader,
					__instance.nakedGraphic.drawSize,
					Color.HSVToRGB(0f, 0f, v));
			}
		}

		///<summary>对设施连接性的后期处理。</summary>
		[HarmonyPostfix]public static void CompAffectedByFacilitiesCanPotentiallyLinkToStaticPostfix(ref bool __result, ThingDef facilityDef, IntVec3 facilityPos, Rot4 facilityRot, ThingDef myDef, IntVec3 myPos, Rot4 myRot)
		{
			if (__result == true &&
				facilityDef?.placeWorkers?.Contains(typeof(PlaceWorker_FacingPort)) == true &&
				!GenAdj.OccupiedRect(myPos, myRot, myDef.size).Cells.Contains(PlaceWorker_FacingPort.PortPosition(facilityDef, facilityPos, facilityRot)))
			{
				__result = false;
				return;
			}
			if (__result == false && myDef == CentaurBedDef && facilityDef.GetCompProperties<CompProperties_Facility>() is CompProperties_Facility comp && comp.mustBePlacedAdjacentCardinalToBedHead)
			{
				__result = GenAdj.OccupiedRect(myPos, myRot, myDef.size).Overlaps(GenAdj.OccupiedRect(facilityPos, facilityRot, facilityDef.size + new IntVec2(2, 2)));
				return;
			}
		}

		///<summary>对人物渲染器的服装选单的补丁。</summary>
		[HarmonyPostfix]public static void PawnGraphicSetResolveApparelGraphicsPostfix(PawnGraphicSet __instance)
		{
			if (__instance.pawn.apparel.WornApparel.Any(ap => ap.def == CentaurHeaddressDef))
			{
				__instance.apparelGraphics.RemoveAll(ag => ag.sourceApparel.def.apparel.layers.Contains(ApparelLayerDefOf.Overhead));

				//ApparelGraphicRecord NullApparelGraphicRecord(ThingDef apparelDef, Color? color = null) => new ApparelGraphicRecord() { graphic = GraphicDatabase.Get<Graphic_Multi>((apparelDef.apparel.LastLayer != ApparelLayerDefOf.Overhead && !PawnRenderer.RenderAsPack(new Apparel() { def = apparelDef }) && !(apparelDef.apparel.wornGraphicPath == BaseContent.PlaceholderImagePath)) ? (apparelDef.apparel.wornGraphicPath + "_" + __instance.pawn.story.bodyType.defName) : apparelDef.apparel.wornGraphicPath, apparelDef.apparel.useWornGraphicMask ? ShaderDatabase.CutoutComplex : ShaderDatabase.Cutout, apparelDef.graphicData.drawSize, color ?? new Color(1f, 1f, 1f)), sourceApparel = new Apparel() { def = apparelDef } };
				//__instance.apparelGraphics.Add(NullApparelGraphicRecord(DefDatabase<ThingDef>.GetNamed("Apparel_CrownStellic"), new Color(1f,1f,0f)));
				//__instance.apparelGraphics.Add(NullApparelGraphicRecord(DefDatabase<ThingDef>.GetNamed("Apparel_Gunlink"), new Color(1f,1f,1f)));
				//__instance.apparelGraphics.Add(NullApparelGraphicRecord(DefDatabase<ThingDef>.GetNamed("Apparel_ArmorHelmetReconPrestige"), new Color(0.5f,0.75f,1f)));
			}
			if (DisableWaistRenderingPredicate(__instance.pawn.def))
			{
				__instance.apparelGraphics.RemoveAll(ag =>
					ag.sourceApparel.def.apparel.bodyPartGroups.Contains(DefDatabase<BodyPartGroupDef>.GetNamed("Waist"))
				 //&& ag.sourceApparel.def.apparel.layers.Contains(ApparelLayerDefOf.Belt)
				 && !ag.sourceApparel.def.apparel.bodyPartGroups.Where(bpgd => bpgd != DefDatabase<BodyPartGroupDef>.GetNamed("Waist")).Any()
					);
			}
		}

		///<summary>在信息页移除半人马的寿命显示。</summary>
		[HarmonyPostfix]public static void RacePropertiesSpecialDisplayStatsPostfix(RaceProperties __instance, ref IEnumerable<StatDrawEntry> __result, ThingDef parentDef, StatRequest req)
		{
			/*_ = new StatDrawEntry(
				StatCategoryDefOf.BasicsPawn, 
				"StatsReport_LifeExpectancy".Translate(),
				float.PositiveInfinity.ToStringByStyle(ToStringStyle.Integer), 
				"Stat_Race_LifeExpectancy_Desc".Translate(), 
				2000);*/
			if (parentDef == AlienCentaurDef)
			{
				List<StatDrawEntry> result = __result.ToList();
				result.RemoveAll(
					stat => stat.LabelCap == "StatsReport_LifeExpectancy".Translate().CapitalizeFirst()
					&& stat.ValueString == "-2147483648" //.Contains('-') // == float.PositiveInfinity.ToStringByStyle(ToStringStyle.Integer)
					//&& stat.DisplayPriorityWithinCategory == 2000
					);
				__result = result;
			}
		}
		///<summary>在信息页移除半人马的疼痛休克阈值和精神崩溃阈值显示。</summary>
		[HarmonyPostfix]public static void StatWorkerShouldShowForPostfix(StatWorker __instance, ref bool __result, StatRequest req)
		{
			if (__result &&
				req.HasThing && req.Thing is Pawn pawn &&
				__instance.GetType().GetField("stat", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is StatDef stat)
			{
				if ((DisablePainShockPredicate(pawn.def) && stat == StatDefOf.PainShockThreshold)
				 || (DisableMentalBreakPredicate(pawn.def) && stat == StatDefOf.MentalBreakThreshold)
					)
				{
					__result = false;
				}
			}
		}
		///<summary>从截肢手术清单中移除半人马的锁骨和子系统。</summary>
		[HarmonyPostfix]public static void RecipeRemoveBodyPartGetPartsToApplyOnPostfix(Recipe_Surgery __instance, ref IEnumerable<BodyPartRecord> __result, Pawn pawn, RecipeDef recipe)
		{
			if (pawn.def == AlienCentaurDef)
			{
				List<BodyPartRecord> result = __result.ToList();
				result.RemoveAll(bpr =>
					bpr.def == CentaurScapularDef
				 || bpr.def == CentaurSubsystemBodyPartDef
					);
				__result = result;
			}
		}
		/*
		///<summary>更改半人马默认服装方案。</summary>
		[HarmonyPostfix]public static void PawnOutfitTrackerGetCurrentOutfitPostfix(Pawn_OutfitTracker __instance)
		{
			if (__instance.GetType().GetField("curOutfit", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Outfit curOutfit
				&& curOutfit == null)
			{
				__instance.CurrentOutfit = Current.Game.outfitDatabase.AllOutfits.First(o => o.label == "OutfitCentaur".Translate());
			}
		}*/
		/*
		//static int ExLogLimit = 0;
		//static readonly MethodInfo IsPredatorHostileToMethod = AccessTools.Method(typeof(GenHostility), "IsPredatorHostileTo", new Type[] { typeof(Pawn), typeof(Pawn) });
		public static bool ExLog(this bool boolen, string msg, ref string str, bool tarfet = true)
		{
			if (boolen == tarfet)
			{
				str += msg;
			}
			return boolen;
		}
		///<summary>检测敌对性状态，输出日志。</summary>
		[HarmonyPostfix] public static void GenHostilityHostileToPostfix(Thing a, Thing b, bool __result)
		{
			try
			{
				if (!__result)
				{
					return;
				}
				if (a.def == AlienCentaurDef || b.def == AlienCentaurDef)
				{

					static bool IsPredatorHostileTo(Pawn predator, Pawn toPawn)
					{
						try
						{
							return false;// (bool)IsPredatorHostileToMethod.Invoke(null, new object[] { predator, toPawn });
						}
						catch
						{
							return false;
						}
					}

					if (a.Destroyed || b.Destroyed || a == b)
					{
						return;
					}
					Pawn pawn = a as Pawn;
					Pawn pawn2 = b as Pawn;
					string an = pawn.Name.ToStringShort;
					string bn = pawn2.Name.ToStringShort;
					string msg = $"[Explorite]Testing hostile with {an} and {bn}.\n";
					_ =
						(pawn != null
							&& pawn.MentalState != null
							&& pawn.MentalState.ForceHostileTo(b)).ExLog($"Hostile to {an} mental state.\n", ref msg)
						|| (pawn2 != null
							&& pawn2.MentalState != null
							&& pawn2.MentalState.ForceHostileTo(a)).ExLog($"Hostile to {bn} mental state.\n", ref msg)
						|| (pawn != null
							&& pawn2 != null
							&& (IsPredatorHostileTo(pawn, pawn2)
								|| IsPredatorHostileTo(pawn2, pawn))).ExLog($"Hostile to predator.\n", ref msg)
						|| ((a.Faction != null
								&& pawn2 != null
								&& pawn2.HostFaction == a.Faction
								&& (pawn == null
									|| pawn.HostFaction == null)
								&& PrisonBreakUtility.IsPrisonBreaking(pawn2))
								|| (b.Faction != null
									&& pawn != null
									&& pawn.HostFaction == b.Faction
									&& (pawn2 == null
										|| pawn2.HostFaction == null)
									&& PrisonBreakUtility.IsPrisonBreaking(pawn))).ExLog($"Hostile to prison breaking.\n", ref msg)
						|| ((a.Faction == null || pawn2 == null || pawn2.HostFaction != a.Faction)
								&& (b.Faction == null || pawn == null || pawn.HostFaction != b.Faction)
								&& (pawn == null
									|| !pawn.IsPrisoner
									|| pawn2 == null
									|| !pawn2.IsPrisoner)
								&& (pawn == null
									|| pawn2 == null
									|| ((!pawn.IsPrisoner
										|| pawn.HostFaction != pawn2.HostFaction
										|| PrisonBreakUtility.IsPrisonBreaking(pawn))
									&& (!pawn2.IsPrisoner
										|| pawn2.HostFaction != pawn.HostFaction
										|| PrisonBreakUtility.IsPrisonBreaking(pawn2))))
							&& (pawn == null
								|| pawn2 == null
								|| ((pawn.HostFaction == null
										|| pawn2.Faction == null
										|| pawn.HostFaction.HostileTo(pawn2.Faction)
										|| PrisonBreakUtility.IsPrisonBreaking(pawn))
									&& (pawn2.HostFaction == null
										|| pawn.Faction == null
										|| pawn2.HostFaction.HostileTo(pawn.Faction)
										|| PrisonBreakUtility.IsPrisonBreaking(pawn2))))
							&& (a.Faction == null
								|| !a.Faction.IsPlayer
								|| pawn2 == null
								|| !pawn2.mindState.WillJoinColonyIfRescued)
							&& (b.Faction == null
								|| !b.Faction.IsPlayer
								|| pawn == null
								|| !pawn.mindState.WillJoinColonyIfRescued)
							&& a.Faction != null
							&& b.Faction != null
							&& a.Faction.HostileTo(b.Faction)
						).ExLog($"Hostile to faction relation.\n", ref msg);

					Log.Message(msg);
				}
			}
			catch
			{ }
		}
		*/
		//TODO : REPLACE WITH TRANSPILER
		///<summary>使半人马即使携带了物品也会被视为威胁。</summary>
		[HarmonyPostfix]public static void PawnThreatDisabledPostfix(Pawn __instance, ref bool __result, IAttackTargetSearcher disabledFor)
		{
			if (__instance.def == AlienCentaurDef)
			{
				if (__result)
				{
					if (!__instance.Spawned)
					{
						__result = true;
						return;
					}
					/* if (!__instance.InMentalState && __instance.GetTraderCaravanRole() == TraderCaravanRole.Carrier && !(__instance.jobs.curDriver is JobDriver_AttackMelee))
					{
						__result = true;
						return;
					} */
					if (__instance.mindState.duty != null && __instance.mindState.duty.def.threatDisabled)
					{
						__result = true;
						return;
					}
					if (!__instance.mindState.Active)
					{
						__result = true;
						return;
					}
					if (__instance.Downed)
					{
						if (disabledFor == null)
						{
							__result = true;
							return;
						}
						if (!(disabledFor.Thing is Pawn pawn) || pawn.mindState == null || pawn.mindState.duty == null || !pawn.mindState.duty.attackDownedIfStarving || !pawn.Starving())
						{
							__result = true;
							return;
						}
					}
					__result = false;
					return;
				}
			}
		}
		///<summary>显示MakeThing的调用堆栈。</summary>
		[HarmonyPrefix]public static void ThingMakerMakeThingPrefix(ThingDef def)
		{
			/*
			if (def == TrishotThing2Def)
			{
				Log.Message("[Explorite]Making TriShot Prototype, with stack trace...");
			}
			*/
		}
		///<summary>显示MakeThing的调用堆栈。</summary>
		[HarmonyPostfix]public static void ThingMakerMakeThingPostfix(Thing __result)
		{
			if (__result.def == OrangiceDef)
			{
				Log.Message($"[Explorite]Making Orangice thing {__result.ThingID} detected. Stack trace below.");
			}
		}
		/*
		///<summary>将被作为合成原材料的三射弓从追踪中移除。</summary>
		[HarmonyPrefix]public static void GenRecipeMakeRecipeProductsPrefix(RecipeDef recipeDef, Pawn worker, List<Thing> ingredients, Thing dominantIngredient, IBillGiver billGiver)
		{
			foreach (Thing thing in ingredients)
			{
				try
				{
					GameComponentCentaurStory.TryRemove(thing);
				}
				catch { }
			}
		}
		///<summary>将被制作出来的三射弓添加到追踪中。</summary>
		[HarmonyPostfix]public static void GenRecipeMakeRecipeProductsPostfix(IEnumerable<Thing> __result, RecipeDef recipeDef, Pawn worker, List<Thing> ingredients, Thing dominantIngredient, IBillGiver billGiver)
		{
			foreach (Thing thing in __result)
			{
				try
				{
					GameComponentCentaurStory.TryAdd(thing);
				}
				catch { }
			}
		}
		*/
		///<summary>将被制作出来的三射弓添加到追踪中。</summary>
		[HarmonyPostfix]public static void GenRecipePostProcessProductPostfix(Thing __result, Thing product, RecipeDef recipeDef, Pawn worker)
		{
			try
			{
				GameComponentCentaurStory.TryAdd(__result);
			}
			catch { }
		}
		///<summary>降低故障三射弓的好感度加成。</summary>
		[HarmonyPostfix]public static void FactionGiftUtilityGetBaseGoodwillChangePostfix(ref float __result, Thing anyThing, int count, float singlePrice, Faction theirFaction)
		{
			if (anyThing?.def?.weaponTags?.Contains("CentaurTracedTrishot") == true)
			{
				/*
				float factor = singlePrice / anyThing.MarketValue;
				//__result = 0f;//-= TrishotThing1Def.BaseMarketValue * count / 40f;
				__result -= TrishotThing1Def.BaseMarketValue * factor * count / 40f;
				*/
				__result = 0f;
			}
		}
		///<summary>移除半人马随机好心情灵感。</summary>
		[HarmonyPostfix]public static void InspirationHandlerGetRandomAvailableInspirationDefPostfix(InspirationHandler __instance, ref InspirationDef __result)
		{
			if (__instance.pawn.def == AlienCentaurDef)
			{
				__result = null;
			}
		}
		///<summary>设置三射弓伤害源为满级三射弓。</summary>
		[HarmonyPrefix]public static void DamageInfoCtorPrefix(ref ThingDef weapon)
		{
			if (weapon?.weaponTags?.Contains("CentaurTracedTrishot") == true)
			{
				weapon = TrishotThingDef;
			}
		}
		///<summary>设置日志中三射弓伤害源为满级三射弓。</summary>
		[HarmonyPrefix]public static void BattleLogEntry_ExplosionImpactCtorPrefix(ref ThingDef weaponDef)
		{
			if (weaponDef?.weaponTags?.Contains("CentaurTracedTrishot") == true)
			{
				weaponDef = TrishotThingDef;
			}
		}
		/*
		///<summary>设置三射弓弹射物伤害来源为满级三射弓。</summary>
		[HarmonyPostfix]public static void ProjectileLaunchPostfix(Projectile __instance)
		{
			if (__instance.GetType().GetField("equipmentDef", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is ThingDef equipmentDef
			 && equipmentDef?.weaponTags?.Contains("CentaurTracedTrishot") == true)
			{
				equipmentDef = TrishotThingDef;
			}
		}
		*/
		///<summary>修正健康面板中的身体部位排序。</summary>
		[HarmonyPostfix]public static void HealthCardUtilityGetListPriorityPostfix(ref float __result, BodyPartRecord rec)
		{
			if (rec?.groups?.Contains(CentaurCorePartGroupDef) ?? false)
			{
				__result += 40002f;
			}
			if (rec?.groups?.Contains(CentaurSubsystemGroup0Def) ?? false)
			{
				__result += 100000f;
				if (rec.groups.Contains(CentaurSubsystemGroup1Def))
				{
					__result += 3f;
				}
				if (rec.groups.Contains(CentaurSubsystemGroup2Def))
				{
					__result += 2f;
				}
				if (rec.groups.Contains(CentaurSubsystemGroup3Def))
				{
					__result += 1f;
				}
			}
		}
		///<summary>移除半人马自我治疗的70%效果惩罚。</summary>
		[HarmonyPrefix]public static void TendUtilityCalculateBaseTendQualityPrefix(Pawn doctor, Pawn patient, ref float medicinePotency, ref float medicineQualityMax)
		{
			if (doctor == patient && DisableSelfHealPenaltyPredicate(doctor?.def))
			{
				medicinePotency /= 0.7f;
			}
		}
		///<summary>检测任务奖励中的橙冰。</summary>
		[HarmonyPostfix]public static void QuestPartDropPodsSetThingsPostfix(IEnumerable<Thing> value)
		{
			IEnumerable<Thing> orangices = value.Where(t => t.def == OrangiceDef);
			foreach (Thing thing in orangices)
			{
				Log.Warning($"[Explorite]Warning, Orangice thing {thing.ThingID} detected in quest part drop pods. Stack trace below.");
			}
		}
		///<summary>阻止特定项目被研究。</summary>
		[HarmonyPostfix]public static void ResearchProjectDefCanStartNowPostfix(ResearchProjectDef __instance, ref bool __result)
		{
			if (__result && __instance?.tags?.Any(t => t.defName == "ExploriteNeverResearchable") == true)
			{
				__result = false;
			}
		}
		///<summary>使自动弩机炮台无视屋顶限制。</summary>
		[HarmonyPostfix]public static void BuildingTurretGunIsMortarOrProjectileFliesOverheadPostfix(Building_TurretGun __instance, ref bool __result)
		{
			if (__instance.def == HyperTrishotTurretBuildingDef)
			{
				__result = false;
			}
		}
		///<summary>使自动弩机炮台可以手动选择目标。</summary>
		[HarmonyPostfix]public static void BuildingTurretGunCanSetForcedTargetPostfix(Building_TurretGun __instance, ref bool __result)
		{
			if (__instance.def == HyperTrishotTurretBuildingDef)
			{
				__result = true;
			}
		}

		private static bool TrishotProjFliesOverheadOverrider(bool boolen, object instance)
		{
			//if (HyperTrishotTurretBuildingDef.Verbs.Contains(verb.verbProps))
			//Log.Message($"[Explorite]Transpiler overrider get {instance.def.defName} and {verb.verbProps}");
			if (instance is Building_TurretGun building && building.def == HyperTrishotTurretBuildingDef)
			{
				return false;
			}
			else
			{
				//return projectileFliesOverheadMethod.Invoke(null, new object[] { verb }) as bool? ?? false;
				//return verb.ProjectileFliesOverhead();
				return boolen;
			}
		}
		///<summary>使自动弩机炮台无视屋顶限制而开火。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> BuildingTurretGunTryStartShootSomethingTranspiler(IEnumerable<CodeInstruction> instr)
		{
			byte patchActionStage = 0;
			MethodBase projectileFliesOverheadMethod = AccessTools.Method(typeof(VerbUtility), nameof(VerbUtility.ProjectileFliesOverhead));
			//Log.Message($"[Explorite]Transpiler patch target: {projectileFliesOverheadMethod?.Name}.");
			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0 && ins.opcode == OpCodes.Call
					&& ins.operand == (projectileFliesOverheadMethod as object)
					)
				{
					patchActionStage++;
					//Log.Message("[Explorite]Find transpiler patch target.");
					//yield return new CodeInstruction(OpCodes.Ldloc_0);
					yield return ins;
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, ((Func<bool, object, bool>)TrishotProjFliesOverheadOverrider).GetMethodInfo());
				}
				else
				{
					//Log.Message("[Explorite]Pass transpiler patch target.");
					yield return ins;
				}
			}
			TranspilerStageCheckout(patchActionStage, 1);
			yield break;
		}

		/*private static float HyperManipulateCapacityLevelOverrider(HediffSet diffSet, PawnCapacityDef capacity, List<PawnCapacityUtility.CapacityImpactor> impactors = null, bool forTradePrice = false, object instance = null)
		{
			float value = PawnCapacityUtility.CalculateCapacityLevel(diffSet, capacity, impactors, forTradePrice);
			if (diffSet.GetFirstHediffOfDef(HyperManipulatorHediffDef)?.Severity == 1f)
			{
				if (capacity == PawnCapacityDefOf.Manipulation)
				{
					value *= Math.Max(1f, PawnCapacityUtility.CalculateCapacityLevel(diffSet, PawnCapacityDefOf.Moving, impactors, forTradePrice));
				}
				if (capacity == PawnCapacityDefOf.Moving)
				{
					value *= Math.Max(1f, PawnCapacityUtility.CalculateCapacityLevel(diffSet, PawnCapacityDefOf.Manipulation, impactors, forTradePrice));
				}
			}
			return value;
		}
		///<summary>使物理操作仪激活效果影响移动能力和操作能力。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> PawnCapacitiesHandlerGetLevelTranspiler(IEnumerable<CodeInstruction> instr)
		{
			MethodBase calculateCapacityLevelMethod = AccessTools.Method(typeof(PawnCapacityUtility), nameof(PawnCapacityUtility.CalculateCapacityLevel));
			foreach (CodeInstruction ins in instr)
			{
				if (ins.opcode == OpCodes.Call
					&& ins.operand == (calculateCapacityLevelMethod as object)
					)
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, ((Func<HediffSet, PawnCapacityDef, List<PawnCapacityUtility.CapacityImpactor>, bool, object, float>)HyperManipulateCapacityLevelOverrider).GetMethodInfo());
				}
				else
				{
					yield return ins;
				}
			}
			yield break;
		}*/
		private static readonly PawnCapacityDef ManipulationAlt = new PawnCapacityDef() { defName = "EManipulationAlt" };
		private static readonly PawnCapacityDef MovingAlt = new PawnCapacityDef() { defName = "EMovingAlt" };
		///<summary>干预半人马健康能力计算预处理。</summary>
		[HarmonyPrefix]public static void PawnCapacityUtilityCalculateCapacityLevelPrefix(ref float? __state, HediffSet diffSet, ref PawnCapacityDef capacity, ref List<CapacityImpactor> impactors, bool forTradePrice)
		{
			__state = null;
			if (diffSet?.pawn?.def == AlienCentaurDef
				&& diffSet?.GetFirstHediffOfDef(HyperManipulatorHediffDef)?.Part?.def == CentaurScapularDef
				&& diffSet?.GetFirstHediffOfDef(HyperManipulatorHediffDef)?.Severity == 1f
				&& (capacity == PawnCapacityDefOf.Manipulation || capacity == PawnCapacityDefOf.Moving)
				)
			{
				__state = 1f;
				if (capacity == PawnCapacityDefOf.Moving)
				{
					__state *= Math.Max(1f, CalculateCapacityLevel(diffSet, ManipulationAlt, impactors, forTradePrice));
					impactors?.Add(new CapacityImpactorCapacity() { capacity = PawnCapacityDefOf.Manipulation });
				}
			}

			if (capacity == ManipulationAlt)
			{
				capacity = PawnCapacityDefOf.Manipulation;
			}
			if (capacity == MovingAlt)
			{
				capacity = PawnCapacityDefOf.Moving;
			}
		}
		///<summary>干预半人马健康能力计算后期处理。</summary>
		[HarmonyPostfix]public static void PawnCapacityUtilityCalculateCapacityLevelPostfix(ref float __result, ref float? __state, HediffSet diffSet, ref PawnCapacityDef capacity, ref List<CapacityImpactor> impactors, bool forTradePrice)
		{
			if (__state.HasValue)
			{
				if (capacity == PawnCapacityDefOf.Manipulation)
				{
					__result *= Math.Max(1f, CalculateCapacityLevel(diffSet, MovingAlt, impactors, forTradePrice));
					impactors?.Add(new CapacityImpactorCapacity() { capacity = PawnCapacityDefOf.Moving });
				}
				__result *= __state.Value;
			}

			if (diffSet.pawn.def == AlienCentaurDef && impactors != null)
			{
				List<CapacityImpactor> replacement = new List<CapacityImpactor>();
				foreach (CapacityImpactor impactor in impactors)
				{
					switch (impactor)
					{
						case CapacityImpactorHediff impH when impH.hediff.def == HyperManipulatorHediffDef
							&& replacement.Any(imp => imp is CapacityImpactorHediff impt && impt.hediff == impH.hediff):
						case CapacityImpactorBodyPartHealth impB when impB.bodyPart.def == CentaurScapularDef
							&& replacement.Any(imp => imp is CapacityImpactorBodyPartHealth impt && impt.bodyPart == impB.bodyPart):
						case CapacityImpactorCapacity impC when
							replacement.Any(imp => imp is CapacityImpactorCapacity impt && impt.capacity == impC.capacity):
						case CapacityImpactorPain _ when
							replacement.Any(imp => imp is CapacityImpactorPain):
							break;

						default:
							replacement.Add(impactor);
							break;
					}
				}
				impactors?.Clear();
				impactors?.AddRange(replacement);
			}
		}
		public static bool IsProjFliesOverheadOverrider(object instance)
		{
			return !(instance is Projectile projectile) || !(projectile?.def?.tradeTags?.Contains("ExRoofBypass") ?? false);
		}
		///<summary>干涉弹射物命中屋顶的效果。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> ProjectileImpactSomethingTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			FieldInfo flyOverheadInfo = AccessTools.Field(typeof(ProjectileProperties), nameof(ProjectileProperties.flyOverhead));
			byte patchActionStage = 0;
			Label? zerolabel = null;
			Label? addlabel = null;
			//StringBuilder stringBuilder = new StringBuilder();
			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0 && ins.opcode == OpCodes.Ldfld && ins.operand == flyOverheadInfo as object)
				{
					patchActionStage = 1;
					yield return ins;
				}
				else if (patchActionStage == 1 && ins.opcode == OpCodes.Brfalse)
				{
					patchActionStage = 2;
					zerolabel = ins.operand as Label?;
					yield return ins;
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, ((Func<object, bool>)IsProjFliesOverheadOverrider).GetMethodInfo());
					CodeInstruction brf = new CodeInstruction(OpCodes.Brfalse, ilg.DefineLabel());
					addlabel = brf.operand as Label?;
					yield return brf;
				}
				else if (patchActionStage == 2 && zerolabel.HasValue && addlabel.HasValue && ins.labels.Contains(zerolabel.Value))
				{
					patchActionStage = 3;
					ins.labels.Add(addlabel.Value);
					yield return ins;
					zerolabel = null;
					addlabel = null;
				}
				else
				{
					yield return ins;
				}
			}
			//Log.Message("[Explorite]instr result:\n"+stringBuilder.ToString());
			TranspilerStageCheckout(patchActionStage, 3);
			yield break;
		}

		///<summary>检查护盾物体。</summary>
		public static bool TestShieldSpotValidator(IntVec3 pos, Map map)
		{
			for (int i = -2; i <= 2; i++)
			{
				if (map.thingGrid.ThingsAt(pos + new IntVec3(i, 0, 0)).Any(thing => thing.def.defName == "TestShieldSpot" && (thing.Rotation.AsByte == 0 || thing.Rotation.AsByte == 2)))
					return true;
				if (map.thingGrid.ThingsAt(pos + new IntVec3(0, 0, i)).Any(thing => thing.def.defName == "TestShieldSpot" && (thing.Rotation.AsByte == 1 || thing.Rotation.AsByte == 3)))
					return true;
			}
			return false;
		}
		///<summary>设置爆炸物不能穿透何种物体。</summary>
		public static bool ExplosionBypassEverything(this DamageDef def)
		{
			return def.defName.Contains("Frostblast");
		}

		///<summary>设置爆炸物不能穿透何种物体。</summary>
		public static bool ExplosionCellsToHitBlockOverrider(IntVec3 start, IntVec3 end, Map map)
		{
			bool validator(IntVec3 pos)
			{
				return !TestShieldSpotValidator(pos, map);
			}
			return GenSight.LineOfSight(start, end, map, skipFirstCell: false, validator: validator, halfXOffset: 0, halfZOffset: 0);
		}
		///<summary>设置爆炸物不能覆盖何种物体。</summary>
		public static bool ExplosionCellsToHitInsideOverrider(IntVec3 pos, Map map)
		{
			return !TestShieldSpotValidator(pos, map);
		}
		///<summary>设置爆炸物无视一切物体。</summary>
		public static bool ExplosionCellsToHitEverythingOverrider(this DamageWorker instance)
		{
			return instance.def.ExplosionBypassEverything();
		}
		///<summary>改变爆炸效果选择的覆盖范围。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> DamageWorkerExplosionCellsToHitTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			MethodInfo lineOfSightInfo = AccessTools.Method(typeof(GenSight), nameof(GenSight.LineOfSight),
				new Type[] { typeof(IntVec3), typeof(IntVec3), typeof(Map), typeof(bool), typeof(Func<IntVec3, bool>), typeof(int), typeof(int) });
			MethodInfo inBoundsInfo = AccessTools.Method(typeof(GenGrid), nameof(GenGrid.InBounds),
				new Type[] { typeof(IntVec3), typeof(Map) });
			byte patchActionStage1 = 0;
			byte patchActionStage2 = 0;
			List<Label?> addlabels = new List<Label?>();
			Label? zerolabel = null;
			Label? olabel = null;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage1 == 0 && ins.opcode == OpCodes.Call && ins.operand == lineOfSightInfo as object)
				{
					patchActionStage1 = 1;
				}
				else if (patchActionStage1 == 1 && ins.opcode == OpCodes.Brfalse_S)
				{
					patchActionStage1 = 2;
					zerolabel = ins.operand as Label?;
					yield return ins;

					yield return new CodeInstruction(OpCodes.Ldloc_2);
					yield return new CodeInstruction(OpCodes.Ldarg_2);
					yield return new CodeInstruction(OpCodes.Call, ((Func<IntVec3, Map, bool>)ExplosionCellsToHitInsideOverrider).GetMethodInfo());
					CodeInstruction brf1 = new CodeInstruction(OpCodes.Brfalse_S, ilg.DefineLabel());
					addlabels.Add(brf1.operand as Label?);
					yield return brf1;

					yield return new CodeInstruction(OpCodes.Ldarg_1);
					yield return new CodeInstruction(OpCodes.Ldloc_2);
					yield return new CodeInstruction(OpCodes.Ldarg_2);
					yield return new CodeInstruction(OpCodes.Call, ((Func<IntVec3, IntVec3, Map, bool>)ExplosionCellsToHitBlockOverrider).GetMethodInfo());
					CodeInstruction brf2 = new CodeInstruction(OpCodes.Brfalse_S, ilg.DefineLabel());
					addlabels.Add(brf2.operand as Label?);
					yield return brf2;
					continue;
				}
				else if (patchActionStage1 == 2)
				{
					if (olabel.HasValue && patchActionStage2 == 2)
					{
						patchActionStage2 = 3;
						ins.labels.Add(olabel.Value);
						yield return ins;
						olabel = null;
						continue;
					}
					if (zerolabel.HasValue && addlabels.Any() && ins.labels.Contains(zerolabel.Value))
					{
						patchActionStage1 = 3;
						foreach (Label? addlabel in addlabels)
						{
							ins.labels.Add(addlabel.Value);
						}
						yield return ins;
						zerolabel = null;
						addlabels.Clear();
						continue;
					}
				}

				if (patchActionStage2 == 0 && ins.opcode == OpCodes.Call && ins.operand == inBoundsInfo as object)
				{
					patchActionStage2 = 1;
				}
				else if (patchActionStage2 == 1 && ins.opcode == OpCodes.Brfalse_S)
				{
					patchActionStage2 = 2;
					yield return ins;

					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, ((Func<DamageWorker, bool>)ExplosionCellsToHitEverythingOverrider).GetMethodInfo());
					CodeInstruction brf1 = new CodeInstruction(OpCodes.Brtrue_S, ilg.DefineLabel());
					olabel = brf1.operand as Label?;
					yield return brf1;

					continue;
				}

				yield return ins;
				continue;
			}

			/*
			Log.Message("[Explorite]instr result:\n" + stringBuilder.ToString());
			if (patchActionStage1 == 3)
			{
				Log.Message($"[Explorite]instr patch 1 done!({patchActionStage1})");
			}
			else
			{
				Log.Error($"[Explorite]instr patch 1 stage error!({patchActionStage1})");
			}
			if (patchActionStage1 == 3)
			{
				Log.Message($"[Explorite]instr patch 2 done!({patchActionStage2})");
			}
			else
			{
				Log.Error($"[Explorite]instr patch 2 stage error!({patchActionStage2})");
			}
			*/
			TranspilerStageCheckout(patchActionStage1, 3, "stage1");
			TranspilerStageCheckout(patchActionStage2, 3, "stage2");
			yield break;
		}

		///<summary>临时函数。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> AlienRaceHarmonyPatchesChooseStyleItemPrefix(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			foreach (CodeInstruction ins in instr)
			{
				if (ins.operand == HairDefOf.Shaved)
				{
					ins.operand = DefDatabase<HairDef>.GetNamed("Flowy");
					yield return ins;
				}
				else
				{
					yield return ins;
				}
			}
			yield break;
		}
		/*
			public static void LocalTest1()
			{
				IntVec3 intVec2 = Fun5();
				Map map = Fun4();

				if (intVec2.Walkable(map))
				{
					Fun3();
				}
			}
			public static void LocalTest2()
			{
				IntVec3 intVec2 = Fun5();
				Map map = Fun4();

				if (ExplosionCellsToHitInsideOverrider(intVec2, map) || intVec2.Walkable(map))
				{
					Fun3();
				}
			}
			public static void Fun3()
			{
			}
			public static Map Fun4()
			{
				return new Map();
			}
			public static IntVec3 Fun5()
			{
				return new IntVec3();
			}
			public static void Fun6(int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8)
			{
				Fun6(a1, a2, a3, a4, a5, a6, a7, a8);
			}
			*/
		/*
		class DamageWorker_Tes1 : DamageWorker
		{
			public override IEnumerable<IntVec3> ExplosionCellsToHit(IntVec3 center, Map map, float radius, IntVec3? needLOSToCell1 = null, IntVec3? needLOSToCell2 = null)
			{
				List<IntVec3> openCells = GetType().GetField("openCells", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this) as List<IntVec3>;
				List<IntVec3> adjWallCells = GetType().GetField("adjWallCells", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this) as List<IntVec3>;

				openCells.Clear();
				adjWallCells.Clear();
				int num = GenRadial.NumCellsInRadius(radius);
				for (int i = 0; i < num; i++)
				{
					IntVec3 intVec = center + GenRadial.RadialPattern[i];
					if (intVec.InBounds(map) && GenSight.LineOfSight(center, intVec, map, true, null, 0, 0))
					{
						if (needLOSToCell1 != null || needLOSToCell2 != null)
						{
							bool flag = needLOSToCell1 != null && GenSight.LineOfSight(needLOSToCell1.Value, intVec, map, false, null, 0, 0);
							bool flag2 = needLOSToCell2 != null && GenSight.LineOfSight(needLOSToCell2.Value, intVec, map, false, null, 0, 0);
							if (!flag && !flag2)
							{
								goto IL_B1;
							}
						}
						openCells.Add(intVec);
					}
				IL_B1:;
				}
				for (int j = 0; j < openCells.Count; j++)
				{
					IntVec3 intVec2 = openCells[j];
					if (intVec2.Walkable(map))
					{
						for (int k = 0; k < 4; k++)
						{
							IntVec3 intVec3 = intVec2 + GenAdj.CardinalDirections[k];
							if (intVec3.InHorDistOf(center, radius) && intVec3.InBounds(map) && !intVec3.Standable(map) && intVec3.GetEdifice(map) != null && !openCells.Contains(intVec3) && !adjWallCells.Contains(intVec3))
							{
								adjWallCells.Add(intVec3);
							}
						}
					}
				}
				return openCells.Concat(adjWallCells);
			}
		}
		class DamageWorker_Tes2 : DamageWorker
		{
			public override IEnumerable<IntVec3> ExplosionCellsToHit(IntVec3 center, Map map, float radius, IntVec3? needLOSToCell1 = null, IntVec3? needLOSToCell2 = null)
			{
				List<IntVec3> openCells = GetType().GetField("openCells", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this) as List<IntVec3>;
				List<IntVec3> adjWallCells = GetType().GetField("adjWallCells", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this) as List<IntVec3>;

				openCells.Clear();
				adjWallCells.Clear();
				int num = GenRadial.NumCellsInRadius(radius);
				for (int i = 0; i < num; i++)
				{
					IntVec3 intVec = center + GenRadial.RadialPattern[i];
					if (ExploritePatches.ExplosionCellsToHitEverythingOverrider(this) || (intVec.InBounds(map) && GenSight.LineOfSight(center, intVec, map, true, null, 0, 0)))
					{
						if (needLOSToCell1 != null || needLOSToCell2 != null)
						{
							bool flag = needLOSToCell1 != null && GenSight.LineOfSight(needLOSToCell1.Value, intVec, map, false, null, 0, 0);
							bool flag2 = needLOSToCell2 != null && GenSight.LineOfSight(needLOSToCell2.Value, intVec, map, false, null, 0, 0);
							if (!flag && !flag2)
							{
								goto IL_B1;
							}
						}
						openCells.Add(intVec);
					}
				IL_B1:;
				}
				for (int j = 0; j < openCells.Count; j++)
				{
					IntVec3 intVec2 = openCells[j];
					if (intVec2.Walkable(map))
					{
						for (int k = 0; k < 4; k++)
						{
							IntVec3 intVec3 = intVec2 + GenAdj.CardinalDirections[k];
							if (intVec3.InHorDistOf(center, radius) && intVec3.InBounds(map) && !intVec3.Standable(map) && intVec3.GetEdifice(map) != null && !openCells.Contains(intVec3) && !adjWallCells.Contains(intVec3))
							{
								adjWallCells.Add(intVec3);
							}
						}
					}
				}
				return openCells.Concat(adjWallCells);
			}
		}
		*/

		///<summary>更改阵营能够使用的模因。</summary>
		[HarmonyPostfix]public static void IdeoUtilityIsMemeAllowedForPostfix(MemeDef meme, FactionDef faction, ref bool __result)
		{
			/*
			if (faction == CentaurPlayerColonyDef)
			{
				if (meme != CentaurMemeDef && meme != CentaurStructureMemeDef)
				{
					__result = false;
					return;
				}
			}
			if (meme.category != MemeCategory.Structure
				&& (meme == CentaurMemeDef || meme == SayersMeme1Def || meme == SayersMeme2Def)
				&& (!(faction.requiredMemes?.Contains(meme) ?? false) || !(faction.allowedMemes?.Contains(meme) ?? false))
				)
			{
				__result = false;
				return;
			}
			*/

			if (__result && meme is MemeDef_Ex memeEx && !(memeEx.exclusiveTo?.Contains(faction) ?? false))
			{
				__result = false;
				return;
			}
		}
		///<summary>更改模因能够兼容的模因。</summary>
		[HarmonyPostfix] public static void IdeoUtilityCanAddPostfix(MemeDef meme, List<MemeDef> memes, FactionDef forFaction, ref bool __result)
		{
			if (__result)
			{
				string islocateGroup = meme is MemeDef_Ex meEx ? meEx.islocateGroup : "";
				if(memes.Select(meme => meme is MemeDef_Ex meEx ? meEx.islocateGroup : meme.category != MemeCategory.Structure ? "" : null).Any(g => g != null && g != islocateGroup))
				{
					__result = false;
					return;
				}
			}
		}
		///<summary>禁止伤害渲染。</summary>
		[HarmonyPostfix]public static bool PawnWoundDrawerRenderOverBodyPrefix(ref PawnWoundDrawer __instance)
		{
			return !(__instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn)
				|| !DisableWoundRenderingPredicate(pawn.def);
		}
		///<summary>更改阵营能够使用的图标。</summary>
		[HarmonyPostfix]public static void IdeoSymbolPartDefCanBeChosenForIdeoPostfix(Ideo ideo, ref IdeoSymbolPartDef __instance, ref bool __result)
		{
			if (__result)
			{
				List<MemeDef> mlist = ideo.memes.Concat(new[] { ideo.StructureMeme }).Where(m => m is MemeDef_Ex meEx && meEx.exclusiveTo.Any()).ToList();
				if (mlist.Any())
				{
					foreach (MemeDef meme in mlist)
					{
						if (__instance.memes?.Contains(meme) ?? false)
						{
							__result = true;
							return;
						}
					}
					__result = false;
					return;
				}
			}
		}
		/*
		///<summary>使服装颜色想法进行对<see cref = "CompColorable" />的非空检查，方案1。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> ThoughtWorkerWearingColorCurrentStateInternalTranspilerA(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			MethodInfo tryGetCompInfo = AccessTools.Method(typeof(ThingCompUtility), nameof(ThingCompUtility.TryGetComp)).MakeGenericMethod(typeof(CompColorable));
			MethodInfo wornApparelCountInfo = AccessTools.Method(typeof(Pawn_ApparelTracker), "get_WornApparelCount");
			byte patchActionStage = 0;

			Label label8 = ilg.DefineLabel();
			Label label9 = ilg.DefineLabel();
			LocalBuilder local6 = ilg.DeclareLocal(typeof(int));
			LocalBuilder local3 = ilg.DeclareLocal(typeof(CompColorable));

			StringBuilder stringBuilder = new StringBuilder();

			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0 && ins.opcode == OpCodes.Stloc_1)
				{
					patchActionStage++;
					yield return ins.Appsb(stringBuilder);
					yield return new CodeInstruction(OpCodes.Ldc_I4_0).Appsb(stringBuilder);
					yield return new CodeInstruction(OpCodes.Stloc_S, local6).Appsb(stringBuilder);
					continue;
				}
				else if (patchActionStage == 1 && ins.opcode == OpCodes.Call && ins.operand == tryGetCompInfo as object)
				{
					patchActionStage++;
					yield return ins.Appsb(stringBuilder);
					continue;
				}
				else if (patchActionStage == 2 && ins.opcode == OpCodes.Stloc_3)
				{
					patchActionStage++;
					yield return new CodeInstruction(OpCodes.Stloc_S, local3).Appsb(stringBuilder);
					continue;
				}
				else if (patchActionStage == 3 && ins.opcode == OpCodes.Ldloc_3)
				{
					patchActionStage++;
					yield return new CodeInstruction(OpCodes.Ldloc_S, local3).Appsb(stringBuilder);
					yield return new CodeInstruction(OpCodes.Brtrue_S, label8).Appsb(stringBuilder);
					yield return new CodeInstruction(OpCodes.Ldloc_S, local6).Appsb(stringBuilder);
					yield return new CodeInstruction(OpCodes.Ldc_I4_1).Appsb(stringBuilder);
					yield return new CodeInstruction(OpCodes.Add).Appsb(stringBuilder);
					yield return new CodeInstruction(OpCodes.Stloc_S, local6).Appsb(stringBuilder);
					yield return new CodeInstruction(OpCodes.Br_S, label9).Appsb(stringBuilder);
					yield return new CodeInstruction(OpCodes.Ldloc_S, local3) { labels = { label8 } }.Appsb(stringBuilder);
					continue;
				}
				else if (patchActionStage == 4 && ins.opcode == OpCodes.Ldloc_3 && ins.operand == null)
				{
					patchActionStage++;
					yield return new CodeInstruction(OpCodes.Ldloc_S, local3).Appsb(stringBuilder);
					continue;
				}
				else if (patchActionStage == 5 && ins.opcode == OpCodes.Ldloca_S && ((LocalBuilder)ins.operand).LocalIndex == 2)
				{
					patchActionStage++;
					ins.labels.Add(label9);
					yield return ins.Appsb(stringBuilder);
					continue;
				}
				else if (patchActionStage == 6 && ins.opcode == OpCodes.Callvirt && ins.operand == wornApparelCountInfo as object)
				{
					patchActionStage++;
					yield return ins.Appsb(stringBuilder);
					continue;
				}
				else if (patchActionStage == 7 && ins.opcode == OpCodes.Conv_R4)
				{
					patchActionStage++;
					yield return ins.Appsb(stringBuilder);
					yield return new CodeInstruction(OpCodes.Ldloc_2).Appsb(stringBuilder);
					yield return new CodeInstruction(OpCodes.Conv_R4).Appsb(stringBuilder);
					yield return new CodeInstruction(OpCodes.Sub).Appsb(stringBuilder);
					continue;
				}
				else
				{
					yield return ins.Appsb(stringBuilder);
					continue;
				}
			}
			Log.Message($"[Explorite]instr result ({patchActionStage}):\n" + stringBuilder.ToString());
			TranspilerStageCheckout(patchActionStage, 8);
			yield break;
		}
		*/
		public static int PawnNonColorableApparelCount(Pawn p)
		{
			return p.apparel.WornApparel.Where(app => app.TryGetComp<CompColorable>() == null).Count();
		}
		///<summary>使服装颜色想法进行对<see cref = "CompColorable" />的非空检查，方案2。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> ThoughtWorkerWearingColorCurrentStateInternalTranspilerB(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			MethodInfo tryGetCompInfo = AccessTools.Method(typeof(ThingCompUtility), nameof(ThingCompUtility.TryGetComp)).MakeGenericMethod(typeof(CompColorable));
			MethodInfo wornApparelCountInfo = AccessTools.Method(typeof(Pawn_ApparelTracker), "get_WornApparelCount");
			byte patchActionStage = 0;
			Label label8 = ilg.DefineLabel();
			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0 && ins.opcode == OpCodes.Call && ins.operand == tryGetCompInfo as object)
				{
					patchActionStage++;
					yield return ins;
					continue;
				}
				else if (patchActionStage == 1 && ins.opcode == OpCodes.Ldloc_3)
				{
					patchActionStage++;
					yield return new CodeInstruction(OpCodes.Ldloc_3);
					yield return new CodeInstruction(OpCodes.Brfalse_S, label8);
					yield return ins;
					continue;
				}
				else if (patchActionStage == 2 && ins.opcode == OpCodes.Ldloca_S && ((LocalBuilder)ins.operand).LocalIndex == 2)
				{
					patchActionStage++;
					ins.labels.Add(label8);
					yield return ins;
					continue;
				}
				else if (patchActionStage == 3 && ins.opcode == OpCodes.Callvirt && ins.operand == wornApparelCountInfo as object)
				{
					patchActionStage++;
					yield return ins;
					yield return new CodeInstruction(OpCodes.Ldarg_1);
					yield return new CodeInstruction(OpCodes.Call, ((Func<Pawn, int>)PawnNonColorableApparelCount).GetMethodInfo());
					yield return new CodeInstruction(OpCodes.Sub);
					continue;
				}
				else
				{
					yield return ins;
					continue;
				}
			}
			TranspilerStageCheckout(patchActionStage, 4);
			yield break;
		}
		public static IEnumerable<Apparel> PawnNonColorableApparels(IEnumerable<Apparel> apparels)
		{
			return apparels.Where(app => app.TryGetComp<CompColorable>() != null).ToList();
		}
		///<summary>使梳妆台服装颜色选单进行对<see cref = "Dialog_StylingStation" />的非空检查。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> DialogStylingStationDrawApparelColorTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			MethodInfo wornApparelInfo = AccessTools.Method(typeof(Pawn_ApparelTracker), "get_WornApparel");
			byte patchActionStage = 0;
			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0 && ins.opcode == OpCodes.Callvirt && ins.operand == wornApparelInfo as object)
				{
					patchActionStage++;
					yield return ins;
					yield return new CodeInstruction(OpCodes.Call, ((Func<IEnumerable<Apparel>, IEnumerable<Apparel>>)PawnNonColorableApparels).GetMethodInfo());
					continue;
				}
				else
				{
					yield return ins;
					continue;
				}
			}
			TranspilerStageCheckout(patchActionStage, 1);
			yield break;
		}
		public static bool NoRitualIdeo(Precept_Ritual ritual, PreceptDef precept)
		{
			return !ritual.ideo.RolesListForReading.Any((Precept_Role r) => r.def == precept);
		}
		///<summary>使仪式可用性进行对<see cref = "List&#60;Precept_Role&#62;" />的合法性检查。</summary>
		public static IEnumerable<CodeInstruction> RitualBehaviorWorkerCanStartRitualNowTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg, string preceptDefName)
		{
			Label label = ilg.DefineLabel();
			byte patchActionStage = 0;
			yield return new CodeInstruction(OpCodes.Ldarg_2);
			yield return new CodeInstruction(OpCodes.Ldsfld, typeof(PreceptDefOf).GetField(preceptDefName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static));
			yield return new CodeInstruction(OpCodes.Call, ((Func<Precept_Ritual, PreceptDef, bool>)NoRitualIdeo).GetMethodInfo());
			yield return new CodeInstruction(OpCodes.Brfalse_S, label);
			yield return new CodeInstruction(OpCodes.Ldstr, "CantStartRitualNoConvertee");
			yield return new CodeInstruction(OpCodes.Ldsfld, typeof(PreceptDefOf).GetField(preceptDefName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static));
			yield return new CodeInstruction(OpCodes.Ldfld, typeof(Def).GetField(nameof(Def.label), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
			yield return new CodeInstruction(OpCodes.Call, typeof(NamedArgument).GetMethod("op_Implicit", new Type[] { typeof(String) }));
			yield return new CodeInstruction(OpCodes.Call, ((Func<string, NamedArgument, TaggedString>)TranslatorFormattedStringExtensions.Translate).GetMethodInfo());
			yield return new CodeInstruction(OpCodes.Call, typeof(TaggedString).GetMethod("op_Implicit", new Type[] { typeof(TaggedString) }));
			yield return new CodeInstruction(OpCodes.Ret);
			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0)
				{
					patchActionStage++;
					ins.labels.Add(label);
				}
				yield return ins;
				continue;
			}
			TranspilerStageCheckout(patchActionStage, 1);
			yield break;
		}
		///<summary>使仪式可用性进行对<see cref = "List&#60;Precept_Role&#62;" />的合法性检查。</summary>
		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> RitualBehaviorWorkerCanStartRitualNowTranspiler_Speech(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			return RitualBehaviorWorkerCanStartRitualNowTranspiler(instr, ilg, "IdeoRole_Leader");
		}
		///<summary>使仪式可用性进行对<see cref = "List&#60;Precept_Role&#62;" />的合法性检查。</summary>
		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> RitualBehaviorWorkerCanStartRitualNowTranspiler_Conversion(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			return RitualBehaviorWorkerCanStartRitualNowTranspiler(instr, ilg, "IdeoRole_Moralist");
		}

		public static IEnumerable<CultureDef> CultureDefsWithoutExclusive(this IEnumerable<CultureDef> defs)
		{
			return defs.Where(def => !(def?.allowedPlaceTags.Contains("ExExclusive") ?? false));
		}
		///<summary>禁止无预设文化阵营随机选择阵营限定文化。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> IdeoFoundationRandomizeCultureTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			byte patchActionStage = 0;
			MethodInfo allCultureDefsInfo = AccessTools.Method(typeof(DefDatabase<CultureDef>), "get_AllDefsListForReading");
			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0 && ins.opcode == OpCodes.Call && ins.operand == allCultureDefsInfo as object)
				{
					patchActionStage++;
					yield return ins;
					yield return new CodeInstruction(OpCodes.Call, ((Func<IEnumerable<CultureDef>, IEnumerable<CultureDef>>)CultureDefsWithoutExclusive).GetMethodInfo());
					continue;
				}
				else
				{
					yield return ins;
					continue;
				}
			}
			TranspilerStageCheckout(patchActionStage, 1);
			yield break;
		}
		public static void GenIdeoBeforePlayerAction(Page_ConfigureIdeo page, Ideo ideo)
		{
			if (SpecPFacInGame())
			{
				if (Find.IdeoManager.IdeosListForReading.Any(ideo => Faction.OfPlayer.ideos.AllIdeos.Contains(ideo)))
				{
					return;
				}
				if (ModsConfig.IdeologyActive)
				{
					ideo = IdeoGenerator.GenerateIdeo(new IdeoGenerationParms()
					{
						forFaction = Faction.OfPlayer.def
					});
				}
				else
				{
					ideo = IdeoGenerator.GenerateNoExpansionIdeo((((Func<List<CultureDef>, List<CultureDef>>)((x) => x.Any() ? x : null))(Faction.OfPlayer.def.allowedCultures) ?? DefDatabase<CultureDef>.AllDefs.Where(def => !(def?.allowedPlaceTags.Contains("ExExclusive") ?? false))).RandomElement(), new IdeoGenerationParms()
					{
						forFaction = Faction.OfPlayer.def,
						forceNoExpansionIdeo = true
					});
				}
				Find.IdeoManager.Add(ideo);
				Faction.OfPlayer.ideos.SetPrimary(ideo);
				page.SelectOrMakeNewIdeo(ideo);
			}
		}
		///<summary>为文化选择界面先生成文化。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> PageConfigureIdeoPostOpenTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			byte patchActionStage = 0;
			MethodInfo allCultureDefsInfo = AccessTools.Method(typeof(DefDatabase<CultureDef>), "get_AllDefsListForReading");
			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0 && ins.opcode == OpCodes.Ldstr && ins.operand == "PageStart-ConfigureIdeo" as object)
				{
					patchActionStage++;
					yield return new CodeInstruction(OpCodes.Ldarg_0) { labels = ins.labels.ToList() };
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldfld, typeof(Page_ConfigureIdeo).GetField(nameof(Page_ConfigureIdeo.ideo), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
					yield return new CodeInstruction(OpCodes.Call, ((Action<Page_ConfigureIdeo, Ideo>)GenIdeoBeforePlayerAction).GetMethodInfo());
					ins.labels.Clear();
					yield return ins;
					continue;
				}
				else
				{
					yield return ins;
					continue;
				}
			}
			TranspilerStageCheckout(patchActionStage, 1);
			yield break;
		}
		///<summary>检测阵营限定模因。</summary>
		[HarmonyPostfix] public static bool IsMemeExclusiveToFaction(MemeDef meme)
		{
			return meme is MemeDef_Ex memeEx && (memeEx.exclusiveTo?.Any() ?? false);
		}
		///<summary>特定阵营存在时，锁定文化选项。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> DialogChooseMemesPlayerNHiddenOffTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			byte patchActionStage = 0;
			MethodInfo EnumeratorCurrentInfo = AccessTools.Method(typeof(IEnumerator<Faction>), "get_Current");
			FieldInfo FactionDefIsPlayerInfo = AccessTools.Field(typeof(FactionDef), "isPlayer");
			FieldInfo FactionDefHiddenInfo = AccessTools.Field(typeof(FactionDef), "hidden");
			Label label = ilg.DefineLabel();
			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0 && ins.opcode == OpCodes.Callvirt && ins.operand == EnumeratorCurrentInfo as object)
				{
					patchActionStage++;
					yield return ins;
					continue;
				}
				else if (patchActionStage == 1)
				{
					patchActionStage++;
					yield return ins;
					yield return new CodeInstruction(OpCodes.Call, ((Func<bool>)SpecPFacInGame).GetMethodInfo());
					yield return new CodeInstruction(OpCodes.Brtrue_S, label);
					yield return new CodeInstruction(OpCodes.Ldarg_1);
					yield return new CodeInstruction(OpCodes.Call, ((Predicate<MemeDef>)IsMemeExclusiveToFaction).GetMethodInfo());
					yield return new CodeInstruction(OpCodes.Brtrue_S, label);
					continue;
				}
				else if ((patchActionStage == 2 || patchActionStage == 3) && ins.opcode == OpCodes.Ldfld && (ins.operand == FactionDefIsPlayerInfo as object || ins.operand == FactionDefHiddenInfo as object))
				{
					patchActionStage++;
					yield return ins;
					continue;
				}
				else if(patchActionStage == 4 && (ins.opcode == OpCodes.Brtrue || ins.opcode == OpCodes.Brtrue_S))
				{
					patchActionStage++;
					yield return ins;
					continue;
				}
				else if (patchActionStage == 5)
				{
					patchActionStage++;
					ins.labels.Add(label);
					yield return ins;
					continue;
				}
				else
				{
					yield return ins;
					continue;
				}
			}
			//Log.Message($"[Explorite]instr result:\n" + sb.ToString());
			TranspilerStageCheckout(patchActionStage, 6);
			yield break;
		}
		///<summary>防止隐藏阵营随机为特殊玩家阵营。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> IdeoUIUtilityFactionForRandomizationTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			byte patchActionStage = 0;
			MethodInfo FindFactionManagerInfo = AccessTools.Method(typeof(Find), "get_FactionManager");
			MethodInfo FactionManagerGetAllFactionsInfo = AccessTools.Method(typeof(FactionManager), "get_AllFactions");
			Label label1 = ilg.DefineLabel();
			Label label2 = ilg.DefineLabel();
			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0 && ins.opcode == OpCodes.Call && ins.operand == FindFactionManagerInfo as object)
				{
					patchActionStage++;
					yield return ins;
					yield return new CodeInstruction(OpCodes.Call, ((Func<bool>)SpecPFacInGame).GetMethodInfo());
					yield return new CodeInstruction(OpCodes.Brfalse_S, label1);
					yield return new CodeInstruction(OpCodes.Callvirt, FactionManagerGetAllFactionsInfo);
					yield return new CodeInstruction(OpCodes.Br_S, label2);
					continue;
				}
				else if (patchActionStage == 1)
				{
					patchActionStage++;
					ins.labels.Add(label1);
					yield return ins;
					continue;
				}
				else if (patchActionStage == 2)
				{
					patchActionStage++;
					ins.labels.Add(label2);
					yield return ins;
					continue;
				}
				else
				{
					yield return ins;
					continue;
				}
			}
			TranspilerStageCheckout(patchActionStage, 3);
			yield break;
		}

		/*
		private static MethodInfo strangeMethod = null;
		///<summary>寻找一个奇怪的方法。</summary>
		[Obsolete][HarmonyTranspiler]public static IEnumerable<CodeInstruction> StrangeMethodFinderTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			MethodInfo method = null;
			bool find = false;
			foreach (CodeInstruction ins in instr)
			{
				if (!find && ins.opcode == OpCodes.Callvirt)
				{
					method = ins.operand as MethodInfo;
				}
				if (!find && ins.opcode == OpCodes.Ldstr && ins.operand == "NotAllowedForFaction" as object)
				{
					find = true;
				}
				yield return ins;
			}
			strangeMethod = method;
			yield break;
		}
		///<summary>特定阵营存在时，锁定文化选项。</summary>
		[Obsolete][HarmonyTranspiler]public static IEnumerable<CodeInstruction> IdeoUIUtility_MT_LT_c__DisplayClass59_0__MT_DoName_LT_g____CultureAllowed_1_Transpiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			byte patchActionStage = 0;
			MethodInfo EnumeratorCurrentInfo = AccessTools.Method(typeof(IEnumerator<Faction>), "get_Current");
			FieldInfo FactionDefHiddenInfo = AccessTools.Field(typeof(FactionDef), "hidden");
			Label label = ilg.DefineLabel();
			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0 && ins.opcode == OpCodes.Callvirt && ins.operand == EnumeratorCurrentInfo as object)
				{
					patchActionStage++;
					yield return ins;
					continue;
				}
				else if (patchActionStage == 1)
				{
					patchActionStage++;
					yield return ins;
					yield return new CodeInstruction(OpCodes.Call, ((Func<bool>)SpecPFacInGame).GetMethodInfo());
					yield return new CodeInstruction(OpCodes.Brtrue_S, label);
					continue;
				}
				else if (patchActionStage == 2 && ins.opcode == OpCodes.Ldfld && ins.operand == FactionDefHiddenInfo as object)
				{
					patchActionStage++;
					yield return ins;
					continue;
				}
				else if (patchActionStage == 3 && (ins.opcode == OpCodes.Brtrue || ins.opcode == OpCodes.Brtrue_S))
				{
					patchActionStage++;
					yield return ins;
					continue;
				}
				else if (patchActionStage == 4)
				{
					patchActionStage++;
					ins.labels.Add(label);
					yield return ins;
					continue;
				}
				else
				{
					yield return ins;
					continue;
				}
			}
			TranspilerStageCheckout(patchActionStage, 5);
			yield break;
		}
		*/
		/*
		public static int MinMemePostfix(int value, Dialog_ChooseMemes dialog)
		{
			if (dialog.GetType().GetField("ideo").GetValue(dialog) is Ideo ideo)
			{
			
			}
			return value;
		}
		///<summary>特定结构模因存在时，移除模因数量下限。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> DialogChooseMemesSpecStructCountinTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			byte patchActionStage = 0;
			FieldInfo MemeCountRangeAbsoluteInfo = AccessTools.Field(typeof(IdeoFoundation), nameof(IdeoFoundation.MemeCountRangeAbsolute));
			FieldInfo IntRangeMinInfo = AccessTools.Field(typeof(IntRange), nameof(IntRange.min));

			Label label = ilg.DefineLabel();
			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0 && ins.opcode == OpCodes.Ldsflda && ins.operand == MemeCountRangeAbsoluteInfo as object)
				{
					patchActionStage++;
					yield return ins;
					continue;
				}
				else if (patchActionStage == 1)
				{
					patchActionStage--;
					yield return ins;
					if (ins.opcode == OpCodes.Ldfld && ins.operand == IntRangeMinInfo as object)
					{
						yield return new CodeInstruction(OpCodes.Ldarg_0);
						yield return new CodeInstruction(OpCodes.Call, ((Func<bool>)SpecPFacInGame).GetMethodInfo());
					}
					continue;
				}
				else
				{
					yield return ins;
					continue;
				}
			}
			TranspilerStageCheckout(patchActionStage, 2);
			yield break;
		}
		*/

		/*
		static bool DoName_g__CultureAllowed_1(CultureDef cultureDef)
		{
			if (IdeoUIUtility.devEditMode)
			{
				return true;
			}
			if (Find.World == null)
			{
				return Find.FactionManager.OfPlayer.def.allowedCultures.Contains(cultureDef);
			}
			foreach (Faction faction in Find.FactionManager.AllFactions)
			{
				if (!faction.def.hidden && !faction.def.allowedCultures.NullOrEmpty<CultureDef>() && faction.ideos != null && (faction.ideos.IsPrimary(this.ideo) || faction.ideos.IsMinor(this.ideo)) && !faction.def.allowedCultures.Contains(cultureDef))
				{
					return false;
				}
			}
			return true;
		}
		*/

		public static bool SpecStructMemePredicate(MemeDef meme)
		{
			return meme is MemeDef_Ex meex && meex.category == MemeCategory.Structure && meex.countForNonStructureGroup;
		}
		///<summary>使特殊结构模因被计数。</summary>
		[HarmonyPostfix]public static void DialogChooseMemesGetMemeCountPostfix(MemeCategory category, List<MemeDef> ___newMemes, Dialog_ChooseMemes __instance, ref int __result)
		{
			if (category == MemeCategory.Normal
			 && ___newMemes != null)
			{
				__result += ___newMemes.Count(SpecStructMemePredicate) * 4;
			}
		}
		/*
		///<summary>使特殊结构模因被计数。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> DialogChooseMemesc___Transpiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			foreach (CodeInstruction ins in instr)
			{
				if (ins.opcode == OpCodes.Ret)
				{
					yield return new CodeInstruction(OpCodes.Ldarg_1);
					yield return new CodeInstruction(OpCodes.Call, ((Predicate<MemeDef>)SpecStructMemePredicate).GetMethodInfo());
					yield return new CodeInstruction(OpCodes.Or);
					yield return ins;
					continue;
				}
				else
				{
					yield return ins;
					continue;
				}
			}
			yield break;
		}
		*/
		public static bool SpecStructMemeInDialogPredicate(object instance)
		{
			return instance.GetType().GetField("ideo", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(instance) is Ideo ideo && ideo.memes.Any(SpecStructMemePredicate);
		}
		///<summary>使特殊结构模因被计数。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> DialogChooseMemesAnyMemeTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			byte patchActionStage = 0;
			//MethodInfo AnyMemeInfo = AccessTools.Method(typeof(GenCollection), nameof(GenCollection.Any), new Type[] { typeof(List<MemeDef>), typeof(Predicate<MemeDef>) }, new Type[] { typeof(MemeDef) });
			//MethodInfo AnyMemeInfo = typeof(GenCollection).GetMethod(nameof(GenCollection.Any), new Type[] { typeof(List<MemeDef>), typeof(Predicate<MemeDef>) }).MakeGenericMethod(typeof(MemeDef));
			//MethodInfo AnyMemeInfo = typeof(GenCollection).GetMethods().First(m => m.Name == nameof(GenCollection.Any) && m. == new Type[] { typeof(List<object>), typeof(Predicate<object>) }).MakeGenericMethod(typeof(MemeDef));

			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0 && ins.opcode == OpCodes.Call && ins.operand is MethodInfo method
				 && method.DeclaringType == typeof(GenCollection)
				 && method.Name == nameof(GenCollection.Any)
				 && method.GetParameters().Any(p => p.ParameterType == typeof(Predicate<MemeDef>))
					)
				{
					patchActionStage++;
					yield return ins;
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, ((Predicate<object>)SpecStructMemeInDialogPredicate).GetMethodInfo());
					yield return new CodeInstruction(OpCodes.Or);
					continue;
				}
				else
				{
					yield return ins;
					continue;
				}
			}
			TranspilerStageCheckout(patchActionStage, 1);
			yield break;
		}

		public static bool PageConfigureIdeoCanDoNextPatch(Page_ConfigureIdeo page)
		{
			Faction player = Find.FactionManager.OfPlayer;
			if (IsSpecFac(player.def)
			  && page.ideo != player.ideos.PrimaryIdeo)
			{
				Messages.Message("MessageIdeoForcedIdeoNotSelected".Translate(player.Name).CapitalizeFirst(), MessageTypeDefOf.RejectInput, false);
				return true;
			}
			return false;
		}
		///<summary>阻止半人马和Sayers阵营选择其他文化。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> PageConfigureIdeoCanDoNextTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			byte patchActionStage = 0;
			MethodInfo FactionOfPlayerInfo = AccessTools.Method(typeof(Faction), "get_OfPlayer");
			Label label = ilg.DefineLabel();
			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0 && ins.opcode == OpCodes.Call && ins.operand == FactionOfPlayerInfo as object)
				{
					patchActionStage++;
					yield return new CodeInstruction(OpCodes.Ldarg_0) { labels = ins.labels.ToList() }; ins.labels.Clear();
					yield return new CodeInstruction(OpCodes.Call, ((Func<Page_ConfigureIdeo, bool>)PageConfigureIdeoCanDoNextPatch).GetMethodInfo());
					yield return new CodeInstruction(OpCodes.Brfalse_S, label);
					yield return new CodeInstruction(OpCodes.Ldc_I4_0);
					yield return new CodeInstruction(OpCodes.Ret);
					ins.labels.Add(label);
					yield return ins;
					continue;
				}
				else
				{
					yield return ins;
					continue;
				}
			}
			TranspilerStageCheckout(patchActionStage, 1);
			yield break;
		}
		public static float PostProcessStatFactor(float value, StatWorker instance, Thing thing, StatDef factorDef)
		{
			if (instance.GetType().GetField("stat", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(instance) is StatDef stat
			 && thing is  Pawn pawn && pawn.def == AlienCentaurDef
			 && factorDef == StatDefOf.PsychicSensitivity && stat == StatDefOf.PsychicEntropyMax)
				{
				if (value * 3 < 1)
				{
					return value * 3;
				}
				else if (value < 3)
				{
					return 1;
				}
				else
				{
					return value - 2;
				}

			}
			return value;
		}
		///<summary>改变心灵敏感度对半人马心灵熵阈值的影响。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> StatWorkerGetValueUnfinalizedTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			byte patchActionStage = 0;
			byte proximityStat = 0;

			Queue<CodeInstruction> ins4 = new Queue<CodeInstruction>();

			FieldInfo StatFactorsInfo = typeof(StatDef).GetField(nameof(StatDef.statFactors), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			MethodInfo StatExtensionGetStatValueInfo = AccessTools.Method(typeof(StatExtension), "GetStatValue");
			MethodInfo StatRequestGetThingInfo = AccessTools.Method(typeof(StatRequest), "get_Thing");

			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0)
				{
					if (proximityStat > 0)
					{
						proximityStat--;
					}
					ins4.Enqueue(ins);
					if (ins.opcode == OpCodes.Ldfld && ins.operand == StatFactorsInfo as object)
					{
						proximityStat = 5;
					}
				}
				if (ins4.Count() > 7)
				{
					ins4.Dequeue();
				}

				if (proximityStat > 0 && patchActionStage == 0 && ins.opcode == OpCodes.Call && ins.operand == StatExtensionGetStatValueInfo as object)
				{
					patchActionStage++;
					yield return ins;
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldarga_S,1);
					yield return new CodeInstruction(OpCodes.Call, StatRequestGetThingInfo);
					yield return ins4.Dequeue();
					yield return ins4.Dequeue();
					yield return ins4.Dequeue();
					yield return ins4.Dequeue();
					yield return ins4.Dequeue(); ins4.Clear();
					yield return new CodeInstruction(OpCodes.Call, ((Func<float, StatWorker, Thing, StatDef, float>)PostProcessStatFactor).GetMethodInfo());
					continue;
				}
				else
				{
					yield return ins;
					continue;
				}
			}
			TranspilerStageCheckout(patchActionStage, 1);
			yield break;
		}
		///<summary>改变心灵敏感度对半人马心灵熵阈值的影响的数值显示。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> StatWorkerGetExplanationUnfinalizedTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			byte patchActionStage = 0;
			byte proximityStat = 0;

			FieldInfo StatFactorsInfo = typeof(StatDef).GetField(nameof(StatDef.statFactors), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			MethodInfo StatWorkerGetValueInfo = AccessTools.Method(typeof(StatWorker), "GetValue", new Type[] { typeof(StatRequest), typeof(bool) });
			MethodInfo StatRequestGetThingInfo = AccessTools.Method(typeof(StatRequest), "get_Thing");

			object opId = null;

			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0)
				{
					if (proximityStat > 0)
					{
						proximityStat--;
					}
					if (ins.opcode == OpCodes.Ldfld && ins.operand == StatFactorsInfo as object)
					{
						proximityStat = 4;
					}
					if (proximityStat > 0 && ins.opcode == OpCodes.Stloc_S)
					{
						opId = ins.operand;
						patchActionStage++;
					}
				}

				if (patchActionStage == 1 && ins.opcode == OpCodes.Callvirt && ins.operand == StatWorkerGetValueInfo as object)
				{
					patchActionStage++;
					yield return ins;
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldarga_S, 1);
					yield return new CodeInstruction(OpCodes.Call, StatRequestGetThingInfo);
					yield return new CodeInstruction(OpCodes.Ldloc_S, opId);
					yield return new CodeInstruction(OpCodes.Call, ((Func<float, StatWorker, Thing, StatDef, float>)PostProcessStatFactor).GetMethodInfo());
					continue;
				}
				else
				{
					yield return ins;
					continue;
				}
			}
			TranspilerStageCheckout(patchActionStage, 2);
			yield break;
		}

		///<summary>使半人马和Sayers不被判断为裸体。</summary>
		[HarmonyPrefix]public static bool ThoughtWorkerPreceptHasUncoveredPrefix(Pawn p, ref bool __result)
		{
			if (DisableThoughtWorkerPreceptHasUncoveredPredicate(p.def))
			{
				__result = false;
				return false;
			}
			return true;
		}

		///<summary>使半人马的头衔建筑物需求使用半人马床而非其他床铺。</summary>
		[HarmonyPostfix]public static void RoyalTitleDefGetBedroomRequirementsPostfix(Pawn p, RoyalTitleDef __instance, ref IEnumerable<RoomRequirement> __result)
		{
			if (p.def == AlienCentaurDef)//__instance.tags.Contains("EmpireTitle") && 
			{
				foreach (RoomRequirement roomRequirement in __result ?? Enumerable.Empty<RoomRequirement>())
				{
					if (roomRequirement is RoomRequirement_ThingAnyOf thingsRequirement && thingsRequirement.things.Any(tdef => tdef.HasComp(typeof(CompAssignableToPawn_Bed))) && !thingsRequirement.things.Contains(CentaurBedDef))
					{
						thingsRequirement.things.Add(CentaurBedDef);
					}
				}
			}
		}
		///<summary>使半人马的头衔服装需求需要半人马衣物，使Sayers的头衔服装需求不需要衣物。</summary>
		[HarmonyPostfix]public static void ApparelRequirementIsMetPostfix(Pawn p, ApparelRequirement __instance, ref bool __result)
		{
			if (!__result && p.def == AlienSayersDef)
			{
				__result = true;
				return;
			}
			if (!__result && p.def == AlienCentaurDef)
			{
				foreach (Apparel apparel in p.apparel.WornApparel)
				{
					if (apparel.def.apparel.tags.Contains("CentaurRoyale"))
					{
						__result = true;
						return;
					}
				}
			}
		}
		///<summary>锁定文化代表颜色。</summary>
		[HarmonyPrefix]public static bool IdeoApparelColorPrefix(Ideo __instance, ref Color __result)
		{
			if (__instance.memes.Any(meme => meme is MemeDef_Ex meEx && meEx.lockAccuIdeoColor))
			{
				__result = __instance.Color;
				return false;
			}
			return true;
		}

		///<summary>跳过文化预设。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> PageChooseIdeoPresetPostOpenTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			byte patchActionStage = 0;
			MethodInfo WindowPostOpenInfo = AccessTools.Method(typeof(Window), "PostOpen");
			MethodInfo PageChooseIdeoPresetDoCustomizeInfo = AccessTools.Method(typeof(Page_ChooseIdeoPreset), "DoCustomize");
			FieldInfo next = typeof(Page).GetField(nameof(Page.next), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			FieldInfo prev = typeof(Page).GetField(nameof(Page.prev), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			Label label = ilg.DefineLabel();
			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0 && ins.opcode == OpCodes.Call && ins.operand == WindowPostOpenInfo as object)
				{
					patchActionStage++;
					yield return ins;
					yield return new CodeInstruction(OpCodes.Call, ((Func<bool>)SpecPFacInGame).GetMethodInfo());
					yield return new CodeInstruction(OpCodes.Brfalse_S, label);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldc_I4_0);
					yield return new CodeInstruction(OpCodes.Call, PageChooseIdeoPresetDoCustomizeInfo);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldfld, prev);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldfld, next);
					yield return new CodeInstruction(OpCodes.Ldfld, prev);
					yield return new CodeInstruction(OpCodes.Stfld, next);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldfld, next);
					yield return new CodeInstruction(OpCodes.Ldfld, prev);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldfld, prev);
					yield return new CodeInstruction(OpCodes.Stfld, prev);
					yield return new CodeInstruction(OpCodes.Ret);
					continue;
				}
				if (patchActionStage == 1)
				{
					patchActionStage++;
					ins.labels.Add(label);
					yield return ins;
					continue;
				}
				else
				{
					yield return ins;
					continue;
				}
			}
			TranspilerStageCheckout(patchActionStage, 2);
			yield break;
		}

		///<summary>增加特殊标签的显示。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> GenLabelNewThingLabelTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			byte patchActionStage = 0;
			MethodInfo StringConcatInfo = AccessTools.Method(typeof(string), nameof(string.Concat), new Type[] { typeof(string), typeof(string) });

			Label label1 = ilg.DefineLabel();
			Label label2 = ilg.DefineLabel();
			LocalBuilder localBool = ilg.DeclareLocal(typeof(bool));
			bool lastOr = false;
			Queue<LocalBuilder> localBools = new Queue<LocalBuilder>();

			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0 && ins.opcode == OpCodes.Stloc_S && ((LocalBuilder)ins.operand).LocalIndex == 8)
				{
					patchActionStage++;
					yield return ins;
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, ((Func<Thing, bool>)ItemStageUtility.AnyItemStageLabels).GetMethodInfo());
					yield return new CodeInstruction(OpCodes.Stloc_S, localBool);
				}
				else if (patchActionStage == 1 && lastOr && (ins.opcode == OpCodes.Brfalse || ins.opcode == OpCodes.Brfalse_S))
				{
					patchActionStage++;
					yield return new CodeInstruction(OpCodes.Ldloc_S, localBool);
					yield return new CodeInstruction(OpCodes.Or);
					yield return ins;
				}
				else if (patchActionStage == 2 && ins.opcode == OpCodes.Ldstr && ins.operand == " (" as object)
				{
					patchActionStage++;
					yield return ins;
				}
				else if (patchActionStage == 3 && ins.opcode == OpCodes.Call && ins.operand == StringConcatInfo as object)
				{
					patchActionStage++;
					yield return ins;
				}
				else if (patchActionStage == 4 && ins.opcode == OpCodes.Stloc_0)
				{
					patchActionStage++;
					yield return ins;
					yield return new CodeInstruction(OpCodes.Ldloc_S, localBool);
					yield return new CodeInstruction(OpCodes.Brfalse_S, label1);
					yield return new CodeInstruction(OpCodes.Ldloc_0);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, ((Func<Thing, string>)ItemStageUtility.AllStateLabels).GetMethodInfo());
					yield return new CodeInstruction(OpCodes.Call, StringConcatInfo);
					yield return new CodeInstruction(OpCodes.Stloc_0);
					yield return new CodeInstruction(OpCodes.Ldloc_3);
					yield return new CodeInstruction(OpCodes.Ldloc_S, localBools.Dequeue());
					yield return new CodeInstruction(OpCodes.Or);
					yield return new CodeInstruction(OpCodes.Ldloc_S, localBools.Dequeue());
					yield return new CodeInstruction(OpCodes.Or);
					yield return new CodeInstruction(OpCodes.Brfalse_S, label2);
					yield return new CodeInstruction(OpCodes.Ldloc_0);
					yield return new CodeInstruction(OpCodes.Ldstr, " ");
					yield return new CodeInstruction(OpCodes.Call, StringConcatInfo);
					yield return new CodeInstruction(OpCodes.Stloc_0);
				}
				else if (patchActionStage == 5)
				{
					patchActionStage++;
					ins.labels.Add(label1);
					ins.labels.Add(label2);
					yield return ins;
				}
				else
				{
					yield return ins;
				}

				if (patchActionStage <= 1)
				{
					if (ins.opcode == OpCodes.Ldloc_S)
					{
						localBools.Enqueue(ins.operand as LocalBuilder);
					}
					if (localBools.Count() > 2)
					{
						localBools.Dequeue();
					}
					lastOr = ins.opcode == OpCodes.Or;
				}
			}
			TranspilerStageCheckout(patchActionStage, 6);
			yield break;
		}

		///<summary>特殊标签禁止使用缓存机制。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> GenLabelThingLabelTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			byte patchActionStage = 0;
			MethodInfo NewThingLabelInfo = AccessTools.Method(typeof(GenLabel), "NewThingLabel", new Type[] { typeof(Thing), typeof(int), typeof(bool) });
			Label label = ilg.DefineLabel();

			yield return new CodeInstruction(OpCodes.Ldarg_0);
			yield return new CodeInstruction(OpCodes.Call, ((Func<Thing, bool>)ItemStageUtility.AnyItemStageLabels).GetMethodInfo());
			yield return new CodeInstruction(OpCodes.Brfalse_S, label);
			yield return new CodeInstruction(OpCodes.Ldarg_0);
			yield return new CodeInstruction(OpCodes.Ldarg_1);
			yield return new CodeInstruction(OpCodes.Ldarg_2);
			yield return new CodeInstruction(OpCodes.Call, NewThingLabelInfo);
			yield return new CodeInstruction(OpCodes.Ret);
			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0)
				{
					patchActionStage++;
					ins.labels.Add(label);
					yield return ins;
				}
				else
				{
					yield return ins;
				}
			}
			TranspilerStageCheckout(patchActionStage, 1);
			yield break;
		}

		///<summary>允许特定文化摘取头颅。</summary>
		[HarmonyPostfix]public static void WorkGiverExtractSkullCanExtractSkullPostfix(Ideo ideo, ref bool __result)
		{
			if (!__result && ideo.PreceptsListForReading.Any(p => p.def.defName == "Skullspike_Doubt_Sayers"))//ideo.IsSayersIdeo())
			{
				__result = true;
			}
		}

		///<summary>禁止种族食物中毒。</summary>
		[HarmonyPrefix]public static bool FoodUtilityGetFoodPoisonChanceFactorPrefix(Pawn ingester, ref float __result)
		{
			if (DisableFoodPoisoningPredicate(ingester.def))
			{
				__result = 0f;
				return false;
			}
			return true;
		}

		public static bool DisablePassionDrawPawn(Pawn pawn)
		{
			return DisablePassionDrawPredicate(pawn.def);
		}
		///<summary>禁止半人马技能页显示兴趣度图标。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> SkillUIDrawSkillTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			byte patchActionStage = 0;
			FieldInfo SkillRecordPassionInfo = typeof(SkillRecord).GetField(nameof(SkillRecord.passion), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			MethodInfo SkillRecordPawnInfo = typeof(SkillRecord).GetMethod("get_Pawn", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0 && ins.opcode == OpCodes.Ldarg_0)
				{
					patchActionStage++;
					yield return ins;
				}
				else if (patchActionStage == 1)
				{
					yield return ins;
					if (ins.opcode == OpCodes.Ldfld && ins.operand == SkillRecordPassionInfo as object)
						patchActionStage++;
					else
						patchActionStage = 0;
				}
				else if (patchActionStage == 2)
				{
					yield return ins;
					if (ins.opcode == OpCodes.Ldc_I4_0)
						patchActionStage++;
					else
						patchActionStage = 0;
				}
				else if (patchActionStage == 3)
				{
					yield return ins;
					if (ins.opcode == OpCodes.Ble_S)
					{
						patchActionStage++;
						yield return new CodeInstruction(OpCodes.Ldarg_0);
						yield return new CodeInstruction(OpCodes.Callvirt, SkillRecordPawnInfo);
						yield return new CodeInstruction(OpCodes.Call, ((Predicate<Pawn>)DisablePassionDrawPawn).Method);
						yield return new CodeInstruction(OpCodes.Brtrue_S, ins.operand);
					}
					else
					{
						patchActionStage = 0;
					}
				}
				else
				{
					yield return ins;
				}
			}
			TranspilerStageCheckout(patchActionStage, 4);
			yield break;
		}
		public static bool CanAutouseNeuralSuperchargerExtra(Pawn pawn)
		{
			return pawn?.def == AlienCentaurDef || (pawn?.ideo?.Ideo?.memes?.Contains(CentaurStructureMemeDef) ?? false);
		}
		///<summary>允许半人马自动使用神经超频。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> CompNeuralSuperchargerCanAutoUseTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			byte patchActionStage = 0;
			MethodInfo IdeoHasPreceptInfo = AccessTools.Method(typeof(Ideo), nameof(Ideo.HasPrecept));
			FieldInfo NeuralSuperchargePreferredPreceptInfo = typeof(PreceptDefOf).GetField(nameof(PreceptDefOf.NeuralSupercharge_Preferred), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0 && ins.opcode == OpCodes.Ldsfld && ins.operand == NeuralSuperchargePreferredPreceptInfo as object)
				{
					patchActionStage++;
					yield return ins;
					continue;
				}
				if (patchActionStage == 1 && ins.opcode == OpCodes.Callvirt && ins.operand == IdeoHasPreceptInfo as object)
				{
					patchActionStage++;
					yield return ins;
					yield return new CodeInstruction(OpCodes.Ldarg_1);
					yield return new CodeInstruction(OpCodes.Call, ((Predicate<Pawn>)CanAutouseNeuralSuperchargerExtra).GetMethodInfo());
					yield return new CodeInstruction(OpCodes.Or);
					continue;
				}
				else
				{
					yield return ins;
					continue;
				}
			}
			TranspilerStageCheckout(patchActionStage, 2);
			yield break;
		}

		public static bool DoPreventAbilityGainFromPsylink(Pawn pawn)
		{
			return DisableAbilityGainFromPsylinkPredicate(pawn?.def);
		}
		///<summary>禁止半人马灵能升级时获得灵能技能。</summary>
		[HarmonyTranspiler]public static IEnumerable<CodeInstruction> HediffPsylinkTryGiveAbilityOfLevelTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			byte patchActionStage = 0;
			MethodInfo IdeoHasPreceptInfo = AccessTools.Method(typeof(Ideo), nameof(Ideo.HasPrecept));
			FieldInfo HediffPawnInfo = typeof(Hediff).GetField(nameof(Hediff.pawn), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0 && ins.opcode == OpCodes.Call && ins.operand is MethodInfo method
				 && method.DeclaringType == typeof(GenCollection)
				 && method.Name == nameof(GenCollection.Any)
				 && method.GetParameters().Any(p => p.ParameterType == typeof(Predicate<Ability>))
					)
				{
					patchActionStage++;
					yield return ins;
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldfld, HediffPawnInfo);
					yield return new CodeInstruction(OpCodes.Call, ((Predicate<Pawn>)DoPreventAbilityGainFromPsylink).GetMethodInfo());
					yield return new CodeInstruction(OpCodes.Or);
					continue;
				}
				else
				{
					yield return ins;
					continue;
				}
			}
			TranspilerStageCheckout(patchActionStage, 1);
			yield break;
		}

		///<summary>禁止半人马发动打架。</summary>
		[HarmonyPrefix]public static bool PawnInteractionsTrackerSocialFightPossiblePrefix(Pawn_InteractionsTracker __instance, ref bool __result, Pawn otherPawn)
		{
			if (
				__instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
				&& DisableSocialFightPossiblePredicate(pawn.def)
				)
			{
				__result = false;
				return false;
			}
			return true;
		}

		///<summary>覆盖身体部件尺寸计算。</summary>
		[HarmonyPrefix]public static bool StatPartAddedBodyPartsMassGetAddedBodyPartsMassPrefix(Pawn p, ref float __result)
		{
			if (OverrideAddedBodyPartsMassPredicate(p.def))
			{
				__result = 0f;
				List<Hediff> hediffs = p.health.hediffSet.hediffs;
				foreach (Hediff hediff in hediffs)
				{
					if (hediff is Hediff_AddedPart hediff_AddedPart)
					{
						__result += hediff_AddedPart.Part.coverageAbs * p.def.GetStatValueAbstract(StatDefOf.Mass) * p.BodySize / p.def.race.baseBodySize;
						/*
						if (hediff_AddedPart.def.spawnThingOnRemoved != null)
						{
							__result += hediff_AddedPart.def.spawnThingOnRemoved.GetStatValueAbstract(StatDefOf.Mass, null) * 0.9f;
						}
						*/
					}
				}
				return false;
			}
			return true;
		}

		private static readonly MethodInfo AddLevels = typeof(QualityUtility).GetMethod("AddLevels", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

		///<summary>增加人物制造的物品的品质。</summary>
		[HarmonyPostfix]public static void QualityUtilityGenerateQualityCreatedByPawnPostfix(Pawn pawn, SkillDef relevantSkill, ref QualityCategory __result)
		{
			if (QualityIncreasedPredicate(pawn.def))
			{
				__result = (QualityCategory)AddLevels.Invoke(null, new object[] { __result, 1 });
			}
			while (__result < QualityCategory.Legendary && pawn.needs?.TryGetNeed<Need_CentaurCreativityInspiration>()?.TryConsume() == true)
			{
				__result = (QualityCategory)AddLevels.Invoke(null, new object[] { __result, 1 });
			}
		}
	}
}
