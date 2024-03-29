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
		public virtual void Apply(Pawn pawn)
		{
			pawn.EnsureSubsystemExist(hediff);
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

			for (int k = 0; k < doCount; k++)
			{
				if (Rand.Range(0, 10000) / 10000f < AttemptSuccessChance(cause.Severity))
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
