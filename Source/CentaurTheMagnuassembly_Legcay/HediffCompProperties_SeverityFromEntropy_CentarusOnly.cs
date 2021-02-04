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
    public class HediffComp_SeverityFromEntropy_CentarusOnly : HediffComp_SeverityFromEntropy
    {
        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Pawn.def == AlienCentaurDef)
            {
                base.CompPostTick(ref severityAdjustment);
            }
            else
                severityAdjustment = 0f;
            return;
        }
    }
}