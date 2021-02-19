/********************
 * 建立半人马之间始终存在的正面评价。
 * --siiftun1857
 */
using Verse;
using RimWorld;
using static Explorite.ExploriteCore;

namespace Explorite
{
    /// <summary>
    /// 建立半人马之间始终存在的正面评价。
    /// </summary>
    /// TODO: Fix relation
    public class ThoughtWorker_AlwaysActive_Centaur : ThoughtWorker // ThoughtWorker_AlwaysActive
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn otherPawn)
        {
            Log.Message("[Explorite]Soving CurrentSocialStateInternal between \"" + p.Name.ToStringShort + "(" + p.def.defName + ")\" and " + otherPawn.Name.ToStringShort + "(" + otherPawn.def.defName + ")\".");
            if (p.def == AlienCentaurDef && otherPawn.def == AlienCentaurDef)
            {
                //return base.CurrentSocialStateInternal(p, otherPawn);
                return ThoughtState.ActiveAtStage(stageIndex: 0);
            }
            else
                return false;
        }
        /*protected override ThoughtState CurrentStateInternal(Pawn p)
        { 
            
        }*/
    }

}
