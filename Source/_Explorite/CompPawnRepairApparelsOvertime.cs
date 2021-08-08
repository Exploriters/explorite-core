/********************
 * 使生物持续修复身上的衣物。
 * 
 * 被半人马使用。
 * --siiftun1857
 */
using System.Collections.Generic;
using RimWorld;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
	///<summary>为<see cref = "CompPawnRepairApparelsOvertime" />接收参数。</summary>
	public class CompProperties_PawnRepairApparelsOvertime : CompProperties_Healer
	{
		public CompProperties_PawnRepairApparelsOvertime()
		{
			compClass = typeof(CompPawnRepairApparelsOvertime);
		}
	}
	///<summary>使生物持续修复身上的衣物。</summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage(null, "IDE1006")]
	public class CompPawnRepairApparelsOvertime : ThingComp
	{
		public int lastHealTick = -1;

		public int ticksBetweenHeal => ((CompProperties_PawnRepairApparelsOvertime)props).ticksBetweenHeal;
		public bool valid => ticksBetweenHeal >= 0;

		public Pawn pawn => (Pawn)parent;
		public override void CompTick()
		{
			base.CompTick();
			if (!valid)
				return;
			if (lastHealTick < 0 || InGameTick >= lastHealTick + ticksBetweenHeal)
			{
				List<Thing> DamagedThings = new List<Thing>();
				List<Apparel> WornApparel = pawn.apparel.WornApparel;
				foreach (Apparel apparel in WornApparel)
				{
					if (apparel.def.useHitPoints && apparel.HitPoints < apparel.MaxHitPoints)
					{
						DamagedThings.Add(apparel);
					}
				}
				List<ThingWithComps> WornEquipment = pawn.equipment.AllEquipmentListForReading;
				foreach (ThingWithComps equipment in WornEquipment)
				{
					if (equipment.def.useHitPoints && equipment.HitPoints < equipment.MaxHitPoints)
					{
						DamagedThings.Add(equipment);
					}
				}

				if (DamagedThings.Any())
				{
					DamagedThings.RandomElement().HitPoints += 1;
					lastHealTick = InGameTick;
				}
				else
				{
					lastHealTick = InGameTick - (ticksBetweenHeal / 20) - (ticksBetweenHeal % 20 >= 1 ? 1 : 0);
				}
			}
		}
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref lastHealTick, "lastHealTick", -1);
		}
	}
}
