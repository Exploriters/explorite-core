/**
 * 该文件包含多个剧本部件。
 * --siiftun1857
 */
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Explorite
{
    ///<summary>向开局的空降仓内塞入Sayers粘液。</summary>
    public class ScenPart_ScatteredGarbage : ScenPart
    {
        public override void PostGameStart()
        {
            base.PostGameStart();
            List<Thing> Things = Find.CurrentMap.listerThings.AllThings;
            foreach (Thing thing in Things)
            {
                if (thing.def == ThingDefOf.DropPodIncoming)
                {
                    ThingDef Filth_SayersMucus = DefDatabase<ThingDef>.GetNamed("Filth_SayersMucus");
                    ((DropPodIncoming)thing).Contents.innerContainer
                        .TryAdd(
                        ThingMaker.MakeThing(Filth_SayersMucus)
                        , 3
                        );
                }
            }
        }
    }
    ///<summary>填满开局的所有电池。</summary>
    public class ScenPart_FillBattery : ScenPart
    {
        //ModContentPack.PatchOperationFindMod
        public override string Summary(Scenario scen)
        {
            return "Magnuassembly_ScenPart_FillBattery_StaticSummary".Translate();
        }
        private static void ProcessBattery(Thing battery)
        {
            //if (battery.def == DefDatabase<ThingDef>.GetNamed("TriBattery"))
            try
            {
                battery?.TryGetComp<CompPowerBattery>()?.AddEnergy(float.PositiveInfinity);
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
                        ProcessBattery(thingInside);
                    }
                }
                if (thing.def == ThingDefOf.DropPodIncoming)
                {
                    foreach (Thing thing3 in ((DropPodIncoming)thing).Contents.innerContainer)
                    {
                        if (thing3.TryGetComp<CompPowerBattery>() != null)
                        {
                            ProcessBattery(thing3);
                        }
                        if (thing3.def == ThingDefOf.MinifiedThing)
                        {
                            Thing thingInside = ((MinifiedThing)thing3).InnerThing;
                            if (thingInside.TryGetComp<CompPowerBattery>() != null)
                            {
                                ProcessBattery(thingInside);
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
    ///<summary>阻止空投舱产生钢渣块。</summary>
    public class ScenPart_WipeoutChunkSlag : ScenPart
    {
        public override string Summary(Scenario scen) => null;
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
    ///<summary>解开空投舱内的打包物品，并且会被直接部署为建筑物。</summary>
    public class ScenPart_UnpackMinified : ScenPart
    {
        public override string Summary(Scenario scen) => null;
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
    ///<summary>填满开局的所有空投舱内三联电池。</summary>
    public class ScenPart_AddFilledTribatteryInPod : ScenPart
    {
        public override string Summary(Scenario scen) => null;
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
                    ((DropPodIncoming)thing).Contents.innerContainer.TryAdd(tribattery, 1);
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
    ///<summary>将物品塞入殖民者的背包中。</summary>
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
        public override string Summary(Scenario scen) => null;
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
                            if (target == null)
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
                    if (tno.thing.TryGetComp<CompForbiddable>() != null)
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
                Log.Error("[Explorite]Null target pawn!");
            }
            return;
        }
    }
}
