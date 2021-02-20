/********************
 * 使亲昵行为中，社交关系会纳入考量。
 * 
 * --siiftun1857
 */
using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;

namespace Explorite
{
    public class CompProperties_AssignableToPawn_NoPostLoadSpecial : CompProperties_AssignableToPawn
    {
        public override void PostLoadSpecial(ThingDef parent) { }
    }
    ///<summary>具有额外限制条件的人型生物亲昵配置。</summary>
    public class JobGiver_Nuzzle_Humanoid : JobGiver_Nuzzle
    {
        private const float MaxNuzzleDistance = 40f;

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.RaceProps.nuzzleMtbHours <= 0f)
            {
                return null;
            }
            if (pawn.Drafted
             || pawn.jobs.curDriver is JobDriver_AttackMelee
             || pawn.jobs.curDriver is JobDriver_AttackStatic
             || pawn.jobs.curDriver is JobDriver_BeatFire
             || pawn.jobs.curDriver is JobDriver_ExtinguishSelf
                )
            {
                return null;
            }
            if (!(
                from p in pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction)
                where !p.NonHumanlikeOrWildMan()
                   && p != pawn
                   && pawn.relations.OpinionOf(p) > -20
                   && p.Position.InHorDistOf(pawn.Position, MaxNuzzleDistance)
                   && pawn.GetRoom() == p.GetRoom()
                   && !p.Position.IsForbidden(pawn)
                   && p.CanCasuallyInteractNow()
                select p)
                .TryRandomElement(out var result))
            {
                return null;
            }
            Job job = JobMaker.MakeJob(JobDefOf.Nuzzle, result);
            job.locomotionUrgency = LocomotionUrgency.Sprint;
            job.expiryInterval = 3000;
            return job;
        }
    }
}