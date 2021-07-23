/********************
 * 来源物品不存在后移除hediff。
 * --siiftun1857
 */
using RimWorld;
using System.Collections.Generic;
using System.Linq;
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
        public List<Apparel> sources = null;
        //base.Pawn.health.RemoveHediff(this.parent);
        public override bool CompShouldRemove => base.CompShouldRemove || CheckSources();
        private bool CheckSources()
		{
			if (sources == null)
				return false;
			sources.RemoveAll(app => app.DestroyedOrNull() || app.Wearer != parent.pawn);
			return !sources.Any();
		}
		public void AddSources(Apparel source)
		{
			if (sources == null)
				sources = new List<Apparel>();
			if (!sources.Contains(source))
                sources.Add(source);
		}
		public override void CompPostMerged(Hediff other)
		{
			base.CompPostMerged(other);
			HediffComp_DisappearsOnSourceApparelLost hediffComp_Disappears = other.TryGetComp<HediffComp_DisappearsOnSourceApparelLost>();
			if (hediffComp_Disappears != null && hediffComp_Disappears.sources != null)
			{
				sources = (sources ?? new List<Apparel>()).Concat(hediffComp_Disappears.sources).ToList();
			}
		}
		public override void CompExposeData()
		{
			Scribe_Collections.Look(ref sources, "sources", LookMode.Reference);
		}
		public override string CompDebugString()
		{
			return "sources: " + string.Join(", ", sources?.Select(s => s.ThingID) ?? new List<string>());
		}
	}

}
