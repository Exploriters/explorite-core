/********************
 * 包含多个继承自Hediff的类。
 * --siiftun1857
 */
using Verse;

namespace Explorite
{
    ///<summary>物理操作仪的hediff类型。无实际效果。</summary>
    public class Hediff_AddedPart_HManipulator : Hediff_AddedPart
    {
        //T___O_NOMORE__D____O: Prevent remove part surgery targeting HM
        //NOT HERE!
        //Solved somewhere other
    }

    ///<summary>空白子系统的hediff类，控制是否显示这一症状。</summary>
    public class Hediff_AddedPart_BlankSubsystem : Hediff_AddedPart
    {
        public override bool Visible => (pawn?.health?.hediffSet?.hediffs?.Any(hediff => hediff?.def?.tags?.Contains("CentaurTechHediff_AccuSubsystem") ?? false)) ?? false;
    }

}
