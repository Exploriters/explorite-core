/**
 * 高速恢复燃料的CompRefuelable变种。
 */
using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
    /**
     * <summary>为<see cref = "CompRefuelable_PassiveRecharge" />接收参数。</summary>
     */
    public class CompProperties_Refuelable_PassiveRecharge : CompProperties_Refuelable
    {
        public CompProperties_Refuelable_PassiveRecharge()
        {
            compClass = typeof(CompRefuelable_PassiveRecharge);
        }

        public float fuelGenFragment;
        public float displayFragment;
        public int fuelGenFragmentTicks;
    }
    /**
     * <summary>高速恢复燃料的<see cref = "CompRefuelable" />变种。</summary>
     */
    public class CompRefuelable_PassiveRecharge : CompRefuelable
    {
        public bool messageSignal = false;

        public int ticksWithoutFuel = 0;
        public float fuelGenFragment => ((CompProperties_Refuelable_PassiveRecharge)Props).fuelGenFragment;
        public float displayFragment => ((CompProperties_Refuelable_PassiveRecharge)Props).displayFragment;
        public int fuelGenFragmentTicks => ((CompProperties_Refuelable_PassiveRecharge)Props).fuelGenFragmentTicks;
        public float fuelCapacity => ((CompProperties_Refuelable_PassiveRecharge)Props).fuelCapacity;
        public float fuelWithFragment => Fuel + ticksWithoutFuel * fuelGenFragment / fuelGenFragmentTicks;
        public float fuelPreSec => fuelGenFragment / (fuelGenFragmentTicks / 60f);
        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            return new List<StatDrawEntry>();
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ticksWithoutFuel, "ticksWithoutFuel", 0);
            Scribe_Values.Look(ref messageSignal, "messageSignal", false);
        }
        public override string CompInspectStringExtra()
        {
            return
                $"{"ChargesRemaining".Translate()}: {Math.Floor(fuelWithFragment / displayFragment)} / {Math.Floor(fuelCapacity / displayFragment)}"
                + "\n" +
                $"{"CanFireIn".Translate()}: {FormattingTickTime((fuelCapacity - fuelWithFragment) / fuelPreSec)}"
                ;
        }
        public override void CompTick()
        {
            base.CompTick();

            if (fuelWithFragment < displayFragment && !messageSignal)
            {
                messageSignal = true;
            }

            ticksWithoutFuel++;
            if (
                ticksWithoutFuel >= fuelGenFragmentTicks
                || fuelWithFragment >= fuelCapacity
            )
            {
                Refuel(fuelGenFragment);
                ticksWithoutFuel = 0;
            }

            if (fuelWithFragment >= fuelCapacity && messageSignal)
            {
                messageSignal = false;
                Messages.Message("Magnuassembly_CompRefuelable_PassiveRecharge_ChargeCompleted".Translate(parent.GetCustomLabelNoCount()), parent, MessageTypeDefOf.PositiveEvent);
            }
        }
    }
}
