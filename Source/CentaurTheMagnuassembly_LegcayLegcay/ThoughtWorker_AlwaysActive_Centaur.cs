using Verse;
using RimWorld;
using static CentaurTheMagnuassembly.RimCentaurCore;
//using AbilityUser;

namespace CentaurTheMagnuassembly
{
    /// <summary>
    /// TODO: Fix relation
    /// </summary>
    public class ThoughtWorker_AlwaysActive_Centaur : ThoughtWorker // ThoughtWorker_AlwaysActive
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn otherPawn)
        {
            Log.Message("[Magnuassembly]Soving CurrentSocialStateInternal between \"" + p.Name.ToStringShort + "(" + p.def.defName + ")\" and " + otherPawn.Name.ToStringShort + "(" + otherPawn.def.defName + ")\".");
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
