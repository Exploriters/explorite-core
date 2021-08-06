/********************
 * 指定健康状态半衰期。
 * 
 * 故障。
 * --siiftun1857
 */
using System;
using System.Text;
using Verse;

namespace Explorite
{
	///<summary>为<see cref = "HediffComp_SeverityHalfLife" />接收参数。</summary>
	public class HediffCompProperties_SeverityHalfLife : HediffCompProperties
	{
		public int halfLife = 1;
		public float lowLimit = 0.001f;
		public HediffCompProperties_SeverityHalfLife() : base()
		{
			compClass = typeof(HediffComp_SeverityHalfLife);
		}
	}
	///<summary>指定健康状态半衰期。</summary>
	public class HediffComp_SeverityHalfLife : HediffComp
	{
		public HediffCompProperties_SeverityHalfLife Props => props as HediffCompProperties_SeverityHalfLife;
		public float HalfLife => Math.Max(0.001f, Props.halfLife);
		public float LowLimit => Math.Max(0.001f, Props.lowLimit);
		public override void CompPostTick(ref float severityAdjustment)
		{
			if (parent == null)
				return;
			parent.Severity = parent.Severity > LowLimit ? (float)(parent.Severity / Math.Pow(2f, 1f / HalfLife)) : 0f;
		}
		public override string CompDebugString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.CompDebugString());
			stringBuilder.AppendLine("half life: " + HalfLife.ToString());
			return stringBuilder.ToString().TrimEndNewlines();
		}
	}

}
