/**
 * 什么都不做的verb。
 * 
 * 未实现。
 * --siiftun1857
 */
using Verse;
//using AbilityUser;

namespace Explorite
{
    /**
     * <summary>
     * 什么都不做。
     * </summary>
     */
    public class Verb_DoNothing : Verb
    {
        protected override bool TryCastShot()
        {
            //throw new NotImplementedException();
            return false;
        }
    }

}
