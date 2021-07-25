/********************
 * 远程效果设备。
 * --siiftun1857
 */
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Explorite
{
    ///<summary>为<see cref = "HediffComp_RemoteActivationEffect" />接收参数。</summary>
    public class HediffCompProperties_RemoteActivationEffect : HediffCompProperties
    {
        public HediffCompProperties_RemoteActivationEffect()
        {
            compClass = typeof(HediffComp_RemoteActivationEffect);
        }

        [NoTranslate]
        public List<string> tags;
    }
    ///<summary>远程效果设备，响应<see cref = "Verb_RemoteActivator" />。</summary>
    public abstract class HediffComp_RemoteActivationEffect : HediffComp, IRemoteActivationEffect
    {
        HediffCompProperties_RemoteActivationEffect Props => props as HediffCompProperties_RemoteActivationEffect;
        public virtual bool CanActiveNow(IEnumerable<string> tags)
        {
            foreach (string tag in tags)
            {
                if (Props.tags.Contains(tag))
                {
                    return true;
                }
            }
            return false;
        }
        public abstract bool ActiveEffect();
    }
}
