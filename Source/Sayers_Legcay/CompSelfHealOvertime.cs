using System;
using Verse;

namespace Sayers
{
    public class CompSelfHealOvertime : ThingComp
    {
        public double detlaHpPerSec => ((CompProperties_SelfHealOvertime)props).detlaHpPerSec;
        public double detlaHpPerTick => detlaHpPerSec / 60;
        public int detlaHpPerTickInteger => (int)Math.Floor(detlaHpPerTick);
        public double detlaHpPerTickDecimal => detlaHpPerTick - detlaHpPerTickInteger;
        public int ticksBetweenHeal => ((CompProperties_SelfHealOvertime)props).ticksBetweenHeal;
        public bool Invalid => detlaHpPerSec == 0D;


        public double exceedHealth = 0;

        public override void PostExposeData()
        {
            
            base.PostExposeData();

            Scribe_Values.Look(ref exceedHealth, "exceedHealth", 0D);
        }
        public override void CompTick()
        {
            base.CompTick();

            if (Invalid)
                return;

            if (parent.HitPoints < parent.MaxHitPoints)
            {
                exceedHealth += detlaHpPerTickDecimal;

                int exceedHealthInteger = (int)Math.Floor(exceedHealth);

                parent.HitPoints = Math.Min(
                    parent.MaxHitPoints,
                    parent.HitPoints + detlaHpPerTickInteger + exceedHealthInteger
                    );

                exceedHealth -= exceedHealthInteger;

                if (parent.HitPoints <= 0)
                    parent.Destroy(DestroyMode.KillFinalize);
            }
            else
            {
                exceedHealth = 0D;
            }
        }
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
    }
}
