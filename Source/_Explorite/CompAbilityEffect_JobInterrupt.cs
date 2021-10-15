/********************
 * 打断目标工作的效果Comp。
 * 
 * --siiftun1857
 */
using RimWorld;
using Verse;
using Verse.AI;

namespace Explorite
{
	///<summary>打断目标工作的效果。</summary>
	public class CompAbilityEffect_JobInterrupt : CompAbilityEffect
	{
		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			Pawn targetPawn = target.Pawn;
			if (targetPawn != null)
			{
				targetPawn.jobs.IsCurrentJobPlayerInterruptible();
				targetPawn.jobs.EndCurrentJob(JobCondition.Incompletable, true, true);
			}
		}
	}
}
