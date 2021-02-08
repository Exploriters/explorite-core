﻿/**
 * 提供容器，在物体被摧毁时释放内容。
 * 
 * --siiftun1857
 */
using System.Collections.Generic;
using Verse;

namespace Explorite
{
    /**
     * <summary>提供容器，在物体被摧毁时释放内容。</summary>
     */
    public class CompThingHolder : ThingComp, IThingHolder
    {
        public ThingOwner<Thing> innerContainer = new ThingOwner<Thing>();

        void IThingHolder.GetChildHolders(List<IThingHolder> outChildren) { }

        ThingOwner IThingHolder.GetDirectlyHeldThings() => innerContainer;
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            //if(mode != DestroyMode.Vanish)
            {   
                innerContainer.TryDropAll(parent.Position, previousMap, ThingPlaceMode.Near);
            }
        }

        public override string CompInspectStringExtra()
        {
            string text = base.CompInspectStringExtra();
            string str = (innerContainer.Any ? innerContainer.ContentsString : ((string)"UnknownLower".Translate()));
            if (!text.NullOrEmpty())
            {
                text += "\n";
            }
            return text + ("CasketContains".Translate() + ": " + str.CapitalizeFirst());
        }
    }
}