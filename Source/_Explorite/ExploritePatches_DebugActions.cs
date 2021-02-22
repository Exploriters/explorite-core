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
    static partial class ExploriteDebugActions
    {

        ///<summary>清除所有迷雾。</summary>
        [DebugAction(category: "Explorite", name: "Clear fog at...", allowedGameStates = AllowedGameStates.PlayingOnMap,
            actionType = DebugActionType.ToolMap)]
        private static void ClearAllFogClickPlace()
        {
            if (UI.MouseCell().InBounds(Find.CurrentMap))
            {
                Find.CurrentMap.fogGrid.Notify_FogBlockerRemoved(UI.MouseCell());
            }
        }
        ///<summary>清除所有迷雾。</summary>
        [DebugAction(category: "Explorite", name: "Clear all fog", allowedGameStates = AllowedGameStates.PlayingOnMap,
            actionType = DebugActionType.Action)]
        private static void ClearAllFog()
        {
            Find.CurrentMap.fogGrid.ClearAllFog();
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

            int confirmNum = (int)Math.Floor(Rand.Value * 10);
            DebugMenuOption blank = new DebugMenuOption("Cancel", DebugMenuOptionMode.Action, delegate () { });

            for (int i = 0; i < confirmNum; i++)
            {
                list.Add(blank);
            }
            list.Add(new DebugMenuOption("--- Confirm ---", DebugMenuOptionMode.Action, delegate ()
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
                Find.CurrentMap.pathGrid.RecalculateAllPerceivedPathCosts();
            }));
            for (int i = 0; i < 9 - confirmNum; i++)
            {
                list.Add(blank);
            }
            Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
        }
        ///<summary>完成物理操作仪极其漫长的整备时间。</summary>
        [DebugAction(category: "Explorite", name: "Complete HyperManipulator", allowedGameStates = AllowedGameStates.PlayingOnMap,
            actionType = DebugActionType.ToolMapForPawns)]
        private static void FinishHyperManipulatorInstell(Pawn p)
        {
            /*foreach (Pawn pawn in Find.CurrentMap.mapPawns.AllPawns){ }*/

            foreach (Hediff hediff in p.health.hediffSet.hediffs.Where(hediff => hediff.def == HyperManipulatorHediffDef))
            {
                hediff.Severity = hediff.def.maxSeverity;
            }
        }
    }
#pragma warning restore IDE0051 // 删除未使用的私有成员
}
