/********************
 * 对远行队成员造成心灵失聪的健康状态。
 * --siiftun1857
 */
using RimWorld;
using RimWorld.Planet;
using System.Linq;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
	///<summary>为<see cref = "HediffComp_CausesCaravanPsychicDeafness" />接收参数。</summary>
	public class HediffCompProperties_CausesCaravanPsychicDeafness : HediffCompProperties
	{
		public HediffCompProperties_CausesCaravanPsychicDeafness()
		{
			compClass = typeof(HediffComp_CausesCaravanPsychicDeafness);
		}
	}
	///<summary>对远行队成员造成心灵失聪。</summary>
	public class HediffComp_CausesCaravanPsychicDeafness : HediffComp
	{
		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			foreach (Pawn pawn in Find.World.worldObjects.Caravans.Where(worldCaravan => Find.WorldGrid.TraversalDistanceBetween(Pawn.Tile, worldCaravan.Tile, true, 2) <= 1).SelectMany(c => c.PawnsListForReading).Where(pawn => !pawn.health.hediffSet.HasHediff(PsychicDeafHediffDef, false)))
			{
				pawn.health.AddHediff(PsychicDeafHediffDef, null, null, null);
			}
		}
	}
}