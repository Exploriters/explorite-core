/********************
 * 确保物种具有指定技能。
 * 
 * --siiftun1857
 */
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Explorite
{
    ///<summary>为<see cref = "CompRequireBuildingInPowerNet" />接收参数。</summary>
    public class CompProperties_RequireBuildingInPowerNet : CompProperties
    {
        [MustTranslate]
        public string errorReportString;
        [XmlInheritanceAllowDuplicateNodes]
        public CheckOutList<ThingDef> requiredBuildings = new CheckOutList<ThingDef>();
        public CompProperties_RequireBuildingInPowerNet()
        {
            compClass = typeof(CompRequireBuildingInPowerNet);
        }
    }
    ///<summary>检测电网中是否存在指定的建筑物。</summary>
    public class CompRequireBuildingInPowerNet : ThingComp
    {
        CompProperties_RequireBuildingInPowerNet Props => props as CompProperties_RequireBuildingInPowerNet;
        public CheckOutList<ThingDef> RequiredBuildings => Props.requiredBuildings;
        public virtual bool BuildingsRequirementMeet
        {
            get
            {
                return RequiredBuildings.CheckOut(parent?.TryGetComp<CompPower>()?.PowerNet?.powerComps?.Where(comp => comp.PowerOn)?.Select(comp => comp.parent.def) ?? Enumerable.Empty<ThingDef>());
            }
        }
        public override string CompInspectStringExtra()
        {
            return BuildingsRequirementMeet ? null : Props.errorReportString;
        }
        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            foreach (StatDrawEntry statDrawEntry in base.SpecialDisplayStats() ?? Enumerable.Empty<StatDrawEntry>())
            {
                yield return statDrawEntry;
            }
            yield return new StatDrawEntry(
                    category: StatCategoryDefOf.Basics,
                    label: "Magnuassembly_CompRequireBuildingInPowerNet_StatLabel".Translate(),
                    valueString: RequiredBuildings.ReportContact(new CheckOutList<ThingDef>.CheckOutListStringRule(CheckOutList<ThingDef>.presetStringRule)
                    {
                        selector = t => t.label,
                    }),
                    reportText: RequiredBuildings.ReportContact(new CheckOutList<ThingDef>.CheckOutListStringRule(CheckOutList<ThingDef>.presetStringRule2)
                    {
                        selector = t => t.label,
                    }),
                    displayPriorityWithinCategory: 0,
                    overrideReportTitle: "Magnuassembly_CompRequireBuildingInPowerNet_StatTitle".Translate(),
                    hyperlinks: null
                    );

        }
    }
}
