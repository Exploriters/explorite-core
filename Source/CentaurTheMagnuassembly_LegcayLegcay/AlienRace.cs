using RimWorld;
using Verse;

namespace CentaurTheMagnuassembly
{
    public class BodyAddon_MX : AlienRace.AlienPartGenerator.BodyAddon
    {
        public override bool CanDrawAddon(Pawn pawn)
        {
            if (pawn.IsDessicated())
            {
                return false;
            }
            return base.CanDrawAddon(pawn);
        }
    }

}
