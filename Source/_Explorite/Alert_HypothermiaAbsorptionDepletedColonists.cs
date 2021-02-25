/********************
 * 在半人马的低温吸收耗尽后发出警告
 * --siiftun1857
 */
using RimWorld;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Explorite
{
    public class Alert_HypothermiaAbsorptionDepletedColonists : Alert_Critical
    {
        private readonly List<Pawn> exposingColonistsResult = new List<Pawn>();

        private List<Pawn> ExposingColonists
        {
            get
            {
                exposingColonistsResult.Clear();
                foreach (Pawn item in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists_NoCryptosleep)
                {
                    if (item?.needs?.TryGetNeed<Need_HypothermiaAbsorption>()?.Depleted == true)
                    {
                        exposingColonistsResult.Add(item);
                    }
                }
                return exposingColonistsResult;
            }
        }

        public Alert_HypothermiaAbsorptionDepletedColonists()
        {
            defaultLabel = "Magnuassembly_HypothermiaAbsorptionDepleted".Translate();
            defaultPriority = AlertPriority.Critical;
        }

        public override TaggedString GetExplanation()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Pawn exposingColonist in ExposingColonists)
            {
                stringBuilder.AppendLine("  - " + exposingColonist.NameShortColored.Resolve());
            }
            return "Magnuassembly_HypothermiaAbsorptionDepletedDesc".Translate(stringBuilder.ToString());
        }

        public override AlertReport GetReport()
        {
            return AlertReport.CulpritsAre(ExposingColonists);
        }
    }
}
