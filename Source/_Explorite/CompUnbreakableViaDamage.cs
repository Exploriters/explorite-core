/**
 * 使物品不被伤害摧毁。
 * 
 * 效果目前不可用。
 * --siiftun1857
 */
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using static Explorite.ExploriteCore;

namespace Explorite
{
    /**
     * <summary>为<see cref = "Explorite.CompUnbreakableViaDamage" />接收参数。</summary>
     */
    public class CompProperties_UnbreakableViaDamage : CompProperties_Healer
    {
        public CompProperties_UnbreakableViaDamage() : base(typeof(CompUnbreakableViaDamage))
        {
        }
        public CompProperties_UnbreakableViaDamage(Type cc) : base(cc == typeof(ThingComp) ? typeof(CompUnbreakableViaDamage) : cc)
        {
        }
    }

    /**
     * <summary>使物品不被伤害摧毁。<br />效果目前不可用。</summary>
     */
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
