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
		public int CooldownTick => ((VerbProperties_Custom)verbProps).ExVerbProp().cooldownTick;
		public bool CooldownTickValid => CooldownTick > 0;
		public int TickSinceLastFire(int currentTime = -1)
		{
			if (!CooldownTickValid || lastShotTick == -1)
				return -1;
			if (currentTime < 0)
			{
				currentTime = InGameTick;
			}
			return currentTime - lastShotTick;
		}
		public bool CanFire(int currentTime = -1)
		{
			if (!CooldownTickValid)
				return true;
			if (currentTime < 0)
			{
				currentTime = InGameTick;
			}
			return lastShotTick == -1 || currentTime - lastShotTick > CooldownTick;
		}
		public int RemainingTickBeforeFire(int currentTime = -1)
		{
			if (currentTime < 0)
			{
				currentTime = InGameTick;
			}
			if (lastShotTick == -1)
				return 0;
			return Math.Max(0, CooldownTick - (currentTime - lastShotTick));
		}
		public float RemainingProgressBeforeFire(int currentTime = -1) => CooldownTickValid ? RemainingTickBeforeFire(currentTime) / ((float)CooldownTick) : 0;

		public override void ExposeData()
		{
			base.ExposeData();
		}
		protected override bool TryCastShot()
		{
			if (!CooldownTickValid)
				return base.TryCastShot();
			bool result = false;
			if (CanFire(InGameTick))
			{
				result = base.TryCastShot();
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
		public ThingDef SecondaryProjectile => ((VerbProperties_Custom)verbProps).ExVerbProp().secondaryProjectile;

		public override ThingDef Projectile
		{
			get
			{
				if (lastShotTick >= 0 && lastShotTick + Math.Max(0, (ShotsPerBurst - 1) * verbProps.ticksBetweenBurstShots) >= InGameTick)
				{
					return SecondaryProjectile;
				}
				else
				{
					return base.Projectile;
				}
			}
		}
	}
	[Obsolete]
	public abstract class Verb_ShootConsumeable : Verb_Shoot
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
