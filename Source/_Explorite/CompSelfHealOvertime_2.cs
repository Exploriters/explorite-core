/**
 * 使物品的生命值随着时间逐渐恢复的Comp类。
 * --siiftun1857
 */
using System;
using Verse;

namespace Explorite
{
    /**
     * <summary>
     * 使物品的生命值随着时间逐渐恢复，该实现使用了浮点数。<br />
     * 另请参阅: <seealso cref = "CompSelfHealOvertime1" />，完全基于整型的实现。
     * </summary>
     */
    public class CompSelfHealOvertime2 : CompSelfHealOvertime
    {
        public double detlaHpPerTick => detlaHpPerSec / 60;
        public int detlaHpPerTickInteger => (int)Math.Floor(detlaHpPerTick);
        public double detlaHpPerTickDecimal => detlaHpPerTick - detlaHpPerTickInteger;


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
}
