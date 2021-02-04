using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using HarmonyLib;
using UnityEngine;
using Verse.AI;
using Verse;
using Verse.Sound;
using static CentaurTheMagnuassembly.RimCentaurCore;
using RimworldMod.VacuumIsNotFun;

namespace CentaurTheMagnuassembly
{
    public class ScenPart_FillBattery : ScenPart
    {
        //ModContentPack.PatchOperationFindMod
        public static string Summary(Scenario scen)
        {
            return "Magnuassembly_ScenPart_FillBattery_StaticSummary".Translate();
        }
        private static void ProcessBattery(ref Thing battery)
        {
            //if (battery.def == DefDatabase<ThingDef>.GetNamed("TriBattery"))
            try
            {
                ((ThingWithComps)battery)?.TryGetComp<CompPowerBattery>()?.AddEnergy(float.PositiveInfinity);
            }
            catch
            { }
        }
        public override void PostGameStart()
        {
            base.PostGameStart();
            List<Thing> things = Find.CurrentMap.listerThings.AllThings;
            foreach (Thing thing in things)
            {
                //if (thing.def == DefDatabase<ThingDef>.GetNamed("TriBattery"))
                //if (thing.TryGetComp<CompPowerBattery>() != null)
                //{
                //    thing?.TryGetComp<CompPowerBattery>()?.AddEnergy(float.PositiveInfinity);
                //}
                if (thing.def == ThingDefOf.MinifiedThing)
                {
                    Thing thingInside = ((MinifiedThing)thing).InnerThing;
                    if (thingInside.TryGetComp<CompPowerBattery>() != null)
                    {
                        thingInside?.TryGetComp<CompPowerBattery>()?.AddEnergy(float.PositiveInfinity);
                    }
                }
                if (thing.def == ThingDefOf.DropPodIncoming)
                {
                    foreach (Thing thing3 in ((DropPodIncoming)thing).Contents.innerContainer)
                    {
                        if (thing3.TryGetComp<CompPowerBattery>() != null)
                        {
                            thing3?.TryGetComp<CompPowerBattery>()?.AddEnergy(float.PositiveInfinity);
                        }
                        if (thing3.def == ThingDefOf.MinifiedThing)
                        {
                            Thing thingInside = ((MinifiedThing)thing3).InnerThing;
                            if (thingInside.TryGetComp<CompPowerBattery>() != null)
                            {
                                thingInside?.TryGetComp<CompPowerBattery>()?.AddEnergy(float.PositiveInfinity);
                            }
                        }
                    }
                }
            }

            /*foreach (Letter letter in Find.LetterStack.LettersListForReading)
            {
                Find.LetterStack.RemoveLetter(letter);
            }*/
            return;
        }
    }
    public class ScenPart_WipeoutChunkSlag : ScenPart
    {
        public static string Summary(Scenario scen) => null;
        public override void PostGameStart()
        {
            base.PostGameStart();
            List<Thing> things = Find.CurrentMap.listerThings.AllThings;
            foreach (Thing thing in things)
            {
                if (thing.def == ThingDefOf.DropPodIncoming)
                {
                    ((DropPodIncoming)thing).Contents.leaveSlag = false;
                }
            }
            return;
        }
    }
    public class ScenPart_UnpackMinified : ScenPart
    {
        public static string Summary(Scenario scen) => null;
        public override void PostGameStart()
        {
            base.PostGameStart();
            List<Thing> things = Find.CurrentMap.listerThings.AllThings;
            foreach (Thing thing in things)
            {
                if (thing.def == ThingDefOf.DropPodIncoming)
                {
                    foreach (Thing thing2 in ((DropPodIncoming)thing).Contents.innerContainer)
                    {
                        if (thing2.def == ThingDefOf.MinifiedThing && (thing2 as MinifiedThing).InnerThing != null)
                        {
                            ((MinifiedThing)thing2).InnerThing.SetFaction(Faction.OfPlayer);
                            ((MinifiedThing)thing2).GetDirectlyHeldThings().TryTransferAllToContainer(((DropPodIncoming)thing).Contents.innerContainer);
                            thing2.Destroy();
                        }
                    }
                }
            }
            return;
        }
    }
    public class ScenPart_AddFilledTribatteryInPod : ScenPart
    {
        public static string Summary(Scenario scen) => null;
        public override void PostGameStart()
        {
            base.PostGameStart();
            List<Thing> things = Find.CurrentMap.listerThings.AllThings;
            foreach (Thing thing in things)
            {
                if (thing.def == ThingDefOf.DropPodIncoming)
                {
                    Thing tribattery = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("TriBattery"));
                    tribattery?.TryGetComp<CompPowerBattery>()?.AddEnergy(float.PositiveInfinity);
                    ((DropPodIncoming)thing).Contents.innerContainer.TryAdd(tribattery,1);
                    break;
                }
            }
            return;
        }
    }
    /*
    public class ScenPart_StartingThingOfPawn_Defined : ScenPart_ThingCount
    {
        public override void PostGameStart()
        {
            base.PostGameStart();
            List<Thing> things = Find.CurrentMap.listerThings.AllThings;
            foreach (Thing thing in things)
            {
                foreach (Thing thing2 in ((DropPodIncoming)thing).Contents.innerContainer)
                {
                    if (thing2.def.race.intelligence == Intelligence.Humanlike)
                    {
                        (thing2 as Pawn).inventory.innerContainer.TryAdd(
                            ThingMaker.MakeThing(thingDef, stuff),count
                            );
                    }
                }
            }
            return;
        }
    }*/
    public class ScenPart_DumpThingsToPawnInv : ScenPart
    {
        protected struct ThingAndOwner
        {
            public Thing thing;
            public ThingOwner thingOwner;
            public ThingAndOwner(Thing thing, ThingOwner thingOwner)
            {
                this.thing = thing;
                this.thingOwner = thingOwner;
            }
        }
        public static string Summary(Scenario scen) => null;
        public override void PostGameStart()
        {
            base.PostGameStart();
            List<Thing> things = Find.CurrentMap.listerThings.AllThings;
            List<ThingAndOwner> queuedThings = new List<ThingAndOwner>();
            Pawn target = null;

            foreach (Thing thingInWorld in things)
            {
                if (thingInWorld.def == ThingDefOf.DropPodIncoming)
                {
                    foreach (Thing thingInDroppod in ((DropPodIncoming)thingInWorld).Contents.innerContainer)
                    {
                        if (thingInDroppod?.def?.race?.Humanlike == true)
                        {
                            if(target == null)
                                target = (Pawn)thingInDroppod;
                        }
                        else if (thingInDroppod?.def?.alwaysHaulable == true)
                        {
                            queuedThings.Add(new ThingAndOwner(thingInDroppod, ((DropPodIncoming)thingInWorld).Contents.innerContainer));
                        }
                    }
                }
            }
            if (target != null)
            {
                bool equipped = target.equipment.Primary != null;
                foreach (ThingAndOwner tno in queuedThings)
                {
                    if(tno.thing.TryGetComp<CompForbiddable>() != null)
                        tno.thing.TryGetComp<CompForbiddable>().Forbidden = false;

                    if (!equipped && tno.thing.TryGetComp<CompEquippable>() != null)
                    {
                        target.equipment.AddEquipment((ThingWithComps)tno.thingOwner.Take(tno.thing));
                    }
                    else
                        tno.thingOwner.TryTransferToContainer(tno.thing, target.inventory.innerContainer);
                }
            }
            else
            {
                Log.Error("[Magnuassembly]Null target pawn!");
            }
            return;
        }
    }
}
