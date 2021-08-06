/********************
 * Ability继承测试。
 * 
 * 被Sayers使用。
 * --siiftun1857
 */
using RimWorld;
using Verse;

namespace Explorite
{
	///<summary>被种族技能使用自定义Ability类。</summary>
	public class AdditionalAbility : Ability
	{
		public AdditionalAbility(Pawn pawn) : base(pawn) { }
		public AdditionalAbility(Pawn pawn, AbilityDef def) : base(pawn, def) { }

		public override bool CanCast => base.CanCast;
	}
}
