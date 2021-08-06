/********************
 * 使人物的年龄不会增长的Hediff效果。
 * 未实现。
 * --siiftun1857
 */
using Verse;

namespace Explorite
{
	public class HediffCompProperties_AgeStop : HediffCompProperties
	{
		public HediffCompProperties_AgeStop() : base()
		{
			compClass = typeof(HediffComp_AgeStop);
		}
	}
	public class HediffComp_AgeStop : HediffComp
	{
		//TODO: 实现功能阻止年龄增长

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			Pawn.ageTracker.AgeBiologicalTicks = 25200000;
		}
	}

}
