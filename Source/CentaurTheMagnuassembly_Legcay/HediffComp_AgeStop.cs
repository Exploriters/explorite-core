using Verse;

namespace CentaurTheMagnuassembly
{
    public class HediffCompProperties_AgeStop : HediffCompProperties
    {
        public HediffCompProperties_AgeStop() : base()
        {
            compClass = typeof(HediffComp_AgeStop);
        }
        //TODO: Supress pawn's age
    }
    public class HediffComp_AgeStop : HediffComp
    {
        //TODO: Supress pawn's age

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            Pawn.ageTracker.AgeBiologicalTicks = 25200000;
        }
    }

}
