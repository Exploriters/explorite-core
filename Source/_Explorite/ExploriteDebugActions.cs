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
using UnityEngine;
using Verse;
using Verse.AI;
using static Explorite.ExploriteCore;

namespace Explorite
{
#pragma warning disable IDE0051 // 删除未使用的私有成员
	public static partial class ExploriteDebugActions
	{
		public static IEnumerable<DebugMenuOption> KeepOutOption(Action action, int fallbackCount = 9)
		{
			DebugMenuOption fallback = new DebugMenuOption("Cancel", DebugMenuOptionMode.Action, delegate () { });
			DebugMenuOption todo = new DebugMenuOption("--- Confirm ---", DebugMenuOptionMode.Action, action);
			List<DebugMenuOption> result = new List<DebugMenuOption>();
			result.Add(todo);
			for (int i = 0; i < fallbackCount; i++)
			{
				result.Add(fallback);
			}
			return result.InRandomOrder();
		}
		public static void FalseOption(Action action, int depth)
		{
			if (depth <= 0)
			{
				action();
				return;
			}
			List<DebugMenuOption> result = new List<DebugMenuOption>();
			for (int i = 0; i < 26; i++)
			{
				result.Add(new DebugMenuOption(((char)('a' + i)).ToString(), DebugMenuOptionMode.Action, delegate () { FalseOption(delegate () { }, depth - 1); }));
			}
			DebugMenuOption[] result2 = result.InRandomOrder().ToArray();
			result2[result2.First().label[0] - 'a'].method = delegate () { FalseOption(action, depth - 1); };

			Find.WindowStack.Add(new Dialog_DebugOptionListLister(result2));
		}


