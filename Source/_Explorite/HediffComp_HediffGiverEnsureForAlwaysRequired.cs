/********************
 * 使HediffGiver_EnsureForAlways指定的健康状态不会异常存在。
 * --siiftun1857
 */
using System.Linq;
using Verse;

namespace Explorite
{
    ///<summary>为<see cref = "HediffComp_HediffGiverEnsureForAlwaysRequired" />接收参数。</summary>
    public class HediffCompProperties_HediffGiverEnsureForAlwaysRequired : HediffCompProperties
    {
        public HediffCompProperties_HediffGiverEnsureForAlwaysRequired()
        {
            compClass = typeof(HediffComp_HediffGiverEnsureForAlwaysRequired);
        }
    }
    ///<summary>指定需要的<see cref = "HediffGiver_EnsureForAlways" />必须存在。</summary>
    public class HediffComp_HediffGiverEnsureForAlwaysRequired : HediffComp
    {
        private ThingDef raceRecorded = null;
        public override void CompPostTick(ref float severityAdjustment)
        {
            if (raceRecorded != Pawn.def && !Pawn.RaceProps.hediffGiverSets.Where(hgsd => hgsd.hediffGivers.Where(hg => hg is HediffGiver_EnsureForAlways hediffGiver && hediffGiver.hediff == parent.def).Any()).Any())
            {
                Pawn.health.RemoveHediff(parent);
            }
            else if (raceRecorded != Pawn.def)
            {
                raceRecorded = Pawn.def;
            }
        }
    }

}
