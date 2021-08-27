/********************
 * 反射镜接收塔建筑物。
 * --siiftun1857
 */

using RimWorld;
using System.Text;
using Verse;
using Explorite;
using System.Collections.Generic;
using System.Linq;

namespace Explorite
{
	public class CompPowerPlantConcentratedSolarPower : CompPowerPlant
	{
		protected override float DesiredPowerOutput => parent.Map.ConcentratedSolarPowerGrid().GetSolarPower(parent.Position, SolarRefectionAimingTypeDefOf.HighGrid);
		public override string CompInspectStringExtra()
		{
			StringBuilder sb = new StringBuilder();
			string baseInspectString = base.CompInspectStringExtra();
			if (!baseInspectString.NullOrEmpty())
			{
				sb.AppendLine(baseInspectString);
			}
			if (Prefs.DevMode)
			{
				foreach (SolarRefectionAimingTypeDef def in DefDatabase<SolarRefectionAimingTypeDef>.AllDefs.Where(d => d.willTrace))
				{
					sb.AppendLine($"[DEV]Cell {def.label}({def.defName}) power: {parent.Map.ConcentratedSolarPowerGrid().GetSolarPower(parent.Position, def)}");
				}
			}
			return sb.ToString().TrimEndNewlines();
		}
		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo gizmo in base.CompGetGizmosExtra())
			{
				yield return gizmo;
			}
			if (Find.Selector.SingleSelectedThing == parent)
			{
				yield return new Command_Action
				{
					defaultLabel = "Assign to auto target",
					action = delegate ()
					{
						parent.Map.ConcentratedSolarPowerGrid().AutoTarget = parent;
					},
					disabled = parent.Map.ConcentratedSolarPowerGrid().AutoTarget == parent
				};
			}
		}
	}
}
