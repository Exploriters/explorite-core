/********************
 * 包含多个verb。
 * --siiftun1857
 */
using System;
using System.Text;
using System.Collections.Generic;
using RimWorld;
using HarmonyLib;
using UnityEngine;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
	///<summary>什么都不做。</summary>
	public class Verb_DoNothing : Verb
	{
		protected override bool TryCastShot()
		{
			//throw new NotImplementedException();
			return false;
		}
	}

	///<summary>具有冷却时间的verb。</summary>
	public class Verb_Shoot_Cooldown : Verb_Shoot
	{
		public int lastFireTick = -1;
		public int CooldownTick => ((VerbProperties_Custom)verbProps).cooldownTick;
		public bool CooldownTickValid => CooldownTick > 0;
		public int TickSinceLastFire(int currentTime = -1)
		{
			if (!CooldownTickValid || lastFireTick == -1)
				return -1;
			if (currentTime < 0)
			{
				currentTime = InGameTick;
			}
			return currentTime - lastFireTick;
		}
		public bool CanFire(int currentTime = -1)
		{
			if (!CooldownTickValid)
				return true;
			if (currentTime < 0)
			{
				currentTime = InGameTick;
			}
			return lastFireTick == -1 || currentTime - lastFireTick > CooldownTick;
		}
		public int RemainingTickBeforeFire(int currentTime = -1)
		{
			if (currentTime < 0)
			{
				currentTime = InGameTick;
			}
			if (lastFireTick == -1)
				return 0;
			return Math.Max(0, CooldownTick - (currentTime - lastFireTick));
		}
		public float RemainingProgressBeforeFire(int currentTime = -1) => CooldownTickValid ? RemainingTickBeforeFire(currentTime) / ((float)CooldownTick) : 0;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref lastFireTick, "lastFireTick", -1, true);
		}
		protected override bool TryCastShot()
		{
			if (!CooldownTickValid)
				return base.TryCastShot();
			bool result = false;
			int currentTime = InGameTick;
			if (CanFire(currentTime))
			{
				result = base.TryCastShot();
				if (result)
				{
					lastFireTick = currentTime;
					/*if (CasterIsPawn)
					{
						CasterPawn.jobs.EndCurrentJob(JobCondition.None);
					}*/
				}
			}
			return result;
		}
		public override void WarmupComplete()
		{
			if (CanFire())
			{
				base.WarmupComplete();
			}
		}
		public override bool Available()
		{
			if (CanFire())
			{
				return base.Available();
			}
			return false;
		}
	}

	///<summary>在一发中打出全部爆发。</summary>
	public class Verb_Shoot_OvertickShotgun : Verb_Shoot
	{
		protected override int ShotsPerBurst => 1;
		protected int ShotsPerBurstAccture => ((VerbProperties)verbProps).burstShotCount;

		protected override bool TryCastShot()
		{
			bool SuccessedOnce = false;
			for (int i = 0; i < ShotsPerBurstAccture; i++)
			{
				if (base.TryCastShot())
					SuccessedOnce = true;
			}
			return SuccessedOnce;
		}
	}
	///<summary>设置首发和后续发射不一致。</summary>
	public class Verb_Shoot_Rainbow2 : Verb_Shoot
	{
		protected int lastFireTick = -1;
		public ThingDef SecondaryProjectile => ((VerbProperties_Custom)verbProps).secondaryProjectile;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref lastFireTick, "lastFireTick", -1);
		}
		public override ThingDef Projectile
		{
			get
			{
				if (lastFireTick >= 0 && lastFireTick + Math.Max(0, (ShotsPerBurst - 1) * verbProps.ticksBetweenBurstShots) >= InGameTick)
				{
					return SecondaryProjectile;
				}
				else
				{
					return base.Projectile;
				}
			}
		}
		protected override bool TryCastShot()
		{
			bool result = base.TryCastShot();
			if (lastFireTick < 0 || lastFireTick + Math.Max(0, (ShotsPerBurst - 1) * verbProps.ticksBetweenBurstShots) < InGameTick)
			{
				lastFireTick = InGameTick;
			}
			return result;
		}
	}
	public class Verb_ShootConsumeable : Verb_Shoot
	{
		public int remainBullet = 0;
		//초기화할때 verbproperties랑remainBullet 동기화해야되

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref remainBullet, "remainBullet");
		}
		protected override bool TryCastShot()
		{
			if (base.TryCastShot())
			{
				if (burstShotsLeft <= 1)
				{
					SelfConsume();
				}
				return true;
			}
			if (burstShotsLeft < verbProps.burstShotCount)
			{
				SelfConsume();
			}
			return false;
		}

		public override void Notify_EquipmentLost()
		{
			base.Notify_EquipmentLost();
			if (state == VerbState.Bursting && burstShotsLeft < verbProps.burstShotCount)
			{
				SelfConsume();
			}
		}

		private void SelfConsume()
		{
			if (remainBullet > 0)
			{
				remainBullet--;
			}
			else
			{
				EquipmentSource.GetComp<CompEquippable>().AllVerbs.Remove(this);
			}
		}
	}
}
