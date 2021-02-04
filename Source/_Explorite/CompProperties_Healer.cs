/**
 * 使物品的生命值随着时间逐渐恢复的Comp类。
 */
using System;
using Verse;

namespace Explorite
{
    /**
     * <summary>治疗类效果参数的基类。<br />
     * 为<see cref = "Explorite.CompSelfHealOvertime" />，<see cref = "Explorite.CompHealWearedOvertime" />，<see cref = "Explorite.CompPawnRepairApparelsOvertime" />，等类型及其子类接收参数。</summary>
     */
    public class CompProperties_Healer : CompProperties
    {
        ///<summary>每秒恢复的生命值，一刻恢复的生命值的60倍。</summary>
        public double detlaHpPerSec = 0;
        ///<summary>恢复生命值的最低间隔。指定该值可以使生命值呈阶段性恢复。</summary>
        public int ticksBetweenHeal = -1;
        public CompProperties_Healer() : base(typeof(ThingComp))
        {
        }
        public CompProperties_Healer(Type cc) : base(cc)
        {
        }
    }
}
