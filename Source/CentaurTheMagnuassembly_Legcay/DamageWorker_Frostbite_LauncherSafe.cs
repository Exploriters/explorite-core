using Verse;

namespace CentaurTheMagnuassembly
{
    public class DamageWorker_Frostbite_LauncherSafe : DamageWorker_Frostbite
    {

        /*protected override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageResult result)
        {
            if (dinfo.Instigator == pawn)
            {
                return;
            }
            base.ApplySpecialEffectsToPart(pawn, totalDamage, dinfo, result);
        }*/

        public override DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            if (dinfo.Instigator == victim)
            {
                dinfo.SetAmount(0f);
            }
            DamageResult result = base.Apply(dinfo, victim);
            if (dinfo.Instigator == victim)
            {
                result.totalDamageDealt = 0f;
            }
            return result;
            /*if (dinfo.Instigator == victim)
            {
                return null;
            }
            return base.Apply(dinfo, victim);*/
        }
    }

}
