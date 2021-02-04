using RimWorld;
using Verse;

namespace Sayers
{
    public class AdditionalAbility : Ability
    {
        public AdditionalAbility(Pawn pawn) : base(pawn) { }
        public AdditionalAbility(Pawn pawn, AbilityDef def) : base(pawn, def) { }

        public override bool CanCast => base.CanCast;
    }
}
