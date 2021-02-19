/********************
 * 使Sayers优先选择尸体作为食物。
 * 
 * 方法被弃用。
 * --siiftun1857
 */
using RimWorld;
using System;
using Verse;
using Verse.AI;
using static Explorite.ExploriteCore;

namespace Explorite
{
    /*
    ///<summary>使Sayers优先选择尸体作为食物。</summary>
    public abstract class JobGiver_GetFood_Sayers : ThinkNode_JobGiver
    {
        private HungerCategory minCategory;

        private float maxLevelPercentage = 1f;

        public bool forceScanWholeMap;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_GetFood_Sayers obj = (JobGiver_GetFood_Sayers)base.DeepCopy(resolve);
            obj.minCategory = minCategory;
            obj.maxLevelPercentage = maxLevelPercentage;
            obj.forceScanWholeMap = forceScanWholeMap;
            return obj;
        }

        public override float GetPriority(Pawn pawn)
        {
            if (pawn?.def != AlienSayersDef)
            {
                return 0f;
            }
            Need_Food food = pawn.needs.food;
            if (food == null)
            {
                return 0f;
            }
            Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Malnutrition);
            bool tooHungry = firstHediffOfDef != null && firstHediffOfDef.Severity > 0.4f;
            if (tooHungry)
            {
                return 0f;
            }
            if ((int)pawn.needs.food.CurCategory < 3 && FoodUtility.ShouldBeFedBySomeone(pawn))
            {
                return 0f;
            }
            if ((int)food.CurCategory < (int)minCategory)
            {
                return 0f;
            }
            if (food.CurLevelPercentage > maxLevelPercentage)
            {
                return 0f;
            }
            if (food.CurLevelPercentage < pawn.RaceProps.FoodLevelPercentageWantEat)
            {
                return 9.55f;
            }
            return 0f;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn?.def != AlienSayersDef)
            {
                return null;
            }
            Need_Food food = pawn.needs.food;
            if (food == null || (int)food.CurCategory < (int)minCategory || food.CurLevelPercentage > maxLevelPercentage)
            {
                return null;
            }
            if (pawn.Downed)
            {
                return null;
            }
            Thing thing = GenClosest.ClosestThingReachable(
                root: pawn.Position,
                map: pawn.Map,
                thingReq: ThingRequest.ForGroup(ThingRequestGroup.Corpse),
                peMode: PathEndMode.Touch,
                traverseParams: TraverseParms.For(pawn),
                maxDistance: 9999f,
                validator: delegate (Thing t)
                {
                    if (!(t is Corpse))
                    {
                        return false;
                    }
                    if (t.IsForbidden(pawn))
                    {
                        return false;
                    }
                    if (!t.IngestibleNow)
                    {
                        return false;
                    }
                    if (!pawn.RaceProps.CanEverEat(t))
                    {
                        return false;
                    }
                    return pawn.CanReserve(t) ? true : false;
                });
            if (thing == null)
            {
                return null;
            }
            return JobMaker.MakeJob(JobDefOf.Ingest, thing);
        }
    }
    */
}