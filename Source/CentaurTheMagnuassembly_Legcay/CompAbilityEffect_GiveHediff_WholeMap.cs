using UnityEngine;
using Verse;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static CentaurTheMagnuassembly.RimCentaurCore;

namespace CentaurTheMagnuassembly
{
    public class CompAbilityEffect_GiveHediff_WholeMap : CompAbilityEffect_GiveHediff
    {
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            //foreach (Thing thing in target.Thing.Map.listerThings.AllThings)
            foreach (Pawn thing in target.Thing.Map.mapPawns.AllPawns)
            {
                base.Apply(thing, dest);
            }
        }
    }



}
