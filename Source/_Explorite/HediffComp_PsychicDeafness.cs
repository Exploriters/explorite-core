/********************
 * 侦测心灵失聪游戏状态的健康状态。
 * --siiftun1857
 */
using RimWorld;
using RimWorld.Planet;
using System.Linq;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
	///<summary>检查心灵失聪合法性的Comp。</summary>
	public class HediffComp_PsychicDeafness : HediffComp
	{
		public override bool CompShouldRemove
		{
			get
			{
				//if (Pawn.health.hediffSet.hediffs.Any(hediff => hediff.TryGetComp<HediffComp_CausesCaravanPsychicDeafness>() != null))
				if (Pawn.health.hediffSet.HasHediff(PsychicDeafSourceHediffDef))
				{
					return false;
				}
				else if (Pawn.SpawnedOrAnyParentSpawned && Pawn.MapHeld.gameConditionManager.GetActiveCondition<GameCondition_PsychicDeafness>() != null)
				{
					return false;
				}
				else if (Pawn.IsCaravanMember())
				{
					Caravan caravan = Pawn.GetCaravan();
					//if (Find.World.worldObjects.Caravans.Where(worldCaravan => Find.WorldGrid.TraversalDistanceBetween(caravan.Tile, worldCaravan.Tile, true, 2) <= 1).SelectMany(c => c.PawnsListForReading).Concat(Find.Maps.Where(map => Find.WorldGrid.TraversalDistanceBetween(caravan.Tile, map.Tile, true, 2) <= 1).SelectMany(m => m.mapPawns.AllPawns)).Any(pawn => pawn.health.hediffSet.hediffs.Any(hediff => hediff.TryGetComp<HediffComp_CausesCaravanPsychicDeafness>() != null))
					//if (Find.World.worldObjects.Caravans.Any(worldCaravan => Find.WorldGrid.TraversalDistanceBetween(caravan.Tile, worldCaravan.Tile, true, 2) <= 1 && worldCaravan.PawnsListForReading.Any(pawn => pawn.health.hediffSet.hediffs.Any(hediff => hediff.TryGetComp<HediffComp_CausesCaravanPsychicDeafness>() != null)))
					//	|| Find.Maps.Any(map => Find.WorldGrid.TraversalDistanceBetween(caravan.Tile, map.Tile, true, 2) <= 1 && map.GameConditionManager.ConditionIsActive(PsychicDeafnessConditionDef))
					if (
						Find.World.worldObjects.Caravans.Any
							(worldCaravan => Find.WorldGrid.TraversalDistanceBetween(caravan.Tile, worldCaravan.Tile, true, 2) <= 1 
								&& worldCaravan.PawnsListForReading.Any(pawn => pawn.health.hediffSet.HasHediff(PsychicDeafSourceHediffDef)))

						|| Find.Maps.Any(map => Find.WorldGrid.TraversalDistanceBetween(caravan.Tile, map.Tile, true, 2) <= 1 && map.GameConditionManager.ActiveConditions.Any(c => c.def == PsychicDeafnessConditionDef && c.conditionCauser.Map == map))
						)
					{
						return false;
					}
				}
				return true;
			}
		}
	}
}
