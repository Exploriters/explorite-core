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
        //TODO: Supress pawn's age
    }
    public class HediffComp_AgeStop : HediffComp
    {
        //TODO: Supress pawn's age

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            Pawn.ageTracker.AgeBiologicalTicks = 25200000;
        }
    }

}
