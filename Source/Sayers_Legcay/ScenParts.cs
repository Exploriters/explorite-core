using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Sayers
{
    class Class2
    {
    }
    public class ScenPart_ScatteredGarbage : ScenPart
    {
        public override void PostGameStart()
        {
            base.PostGameStart();
            List<Thing> Things = Find.CurrentMap.listerThings.AllThings;
            foreach (Thing thing in Things)
            {
                if (thing.def == ThingDefOf.DropPodIncoming)
                {
                    ThingDef Filth_SayersMucus = DefDatabase<ThingDef>.GetNamed("Filth_SayersMucus");
                    ((DropPodIncoming)thing).Contents.innerContainer
                        .TryAdd(
                        ThingMaker.MakeThing(Filth_SayersMucus)
                        , 3
                        );
                }
            }
        }
    }
}
