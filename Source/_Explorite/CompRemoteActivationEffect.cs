/********************
 * 远程效果设备。
 * --siiftun1857
 */
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace Explorite
{
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
    public class CompRemoteActivationEffect : ThingComp, IRemoteActivationEffect
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
        public virtual bool ActiveEffect()
        {
            return true;
        }
    }

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
    public class HediffComp_RemoteActivationEffect : HediffComp, IRemoteActivationEffect
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

        public HediffDef hediff;
        public BodyPartDef part;
    }
    ///<summary>响应效果时为穿戴者施加hediff。</summary>
    public class CompRemoteActivationEffect_Apparel_ApplyHediff : CompRemoteActivationEffect_Apparel
    {
        CompProperties_RemoteActivationEffect_Apparel_ApplyHediff Props => props as CompProperties_RemoteActivationEffect_Apparel_ApplyHediff;
        public override bool ActiveEffect()
        {
            BodyPartRecord partrec = Props.part == null ? null : Wearer.def.race.body.AllParts.Where(p => p.def == Props.part).RandomElement();

            Hediff hediff = HediffMaker.MakeHediff(Props.hediff, Wearer, partrec);
            hediff.TryGetComp<HediffComp_DisappearsOnSourceApparelLost>()?.AddSources(parent as Apparel);
            Wearer.health.AddHediff(hediff);
            return true;
        }
    }

    ///<summary>为<see cref = "CompRemoteActivationEffect_Destroy" />接收参数。</summary>
    public class CompProperties_RemoteActivationEffect_Destroy : CompProperties_RemoteActivationEffect
    {
        public CompProperties_RemoteActivationEffect_Destroy()
        {
            compClass = typeof(CompRemoteActivationEffect_Destroy);
        }
        public DestroyMode destroyMode = DestroyMode.KillFinalize;
    }
    ///<summary>响应效果时自毁。</summary>
    public class CompRemoteActivationEffect_Destroy : CompRemoteActivationEffect
    {
        CompProperties_RemoteActivationEffect_Destroy Props => props as CompProperties_RemoteActivationEffect_Destroy;
        public override bool ActiveEffect()
        {
            if (!parent.Destroyed)
                parent.Destroy(Props.destroyMode);
            return true;
        }
    }
    ///<summary>为<see cref = "CompRemoteActivationEffect_InvokeExplode" />接收参数。</summary>
    public class CompProperties_RemoteActivationEffect_InvokeExplode : CompProperties_RemoteActivationEffect
    {
        public CompProperties_RemoteActivationEffect_InvokeExplode()
        {
            compClass = typeof(CompRemoteActivationEffect_InvokeExplode);
        }
    }
    ///<summary>响应效果时爆炸。</summary>
    public class CompRemoteActivationEffect_InvokeExplode : CompRemoteActivationEffect
    {
        public override bool ActiveEffect()
        {
            foreach (ThingComp comp in parent.AllComps)
            {
                if (comp is CompExplosive explosive)
                {
                    typeof(CompExplosive).GetMethod("Detonate", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(explosive, new object[]{ parent.MapHeld , true});
                }
            }
            return true;
        }
    }
}
