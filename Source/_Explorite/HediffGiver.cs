/********************
 * 该文件包含多个HediffGiver。
 * --siiftun1857
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
    ///<summary>始终会确保健康状态存在。</summary>
    public class HediffGiver_EnsureForAlways : HediffGiver
    {
        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            if (!pawn.health.hediffSet.HasHediff(hediff))
                TryApply(pawn);
        }
    }
    ///<summary>始终会确保子系统存在。</summary>
    public class HediffGiver_EnsureForBlankSubsystem : HediffGiver
    {
        public static void Apply(Pawn pawn, HediffDef hediffDef)
        {
            List<BodyPartRecord> partsToRestore = new List<BodyPartRecord>();
            IEnumerable<Hediff_MissingPart> missingPartHediffs = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
            foreach (Hediff_MissingPart hediff_MissingPart in missingPartHediffs)
            {
                if (hediff_MissingPart.Part.groups.Contains(CentaurSubsystemGroup0Def))
                {
                    partsToRestore.Add(hediff_MissingPart.Part);
                }
            }
            foreach (BodyPartRecord part in partsToRestore)
            {
                pawn.health.RestorePart(part, null, true);
            }

            IEnumerable<BodyPartRecord> parts = pawn?.health?.hediffSet?.GetNotMissingParts()?.Where(bpr =>
                bpr?.groups?.Contains(CentaurSubsystemGroup0Def) ?? false);
            foreach (BodyPartRecord part in parts)
            {
                if (!(pawn?.health?.hediffSet?.hediffs?.Any(h =>
                        (h?.def?.tags?.Contains("CentaurSubsystem") ?? false) && h?.Part == part
                    ) ?? false))
                {
                    pawn.health.AddHediff(hediffDef, part);
                }
            }
        }
        public virtual void Apply(Pawn pawn)
        {
            Apply(pawn, hediff);
        }
        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            Apply(pawn);
        }
    }

    ///<summary>按照几率来持续确保健康状态存在。</summary>
    public class HediffGiver_Overtime : HediffGiver
    {
        public float chancePerTick = 1;
        public virtual float AttemptSuccessChance(float Severity)
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
                if (rnd.Next(0, 10000) / 10000f < AttemptSuccessChance(cause.Severity))
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
        public override float AttemptSuccessChance(float Severity)
        {
            return chancePerTick * Severity;
        }
    }

}