		///<summary>清除所有迷雾。</summary>
		[DebugAction(category: "Explorite", name: "Clear fog at...", allowedGameStates = AllowedGameStates.PlayingOnMap,
			actionType = DebugActionType.ToolMap)]
		private static void ClearAllFogClickPlace()
		{
			if (UI.MouseCell().InBounds(Find.CurrentMap))
			{
				Find.CurrentMap.fogGrid.RevealFogCluster(UI.MouseCell());
			}
		}
		///<summary>清除所有迷雾。</summary>
		[DebugAction(category: "Explorite", name: "Clear all fog", allowedGameStates = AllowedGameStates.PlayingOnMap,
			actionType = DebugActionType.Action)]
		private static void ClearAllFog()
		{
			Find.CurrentMap.fogGrid.ClearAllFog();
		}
		///<summary>清除污渍。</summary>
		[DebugAction(category: "Explorite", name: "Wipe filth and mote", allowedGameStates = AllowedGameStates.PlayingOnMap,
			actionType = DebugActionType.Action)]
		private static void WipeFilthAndMote()
		{
			List<DebugMenuOption> list = new List<DebugMenuOption>();
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(KeepOutOption(delegate ()
			{

				foreach (Thing thing in Find.CurrentMap.listerThings.AllThings.ToList())
				{
					if (thing is Filth || thing is Mote)
					{
						thing.DeSpawn();
						thing.Destroy();
					}
				}

				Find.CurrentMap.mapDrawer.WholeMapChanged((MapMeshFlag)1023);
				Find.CurrentMap.pathing.RecalculateAllPerceivedPathCosts();
			})));
		}
		///<summary>完全积雪。</summary>
		[DebugAction(category: "Explorite", name: "Add snow to whole map", allowedGameStates = AllowedGameStates.PlayingOnMap,
			actionType = DebugActionType.Action)]
		private static void SnowWholeMap()
		{
			foreach (IntVec3 c in Find.CurrentMap.AllCells)
			{
				Find.CurrentMap.snowGrid.SetDepth(c, 1f);
			}
			Find.CurrentMap.mapDrawer.WholeMapChanged(MapMeshFlag.Snow);
		}
		///<summary>将整个地图的所有水体都变成冰。</summary>
		[DebugAction(category: "Explorite", name: "Frozen all water to ice", allowedGameStates = AllowedGameStates.PlayingOnMap,
			actionType = DebugActionType.Action)]
		private static void FrozenWholeMapWater()
		{
			List<DebugMenuOption> list = new List<DebugMenuOption>();
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(KeepOutOption(delegate ()
			{
				int count = Find.CurrentMap.terrainGrid.topGrid.Count();
				TerrainDef Marsh = DefDatabase<TerrainDef>.GetNamed("Marsh");
				for (int i = 0; i < count; i++)
				{
					if (Find.CurrentMap.terrainGrid.topGrid[i] == TerrainDefOf.WaterDeep
					 || Find.CurrentMap.terrainGrid.topGrid[i] == TerrainDefOf.WaterMovingChestDeep
					 || Find.CurrentMap.terrainGrid.topGrid[i] == TerrainDefOf.WaterMovingShallow
					 || Find.CurrentMap.terrainGrid.topGrid[i] == TerrainDefOf.WaterOceanDeep
					 || Find.CurrentMap.terrainGrid.topGrid[i] == TerrainDefOf.WaterOceanShallow
					 || Find.CurrentMap.terrainGrid.topGrid[i] == TerrainDefOf.WaterShallow
					 || Find.CurrentMap.terrainGrid.topGrid[i] == Marsh
						)
					{
						Find.CurrentMap.terrainGrid.topGrid[i] = TerrainDefOf.Ice;
					}
				}
				Find.CurrentMap.mapDrawer.WholeMapChanged(MapMeshFlag.Terrain);
				//Find.CurrentMap.pathGrid.RecalculateAllPerceivedPathCosts();
				Find.CurrentMap.pathing.RecalculateAllPerceivedPathCosts();
			})));
		}
		///<summary>完成半人马身体部件极其漫长的整备时间。</summary>
		[DebugAction(category: "Explorite", name: "Complete HyperManipulator", allowedGameStates = AllowedGameStates.PlayingOnMap,
			actionType = DebugActionType.ToolMapForPawns)]
		private static void FinishHyperManipulatorInstell(Pawn p)
		{
			/*foreach (Pawn pawn in Find.CurrentMap.mapPawns.AllPawns){ }*/

			foreach (Hediff hediff in p.health.hediffSet.hediffs.Where(hediff => hediff?.def?.tags?.Contains("CentaurTechHediff_InitializationNeeded") ?? false))
			{
				hediff.Severity = hediff.def.maxSeverity;
			}
		}
		private static void MoveSomewhere(IntVec3 offset)
		{
			foreach (object obj in Find.Selector.SelectedObjects)
			{
				if (obj is Thing thing)
				{
					thing.Position += offset;
					if (thing is Pawn pawn)
					{
						pawn.Notify_Teleported();
					}
				}
			}
		}
		///<summary>向上移动选中的物体。</summary>
		[DebugAction(category: "Explorite", name: "Move z+ (^)", allowedGameStates = AllowedGameStates.PlayingOnMap,
			actionType = DebugActionType.Action)]
		private static void MoveUp()
		{
			MoveSomewhere(new IntVec3(0, 0, 1));
		}
		///<summary>向右移动选中的物体。</summary>
		[DebugAction(category: "Explorite", name: "Move x+ (>)", allowedGameStates = AllowedGameStates.PlayingOnMap,
			actionType = DebugActionType.Action)]
		private static void MoveRight()
		{
			MoveSomewhere(new IntVec3(1, 0, 0));
		}
		///<summary>向下移动选中的物体。</summary>
		[DebugAction(category: "Explorite", name: "Move z- (v)", allowedGameStates = AllowedGameStates.PlayingOnMap,
			actionType = DebugActionType.Action)]
		private static void MoveDown()
		{
			MoveSomewhere(new IntVec3(0, 0, -1));
		}
		///<summary>向左移动选中的物体。</summary>
		[DebugAction(category: "Explorite", name: "Move x- (<)", allowedGameStates = AllowedGameStates.PlayingOnMap,
			actionType = DebugActionType.Action)]
		private static void MoveLeft()
		{
			MoveSomewhere(new IntVec3(-1, 0, 0));
		}

		///<summary>。</summary>
		[DebugAction(category: "Explorite", name: "Cataclysm", allowedGameStates = AllowedGameStates.PlayingOnMap,
			actionType = DebugActionType.Action)]
		private static void RuinThis()
		{
			List<DebugMenuOption> list = new List<DebugMenuOption>();
			FalseOption(delegate ()
			{
				Map map = Find.CurrentMap;

				map.fogGrid.ClearAllFog();

				int count = map.terrainGrid.topGrid.Count();
				TerrainDef Marsh = DefDatabase<TerrainDef>.GetNamed("Marsh");
				for (int i = 0; i < count; i++)
				{
					Find.CurrentMap.terrainGrid.topGrid[i] = DefDatabase<TerrainDef>.GetRandom();
				}
				Find.CurrentMap.mapDrawer.WholeMapChanged(MapMeshFlag.Terrain);
				Find.CurrentMap.pathing.RecalculateAllPerceivedPathCosts();

				foreach (Thing thing in map.listerThings.AllThings.ToList())
				{
					try
					{
						thing.DeSpawn();
						thing.Destroy();
					}
					catch (Exception)
					{

					}
				}

				foreach (IntVec3 cell in map.AllCells.InRandomOrder())
				{
					try
					{
						Thing thing = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetRandom());
						Rot4 rot = new Rot4(Rand.RangeInclusive(0, 4));
						GenSpawn.Spawn(thing, cell, map, rot, WipeMode.VanishOrMoveAside);
					}
					catch (Exception)
					{

					}
				}

			}, 3);
		}
	}
#pragma warning restore IDE0051 // 删除未使用的私有成员
}
