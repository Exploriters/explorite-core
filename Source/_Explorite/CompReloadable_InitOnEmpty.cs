/********************
 * 初始化装填数量为0的CompReloadable。
 * --siiftun1857
 */
using RimWorld;
using System.Reflection;

namespace Explorite
{
    ///<summary>初始化装填数量为0的<see cref = "CompReloadable" />。</summary>
    public class CompReloadable_InitOnEmpty : CompReloadable
    {
        public override void PostPostMake()
        {
            /*
            base.PostPostMake();
            //remainingCharges = MaxCharges;
            GetType().GetField("remainingCharges", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, 0);
            */
        }
    }

}
