using System;
using Verse;

namespace CentaurTheMagnuassembly
{
    public class HediffGiver_Overtime : HediffGiver
    {
        public float chancePerTick = 1;
        public virtual float attemptSuccessChance(float Severity)
        {
            return chancePerTick;
        }
        public float severityAdjust = 0.001f;
        public float doCount = 1;
        public bool doTryApply = true;


        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            /* FOOLED
            IEnumerable<BodyPartRecord> parts = pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined);

            BodyPartRecord Record = new BodyPartRecord();

            foreach (BodyPartRecord part in parts)
            {
                if (partsToAffect.Contains(part.def))
                {
                    Record = part;
                    break;
                }
            }

             pawn.health.AddHediff(this.hediff, null, null);
            HealthUtility.AdjustSeverity(pawn, this.hediff, 1f);*/

            Random rnd = new Random();
            for (int k = 0; k < doCount; k++)
            {
                if (rnd.Next(0, 9999) / 10000f < attemptSuccessChance(cause.Severity))
                {
                    if (doTryApply)
                        TryApply(pawn);
                    HealthUtility.AdjustSeverity(pawn, hediff, severityAdjust);
                }
            }
        }
    }
    public class HediffGiver_FactorBySeverity : HediffGiver_Overtime
    {
        public override float attemptSuccessChance(float Severity)
        {
            return chancePerTick * Severity;
        }
    }

}
