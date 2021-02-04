using Verse;
using System;

namespace CentaurTheMagnuassembly
{
    public class CompProperties_HealWearedOvertime : CompProperties
    {
        public double detlaHpPerSec = 0;
        public int ticksBetweenHeal = -1;

        public CompProperties_HealWearedOvertime() : base(typeof(CompHealWearedOvertime))
        {
        }
        public CompProperties_HealWearedOvertime(Type cc) : base(cc == typeof(ThingComp) ? typeof(CompHealWearedOvertime) : cc)
        {
        }
    }
    /// <summary>
    /// TODO
    /// </summary>
    public class CompHealWearedOvertime : ThingComp
    {
        public int ticksWithoutHeal = 0;
        public double detlaHpPerSec { get { return ((CompProperties_SelfHealOvertime)props).detlaHpPerSec; } }
        public int ticksBetweenHeal
        {
            get
            {
                if (((CompProperties_SelfHealOvertime)props).ticksBetweenHeal <= 0)
                {
                    return (int)((60 / detlaHpPerSec) + 0.5);
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
            Scribe_Values.Look(ref ticksWithoutHeal, "ticksWithoutHeal", 0, true);
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
            OnTickAction(60);
        }
        /*public override void CompTickRare()
        {
            base.CompTickRare();
            OnTickAction(60);
        }*/
    }
}
