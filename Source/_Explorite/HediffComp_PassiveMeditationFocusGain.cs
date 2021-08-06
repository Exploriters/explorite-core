/********************
 * 使生物持续获取精神力。
 * --siiftun1857
 */
using Verse;

namespace Explorite
{
	///<summary>为<see cref = "HediffComp_PassiveMeditationFocusGain" />接收参数。</summary>
	public class HediffCompProperties_PassiveMeditationFocusGain : HediffCompProperties_StageRange
	{
		public float focusPerDay = 0f;
		public HediffCompProperties_PassiveMeditationFocusGain()
		{
			compClass = typeof(HediffComp_PassiveMeditationFocusGain);
		}
	}
	///<summary>使生物持续获取精神力。</summary>
	public class HediffComp_PassiveMeditationFocusGain : HediffComp
	{
		public bool InStage => (props as HediffCompProperties_StageRange)?.InStage(parent.CurStageIndex) ?? false;
		public float FocusPerDay => ((HediffCompProperties_PassiveMeditationFocusGain)props).focusPerDay;
		public float FocusPerTick => FocusPerDay / 60000;
		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (InStage)
			{
				Pawn.psychicEntropy.OffsetPsyfocusDirectly(FocusPerTick);
			}
		}
		public override string CompTipStringExtra => InStage ? "Magnuassembly_HediffComp_PassiveMeditationFocus_TipString".Translate(FocusPerDay.ToStringPercent()) : null;
	}
}
