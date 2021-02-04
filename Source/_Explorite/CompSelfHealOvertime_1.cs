using System;
using Verse;

namespace Explorite
{
    /**
     * <summary>
     * 使物品的生命值随着时间逐渐恢复，该实现完全基于整型。<br />
     * 另请参阅: <seealso cref = "Explorite.CompSelfHealOvertime2" />，使用了浮点数的实现。
     * </summary>
     */
    [System.Diagnostics.CodeAnalysis.SuppressMessage("", "IDE1006")]
    public class CompSelfHealOvertime1 : CompSelfHealOvertime
    {
        public int ticksWithoutHeal = 0;
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
        public override void CompTickRare()
        {
            base.CompTickRare();
            //OnTickAction((float)GenTicks.TicksPerRealSecond / (float)GenTicks.TickRareInterval);
            for (int i = 0; i < 250; i++)
            {
                OnTickAction(GenTicks.TicksPerRealSecond);
            }
        }
    }
}
