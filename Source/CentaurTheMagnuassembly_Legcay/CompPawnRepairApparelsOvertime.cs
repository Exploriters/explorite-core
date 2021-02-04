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
    class CompProperties_PawnRepairApparelsOvertime : CompProperties
    {
        public int ticksBetweenHeal = -1;
        public CompProperties_PawnRepairApparelsOvertime()
        {
            this.compClass = typeof(CompPawnRepairApparelsOvertime);
        }
    }
    [StaticConstructorOnStartup]
    public class CompPawnRepairApparelsOvertime : ThingComp
    {
        public int lastHealTick = -1;
        public int ticksBetweenHeal => ((CompProperties_PawnRepairApparelsOvertime)props).ticksBetweenHeal;
        public bool valid => ticksBetweenHeal >= 0;
        public Pawn pawn => (Pawn)parent;
        public override void CompTick()
        {
            base.CompTick();
            if (!valid)
                return;
            if (lastHealTick < 0 || InGameTick >= lastHealTick + ticksBetweenHeal )
            {
                List<Thing> DamagedThings = new List<Thing>();
                List<Apparel> WornApparel = pawn.apparel.WornApparel;
                foreach (Apparel apparel in WornApparel)
                {
                    if (apparel.def.useHitPoints && apparel.HitPoints < apparel.MaxHitPoints)
                    {
                        DamagedThings.Add(apparel);
                    }
                }
                List<ThingWithComps> WornEquipment = pawn.equipment.AllEquipmentListForReading;
                foreach (ThingWithComps equipment in WornEquipment)
                {
                    if (equipment.def.useHitPoints && equipment.HitPoints < equipment.MaxHitPoints)
                    {
                        DamagedThings.Add(equipment);
                    }
                }

                if (DamagedThings.Count > 0)
                {
                    DamagedThings.RandomElement().HitPoints += 1;
                    lastHealTick = InGameTick;
                }
                else
                {
                    lastHealTick = InGameTick - ticksBetweenHeal / 20 - ( ticksBetweenHeal % 20 >= 1 ? 1:0 );
                }
            }
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref lastHealTick, "lastHealTick", -1);
        }
    }
}
