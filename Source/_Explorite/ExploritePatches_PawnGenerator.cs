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

namespace Explorite
{
    internal static partial class ExploritePatches
    {

        internal static bool GenerateCentaurPostprocess(ref Pawn pawn, PawnGenerationRequest request, ref bool matchError)
        {
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

            pawn.ageTracker.AgeChronologicalTicks = (long)Math.Floor(
                pawn.ageTracker.AgeChronologicalTicks * ((pawn.ageTracker.AgeBiologicalTicks + 360000000f) / pawn.ageTracker.AgeBiologicalTicks)
                );
            pawn.ageTracker.AgeBiologicalTicks += 360000000;

            pawn.story.traits.allTraits.Clear();
            if (pawn.story.hairDef == DefDatabase<HairDef>.GetNamed("Mohawk"))
            {
                pawn.story.hairDef = DefDatabase<HairDef>.GetNamed("Flowy");
            }
            //__result.story.traits.GainTrait(new Trait(TraitDefOf.Asexual, 0, forced: true));

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
            return true;
        }
        internal static bool GenerateSayersPostprocess(ref Pawn pawn, PawnGenerationRequest request, ref bool matchError)
        {
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

            return true;
        }
        internal static bool GenerateGuoguoPostprocess(ref Pawn pawn, PawnGenerationRequest request, ref bool matchError)
        {
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
                __result = PawnGenerator.GeneratePawn(PawnKindDefOf.Villager, request.Faction);
            }

            if (__result?.TryGetComp<CompEnsureAbility>() != null)
            {
                __result?.TryGetComp<CompEnsureAbility>().ApplayAbilities();
            }
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
