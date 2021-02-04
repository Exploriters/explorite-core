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
    public static class PawnGeneratorPatch
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Type patchType = typeof(PawnGeneratorPatch);

        static PawnGeneratorPatch()
        {
            Harmony harmonyInstance = new Harmony(id: "CentaurTheMagnuassembly.rimworld.mod.PawnGeneratorPatch");

            harmonyInstance.Patch(AccessTools.Method(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new[] { typeof(PawnGenerationRequest) }),
                postfix: new HarmonyMethod(patchType, nameof(GeneratePawnPostfix) ));
        }
        [HarmonyPostfix]
        public static void GeneratePawnPostfix(ref Pawn __result)
        {
            if (__result.def == AlienCentaurDef)
            {
                if (__result.kindDef.race == AlienCentaurDef)
                {
                    __result.story.bodyType = __result.gender == Gender.Female ?
                        DefDatabase<BodyTypeDef>.GetNamed("CentaurFemale") : DefDatabase<BodyTypeDef>.GetNamed("CentaurMale");

                    __result.abilities.abilities.Add(new Ability(__result, DefDatabase<AbilityDef>.GetNamed("MassPsychicDeafCentaur")));
                    __result.ageTracker.AgeBiologicalTicks += 360000000;
                    __result.ageTracker.AgeChronologicalTicks += 360000000;

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
                    __result.def = ThingDefOf.Human;
                }
            }
            if (__result.def == ThingDefOf.Human && __result.story.childhood == CentaurCivilRetro)
            {
                __result.story.childhood = BackstoryDatabase.ShuffleableBackstoryList(
                    BackstorySlot.Childhood,
                    new BackstoryCategoryFilter { categories = __result.kindDef.backstoryCategories }
                ).RandomElement();
            }
        }
        /*
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
        */
    }
    
    /*
    [HarmonyPatch(typeof(PawnGenerator), "GenerateBodyType")]
    public static class Patch_PawnGenerator_GenerateBodyType
    {
        public static bool Prefix(Pawn pawn)
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
    }*/

}
