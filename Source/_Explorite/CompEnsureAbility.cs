/**
 * 确保物种具有指定技能。
 * 
 * --siiftun1857
 */
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Explorite
{
    /**
     * <summary>为<see cref = "CompEnsureAbility" />接收参数。</summary>
     */
    public class CompProperties_EnsureAbility : CompProperties
    {
        public List<AbilityDef> abilities = new List<AbilityDef>();
        public CompProperties_EnsureAbility()
        {
            compClass = typeof(CompEnsureAbility);
        }
    }
    /**
        * <summary>确保人物具有指定技能。</summary>
        */
    public class CompEnsureAbility : ThingComp
    {
        CompProperties_EnsureAbility Props => (CompProperties_EnsureAbility)props;
        public List<AbilityDef> abilities => Props.abilities;
        public override void CompTick()
        {
            base.CompTick();
            ApplayAbilities();
        }
        public virtual void ApplayAbilities()
        {
            if (
                abilities.Count >= 1
                && parent is Pawn pawn)
            {
                foreach (AbilityDef abilityDef in abilities)
                {
                    if (pawn.abilities.GetAbility(abilityDef) == null)
                    {
                        pawn.abilities.GainAbility(abilityDef);
                    }
                }
            }
        }
    }
}
