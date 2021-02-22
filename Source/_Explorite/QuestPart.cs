using System.Collections.Generic;
using Verse;
using RimWorld;
using System.Collections;

namespace Explorite
{
    public class QuestPart_HyperLinks : QuestPart, IEnumerable<Dialog_InfoCard.Hyperlink>
    {
        private List<Dialog_InfoCard.Hyperlink> hyperlinks = new List<Dialog_InfoCard.Hyperlink>();
        public override IEnumerable<Dialog_InfoCard.Hyperlink> Hyperlinks => hyperlinks;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref hyperlinks, "hyperlinks", LookMode.Deep);
        }

        public IEnumerator<Dialog_InfoCard.Hyperlink> GetEnumerator() => hyperlinks.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => hyperlinks.GetEnumerator();
        public void Add(Dialog_InfoCard.Hyperlink hyperlink)
        {
            hyperlinks.Add(hyperlink);
        }
        public void Add(Dialog_InfoCard infoCard, int statIndex = -1)
        {
            hyperlinks.Add(new Dialog_InfoCard.Hyperlink(infoCard, statIndex));
        }
        public void Add(Def def, int statIndex = -1)
        {
            hyperlinks.Add(new Dialog_InfoCard.Hyperlink(def, statIndex));
        }
        public void Add(Thing thing, int statIndex = -1)
        {
            hyperlinks.Add(new Dialog_InfoCard.Hyperlink(thing, statIndex));
        }
        public void Add(RoyalTitleDef titleDef, Faction faction, int statIndex = -1)
        {
            hyperlinks.Add(new Dialog_InfoCard.Hyperlink(titleDef, faction, statIndex));
        }
    }
}
