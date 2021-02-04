using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
//using Harmony;
using HarmonyLib;
using UnityEngine;
using Verse.AI;
using Verse;
using Verse.Sound;
using static CentaurTheMagnuassembly.RimCentaurCore;

namespace CentaurTheMagnuassembly
{
    class CompProperties_Refuelable_PassiveRecharge : CompProperties_Refuelable
    {
        public CompProperties_Refuelable_PassiveRecharge()
        {
            this.compClass = typeof(CompRefuelable_PassiveRecharge);
        }

        public float fuelGenFragment;
        public float displayFragment;
        public int fuelGenFragmentTicks;
    }
    class CompRefuelable_PassiveRecharge : CompRefuelable
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
                $"{"ChargesRemaining".Translate()}: {Math.Floor(fuelWithFragment/displayFragment)} / {Math.Floor(fuelCapacity/displayFragment)}"
                +"\n"+
                $"{"CanFireIn".Translate()}: {FormattingTickTime((fuelCapacity - fuelWithFragment)/fuelPreSec)}"
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
