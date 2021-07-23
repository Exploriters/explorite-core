/********************
 * 远程效果设备。
 * --siiftun1857
 */
using System.Collections.Generic;
using Verse;
using RimWorld;
using Verse.AI;
using System.Linq;

namespace Explorite
{
    ///<summary>为<see cref = "Verb_RemoteActivator" />接收参数。</summary>
    public class VerbProperties_RemoteApparelActivator : VerbProperties
    {
        [NoTranslate]
        public List<string> tags;
        public VerbProperties_RemoteApparelActivator()
        {
            verbClass = typeof(Verb_RemoteActivator);
        }
    }
    ///<summary>远程效果激活设备，激活<see cref = "CompRemoteActivationEffect" />。</summary>
    public class Verb_RemoteActivator : Verb_CastBase
    {
        VerbProperties_RemoteApparelActivator VerbProps => verbProps as VerbProperties_RemoteApparelActivator;
        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            if (!ValidateTarget(target, showMessages))
            {
                return false;
            }
            if (target.HasThing)
            {
                if (target.Thing.AnyTriggers(VerbProps.tags))
                {
                    return true;
                }
            }
            if (showMessages)
            {
                Messages.Message("AbilityCantApplyOnMissingActivationDevice".Translate(), target.Thing, MessageTypeDefOf.RejectInput, false);
            }
            return false;
        }
        protected override bool TryCastShot()
        {
            if (ValidateTarget(currentTarget))
            {
                return false;
            }
            IEnumerable<CompRemoteActivationEffect> effectComps = currentTarget.Thing.RemoteTriggers(VerbProps.tags);
            bool flag = false;
            foreach (CompRemoteActivationEffect effectComp in effectComps)
            {
                if (effectComp?.TryActive(VerbProps.tags) ?? false)
                    flag = true;
            }
            return flag;
        }
    }
}
