/**
 * 该文件包含多个HediffGiver。
 * --siiftun1857
 */
using System;
using Verse;

namespace Explorite
{
    ///<summary>始终会确保健康状态存在。</summary>
    public class HediffGiver_EnsureForAlways : HediffGiver
    {
        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            TryApply(pawn);
        }
    }
    ///<summary>按照几率来持续确保健康状态存在。</summary>
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
    ///<summary>按照受严重性影响的几率来持续确保健康状态存在。</summary>
    public class HediffGiver_FactorBySeverity : HediffGiver_Overtime
    {
        public override float attemptSuccessChance(float Severity)
        {
            return chancePerTick * Severity;
        }
    }

}
