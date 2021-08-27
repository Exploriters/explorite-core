/********************
 * 仙树生长仙草的效果。
 * --siiftun1857
 */
using Verse;
using RimWorld;

namespace Explorite
{
	///<summary>仙树生长仙草的效果。</summary>
	public class CompTargetEffect_AnimaAmplifier : CompTargetEffect
	{
		public override void DoEffectOn(Pawn user, Thing target)
		{
			if (target.TryGetComp<CompPsylinkable>() is CompPsylinkable compPsylinkable
			 && target.TryGetComp<CompSpawnSubplant>() is CompSpawnSubplant compSpawnSubplant)
			{
				int count = 0;
				foreach (int count2 in compPsylinkable.Props.requiredSubplantCountPerPsylinkLevel)
				{
					if (count2 > count)
					{
						count = count2;
					}
				}
				compSpawnSubplant.AddProgress(count, true);
			}
			/*
			if (target is Plant plant)
			{
				Map map = target.Map;
				IntVec3 position = target.Position;
				plant.Growth = 1f;
				GenPlace.TryPlaceThing(plant.MakeMinifiedForced(ThingDefOf.MinifiedTree), position, map, ThingPlaceMode.Near, null, null, default);
			}
			*/
			Find.BattleLog.Add(new BattleLogEntry_ItemUsed(user, target, parent.def, RulePackDefOf.Event_ItemUsed));
		}
	}
}

