/********************
 * 鹿狐的角可收获类。
 * 
 * --siiftun1857
 */
using RimWorld;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
	public class JobDriver_HarvestDeerFoxChocolateHorn : JobDriver_GatherAnimalBodyResources
	{
		protected override float WorkTotal => 100f;
		protected override CompHasGatherableBodyResource GetComp(Pawn animal)
		{
			return animal.TryGetComp<CompDeerFoxChocolateHorn>();
		}
	}
	public class WorkGiver_HarvestDeerFoxChocolateHorn : WorkGiver_GatherAnimalBodyResources
	{
		protected override JobDef JobDef => HarvestDeerFoxChocolateHornJobDef;
		protected override CompHasGatherableBodyResource GetComp(Pawn animal)
		{
			return animal.TryGetComp<CompDeerFoxChocolateHorn>();
		}
	}
	public class CompProperties_DeerFoxChocolateHorn : CompProperties
	{
		public CompProperties_DeerFoxChocolateHorn()
		{
			compClass = typeof(CompDeerFoxChocolateHorn);
		}

		public int gatherResourcesIntervalDays;

		public int resourceAmount = 1;

		public ThingDef resourceDef;
	}
	public class CompDeerFoxChocolateHorn : CompHasGatherableBodyResource
	{
		protected override int GatherResourcesIntervalDays => Props.gatherResourcesIntervalDays;
		protected override int ResourceAmount => Props.resourceAmount;
		protected override ThingDef ResourceDef => Props.resourceDef;
		protected override string SaveKey => "chocolateHornGrowth";
		public CompProperties_DeerFoxChocolateHorn Props => (CompProperties_DeerFoxChocolateHorn)props;

		protected override bool Active
		{
			get
			{
				if (!base.Active)
				{
					return false;
				}
				return true;
			}
		}

		public override string CompInspectStringExtra()
		{
			if (!Active)
			{
				return null;
			}
			return "ChocolateHornGrowth".Translate() + ": " + Fullness.ToStringPercent();
		}
	}
}
