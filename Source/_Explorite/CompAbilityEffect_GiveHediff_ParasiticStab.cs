/********************
 * 植物液注入技能的主要效果Comp。
 * 
 * --siiftun1857
 */
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
	///<summary>为<see cref = "CompAbilityEffect_GiveHediff_ParasiticStab" />接收参数。</summary>
	public class CompProperties_AbilityGiveHediff_ParasiticStab : CompProperties_AbilityGiveHediff
	{
		public DamageDef injuryOnHit;
		public IntRange injuryCount;
		public FloatRange? injuryDamage;
		public CompProperties_AbilityGiveHediff_ParasiticStab()
		{
			compClass = typeof(CompAbilityEffect_GiveHediff_ParasiticStab);
		}
	}
	///<summary>Sayers的群体植物液注入的粒子效果功能实现。</summary>
	public class CompAbilityEffect_MoteOnTarget_ParasiticStab : CompAbilityEffect_MoteOnTarget
	{
		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			if (target.Pawn != null)
			{
				base.Apply(target, dest);
			}
		}
	}
	///<summary>Sayers的群体植物液注入的粒子效果功能实现。</summary>
	public class CompAbilityEffect_FleckOnTarget_ParasiticStab : CompAbilityEffect_FleckOnTarget
	{
		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			if (target.Pawn != null)
			{
				base.Apply(target, dest);
			}
		}
	}
	///<summary>Sayers的群体植物液注入的主要功能实现。</summary>
	public class CompAbilityEffect_GiveHediff_ParasiticStab : CompAbilityEffect_GiveHediff
	{
		public new CompProperties_AbilityGiveHediff_ParasiticStab Props => (CompProperties_AbilityGiveHediff_ParasiticStab)props;
		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			//dest.Pawn?.records?.Increment(RecordDefOf.DamageDealt);
			Pawn targetPawn = target.Pawn;
			if (targetPawn != null
				&& parent.pawn != targetPawn
				&& targetPawn?.def != AlienSayersDef)
			{
				//BattleLogEntry_MeleeCombat battleLogEntry_RangedImpact = new BattleLogEntry_MeleeCombat(launcher, hitThing, intendedTarget.Thing, equipmentDef, def, targetCoverDef);
				BattleLogEntry_MeleeCombat battleLogEntry = new BattleLogEntry_MeleeCombat(
					ruleDef: DefDatabase<RulePackDef>.GetNamed("Maneuver_Slash_MeleeHit"),
					alwaysShowInCompact: true,
					initiator: parent.pawn,
					recipient: targetPawn,
					implementType: ImplementOwnerTypeDefOf.NativeVerb,
					toolLabel: "",
					ownerEquipmentDef: null,
					ownerHediffDef: null,
					def: LogEntryDefOf.MeleeAttack
					);
				Find.BattleLog.Add(battleLogEntry);
				/*
				int hitcount = 0;

				int num = Mathf.Min(Props.injuryCount.RandomInRange);
				for (int i = 0; i < num; i++)
				{
					if (true
						/ *Rand.Chance(Mathf.Clamp(
							parent.pawn.GetStatValue(StatDefOf.MeleeHitChance) *
							(targetPawn.CurJobDef == JobDefOf.AttackStatic ? 0f : targetPawn.GetStatValue(StatDefOf.MeleeDodgeChance))
							, 0.0f, 1.0f))* /

						)
					{
						hitcount += 1;
						break;
					}
				}*/
				if (!targetPawn.def.race.IsMechanoid)
				{
					base.Apply(target, dest);
				}
				if (Props.injuryOnHit != null)
				{
					targetPawn.TakeDamage(
					new DamageInfo(
						Props.injuryOnHit, Props.injuryDamage?.RandomInRange ?? Props.injuryOnHit.defaultDamage,
						instigator: parent.pawn,
						weapon: parent.pawn.def
						)
					);
				}
			}
		}
	}
}
