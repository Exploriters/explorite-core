/********************
 * 血肉之树类。
 * 
 * --siiftun1857
 */
using RimWorld;
using System;
using Verse;

namespace Explorite
{
	///<summary>血肉之树类型。</summary>
	public class Plant_FleshTree : Plant
	{
		private float? corpseScaleStash = null;
		private float GetcorpseScaleInt
		{
			get
			{
				float num = 0f;
				if (GetComp<CompThingHolder>()?.innerContainer?.Any == true)
				{
					foreach (Thing thing in GetComp<CompThingHolder>().innerContainer)
					{
						if (thing is Corpse corpse)
						{
							num += corpse.InnerPawn.BodySize;
						}
					}
				}
				return num;
			}
		}
		public float GetcorpseScale => corpseScaleStash ??= GetcorpseScaleInt;

		public override string GetInspectString()
		{
			return $"{base.GetInspectString()}\n{"HarvestYield".Translate()}: {def.plant.harvestYield * GetcorpseScale:0.##}";
		}
		public override void ExposeData()
		{
			base.ExposeData();

			//Scribe_Values.Look(ref GetcorpseScale, "innerContainer", 1f, true);
		}
		public override float GrowthRate => Math.Max(1f, base.GrowthRate);
		protected override bool Resting => false;
		public override int YieldNow()
		{
			int result = base.YieldNow();
			if (result > 0)
			{
				result = Math.Max(1, (int)Math.Floor(result * GetcorpseScale));
			}
			return result;
		}
	}

}
