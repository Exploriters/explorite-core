/********************
 * 群体心灵失聪的游戏状态。
 * --siiftun1857
 */
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Grammar;
using static Explorite.ExploriteCore;

namespace Explorite
{
	///<summary>造成心灵失聪的游戏状态。</summary>
	public class GameCondition_PsychicDeafness : GameCondition
	{

		public override string Description => base.Description.Formatted(conditionCauser.DestroyedOrNull() ? "UnknownLower".Translate() : (NamedArgument)(conditionCauser is Pawn ? conditionCauser.LabelShort : conditionCauser.LabelNoCount));

		public override void Init()
		{
			base.Init();
		}

		public override void ExposeData()
		{
			base.ExposeData();
		}

		public override void GameConditionTick()
		{
			/*
			if (!conditionCauser.DestroyedOrNull())
			{
				foreach (Pawn pawnInWorld in Find.World.worldObjects.Caravans.Where(worldCaravan => Find.WorldGrid.TraversalDistanceBetween(conditionCauser.Tile, worldCaravan.Tile, true, 2) <= 1).SelectMany(c => c.PawnsListForReading).Where(pawn => !pawn.health.hediffSet.HasHediff(PsychicDeafHediffDef, false)))
				{
					pawnInWorld.health.AddHediff(PsychicDeafHediffDef, null, null, null);
				}
			}
			*/
			foreach (Map map in AffectedMaps)
			{
				foreach (Pawn pawnInMap in map.mapPawns.AllPawns)
				{
					if (!pawnInMap.health.hediffSet.HasHediff(PsychicDeafHediffDef, false))
					{
						pawnInMap.health.AddHediff(PsychicDeafHediffDef, null, null, null);
					}
				}
			}
		}
		public override void RandomizeSettings(float points, Map map, List<Rule> outExtraDescriptionRules, Dictionary<string, string> outExtraDescriptionConstants)
		{
			base.RandomizeSettings(points, map, outExtraDescriptionRules, outExtraDescriptionConstants);
			outExtraDescriptionRules.Add(new Rule_String("psychicDeafnessCauser", conditionCauser is Pawn ? conditionCauser.LabelShort : conditionCauser.LabelNoCount));
		}
	}
}