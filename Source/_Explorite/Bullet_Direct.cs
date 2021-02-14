/**
 * 发射后直接命中目标的子弹。
 * --siiftun1857
 */
using RimWorld;
using Verse;

namespace Explorite
{
    ///<summary>发射后直接命中目标的子弹。</summary>
    public class Bullet_Direct : Bullet
    {
        public override void Tick()
        {
            if (AllComps != null)
            {
                int i = 0;
                for (int count = AllComps.Count; i < count; i++)
                {
                    AllComps[i].CompTick();
                }
            }
            Impact(intendedTarget.Thing as Pawn ?? intendedTarget.Pawn ?? null);
            //if (!Destroyed) Destroy();
        }
    }
    ///<summary>只能杀伤大脑的伤害。</summary>
    public class DamageWorker_Frostbrainer : DamageWorker_AddInjury
    {
        public override DamageResult Apply(DamageInfo dinfo, Thing thing)
        {
            if (thing is Pawn pawn)
            {
                dinfo.SetHitPart(pawn.health.hediffSet.GetBrain() ?? pawn.RaceProps.body.corePart);
                dinfo.SetAmount(dinfo.Amount / 2f);
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
}
