/********************
 * 只能杀伤大脑的伤害。
 * --siiftun1857
 */
using Verse;

namespace Explorite
{
    ///<summary>只能杀伤大脑的伤害。</summary>
    public class DamageWorker_Frostbrainer : DamageWorker_AddInjury
    {
        public override DamageResult Apply(DamageInfo dinfo, Thing thing)
        {
            if (thing is Pawn pawn)
            {
                dinfo.SetHitPart(pawn.health.hediffSet.GetBrain() ?? pawn.RaceProps.body.corePart);
                //dinfo.SetAmount(dinfo.Amount / 2f);
                return base.Apply(dinfo, pawn);
            }
            else
            {
                return base.Apply(dinfo, thing);
            }
        }
        protected override BodyPartRecord ChooseHitPart(DamageInfo dinfo, Pawn pawn)
        {
            return pawn.health.hediffSet.GetBrain() ?? pawn.RaceProps.body.corePart;
        }
    }

    ///<summary>干涉伤痕永久性几率的伤害。</summary>
    public class DamageWorker_Scarshot : DamageWorker_AddInjury
    {
        public override DamageResult Apply(DamageInfo dinfo, Thing thing)
        {
            dinfo.SetInstantPermanentInjury(true);
            return base.Apply(dinfo, thing);
        }
    }
}
