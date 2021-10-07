/********************
 * 植物液注入技能的主要效果Comp。
 * 
 * --siiftun1857
 */
using RimWorld;
using Verse;
using Verse.Sound;

namespace Explorite
{
	///<summary>为<see cref = "CompAbilityEffect_NeedImpact" />接收参数。</summary>
	public class CompProperties_Ability_NeedImpact : CompProperties_AbilityEffect
	{
		public NeedDef needDef;
		public float needOffset = 0f;
		public float needFactor = 1f;
		public CompProperties_Ability_NeedImpact()
		{
			compClass = typeof(CompAbilityEffect_NeedImpact);
		}
	}
	///<summary>鹿狐的群体冬眠的主要功能实现。</summary>
	public class CompAbilityEffect_NeedImpact : CompAbilityEffect
	{
		public new CompProperties_Ability_NeedImpact Props => (CompProperties_Ability_NeedImpact)props;
		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			//dest.Pawn?.records?.Increment(RecordDefOf.DamageDealt);
			Pawn targetPawn = target.Pawn;
			if (targetPawn != null)
			{
				/*
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
				*/

				if (targetPawn?.needs.TryGetNeed(Props.needDef) is Need need)
				{
					need.CurLevel = need.CurLevel * Props.needFactor + Props.needOffset;
				}
			}
		}
	}
}
