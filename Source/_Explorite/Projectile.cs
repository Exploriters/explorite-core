/********************
 * 该文件包含多个弹射物。
 * --siiftun1857
 */
using RimWorld;
using System;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
	public interface IBullet_Direct
	{

	}

	///<summary>发射后直接命中目标的子弹。</summary>
	public class Bullet_Direct : Bullet, IBullet_Direct
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
	public class Projectile_Explosive_Spotshot : Projectile_Explosive
	{
		protected override void Impact(Thing hitThing)
		{
			if (launcher.Faction.IsPlayer)
			{
				Map.fogGrid.RevealFogCluster(Position);
			}

			base.Impact(hitThing);
		}
	}

	///<summary>该弹射物会将发射者传送至爆炸位置。</summary>
	public class Projectile_Explosive_Teleshot : Projectile_Explosive
	{
		// TO-NOMORE-DO: 需要修复传送失败问题。
		// 问题已修复
		protected override void Impact(Thing hitThing)
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
		/*public override void Tick()
		{
			base.Tick();
			if (!landed && intendedTarget.HasThing && intendedTarget.Thing.Map.uniqueID == Map.uniqueID)
			{
				origin = Position.ToVector3();
				destination = intendedTarget.Thing.Position.ToVector3();
			}
		}*/
		/*private void TickBASEBASE()
		{
			if (AllComps != null)
			{
				int i = 0;
				int count = AllComps.Count;
				while (i < count)
				{
					AllComps[i].CompTick();
					i++;
				}
			}
		}*/
	}

	/*
	///<summary>该弹射物会在空中随机加速或减速，大量发射时之间会错开。</summary>
	[Obsolete]public class Projectile_Explosive_Waggingshot : Projectile_Explosive
	{
		public override void Tick()
		{
			base.Tick();
			if (!landed)
			{
				ticksToImpact = Math.Max(ticksToImpact + Rand.Range(-1, 1), 0);
			}
		}
	}
	*/

}
