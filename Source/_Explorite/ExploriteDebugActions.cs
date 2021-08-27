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
		///<summary>点燃整个地图。</summary>
		[DebugAction(category: "Explorite", name: "Burn Whole map", allowedGameStates = AllowedGameStates.PlayingOnMap,
			actionType = DebugActionType.Action)]
		private static void BurnWholeMap()
		{

			List<DebugMenuOption> list = new List<DebugMenuOption>();
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(KeepOutOption(delegate ()
			{
				Map map = Find.CurrentMap;
				foreach (Thing thing in map.spawnedThings)
				{
					if (thing.GetAttachment(ThingDefOf.Fire) is Fire fire)
					{
						fire.fireSize = 1.75f;
					}
					else
					{
						thing.TryAttachFire(1.75f);
					}
				}
				foreach (IntVec3 cell in map.AllCells)
				{
					if (!(map.thingGrid.ThingsAt(cell).FirstOrFallback(x => x is Fire fire) is Fire fire))
					{
						if (map.thingGrid.ThingsAt(cell).Any(thing => !thing.DestroyedOrNull() && thing.FlammableNow)
						 || FireUtility.TerrainFlammableNow(cell, map)
						)
						{
							fire = (Fire)ThingMaker.MakeThing(ThingDefOf.Fire);
							GenSpawn.Spawn(fire, cell, map, WipeMode.Vanish);
						}
						else
						{
							fire = null;
						}
					}
					if (fire != null)
					{
						fire.fireSize = 1.75f;
					}
				}
				Find.CurrentMap.mapDrawer.WholeMapChanged(MapMeshFlag.Things);
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

		///<summary>更改物品的品质。</summary>
		[DebugAction(category: "Explorite", name: "Set quality of selected things", allowedGameStates = AllowedGameStates.PlayingOnMap,
			actionType = DebugActionType.Action)]
		private static void SetSelectedThingsQuality()
		{
			static void SetQuality(QualityCategory q)
			{
				foreach (object obj in Find.Selector.SelectedObjects)
				{
					if (obj is ThingWithComps thing && thing.TryGetComp<CompQuality>() is CompQuality comp)
					{
						comp.SetQuality(q, ArtGenerationContext.Colony);
					}
					if (obj is Pawn pawn && pawn.apparel is Pawn_ApparelTracker apparelTracker)
					{
						foreach (Thing apparel in apparelTracker.GetDirectlyHeldThings())
						{
							if (apparel.TryGetComp<CompQuality>() is CompQuality compA)
							{
								compA.SetQuality(q, ArtGenerationContext.Colony);
							}
						}
					}
				}
			}

			List<DebugMenuOption> list = new List<DebugMenuOption>();
			foreach (QualityCategory qualityValue in Enum.GetValues(typeof(QualityCategory)))
			{
				string QualityName = Enum.GetName(typeof(QualityCategory), qualityValue);
				list.Add(new DebugMenuOption(QualityName, DebugMenuOptionMode.Action, delegate ()
				{
					SetQuality(qualityValue);
				}));
			}
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
		}

		///<summary>更改物品的材料。</summary>
		[DebugAction(category: "Explorite", name: "Set stuff of selected things", allowedGameStates = AllowedGameStates.PlayingOnMap,
			actionType = DebugActionType.Action)]
		private static void SetSelectedThingsStuff()
		{
			static void SetStuff(ThingDef stuffDef)
			{
				foreach (object obj in Find.Selector.SelectedObjects)
				{
					if (obj is Thing thing && thing.def.MadeFromStuff)
					{
						thing.SetStuffDirect(stuffDef);
					}
					if (obj is Pawn pawn && pawn.apparel is Pawn_ApparelTracker apparelTracker)
					{
						foreach (Thing apparel in apparelTracker.GetDirectlyHeldThings())
						{
							if (apparel.def.MadeFromStuff)
							{
								apparel.SetStuffDirect(stuffDef);
							}
						}
					}
				}
			}

			List<DebugMenuOption> list = new List<DebugMenuOption>();
			foreach (ThingDef stuffDef in DefDatabase<ThingDef>.AllDefs.Where(d => d.IsStuff))
			{
				list.Add(new DebugMenuOption(stuffDef.LabelAsStuff, DebugMenuOptionMode.Action, delegate ()
				{
					SetStuff(stuffDef);
				}));
			}
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
		}

		///<summary>更改物品的颜色。</summary>
		[DebugAction(category: "Explorite", name: "Set color of selected things", allowedGameStates = AllowedGameStates.PlayingOnMap,
			actionType = DebugActionType.Action)]
		private static void SetSelectedThingsColor()
		{
			static void SetColor(Color c)
			{
				foreach (object obj in Find.Selector.SelectedObjects)
				{
					if (obj is ThingWithComps thing && thing.TryGetComp<CompColorable>() is CompColorable comp)
					{
						comp.SetColor(c);
					}
					if (obj is Pawn pawn && pawn.apparel is Pawn_ApparelTracker apparelTracker)
					{
						foreach (Thing apparel in apparelTracker.GetDirectlyHeldThings())
						{
							if (apparel.TryGetComp<CompColorable>() is CompColorable compA)
							{
								compA.SetColor(c);
							}
						}
					}
				}
			}

			List<DebugMenuOption> list = new List<DebugMenuOption>();
			foreach (ColorDef colorDef in DefDatabase<ColorDef>.AllDefs)
			{
				list.Add(new DebugMenuOption(colorDef.defName, DebugMenuOptionMode.Action, delegate ()
				{
					SetColor(colorDef.color);
				}));
			}
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
		}

		///<summary>修复物品。</summary>
		[DebugAction(category: "Explorite", name: "Fix things", allowedGameStates = AllowedGameStates.PlayingOnMap,
			actionType = DebugActionType.Action)]
		private static void FixSelectedThings()
		{
			foreach (Thing thing in Find.Selector.SelectedObjects.NullOrEmpty() ? Find.Selector.SelectedObjects.Where(obj => obj is Thing thing).Select(obj => obj as Thing) : Find.CurrentMap.listerThings.AllThings)
			{
				if (thing.def.useHitPoints)
				{
					thing.HitPoints = thing.MaxHitPoints;
				}
				if (thing is Pawn pawn && pawn.apparel is Pawn_ApparelTracker apparelTracker)
				{
					foreach (Thing apparel in apparelTracker.GetDirectlyHeldThings())
					{
						if (apparel.def.useHitPoints)
						{
							apparel.HitPoints = apparel.MaxHitPoints;
						}
					}
				}
			}
		}
		///<summary>增加流动文化的发展点数。</summary>
		[DebugAction(category: "Explorite", name: "Points to fluid ideo", allowedGameStates = AllowedGameStates.Playing,
			actionType = DebugActionType.Action)]
		private static void IncreaseIdeoFluidPoint()
		{
			List<DebugMenuOption> list = new List<DebugMenuOption>();
			foreach (Ideo ideo in Find.IdeoManager.IdeosListForReading.Where(ideo => ideo.Fluid))
			{
				list.Add(new DebugMenuOption(ideo.name, DebugMenuOptionMode.Action, delegate ()
				{
					List<DebugMenuOption> list2 = new List<DebugMenuOption>();
					for (int i = -20; i <= 20; i++)
					{
						if (i != 0)
						{
							list2.Add(new DebugMenuOption($"{(i >= 0 ? "+" : string.Empty)}{i}", DebugMenuOptionMode.Action, delegate ()
							{
								ideo.development.points += i;
							}));
						}
					}
					Find.WindowStack.Add(new Dialog_DebugOptionListLister(list2));
				}));
			}
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
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

		///<summary>提升人物荣耀到65。</summary>
		[DebugAction("Explorite", "Award until 65 honor", false, false, allowedGameStates = AllowedGameStates.PlayingOnMap, requiresRoyalty = true)]
		private static void Award10RoyalFavor()
		{
			if (!(bool)typeof(DebugActionsRoyalty).GetMethod("CheckAnyFactionWithRoyalTitles", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null))
			{
				return;
			}
			List<DebugMenuOption> list = (
				from Faction localFaction2 in (IEnumerable<Faction>)typeof(DebugActionsRoyalty).GetMethod("get_FactionsWithRoyalTitles", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null)
				let localFaction = localFaction2
				select new DebugMenuOption(localFaction.Name, DebugMenuOptionMode.Tool, delegate ()
				{
					Pawn firstPawn = UI.MouseCell().GetFirstPawn(Find.CurrentMap);
					if (firstPawn != null)
					{
						int delta = 65 - firstPawn.royalty.GetFavor(localFaction);
						RoyalTitleDef tmpTitle = firstPawn.royalty.GetCurrentTitleInFaction(localFaction)?.def;
						while (tmpTitle != null)
						{
							delta -= tmpTitle.favorCost;
							tmpTitle = tmpTitle.GetPreviousTitle_IncludeNonRewardable(localFaction);
						}

						if (delta > 0)
						{
							firstPawn.royalty.GainFavor(localFaction, delta);
						}
					}
				})
				).ToList();
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
		}
	}
#pragma warning restore IDE0051 // 删除未使用的私有成员
}
