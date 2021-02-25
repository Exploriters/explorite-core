/********************
 * 持续修复穿戴的物品。
 * --siiftun1857
 */
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
    ///<summary>为<see cref = "HediffComp_RepairApparelsOvertime" />接收参数。</summary>
    public class HediffCompProperties_RepairApparelsOvertime : HediffCompProperties_StageRange
    {
        public int ticksBetweenHeal = -1;
        public HediffCompProperties_RepairApparelsOvertime()
        {
            compClass = typeof(HediffComp_RepairApparelsOvertime);
        }
    }
    ///<summary>持续修复生物身上的衣物。</summary>
    public class HediffComp_RepairApparelsOvertime : HediffComp
    {
        public bool InStage => (props as HediffCompProperties_StageRange)?.InStage(parent.CurStageIndex) ?? false;

        public int lastHealTick = -1;

        public int TicksBetweenHeal => (props as HediffCompProperties_RepairApparelsOvertime).ticksBetweenHeal;
        public float HealPerDay =>  60000f / TicksBetweenHeal;
        public bool Valid => TicksBetweenHeal >= 0;

        //public Pawn Pawn => Pawn;
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (!Valid)
                return;
            IEnumerable<Thing> Things = Enumerable.Empty<Thing>()
                .Concat(Pawn.apparel.WornApparel)
                .Concat(Pawn.equipment.AllEquipmentListForReading)
                .Concat(Pawn.inventory.GetDirectlyHeldThings())
                ;
            IEnumerable<Thing> RotableThings = Things.Where(thing =>
                thing.TryGetComp<CompRottable>() is CompRottable comp
                && comp.Stage == RotStage.Fresh
                && comp.RotProgressPct < 1f
                );
            foreach (CompRottable comp in RotableThings.Select(thing => thing.TryGetComp<CompRottable>()))
            {
                comp.RotProgress = Math.Min(comp.PropsRot.TicksToRotStart, comp.RotProgress + (comp.PropsRot.TicksToRotStart * 0.05f));
            }
            if (lastHealTick < 0 || InGameTick >= lastHealTick + TicksBetweenHeal)
            {
                IEnumerable<Thing> DamagedThings = Things.Where(thing => thing.def.useHitPoints && thing.HitPoints < thing.MaxHitPoints);
                if (DamagedThings.Any())
                {
                    DamagedThings.RandomElement().HitPoints += 1;
                    lastHealTick = InGameTick;
                }
                else
                {
                    lastHealTick = InGameTick - (TicksBetweenHeal / 20) - (TicksBetweenHeal % 20 >= 1 ? 1 : 0);
                }
            }
        }
        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref lastHealTick, "lastHealTick", -1);
        }
        public override string CompTipStringExtra => InStage ? "Magnuassembly_HediffComp_RepairApparelsOvertime_TipString".Translate(HealPerDay.ToString("0.00")) : null;
    }
}
