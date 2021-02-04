/**
 * 植物液注入技能的主要效果Comp。
 * 
 * --siiftun1857
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Explorite
{
    /**
     * <summary>Sayers的群体植物液注入的主要功能实现。</summary>
     */
    public class CompAbilityEffect_GiveHediff_ParasiticStab : CompAbilityEffect_GiveHediff
    {
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

        }
    }
}
