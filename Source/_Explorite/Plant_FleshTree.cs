/**
 * 血肉之树类。
 * 
 * --siiftun1857
 */
using RimWorld;
using System;
using Verse;

namespace Explorite
{
    ///<summary>血肉之树类型。</summary>
    public class Plant_FleshTree : Plant
    {
        private float? corpseScaleStash = null;
        public float GetcorpseScale
        {
            get
            {
                if (corpseScaleStash.HasValue)
                    return corpseScaleStash.Value;
                if ((GetComp<CompThingHolder>()?.innerContainer?.Any != true ? null : GetComp<CompThingHolder>()?.innerContainer[0]) is Corpse corpse)
                    return (corpseScaleStash = corpse.InnerPawn.BodySize).Value;
                else
                    return (corpseScaleStash = 1f).Value;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            //Scribe_Values.Look(ref GetcorpseScale, "innerContainer", 1f, true);
        }
        public override float GrowthRate => Math.Max(1f, base.GrowthRate);
        protected override bool Resting => false;
        public override int YieldNow()
        {
            int result = base.YieldNow();
            if (result > 0)
            {
                result = Math.Max(1, (int)Math.Floor(result * GetcorpseScale));
            }
            return result;
        }
    }

}
