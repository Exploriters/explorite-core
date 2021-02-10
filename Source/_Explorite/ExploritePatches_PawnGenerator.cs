/**
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
        /**
         * <summary>
         * 对人物生成器的补丁。
         * </summary>
         */
        [HarmonyPostfix]
        public static void GeneratePawnPostfix(ref Pawn __result, PawnGenerationRequest request)
        {
            if (__result.def == AlienCentaurDef)
            {
                if (__result.kindDef.race == AlienCentaurDef)
                {
                    __result.story.bodyType = __result.gender == Gender.Female ?
                        DefDatabase<BodyTypeDef>.GetNamed("CentaurFemale") : DefDatabase<BodyTypeDef>.GetNamed("CentaurMale");

                    //__result.abilities.abilities.Add(new Ability(__result, DefDatabase<AbilityDef>.GetNamed("MassPsychicDeafCentaur")));
                    __result.ageTracker.AgeBiologicalTicks += 360000000;
                    __result.ageTracker.AgeChronologicalTicks += 360000000;

                    //__result.story.traits.GainTrait(new Trait(TraitDefOf.Asexual, 0, forced: true));

                    /*__result.story.traits.allTraits.Sort(delegate (Trait t1, Trait t2) {
                        if (t1.def == TraitDefOf.Asexual && t2.def != TraitDefOf.Asexual)
                        {
                            return 1;
                        }
                        return 0;
                    });*/

                    foreach (SkillRecord sr in __result.skills.skills)
                    {
                        sr.Level =
                            __result.story.childhood.skillGainsResolved.TryGetValue(sr.def) +
                            __result.story.adulthood.skillGainsResolved.TryGetValue(sr.def);
                        if (sr.passion == Passion.None)
                            sr.passion = Passion.Minor;
                        sr.xpSinceLastLevel = sr.XpRequiredForLevelUp / 2f;
                    }
                }
                else
                {
                    //__result.def = ThingDefOf.Human;
                    __result = PawnGenerator.GeneratePawn(PawnKindDefOf.Villager, request.Faction);
                }
            }
            if (__result.def == AlienSayersDef)
            {
                if (__result.kindDef.race == AlienSayersDef)
                {
                    //__result.abilities.abilities.Add(new Ability(__result, DefDatabase<AbilityDef>.GetNamed("ParasiticStab_Sayers")));
                }
                else
                {
                    //__result.def = ThingDefOf.Human;
                    __result = PawnGenerator.GeneratePawn(PawnKindDefOf.Villager, request.Faction);
                }
            }

            if (__result.def == ThingDefOf.Human && __result.story.childhood == CentaurCivilRetro)
            {
                __result.story.childhood = BackstoryDatabase.ShuffleableBackstoryList(
                    BackstorySlot.Childhood,
                    new BackstoryCategoryFilter { categories = __result.kindDef.backstoryCategories }
                ).RandomElement();
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
