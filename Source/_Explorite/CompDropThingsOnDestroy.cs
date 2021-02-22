/********************
 * 指定物体在被摧毁时掉落物品的部件类。
 * --siiftun1857
 */
using System.Collections.Generic;
using Verse;
using RimWorld;
using System;
using static Explorite.ExploriteCore;

namespace Explorite
{
    ///<summary>设置一个物体和数量。</summary>
    public struct ThingGenProp
    {
        public ThingDef thingDef;
        public ThingDef stuff;
        public IntRange count;
        public ThingGenProp(ThingDef thingDef, ThingDef stuff = null, IntRange? count = null)
        {
            this.thingDef = thingDef ?? throw new ArgumentNullException(nameof(thingDef));
            this.stuff = stuff;
            this.count = count ?? new IntRange(1, 1);
        }
    }
    ///<summary>为<see cref = "CompProperties_DropThingsOnDestroy" />设置在何种情况下掉落物品。</summary>
    public struct ThingOnDestroy
    {
        public List<DestroyMode> destroyModes;
        public List<ThingGenProp> things;
        public ThingOnDestroy(List<DestroyMode> destroyModes, List<ThingGenProp> things)
        {
            this.destroyModes = destroyModes ?? new List<DestroyMode>();
            this.things = things ?? new List<ThingGenProp>();
        }
    }
    ///<summary>为<see cref = "CompDropThingsOnDestroy" />接收参数。</summary>
    public class CompProperties_DropThingsOnDestroy : CompProperties
    {
        public List<ThingOnDestroy> cases;
        public CompProperties_DropThingsOnDestroy()
        {
            compClass = typeof(CompDropThingsOnDestroy);
        }
    }
    ///<summary>指定物体在被摧毁时掉落物品。</summary>
    public class CompDropThingsOnDestroy : ThingComp
    {
        private CompProperties_DropThingsOnDestroy Props => props as CompProperties_DropThingsOnDestroy;
        private bool Vaild => Props?.cases != null;
        private List<ThingOnDestroy> Cases => Props.cases;
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if (Vaild)
            {
                ThingOwner<Thing> thingsToDrop = new ThingOwner<Thing>();
                foreach (ThingOnDestroy thingOnDestroy in Cases)
                {
                    if (thingOnDestroy.destroyModes?.Contains(mode) == true)
                    {
                        foreach (ThingGenProp thingGenProp in thingOnDestroy.things)
                        {
                            int count = thingGenProp.count.RandomInRange;
                            if (count > 0)
                            {
                                Thing thing = ThingMaker.MakeThing(thingGenProp.thingDef, thingGenProp.stuff);
                                thing.stackCount = Math.Min(count, thing.def.stackLimit);
                                thingsToDrop.TryAddOrTransfer(thing);
                            }
                        }
                    }
                }
                thingsToDrop.TryDropAll(parent.Position, previousMap, ThingPlaceMode.Near);
            }
        }
    }
    ///<summary>指定物体在被摧毁时掉落一个失温器。</summary>
    public class CompDropThermoDamperOnDestroy : ThingComp
    {
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if (mode == DestroyMode.KillFinalize || mode == DestroyMode.Deconstruct)
            {
                GenSpawn.Spawn(ThingMaker.MakeThing(ThermoDamperDef), parent.Position, previousMap);
            }
        }
    }

}
