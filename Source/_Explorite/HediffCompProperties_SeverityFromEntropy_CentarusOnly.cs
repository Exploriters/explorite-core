/**
 * 半人马的自定义心灵熵效果。
 * 
 * 未实现。
 * --siiftun1857
 */
using Verse;
using static Explorite.ExploriteCore;


namespace Explorite
{
    /**
     * <summary>
     * 半人马的自定义心灵熵效果。<br/>
     * 未实现。
     * </summary>
     */
    public class HediffComp_SeverityFromEntropy_CentarusOnly : HediffComp_SeverityFromEntropy
    {
        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Pawn.def == AlienCentaurDef)
            {
                base.CompPostTick(ref severityAdjustment);
            }
            else
                severityAdjustment = 0f;
            return;
        }
    }
}