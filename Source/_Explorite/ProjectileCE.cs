using System;
using Verse;
using RimWorld;
using CombatExtended;
using static Explorite.ExploriteCore;

namespace Explorite
{
	///<summary>发射后直接命中目标的子弹。</summary>
	public class BulletCE_Direct : BulletCE, IBullet_Direct
	{
		public override void Tick()
		{
			if (AllComps != null)
			{
				int i = 0;
				for (int count = AllComps.Count; i < count; i++)
				{
					AllComps[i].CompTick();
				}
			}
			Impact(intendedTarget.Thing as Pawn ?? intendedTarget.Pawn ?? null);
			//if (!Destroyed) Destroy();
		}
	}

	///<summary>该弹射物会探查目的地区域。</summary>
	public class ProjectileCE_Explosive_Spotshot : ProjectileCE_Explosive
	{
		public override void Impact(Thing hitThing)
		{
			if (launcher.Faction.IsPlayer)
			{
				Map.fogGrid.RevealFogCluster(Position);
			}

			base.Impact(hitThing);
		}
	}

	///<summary>该弹射物会将发射者传送至爆炸位置。</summary>
	public class ProjectileCE_Explosive_Teleshot : ProjectileCE_Explosive
	{
		public override void Impact(Thing hitThing)
		{
			if (launcher.Faction.IsPlayer)
			{
				if (launcher is Pawn pawn)
				{
					IntVec3 pos = Position;
					if (pawn.Map == Map)
					{
						if (TeleportPawn(pawn, new TargetInfo(pos, Map)))
						{
							Map.fogGrid.RevealFogCluster(pawn.Position);
						}
					}
				}
				Map.fogGrid.RevealFogCluster(Position);
			}
			base.Impact(hitThing);
		}
	}

}