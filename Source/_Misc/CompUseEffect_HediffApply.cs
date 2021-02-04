using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
    class CompUseEffect_HediffApply : CompUseEffect
    {
        public override void DoEffect(Pawn usedBy)
        {
			HediffDef HyperManipulatorHediff = DefDatabase<HediffDef>.GetNamed ("HyperManipulator");
            base.DoEffect(usedBy);
			if (!usedBy.health.hediffSet.HasHediff (HyperManipulatorHediff))
				HealthUtility.AdjustSeverity (usedBy, HyperManipulatorHediff, 0.001f);
			
        //    this.parent.Destroy();
        }
    }
}
