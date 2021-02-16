/********************
 * 该文件包含多个剧本部件。
 * --siiftun1857
 */
using System.Collections.Generic;
using System.Linq;
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
            List<Thing> Things = Find.CurrentMap.listerThings.AllThings.Where(thing => thing is DropPodIncoming).ToList();
            foreach (Thing thing in Things)
            {
                if (thing is DropPodIncoming droppod)
                {
                    ThingDef Filth_SayersMucus = DefDatabase<ThingDef>.GetNamed("Filth_SayersMucus");
                    droppod.Contents.innerContainer
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
        public override string Summary(Scenario scen) => "Magnuassembly_ScenPart_FillBattery_StaticSummary".Translate();
        private static void ProcessBattery(Thing battery)
        {
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
            List<Thing> things = Find.CurrentMap.listerThings.AllThings.Where(thing => thing is DropPodIncoming || thing is MinifiedThing).ToList();
            foreach (Thing thing in things)
            {
                //不处理世界中散落的电池
                /* if (thing.TryGetComp<CompPowerBattery>() != null)
                {
                    thing?.TryGetComp<CompPowerBattery>()?.AddEnergy(float.PositiveInfinity);
                }*/
                if (thing is MinifiedThing minifiedThing)
                {
                    Thing thingInside = minifiedThing.InnerThing;
                    if (thingInside.TryGetComp<CompPowerBattery>() != null)
                    {
                        ProcessBattery(thingInside);
                    }
                }
                if (thing is DropPodIncoming droppod)
                {
                    foreach (Thing thing3 in droppod.Contents.innerContainer)
                    {
                        if (thing3.TryGetComp<CompPowerBattery>() != null)
                        {
                            ProcessBattery(thing3);
                        }
                        if (thing3 is MinifiedThing minifiedThing2)
                        {
                            Thing thingInside = minifiedThing2.InnerThing;
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
        public override string Summary(Scenario scen) => "Magnuassembly_ScenPart_WipeoutChunkSlag_StaticSummary".Translate();
        public override void PostGameStart()
        {
            base.PostGameStart();
            List<Thing> things = Find.CurrentMap.listerThings.AllThings.Where(thing => thing is DropPodIncoming).ToList();
            foreach (Thing thing in things)
            {
                if (thing is DropPodIncoming droppod)
                {
                    droppod.Contents.leaveSlag = false;
                }
            }
            return;
        }
    }
    ///<summary>解开空投舱内的打包物品，并且会被直接部署为建筑物。</summary>
    public class ScenPart_UnpackMinified : ScenPart
    {
        public override string Summary(Scenario scen) => "Magnuassembly_ScenPart_UnpackMinified_StaticSummary".Translate();
        public override void PostGameStart()
        {
            base.PostGameStart();
            foreach (Thing thing in Find.CurrentMap.listerThings.AllThings.Where(thing => thing is DropPodIncoming))
            {
                if (thing is DropPodIncoming droppod)
                {
                    foreach (Thing thing2 in droppod.Contents.innerContainer.Where(thing => thing is MinifiedThing))
                    {
                        if (thing2 is MinifiedThing minifiedThing && minifiedThing.InnerThing != null)
                        {
                            minifiedThing.InnerThing.SetFaction(Faction.OfPlayer);
                            minifiedThing.GetDirectlyHeldThings().TryTransferAllToContainer(droppod.Contents.innerContainer);
                            minifiedThing.Destroy();
                        }
                    }
                }
            }
            return;
        }
    }
    ///<summary>将物品塞入殖民者的背包中。</summary>
    public class ScenPart_DumpThingsToPawnInv : ScenPart
    {
        public override string Summary(Scenario scen) => "Magnuassembly_ScenPart_DumpThingsToPawnInv_StaticSummary".Translate();
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
        public override void PostGameStart()
        {
            base.PostGameStart();
            List<ThingAndOwner> queuedThings = new List<ThingAndOwner>();
            Pawn target = null;

            foreach (Thing thingInWorld in Find.CurrentMap.listerThings.AllThings.Where(thing => thing is DropPodIncoming))
            {
                if (thingInWorld is DropPodIncoming droppod)
                {
                    foreach (Thing thingInDroppod in droppod.Contents.innerContainer)
                    {
                        if (target == null &&
                            thingInDroppod is Pawn pawnInDroppod &&
                            pawnInDroppod?.def?.race?.Humanlike == true)
                        {
                            target = pawnInDroppod;
                        }
                        else if (thingInDroppod?.def?.alwaysHaulable == true)
                        {
                            queuedThings.Add(new ThingAndOwner(thingInDroppod, droppod.Contents.innerContainer));
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
