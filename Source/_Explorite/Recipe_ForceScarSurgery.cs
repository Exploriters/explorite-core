/********************
 * 制造永久性伤口的手术。
 * --siiftun1857
 */
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Explorite
{
    ///<summary>制造永久性伤口的手术。</summary>
    public class Recipe_ForceScarSurgery : RecipeWorker
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            /*if (IsViolationOnPawn(pawn, part, Faction.OfPlayer))
            {
                ReportViolation(pawn, billDoer, pawn.FactionOrExtraMiniOrHomeFaction, -20, "GoodwillChangedReason_EuthanizedPawn".Translate(pawn.Named("PAWN")));
            }*/
            foreach (Hediff diff in pawn.health.hediffSet.hediffs.Where(d => d.TryGetComp<HediffComp_GetsPermanent>() != null))
            {
                diff.TryGetComp<HediffComp_GetsPermanent>().IsPermanent = true;
                pawn.health.Notify_HediffChanged(diff);
            }
        }
    }
}
