/********************
 * 使Hediff的严重性下降速率受到严重性影响。
 * --siiftun1857
 */
using System;
using Verse;

namespace Explorite
{
    ///<summary>为<see cref = "HediffComp_FactorBySeverity" />接收参数。</summary>
    public class HediffCompProperties_FactorBySeverity : HediffCompProperties
    {
        public float chancePerTick = 1;
        public float severityAdjust = -0.001f;
        public float doCount = 1;
        public HediffCompProperties_FactorBySeverity() : base()
        {
            compClass = typeof(HediffComp_FactorBySeverity);
        }
    }
    ///<summary>使严重性下降速率受到严重性影响。</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, "IDE1006")]
    public class HediffComp_FactorBySeverity : HediffComp
    {
        public float chancePerTick => ((HediffCompProperties_FactorBySeverity)props).chancePerTick;
        public float severityAdjust => ((HediffCompProperties_FactorBySeverity)props).severityAdjust;
        public float doCount => ((HediffCompProperties_FactorBySeverity)props).doCount;
        public override void CompPostTick(ref float severityAdjustment)
        {
            if (parent == null)
                return;
            Random rnd = new Random();
            for (int k = 0; k < doCount; k++)
            {
                if (rnd.Next(0, 9999) / 10000f < chancePerTick * parent.Severity)
                {
                    parent.Severity += severityAdjust;
                }
            }
        }
    }

}
