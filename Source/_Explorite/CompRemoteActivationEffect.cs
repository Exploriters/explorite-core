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
    public static class RemoteActiveUtility
    {
        public static bool CompPredicate(ThingComp comp, List<string> tags)
        {
            return comp is CompRemoteActivationEffect compa && compa.CanActiveNow(tags);
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
                }
            }
            return false;
        }
        public static IEnumerable<CompRemoteActivationEffect> RemoteTriggers(this Thing thing, List<string> tags)
        {
            if (thing is ThingWithComps thingWithComps)
            {
                foreach (CompRemoteActivationEffect comp in thingWithComps.AllComps.Where(comp => CompPredicate(comp, tags)) as IEnumerable<CompRemoteActivationEffect>)
                {
                    yield return comp;
                }
                if (thing is Pawn pawn)
                {
                    foreach (Apparel apparel in pawn.apparel.WornApparel)
                    {
                        foreach (CompRemoteActivationEffect comp in apparel.AllComps.Where(comp => CompPredicate(comp, tags)) as IEnumerable<CompRemoteActivationEffect>)
                        {
                            yield return comp;
                        }
                    }
                }
            }
        }
    }
    ///<summary>为<see cref = "CompRemoteActivationEffect" />接收参数。</summary>
    public class CompProperties_RemoteActivationEffect : CompProperties
    {
        public CompProperties_RemoteActivationEffect()
        {
            compClass = typeof(CompRemoteActivationEffect);
        }

        [NoTranslate]
        public List<string> tags;
    }
    ///<summary>远程效果设备，响应<see cref = "Verb_RemoteActivator" />。</summary>
    public class CompRemoteActivationEffect : ThingComp
    {
        CompProperties_RemoteActivationEffect Props => props as CompProperties_RemoteActivationEffect;
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
        public bool TryActive(IEnumerable<string> tags)
        {
            if (CanActiveNow(tags))
                return ActiveEffect();
            return false;
        }
        public virtual bool ActiveEffect()
        {
            return true;
        }
    }

    ///<summary>远程效果服装设备，仅在被穿戴时才会响应。</summary>
    public class CompRemoteActivationEffect_Apparel : CompRemoteActivationEffect
    {
        public Pawn Wearer => parent is Apparel apparel ? apparel.Wearer : null;
        public override bool CanActiveNow(IEnumerable<string> tags)
        {
            return base.CanActiveNow(tags) && Wearer != null;
        }
    }

    ///<summary>为<see cref = "CompRemoteActivationEffect_Apparel_ApplyHediff" />接收参数。</summary>
    public class CompProperties_RemoteActivationEffect_Apparel_ApplyHediff : CompProperties_RemoteActivationEffect
    {
        public CompProperties_RemoteActivationEffect_Apparel_ApplyHediff()
        {
            compClass = typeof(CompRemoteActivationEffect_Apparel_ApplyHediff);
        }

        public Hediff hediff;
        public BodyPartDef part;
    }
    ///<summary>响应效果时为穿戴者施加hediff。</summary>
    public class CompRemoteActivationEffect_Apparel_ApplyHediff : CompRemoteActivationEffect_Apparel
    {
        CompProperties_RemoteActivationEffect_Apparel_ApplyHediff Props => props as CompProperties_RemoteActivationEffect_Apparel_ApplyHediff;
        public override bool ActiveEffect()
        {
            BodyPartRecord partrec = Props.part == null ? null: Wearer.def.race.body.AllParts.Where(p => p.def == Props.part).RandomElement();
            Wearer.health.AddHediff(Props.hediff, partrec);
            return true;
        }
    }
}
