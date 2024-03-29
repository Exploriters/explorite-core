/********************
 * 群体心灵失聪的主要效果Comp。
 * 
 * --siiftun1857
 */
using Verse;
using RimWorld;
using System.Linq;

namespace Explorite
{
	///<summary>半人马的群体心灵失聪技能的主要功能实现。
	///<br />会对目标所处地图的所有生物造成健康状态效果。
	///<br />务必确保技能对单一目标生效，或为对施法者自己生效。</summary>
	public class CompAbilityEffect_GiveHediff_WholeMap : CompAbilityEffect_GiveHediff
	{
		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			//foreach (Thing thing in target.Thing.Map.listerThings.AllThings)
			if (target.Pawn == parent.pawn)
			{
				foreach (Pawn thing in target.Thing.Map.mapPawns.AllPawns.Where(pawn => pawn.SpawnedOrAnyParentSpawned && !pawn.Dead))
				{
					base.Apply(thing, dest);
				}
			}
		}
	}



}
