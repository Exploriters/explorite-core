using System;
using Verse;
using RimWorld;
using CombatExtended;
using static Explorite.ExploriteCore;

namespace Explorite
{
	public class VerbPropertiesCE_EX : VerbPropertiesCE, IVerbPropertiesCustom
	{
		public ExploriteVerbProperties exProps = new ExploriteVerbProperties();
		public ExploriteVerbProperties ExVerbProp()
		{
			return exProps;
		}
	}

	///<summary>具有冷却时间的verb。</summary>
	public class Verb_ShootCE_Cooldown : Verb_ShootCE, IVerb_Shoot_Cooldown
	{
		public int CooldownTick => (verbProps as IVerbPropertiesCustom)?.ExVerbProp()?.cooldownTick ?? 0;
		public bool CooldownTickValid => CooldownTick > 0;
		public int TickSinceLastFire(int? currentTime = null)
		{
			if (!CooldownTickValid || lastShotTick == -1)
				return -1;
			if (currentTime == null)
			{
				currentTime = InGameTick;
			}
			return currentTime.Value - lastShotTick;
		}
		public bool CanFire(int? currentTime = null)
		{
			if (!CooldownTickValid)
				return true;
			if (currentTime == null)
			{
				currentTime = InGameTick;
			}
			return lastShotTick == -1 || currentTime.Value - lastShotTick > CooldownTick;
		}
		public int RemainingTickBeforeFire(int? currentTime = null)
		{
			if (currentTime == null)
			{
				currentTime = InGameTick;
			}
			if (lastShotTick == -1)
				return 0;
			return Math.Max(0, CooldownTick - (currentTime.Value - lastShotTick));
		}
		public float RemainingProgressBeforeFire(int? currentTime = null) => CooldownTickValid ? RemainingTickBeforeFire(currentTime) / ((float)CooldownTick) : 0;

		public override void ExposeData()
		{
			base.ExposeData();
		}
		public override bool TryCastShot()
		{
			bool result = false;
			if (!CooldownTickValid || CanFire(InGameTick))
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
	/*
	///<summary>具有燃料需求的verb。</summary>
	public class Verb_ShootCE_Refuelable : Verb_ShootCE
	{
		public bool ValidRefuelable => verbProps.consumeFuelPerShot > 0f && RefuelableComp != null;
		public CompRefuelable RefuelableComp => caster.TryGetComp<CompRefuelable>();
		public float FuelPerShot => Math.Max(0f, verbProps.consumeFuelPerShot);
		public bool CanFire => !ValidRefuelable || RefuelableComp.Fuel >= FuelPerShot;

		public override void ExposeData()
		{
			base.ExposeData();
		}
		public override bool TryCastShot()
		{
			bool result = false;
			if (!ValidRefuelable || CanFire)
			{
				result = base.TryCastShot();
			}
			RefuelableComp?.ConsumeFuel(FuelPerShot);
			return result;
		}
		public override void WarmupComplete()
		{
			if (CanFire)
			{
				base.WarmupComplete();
			}
		}
		public override bool Available()
		{
			if (CanFire)
			{
				return base.Available();
			}
			return false;
		}
	}
	*/
}
