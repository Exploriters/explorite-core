/********************
 * 来源物品不存在后移除hediff。
 * --siiftun1857
 */
using RimWorld;
using Verse;

namespace Explorite
{
    ///<summary>为<see cref = "HediffComp_SpawnThingOnDeath" />接收参数。</summary>
    public class HediffCompProperties_DisappearsOnSourceApparelLost : HediffCompProperties
    {
        public HediffCompProperties_DisappearsOnSourceApparelLost()
        {
            compClass = typeof(HediffComp_DisappearsOnSourceApparelLost);
        }
    }
    ///<summary>来源衣物不存在后移除hediff。</summary>
    public class HediffComp_DisappearsOnSourceApparelLost : HediffComp
    {
        public Apparel source;
		//base.Pawn.health.RemoveHediff(this.parent);
		public override bool CompShouldRemove
		{
			get
			{
				return base.CompShouldRemove || source == null || source.Wearer != parent.pawn;
			}
		}
		public override void CompExposeData()
		{
			Scribe_References.Look<Apparel>(ref source, "source", false);
		}
		public override string CompDebugString()
		{
			return "source: " + source.LabelShort;
		}
	}

}
