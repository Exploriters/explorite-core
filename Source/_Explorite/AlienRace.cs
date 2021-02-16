/********************
 * 对AlienRace.AlienPartGenerator.BodyAddon的拓展
 * --siiftun1857
 */
using RimWorld;
using Verse;

namespace Explorite
{
    public class BodyAddon_MX : AlienRace.AlienPartGenerator.BodyAddon
    {
        public override bool CanDrawAddon(Pawn pawn)
        {
            //如果尸体已经风干了，则不再显示耳朵和尾巴等部位
            if (pawn.IsDessicated())
            {
                return false;
            }
            return base.CanDrawAddon(pawn);
        }
    }

}
