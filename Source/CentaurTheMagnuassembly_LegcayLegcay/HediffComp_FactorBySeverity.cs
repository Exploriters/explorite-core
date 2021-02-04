using System;
using Verse;

namespace CentaurTheMagnuassembly
{
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("", "IDE1006")]
    public class HediffComp_FactorBySeverity : HediffComp
    {
        public float chancePerTick { get { return ((HediffCompProperties_FactorBySeverity)props).chancePerTick; } }
        public float severityAdjust { get { return ((HediffCompProperties_FactorBySeverity)props).severityAdjust; } }
        public float doCount { get { return ((HediffCompProperties_FactorBySeverity)props).doCount; } }
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
