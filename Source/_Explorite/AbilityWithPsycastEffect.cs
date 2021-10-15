/********************
 * 具有灵能效果的技能类。
 * --siiftun1857
 */
using Verse;
using RimWorld;
using Verse.Sound;

namespace Explorite
{
	///<summary>具有灵能效果的技能类。</summary>
	public class AbilityWithPsycastEffect : Ability
	{
		public AbilityWithPsycastEffect() : base() { }
		public AbilityWithPsycastEffect(Pawn pawn) : base(pawn) { }
		public AbilityWithPsycastEffect(Pawn pawn, AbilityDef def) : base(pawn, def) { }
		public AbilityWithPsycastEffect(Pawn pawn, Precept sourcePrecept) : base(pawn, sourcePrecept) { }
		public AbilityWithPsycastEffect(Pawn pawn, Precept sourcePrecept, AbilityDef def) : base(pawn, sourcePrecept, def) { }
		public override bool Activate(LocalTargetInfo target, LocalTargetInfo dest)
		{
			if (def.showPsycastEffects)
			{
				if (EffectComps.Any((CompAbilityEffect c) => c.Props.psychic))
				{
					if (def.HasAreaOfEffect)
					{
						FleckMaker.Static(target.Cell, pawn.Map, FleckDefOf.PsycastAreaEffect, def.EffectRadius);
						SoundDefOf.PsycastPsychicPulse.PlayOneShot(new TargetInfo(target.Cell, pawn.Map, false));
					}
					else
					{
						SoundDefOf.PsycastPsychicEffect.PlayOneShot(new TargetInfo(target.Cell, pawn.Map, false));
					}
				}
				else if (def.HasAreaOfEffect && def.canUseAoeToGetTargets)
				{
					SoundDefOf.Psycast_Skip_Pulse.PlayOneShot(new TargetInfo(target.Cell, pawn.Map, false));
				}
			}
			return base.Activate(target, dest);
		}
	}
}