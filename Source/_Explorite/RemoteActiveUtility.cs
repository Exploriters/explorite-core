/********************
 * 远程效果设备。
 * --siiftun1857
 */
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Explorite
{
    public interface IRemoteActivationEffect
    {
        bool CanActiveNow(IEnumerable<string> tags);
        bool ActiveEffect();
    }
    public static class RemoteActiveUtility
    {
        public static bool TryActive(this IRemoteActivationEffect activeDevice, IEnumerable<string> tags)
        {
            if (activeDevice.CanActiveNow(tags))
                return activeDevice.ActiveEffect();
            return false;
        }
        public static bool ActiveTriggers(IEnumerable<IRemoteActivationEffect> effects, List<string> tags)
        {
            bool flag = false;
            foreach (IRemoteActivationEffect effect in effects)
            {
                if (effect?.TryActive(tags) ?? false)
                    flag = true;
            }
            return flag;
        }
        public static bool CompPredicate(object trigger, List<string> tags)
        {
            return trigger is IRemoteActivationEffect compa && compa.CanActiveNow(tags);
        }
        public static IRemoteActivationEffect CompSelecter(object trigger)
        {
            return trigger as IRemoteActivationEffect;
        }
        public static bool AnyTriggers(this Thing thing, List<string> tags)
        {
            if (thing is ThingWithComps thingWithComps)
            {
                if (thingWithComps.AllComps.Any(comp => CompPredicate(comp, tags)))
                {
                    return true;
                }
                if (thing is Pawn pawn)
                {
                    foreach (Apparel apparel in pawn.apparel.WornApparel)
                    {
                        if (apparel.AllComps.Any(comp => CompPredicate(comp, tags)))
                        {
                            return true;
                        }
                    }
                    foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
                    {
                        if (hediff is HediffWithComps hediffWithComps && hediffWithComps.comps.Any(comp => CompPredicate(comp, tags)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static IEnumerable<IRemoteActivationEffect> RemoteTriggers(this Thing thing, List<string> tags)
        {
            if (thing is ThingWithComps thingWithComps)
            {
                foreach (IRemoteActivationEffect effect in thingWithComps.AllComps.Where(comp => CompPredicate(comp, tags)).Select(CompSelecter))
                {
                    yield return effect;
                }
                if (thing is Pawn pawn)
                {
                    foreach (Apparel apparel in pawn.apparel.WornApparel)
                    {
                        foreach (IRemoteActivationEffect effect in apparel.AllComps.Where(comp => CompPredicate(comp, tags)).Select(CompSelecter))
                        {
                            yield return effect;
                        }
                    }
                    foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
                    {
                        if (hediff is HediffWithComps hediffWithComps)
                        {
                            foreach (IRemoteActivationEffect effect in hediffWithComps.comps.Where(comp => CompPredicate(comp, tags)).Select(CompSelecter))
                            {
                                yield return effect;
                            }
                        }
                    }
                }
            }
        }
    }
}
