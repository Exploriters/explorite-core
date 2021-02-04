using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using static CentaurTheMagnuassembly.RimCentaurCore;

namespace CentaurTheMagnuassembly
{
    /// <summary>
    /// TODO
    /// </summary>
    public class CompProperties_UnbreakableViaDamage : CompProperties
    {
        public double detlaHpPerSec = 0;
        public int ticksBetweenHeal = -1;

        public CompProperties_UnbreakableViaDamage() : base(typeof(CompUnbreakableViaDamage))
        {
        }
        public CompProperties_UnbreakableViaDamage(Type cc) : base(cc == typeof(ThingComp) ? typeof(CompUnbreakableViaDamage) : cc)
        {
        }
    }

    public class CompUnbreakableViaDamage : ThingComp
    {
        
        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            base.PostPreApplyDamage(dinfo, out absorbed);
            //absorbed = false;
            float dmgamount = dinfo.Amount;

            if (parent.HitPoints - dmgamount <= 2)
            {
                dmgamount = Math.Max(0f,parent.HitPoints - 2);
                if (dmgamount == 0f)
                    absorbed = true;
                dinfo.SetAmount(dmgamount);
            }
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
        }

    }

}
