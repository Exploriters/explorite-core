/********************
 * 提供容器，在物体被摧毁时释放内容。
 * 
 * --siiftun1857
 */
using System.Collections.Generic;
using Verse;

namespace Explorite
{
    ///<summary>提供容器，在物体被摧毁时释放内容。</summary>
    public class CompThingHolder : ThingComp, IThingHolder
    {
        public ThingOwner<Thing> innerContainer = new ThingOwner<Thing>();

        ThingOwner IThingHolder.GetDirectlyHeldThings() => innerContainer;
        void IThingHolder.GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, ((IThingHolder)this).GetDirectlyHeldThings());
        }
        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
            //Scribe_Values.Look(ref innerContainer, "innerContainer", new ThingOwner<Thing>(), true);
        }

        public override void CompTick()
        {
            base.CompTick();
            innerContainer.ThingOwnerTick();
        }
        public override void CompTickRare()
        {
            base.CompTickRare();
            innerContainer.ThingOwnerTickRare();
        }
        public override void CompTickLong()
        {
            base.CompTickLong();
            innerContainer.ThingOwnerTickLong();
        }
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
            string str = innerContainer.Any ? innerContainer.ContentsString : ((string)"NothingLower".Translate());
            if (!text.NullOrEmpty())
            {
                text += "\n";
            }
            return text + ("CasketContains".Translate() + ": " + str.CapitalizeFirst());
        }
    }
}
