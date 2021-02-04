using Verse;

namespace Sayers
{
    public class HediffGiver_EnsureForAlways : HediffGiver
    {
        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            TryApply(pawn);
        }
    }
}
