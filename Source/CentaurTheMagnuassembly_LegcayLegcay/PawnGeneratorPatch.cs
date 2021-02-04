using Verse;
using RimWorld;
//using Harmony;

namespace CentaurTheMagnuassembly
{
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
