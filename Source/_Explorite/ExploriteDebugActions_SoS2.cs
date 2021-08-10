/********************
 * 包含调试内容。
 * --siiftun1857
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using static Explorite.ExploriteCore;
using static Explorite.ExploriteDebugActions;

namespace Explorite
{
#pragma warning disable IDE0051 // 删除未使用的私有成员
	public static partial class ExploriteDebugActions_SoS2
	{
		///<summary>清除所有自动舰船屋顶上的屋顶。</summary>
		[DebugAction(category: "Explorite", name: "Wipe roofed roofme", allowedGameStates = AllowedGameStates.PlayingOnMap,
			actionType = DebugActionType.Action)]
		private static void WipeRoofedRoofme()
		{
			List<DebugMenuOption> list = new List<DebugMenuOption>();

			ThingDef ShipHullTile = DefDatabase<ThingDef>.GetNamed("ShipHullTile");
			ThingDef ShipHullTileArchotech = DefDatabase<ThingDef>.GetNamed("ShipHullTileArchotech");
			ThingDef ShipHullTileMech = DefDatabase<ThingDef>.GetNamed("ShipHullTileMech");
			ThingDef ShipHullTileWrecked = DefDatabase<ThingDef>.GetNamed("ShipHullTileWrecked");

			Find.WindowStack.Add(new Dialog_DebugOptionListLister(KeepOutOption(delegate ()
			{

				foreach (Thing thing in Find.CurrentMap.listerThings.AllThings.ToList())
				{
					if (thing.def != ShipHullTile
					 && thing.def != ShipHullTileArchotech
					 && thing.def != ShipHullTileMech
					 && thing.def != ShipHullTileWrecked
					 && thing?.TryGetComp<CompRoofMe>() is CompRoofMe compRoofMe)
					{
						ThingDef oofType = null;
						if (compRoofMe.Props.archotech)
						{
							oofType = DefDatabase<ThingDef>.GetNamed("ShipHullTileArchotech");
						}
						else if (compRoofMe.Props.mechanoid)
						{
							oofType = DefDatabase<ThingDef>.GetNamed("ShipHullTileMech");
						}
						else if (compRoofMe.Props.wreckage)
						{
							oofType = DefDatabase<ThingDef>.GetNamed("ShipHullTileWrecked");
						}
						else
						{
							oofType = DefDatabase<ThingDef>.GetNamed("ShipHullTile");
						}
						foreach (IntVec3 cell in thing.OccupiedRect().Cells)
						{
							foreach (Thing off in Find.CurrentMap.thingGrid.ThingsAt(cell).Where(thi => thi.def == oofType).ToList())
							{
								off.DeSpawn();
								off.Destroy();
							}
						}
					}
				}

				Find.CurrentMap.mapDrawer.WholeMapChanged((MapMeshFlag)1023);
				Find.CurrentMap.pathing.RecalculateAllPerceivedPathCosts();
			})));
		}
	}
}
