/********************
 * 高速恢复燃料的CompRefuelable变种。
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorld;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
	///<summary>为<see cref = "CompRefuelable_PassiveRecharge" />接收参数。</summary>
	public class CompProperties_Refuelable_PassiveRecharge : CompProperties_Refuelable
	{
		public CompProperties_Refuelable_PassiveRecharge()
		{
			compClass = typeof(CompRefuelable_PassiveRecharge);
		}
		private static readonly MethodInfo baseBaseSpecialDisplayStats = typeof(CompProperties).GetMethod(nameof(CompProperties.SpecialDisplayStats));
		public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
		{
			foreach (StatDrawEntry statDrawEntry in baseBaseSpecialDisplayStats.InvokeForce(new object[] { this, req }) as IEnumerable<StatDrawEntry>)
			{
				yield return statDrawEntry;
			}
			yield break;
		}

		public float fuelGenFragment;
		public float displayFragment;
		public int fuelGenFragmentTicks;
		public bool requirePower;
		public float? powerWhenNotSpike;
	}
	///<summary>高速恢复燃料的<see cref = "CompRefuelable" />变种。</summary>
	public class CompRefuelable_PassiveRecharge : CompRefuelable
	{
		public new CompProperties_Refuelable_PassiveRecharge Props => props as CompProperties_Refuelable_PassiveRecharge;

		public bool messageSignal = false;

		[Unsaved(false)]
		public bool? operatingAtHighPower;

		public int ticksWithoutFuel = 0;
		public float FuelGenFragment => Props.fuelGenFragment;
		public float DisplayFragment => Props.displayFragment;
		public int FuelGenFragmentTicks => Props.fuelGenFragmentTicks;
		public float FuelCapacity => Props.fuelCapacity;
		public float FuelWithFragment => Fuel + (ticksWithoutFuel * FuelGenFragment / FuelGenFragmentTicks);
		public float FuelPreSec => FuelGenFragment / (FuelGenFragmentTicks / 60f);
		public float? PowerWhenNotSpike => Props.powerWhenNotSpike;
		public bool RequirePower => Props.requirePower;
		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			return Enumerable.Empty<StatDrawEntry>();
		}
		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			operatingAtHighPower = null;
		}
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref ticksWithoutFuel, "ticksWithoutFuel", 0);
			Scribe_Values.Look(ref messageSignal, "messageSignal", false);
		}
		public override string CompInspectStringExtra()
		{
			StringBuilder sb = new StringBuilder();

			/*
			string baseInspectString = base.CompInspectStringExtra();
			if (!baseInspectString.NullOrEmpty())
			{
				sb.AppendLine(baseInspectString);
			}
			*/
			sb.AppendLine(
				$"{"ChargesRemaining".Translate()}: {Math.Floor(FuelWithFragment / DisplayFragment)} / {Math.Floor(FuelCapacity / DisplayFragment)}"
				+ "\n" +
				$"{"CanFireIn".Translate()}: {FormattingTickTime((FuelCapacity - FuelWithFragment) / FuelPreSec)}"
				);

			if (PowerWhenNotSpike.HasValue)
			{
				sb.AppendLine("PowerConsumptionMode".Translate() + ": " + (operatingAtHighPower switch
				{
					true => "PowerConsumptionHigh",
					false => "PowerConsumptionLow",
					_ => "PowerConsumptionHigh",
				}).Translate().CapitalizeFirst());
			}
			return sb.ToString().TrimEndNewlines();

		}
		public override void CompTick()
		{
			base.CompTick();
			bool wantOperatingAtHighPower = true;

			CompPowerTrader comp = parent.TryGetComp<CompPowerTrader>();
			if (!RequirePower || comp == null || comp.PowerOn)
			{
				if (Fuel >= FuelCapacity)
				{
					wantOperatingAtHighPower = false;
				}
				if (FuelWithFragment < DisplayFragment && !messageSignal)
				{
					messageSignal = true;
				}

				ticksWithoutFuel++;
				if (
					ticksWithoutFuel >= FuelGenFragmentTicks
					|| FuelWithFragment >= FuelCapacity
				)
				{
					Refuel(FuelGenFragment);
					ticksWithoutFuel = 0;
				}

				if (FuelWithFragment >= FuelCapacity && messageSignal)
				{
					messageSignal = false;
					Messages.Message("Magnuassembly_CompRefuelable_PassiveRecharge_ChargeCompleted".Translate(parent.GetCustomLabelNoCount()), parent, MessageTypeDefOf.PositiveEvent);
				}
			}
			//if (PowerWhenNotSpike.HasValue && comp != null && operatingAtHighPower != wantOperatingAtHighPower)
			//{
				operatingAtHighPower = wantOperatingAtHighPower;
				if (wantOperatingAtHighPower)
				{
					comp.PowerOutput = -comp.Props.basePowerConsumption;
				}
				else
				{
					comp.PowerOutput = -PowerWhenNotSpike.Value;
				}
			//}
		}
	}
}
