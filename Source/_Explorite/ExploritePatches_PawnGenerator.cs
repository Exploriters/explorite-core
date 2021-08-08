/********************
 * 对人物生成器的补丁。
 * --siiftun1857
 */
using System;
using RimWorld;
using HarmonyLib;
using Verse;
using static Explorite.ExploriteCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Explorite
{
	internal static partial class ExploritePatches
	{

		internal static bool GenerateCentaurPostprocess(ref Pawn pawn, PawnGenerationRequest request, ref bool matchError)
		{
			if (!InstelledMods.RimCentaurs)
			{
				return false;
			}
			if (matchError)
			{
				return false;
			}
			if (pawn.def != AlienCentaurDef)
			{
				if (pawn?.story?.childhood == CentaurCivilRetro)
				{
					pawn.story.childhood = BackstoryDatabase.ShuffleableBackstoryList(
						BackstorySlot.Childhood,
						new BackstoryCategoryFilter { categories = pawn.kindDef.backstoryCategories }
					).RandomElement();
				}
				return false;
			}
			if (pawn.kindDef.race != AlienCentaurDef)
			{
				matchError = true;
				return false;
			}
			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			
			pawn.relations.ClearAllRelations();

			pawn.story.bodyType = pawn.gender == Gender.Female ?
				DefDatabase<BodyTypeDef>.GetNamed("CentaurFemale") : DefDatabase<BodyTypeDef>.GetNamed("CentaurMale");

			//__result.abilities.abilities.Add(new Ability(__result, DefDatabase<AbilityDef>.GetNamed("MassPsychicDeafCentaur")));

			/*
			pawn.ageTracker.AgeChronologicalTicks = (long)Math.Floor(
				pawn.ageTracker.AgeChronologicalTicks * ((pawn.ageTracker.AgeBiologicalTicks + 360000000f) / pawn.ageTracker.AgeBiologicalTicks)
				) + 360000000;
			pawn.ageTracker.AgeBiologicalTicks += 360000000;
			*/
			pawn.ageTracker.AgeChronologicalTicks = Math.Max(pawn.ageTracker.AgeChronologicalTicks, (long)Math.Floor(pawn.ageTracker.AgeBiologicalTicks / 0.95f));

			pawn.story.traits.allTraits.Clear();
			/* if (pawn.story.hairDef == DefDatabase<HairDef>.GetNamed("Mohawk"))
			{
				pawn.story.hairDef = DefDatabase<HairDef>.GetNamed("Flowy");
			} */
			//__result.story.traits.GainTrait(new Trait(TraitDefOf.Asexual, 0, forced: true));

			pawn.story.favoriteColor = null;
			foreach (SkillRecord sr in pawn.skills.skills)
			{
				sr.Level =
					(pawn?.story?.childhood?.skillGainsResolved?.TryGetValue(sr.def) ?? 0) +
					(pawn?.story?.adulthood?.skillGainsResolved?.TryGetValue(sr.def) ?? 0) + 9;
				sr.passion = sr.passion switch
				{
					Passion.None => Passion.Minor,
					Passion.Minor => Passion.Major,
					Passion.Major => Passion.Major,
					_ => Passion.Minor,
				};
				sr.xpSinceLastLevel = 0f; //sr.XpRequiredForLevelUp / 2f;
			}

			pawn.EnsureSubsystemExist();
			return true;
		}
		internal static bool GenerateSayersPostprocess(ref Pawn pawn, PawnGenerationRequest request, ref bool matchError)
		{
			if (!InstelledMods.Sayers)
			{
				return false;
			}
			if (matchError)
			{
				return false;
			}
			if (pawn.def != AlienSayersDef)
			{
				return false;
			}
			if (pawn.kindDef.race != AlienSayersDef)
			{
				matchError = true;
				return false;
			}
			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// 在这里写后期处理
			//Bruh... I'm a fool :( -- Abrel
			/*
			pawn.ageTracker.AgeBiologicalTicks = Clamp(pawn.ageTracker.AgeBiologicalTicks / 10, 0, 3600000);
			pawn.ageTracker.AgeChronologicalTicks = pawn.ageTracker.AgeBiologicalTicks;
			*/

			pawn.relations.ClearAllRelations();

			/*
			Type AlienCompType = AccessTools.TypeByName("AlienRace.AlienPartGenerator.AlienComp");
			if (AccessTools.TypeByName("AlienRace.AlienPartGenerator.ExposableValueTuple")?.MakeGenericType(typeof(Color), typeof(Color))?.GetField("second")?.GetValue(AlienCompType.GetMethod("GetChannel")?.Invoke(pawn.AllComps.Find(c => AlienCompType.IsAssignableFrom(c.GetType())), new object[] { "skin" })) is Color color)
			{
				pawn.story.favoriteColor = color;
			}
			*/
			if (pawn.GetAlienRaceCompColor("skin", out _ , out Color? second))
			{
				pawn.story.favoriteColor = second.Value;
			}

			if (pawn.Name is NameTriple name)
			{
				//__result.Name = new NameTriple(name.Last, name.Last, null);
				string nameFirst = PawnNameDatabaseShuffled.BankOf(PawnNameCategory.HumanStandard).GetName(PawnNameSlot.First, Rand.Bool ? Gender.Female : Gender.Male);
				pawn.Name = new NameTriple(nameFirst, nameFirst, "Sayers");
			}
			return true;
		}
		internal static bool GenerateGuoguoPostprocess(ref Pawn pawn, PawnGenerationRequest request, ref bool matchError)
		{
			if (!InstelledMods.GuoGuo)
			{
				return false;
			}
			if (matchError)
			{
				return false;
			}
			if (pawn.def != AlienGuoguoDef)
			{
				return false;
			}
			if (pawn.kindDef.race != AlienGuoguoDef)
			{
				matchError = true;
				return false;
			}
			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			pawn.ageTracker.AgeBiologicalTicks = 0;
			pawn.ageTracker.AgeChronologicalTicks = 0;
			pawn.relations.ClearAllRelations();

			if (pawn.Name is NameTriple name)
			{
				//__result.Name = new NameTriple(name.Last, name.Last, null);
				string nameFirst = PawnNameDatabaseShuffled.BankOf(PawnNameCategory.HumanStandard).GetName(PawnNameSlot.First, Rand.Bool ? Gender.Female : Gender.Male);
				pawn.Name = new NameTriple(nameFirst, nameFirst, "Ringo");
			}

			pawn.health.RestorePart(pawn.RaceProps.body.corePart);
			pawn.health.hediffSet.hediffs.RemoveAll(hediff => hediff.def.isBad);

			if (!pawn.HasPsylink)
				pawn.ChangePsylinkLevel(1, false);

			foreach (SkillRecord sr in pawn.skills.skills)
			{
				sr.Level =
					(pawn?.story?.childhood?.skillGainsResolved?.TryGetValue(sr.def) ?? 0) +
					(pawn?.story?.adulthood?.skillGainsResolved?.TryGetValue(sr.def) ?? 0) + 9;

				sr.xpSinceLastLevel = 0f;
				sr.passion = sr.Level > 0 ? Passion.Major : Passion.None;
			}

			return true;
		}
		
		internal static bool FinalPostprocess(ref Pawn pawn, PawnGenerationRequest request)
		{
			if (pawn == null)
			{
				return false;
			}
			if (pawn?.TryGetComp<CompEnsureAbility>() != null)
			{
				pawn?.TryGetComp<CompEnsureAbility>().ApplayAbilities();
			}

			try
			{
				if (pawn?.RaceProps?.hediffGiverSets != null)
				{
					foreach (HediffGiverSetDef hediffGiverSetDef in pawn?.RaceProps?.hediffGiverSets ?? Enumerable.Empty<HediffGiverSetDef>())
					{
						foreach (HediffGiver hediffGiver in hediffGiverSetDef?.hediffGivers ?? Enumerable.Empty<HediffGiver>())
						{
							if (hediffGiver is HediffGiver_EnsureForAlways giver)
							{
								giver?.TryApply(pawn);
							}
						}
					}
				}
			}
			catch (NullReferenceException e)
			{
				Log.Message(string.Concat(
					$"[Explorite]an exception ({e.GetType().Name}) occurred during pawn generating.\n",
					$"Message:\n   {e.Message}\n",
					$"Stack Trace:\n{e.StackTrace}\n"
					));
			}
			catch (Exception e)
			{
				Log.Error(string.Concat(
					$"[Explorite]an exception ({e.GetType().Name}) occurred during pawn generating.\n",
					$"Message:\n   {e.Message}\n",
					$"Stack Trace:\n{e.StackTrace}\n"
					));
			}

			return true;
		}


		/**
		  * <summary>
		  * 对人物生成器的补丁。
		  * </summary>
		  */
		[HarmonyPostfix]
		public static void GeneratePawnPostfix(ref Pawn __result, PawnGenerationRequest request)
		{
			bool matchError = false;
			GenerateCentaurPostprocess(ref __result, request, ref matchError);
			GenerateSayersPostprocess(ref __result, request, ref matchError);
			GenerateGuoguoPostprocess(ref __result, request, ref matchError);

			if (matchError)
			{
				/*
				faction: null,
				context: PawnGenerationContext.NonPlayer,
				tile: -1,
				forceGenerateNewPawn: false,
				newborn: false,
				allowDead: false,
				allowDowned: false,
				canGeneratePawnRelations: true,
				mustBeCapableOfViolence: false,
				colonistRelationChanceFactor: 1,
				forceAddFreeWarmLayerIfNeeded: false,
				allowGay: true,
				allowFood: true,
				allowAddictions: true,
				inhabitant: false,
				certainlyBeenInCryptosleep: false,
				forceRedressWorldPawnIfFormerColonist: false,
				worldPawnFactionDoesntMatter: false,
				biocodeWeaponChance: 0,
				extraPawnForExtraRelationChance: null,
				relationWithExtraPawnChanceFactor: 1,
				validatorPreGear: null,
				validatorPostGear: null,
				forcedTraits: null,
				prohibitedTraits: null,
				minChanceToRedressWorldPawn: null,
				fixedBiologicalAge: null,
				fixedChronologicalAge: null,
				fixedGender: null,
				fixedMelanin: null,
				fixedLastName: null,
				fixedBirthName: null,
				fixedTitle: null
				*/
				//__result = PawnGenerator.GeneratePawn(PawnKindDefOf.Villager, request.Faction);
				/*new PawnGenerationRequest(
				kind: PawnKindDefOf.Villager,
				faction: request.Faction,
				context: request.Context,
				tile: request.Tile,
				forceGenerateNewPawn: request.ForceGenerateNewPawn,
				newborn: request.Newborn,
				allowDead: request.AllowDead,
				allowDowned: request.AllowDowned,
				canGeneratePawnRelations: request.CanGeneratePawnRelations,
				mustBeCapableOfViolence: request.MustBeCapableOfViolence,
				colonistRelationChanceFactor: request.ColonistRelationChanceFactor,
				forceAddFreeWarmLayerIfNeeded: request.ForceAddFreeWarmLayerIfNeeded,
				allowGay: request.AllowGay,
				allowFood: request.AllowFood,
				allowAddictions: request.AllowAddictions,
				inhabitant: request.Inhabitant,
				certainlyBeenInCryptosleep: request.CertainlyBeenInCryptosleep,
				forceRedressWorldPawnIfFormerColonist: request.ForceRedressWorldPawnIfFormerColonist,
				worldPawnFactionDoesntMatter: request.WorldPawnFactionDoesntMatter,
				biocodeWeaponChance: request.BiocodeWeaponChance,
				extraPawnForExtraRelationChance: request.ExtraPawnForExtraRelationChance,
				relationWithExtraPawnChanceFactor: request.RelationWithExtraPawnChanceFactor,
				validatorPreGear: request.ValidatorPreGear,
				validatorPostGear: request.ValidatorPostGear,
				forcedTraits: request.ForcedTraits,
				prohibitedTraits: request.ProhibitedTraits,
				minChanceToRedressWorldPawn: request.MinChanceToRedressWorldPawn,
				fixedBiologicalAge: request.FixedBiologicalAge,
				fixedChronologicalAge: request.FixedChronologicalAge,
				fixedGender: request.FixedGender,
				fixedMelanin: request.FixedMelanin,
				fixedLastName: request.FixedLastName,
				fixedBirthName: request.FixedBirthName,
				fixedTitle: request.FixedTitle
				)*/
				request.KindDef = PawnKindDefOf.Villager;
				__result = PawnGenerator.GeneratePawn(request);
				matchError = false;
			}

			FinalPostprocess(ref __result, request);
		}
	}

	/*
	[HarmonyPatch(typeof(PawnGenerator), "GenerateBodyType")]
	public static class Patch_PawnGenerator_GenerateBodyType
	{
		public static bool GenerateBodyTypePostfix(Pawn pawn)
		{
			BodyTypeDef bodyGlobal;
			BodyTypeDef bodyMale;
			BodyTypeDef bodyFemale;
			float randBodyType;
			if (pawn.story.adulthood != null)
			{
				bodyGlobal = pawn.story.adulthood.BodyTypeFor(Gender.None);
				bodyMale = pawn.story.adulthood.BodyTypeFor(Gender.Male);
				bodyFemale = pawn.story.adulthood.BodyTypeFor(Gender.Female);
			}
			else
			{
				bodyGlobal = pawn.story.childhood.BodyTypeFor(Gender.None);
				bodyMale = pawn.story.childhood.BodyTypeFor(Gender.Male);
				bodyFemale = pawn.story.childhood.BodyTypeFor(Gender.Female);
			}
			if (bodyGlobal != null)
			{
				pawn.story.bodyType = bodyGlobal;
			}
			else if ((bodyMale == BodyTypeDefOf.Male && pawn.gender == Gender.Male)
			  || (bodyFemale == BodyTypeDefOf.Female && pawn.gender == Gender.Female))
			{
				randBodyType = Rand.Value;
				if (randBodyType < 0.05)
				{
					pawn.story.bodyType = BodyTypeDefOf.Hulk;
				}
				else if (randBodyType < 0.1)
				{
					pawn.story.bodyType = BodyTypeDefOf.Fat;
				}
				else if (randBodyType < 0.5)
				{
					pawn.story.bodyType = BodyTypeDefOf.Thin;
				}
				else if (pawn.gender == Gender.Female)
				{
					pawn.story.bodyType = BodyTypeDefOf.Female;
				}
				else
				{
					pawn.story.bodyType = BodyTypeDefOf.Male;
				}
			}
			else if (pawn.gender == Gender.Female)
			{
				pawn.story.bodyType = bodyFemale;
			}
			else
			{
				pawn.story.bodyType = bodyMale;
			}
			return false;
		}
	}
	*/

}
