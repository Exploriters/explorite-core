/********************
 * 指示一个HediffComp在什么阶段下才会工作。
 * --siiftun1857
 */
using Verse;

namespace Explorite
{
	///<summary>指示一个<see cref = "HediffComp" />在什么阶段下才会工作。</summary>
	public abstract class HediffCompProperties_StageRange : HediffCompProperties
	{
		public int minStage = int.MinValue;
		public int maxStage = int.MaxValue;
		public IntRange StageRange => new IntRange(minStage, maxStage);
		public bool InStage(int stage)
		{
			return stage >= minStage && stage <= maxStage;
		}
	}
	///<summary>为<see cref = "HediffComp_StageRange_CustomDesc" />接收参数。</summary>
	public class HediffCompProperties_StageRange_CustomDesc : HediffCompProperties_StageRange
	{
		[MustTranslate]
		public string tip = string.Empty;
		public HediffCompProperties_StageRange_CustomDesc()
		{
			compClass = typeof(HediffComp_StageRange_CustomDesc);
		}
	}
	///<summary>在范围内时显示额外描述。</summary>
	public class HediffComp_StageRange_CustomDesc : HediffComp
	{
		public bool InStage => (props as HediffCompProperties_StageRange)?.InStage(parent.CurStageIndex) ?? false;
		public string Tip => (props as HediffCompProperties_StageRange_CustomDesc)?.tip ?? string.Empty;
		public override string CompTipStringExtra => InStage ? Tip : null;
	}
}
