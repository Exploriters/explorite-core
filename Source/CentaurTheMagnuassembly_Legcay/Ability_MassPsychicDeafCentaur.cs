using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;
using static CentaurTheMagnuassembly.RimCentaurCore;


namespace CentaurTheMagnuassembly
{
    public class Verb_CastAbility_MassPsychicDeafCentaur : Verb_CastAbility
    {
        public bool ValidToCast => (((Pawn)caster)?.health?.hediffSet.HasHediff(DefDatabase<HediffDef>.GetNamed("PsychicDeafCentaur"))!=true);
        public override bool Available()
        {
            if (!ValidToCast)
                return false;
            return base.Available();
        }
        protected override bool TryCastShot()
        {
            if (!ValidToCast)
                return false;
            return base.TryCastShot();
        }
    }
    /*
    public class Ability_MassPsychicDeafCentaur : Ability
    {
        public Ability_MassPsychicDeafCentaur(Pawn pawn) : base(pawn)
        {
        }

        public Ability_MassPsychicDeafCentaur(Pawn pawn, AbilityDef def) : base(pawn, def)
        {
        }
        public override void AbilityTick()
        {
            base.AbilityTick();
            if (pawn.health.hediffSet.HasHediff(DefDatabase<HediffDef>.GetNamed("PsychicDeafCentaur")))
            {
                gizmo.Disable("Magnuassembly_Ability_MassPsychicDeafCentaur_DeafnessCantCast"
                    .Translate(FormattingTickTime(pawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed("PsychicDeafCentaur"))
                    .TryGetComp<HediffComp_Disappears>().ticksToDisappear)));
            }
            else if(pawn.Drafted)
            {
                gizmo.disabled = false;
            }
        }
        public virtual void QueueCastingJob(LocalTargetInfo target, LocalTargetInfo destination)
        {
            if (!pawn.health.hediffSet.HasHediff(DefDatabase<HediffDef>.GetNamed("PsychicDeafCentaur")))
            {
                base.QueueCastingJob(target, destination);
            }
        }
    }*/

    public class Command_AbilityMassPsychicDeafCentaur : Command_Ability
    {
        bool disabledToDeaf = false;
        public Command_AbilityMassPsychicDeafCentaur(Ability ability) : base(ability)
        {
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
            if (ability.pawn.health.hediffSet.HasHediff(DefDatabase<HediffDef>.GetNamed("PsychicDeafCentaur")))
            {
                Disable("Magnuassembly_Ability_MassPsychicDeafCentaur_DeafnessCantCast"
                    .Translate(FormattingTickTime(ability.pawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed("PsychicDeafCentaur"))
                    .TryGetComp<HediffComp_Disappears>().ticksToDisappear)));
            }
            else if (ability.pawn.Drafted)
            {
                disabled = false;
            }
            GizmoResult gizmoResult = base.GizmoOnGUI(topLeft, maxWidth);
            return gizmoResult;
        }
    }
}