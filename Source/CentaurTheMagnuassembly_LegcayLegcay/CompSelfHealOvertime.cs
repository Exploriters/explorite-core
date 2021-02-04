using System;
using Verse;

namespace CentaurTheMagnuassembly
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("", "IDE1006")]
    public class CompSelfHealOvertime : ThingComp
    {
        public int ticksWithoutHeal = 0;
        public double detlaHpPerSec { get { return ((CompProperties_SelfHealOvertime)props).detlaHpPerSec; } }
        public int ticksBetweenHeal
        {
            get
            {
                if (((CompProperties_SelfHealOvertime)props).ticksBetweenHeal <= 0)
                {
                    return (int)((GenTicks.TicksPerRealSecond / detlaHpPerSec) + 0.5);
                }
                else
                {
                    return ((CompProperties_SelfHealOvertime)props).ticksBetweenHeal;
                }
            }
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ticksWithoutHeal, "ticksWithoutHeal", 0);
        }
        private void OnTickAction(double tickRateFactor = 60.0)
        {
            if (detlaHpPerSec == 0 || parent.HitPoints == parent.MaxHitPoints || parent.HitPoints <= 0)
                return;

            ticksWithoutHeal++;

            if (ticksWithoutHeal >= ticksBetweenHeal)
            {
                double detlaHpAmount = detlaHpPerSec * (ticksWithoutHeal / tickRateFactor);


                double absheal = detlaHpAmount;
                int leastamount;
                if (detlaHpAmount < 0)
                {
                    absheal = -absheal;
                    leastamount = -(int)absheal;
                }
                else
                {
                    leastamount = (int)absheal;
                }

                Random rnd = new Random();

                double chancePerTick = detlaHpAmount % 1;
                parent.HitPoints += leastamount;

                for (int k = 0; k < chancePerTick; k++)
                {
                    if (rnd.Next(0, 9999) / 10000.0 < chancePerTick)
                    {
                        parent.HitPoints += 1;
                    }
                }
                if (parent.HitPoints > parent.MaxHitPoints)
                    parent.HitPoints = parent.MaxHitPoints;

                ticksWithoutHeal -= ticksBetweenHeal;
            }
        }
        public override void CompTick()
        {
            base.CompTick();
            OnTickAction(GenTicks.TicksPerRealSecond);
        }
        /*public override void CompTickRare()
        {
            base.CompTickRare();
            OnTickAction(60);
        }*/
    }
    public class CompProperties_SelfHealOvertime : CompProperties
    {
        public double detlaHpPerSec = 0;
        public int ticksBetweenHeal = -1;

        public CompProperties_SelfHealOvertime() : base(typeof(CompSelfHealOvertime))
        {
        }
        public CompProperties_SelfHealOvertime(Type cc) : base(cc == typeof(ThingComp) ? typeof(CompSelfHealOvertime) : cc)
        {
        }
        //GIVE UP FOR NOW TILL I K HOW TO DO
        /*public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
        {
            IEnumerable<StatDrawEntry> result = base.SpecialDisplayStats(req);
            double detlaHpPerSec=0;
            foreach (object cp in req.Thing.def.comps)
            {
                if (cp.GetType() == typeof(CompProperties_SelfHealOvertime))
                {
                    if (((CompProperties_SelfHealOvertime)cp)?.detlaHpPerSec != null && ((CompProperties_SelfHealOvertime)cp) != null)
                    {
                        detlaHpPerSec = ((CompProperties_SelfHealOvertime)cp).detlaHpPerSec;
                        if (detlaHpPerSec != 0D)
                        {

                            string valueString;
                            if (1 / Math.Abs(detlaHpPerSec) > 60000D)
                            {
                                valueString = "PeriodYears".Translate(detlaHpPerSec * 60000 + " /");
                            }
                            else if (1 / Math.Abs(detlaHpPerSec) > 15000D)
                            {
                                valueString = "PeriodQuadrums".Translate(detlaHpPerSec * 15000 + " /");
                            }
                            else if (1 / Math.Abs(detlaHpPerSec) > 1000D)
                            {
                                valueString = "PeriodDays".Translate(detlaHpPerSec * 1000 + " /");
                            }
                            else if (1 / Math.Abs(detlaHpPerSec) > 41.666666666666666666666666666667)
                            {
                                valueString = "PeriodHours".Translate(detlaHpPerSec * 41.666666666666666666666666666667 + " /");
                            }
                            else
                            {
                                valueString = "PeriodSeconds".Translate(detlaHpPerSec + " /");
                            }

                            StatDrawEntry stat_detla = new StatDrawEntry(
                                StatCategoryDefOf.Basics,
                                "Magnuassembly_CompProperties_SelfHealOvertime_detlaHpPerSec_label".Translate(),
                                valueString,
                                0,
                                "Magnuassembly_CompProperties_SelfHealOvertime_detlaHpPerSec_description".Translate());
                            (result as List<StatDrawEntry>).Add(stat_detla);
                        }
                    }
                }
            }
            return result;
        }*/
    }
}
