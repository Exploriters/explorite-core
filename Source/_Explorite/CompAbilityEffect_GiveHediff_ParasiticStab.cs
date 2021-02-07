/**
 * 植物液注入技能的主要效果Comp。
 * 
 * --siiftun1857
 */
using RimWorld;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
    /**
     * <summary>Sayers的群体植物液注入的主要功能实现。</summary>
     */
    public class CompAbilityEffect_GiveHediff_ParasiticStab : CompAbilityEffect_GiveHediff
    {
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (target.Pawn?.def != AlienSayersDef)
                base.Apply(target, dest);

        }
    }
}
