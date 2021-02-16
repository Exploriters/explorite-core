/********************
 * 使物品的生命值随着时间逐渐恢复的Comp类。
 * --siiftun1857
 */
using System;
using Verse;

namespace Explorite
{
    /**
     * <summary>
     * 使物品的生命值随着时间逐渐恢复。<br />
     * 该类为抽象类，<see cref = "CompSelfHealOvertime1" />和<see cref = "CompSelfHealOvertime1" />分别以不同的方式完成了实现。
     * </summary>
     */
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, "IDE1006")]
    public abstract class CompSelfHealOvertime : ThingComp
    {
        public double detlaHpPerSec => ((CompProperties_SelfHealOvertime)props).detlaHpPerSec;
        public int ticksBetweenHeal => ((CompProperties_SelfHealOvertime)props).ticksBetweenHeal;
        public bool Invalid => detlaHpPerSec == 0D;
    }
    /**
     * <summary>为<see cref = "CompSelfHealOvertime" />及其子类接收参数。</summary>
     */
    public class CompProperties_SelfHealOvertime : CompProperties_Healer
    {

        public CompProperties_SelfHealOvertime() : base(typeof(CompSelfHealOvertime2))
        {
        }
        public CompProperties_SelfHealOvertime(Type cc) : base(cc == typeof(ThingComp) ? typeof(CompSelfHealOvertime2) : cc)
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
