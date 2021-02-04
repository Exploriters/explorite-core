using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
//using Harmony;
using HarmonyLib;
using UnityEngine;
using Verse.AI;
using Verse;
using Verse.Sound;
using static CentaurTheMagnuassembly.RimCentaurCore;

namespace CentaurTheMagnuassembly
{
    class CompProperties_PassiveMeditationFocusGain : CompProperties
    {
        public float focusPerDay = 0f;
        public CompProperties_PassiveMeditationFocusGain()
        {
            this.compClass = typeof(CompPassiveMeditationFocusGain);
        }
    }

    [StaticConstructorOnStartup]
    public class CompPassiveMeditationFocusGain : ThingComp
    {
        public float FocusPerRate => ((CompProperties_PassiveMeditationFocusGain)props).focusPerDay;
        public override void CompTick()
        {
            base.CompTick();
            //((Pawn)parent).psychicEntropy.
        }
    }
}
