/********************
 * 为半人马覆盖PathGrid。
 * --siiftun1857
 */
using System;
using System.Text;
using System.Collections.Generic;
using RimWorld;
using HarmonyLib;
using UnityEngine;
using Verse;
using static Explorite.ExploriteCore;
using static Explorite.PathingOverriderUtility;
using System.Reflection.Emit;
using System.Reflection;
using Verse.AI;
using System.Linq;

namespace Explorite
{
	///<summary>寻路覆盖器的功能集。</summary>
	[StaticConstructorOnStartup]
	internal static class PathingOverriderUtility
	{
		public static readonly List<ISpecialPathingConfig> PathingConfigs = new List<ISpecialPathingConfig>();
		static PathingOverriderUtility()
		{
			PathingConfigs.Add(new CentaurNormalPathingConfig());
		}
		public static bool ContainsPathCostIgnoreRepeater(IntVec3 c, Map map)
		{
			List<Thing> list = map.thingGrid.ThingsListAt(c);
			for (int i = 0; i < list.Count; i++)
			{
				if (IsPathCostIgnoreRepeater(list[i].def))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsPathCostIgnoreRepeater(ThingDef def)
		{
			return def.pathCost >= 25 && def.pathCostIgnoreRepeat;
		}

		static readonly FieldInfo pathGridMapField = typeof(PathGrid).GetField("map", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		public static PathGridOverriderMapComponent PathingMapComp(Pathing pathing)
		{
			return ((Map)pathGridMapField.GetValue(pathing.Normal.pathGrid)).GetComponent<PathGridOverriderMapComponent>();
		}
	}
	///<summary>寻路配置。</summary>
	public interface ISpecialPathingConfig
	{
		///<summary>是否应当被初始化。</summary>
		public bool ShouldExecute(Map map);
		///<summary>是否应当为该人物执行。</summary>
		public bool ShouldFor(Pawn pawn);
		///<summary>是否应当为该目标执行。</summary>
		public bool ShouldFor(TraverseParms parms);
		///<summary>寻路方案。</summary>
		public int CalculatedCostAt(IntVec3 c, bool perceivedStatic, IntVec3 prevCell, Map map, bool fenceArePassable);
		///<summary>初始化时指定是否被栅栏阻挡。</summary>
		public bool InitFenceBlocked { get; }
	}
	///<summary>半人马的寻路配置。</summary>
	public class CentaurNormalPathingConfig : ISpecialPathingConfig
	{
		public bool InitFenceBlocked => false;
		public bool ShouldExecute(Map map)
		{
			return InstelledMods.RimCentaurs;
		}
		public bool ShouldFor(Pawn pawn)
		{
			return pawn?.def == AlienCentaurDef;
		}
		public bool ShouldFor(TraverseParms parms)
		{
			return parms.pawn?.def == AlienCentaurDef;
		}

		public int CalculatedCostAt(IntVec3 c, bool perceivedStatic, IntVec3 prevCell, Map map, bool fenceArePassable)
		{
			bool flag = false;
			TerrainDef terrainDef = map.terrainGrid.TerrainAt(c);
			if (terrainDef == null || terrainDef.passability == Traversability.Impassable)
			{
				return 10000;
			}
			int num = terrainDef == TerrainDefOf.Ice ? 0 : terrainDef.pathCost;
			List<Thing> list = map.thingGrid.ThingsListAt(c);
			for (int i = 0; i < list.Count; i++)
			{
				Thing thing = list[i];
				if (thing.def.passability == Traversability.Impassable)
				{
					return 10000;
				}
				if (!fenceArePassable && thing.def.building != null && thing.def.building.isFence)
				{
					return 10000;
				}
				if (!IsPathCostIgnoreRepeater(thing.def) || !prevCell.IsValid || !ContainsPathCostIgnoreRepeater(prevCell, map))
				{
					int pathCost = thing.def.pathCost;
					if (pathCost > num)
					{
						num = pathCost;
					}
				}
				if (thing is Building_Door && prevCell.IsValid)
				{
					Building edifice = prevCell.GetEdifice(map);
					if (edifice != null && edifice is Building_Door)
					{
						flag = true;
					}
				}
			}
			/*
			int num2 = SnowUtility.MovementTicksAddOn(map.snowGrid.GetCategory(c));
			if (num2 > num)
			{
				num = num2;
			}
			*/
			if (flag)
			{
				num += 45;
			}
			if (perceivedStatic)
			{
				for (int j = 0; j < 9; j++)
				{
					IntVec3 intVec = GenAdj.AdjacentCellsAndInside[j];
					IntVec3 c2 = c + intVec;
					if (c2.InBounds(map))
					{
						Fire fire = null;
						list = map.thingGrid.ThingsListAtFast(c2);
						for (int k = 0; k < list.Count; k++)
						{
							fire = (list[k] as Fire);
							if (fire != null)
							{
								break;
							}
						}
						if (fire != null && fire.parent == null)
						{
							if (intVec.x == 0 && intVec.z == 0)
							{
								num += 1000;
							}
							else
							{
								num += 150;
							}
						}
					}
				}
			}
			return num;
		}
	}
	public class SpecialPathingContext : PathingContext
	{
		public SpecialPathingContext(Map map, PathGrid pathGrid, ISpecialPathingConfig config) : base(map, pathGrid)
		{
			this.config = config;
		}
		public ISpecialPathingConfig config;
	}
	///<summary>存储寻路结果。</summary>
	public sealed class PathGridOverriderMapComponent : MapComponent
	{
		public readonly List<SpecialPathingContext> PathingContextList = new List<SpecialPathingContext>();
		public ISpecialPathingConfig PathGridConfig(PathGrid pathGrid)
		{
			foreach (SpecialPathingContext pathingContext in PathingContextList)
			{
				if (ReferenceEquals(pathingContext.pathGrid, pathGrid))
					return pathingContext.config;
			}
			return null;
		}
		public PathGridOverriderMapComponent(Map map) : base(map)
		{
			foreach (ISpecialPathingConfig config in PathingConfigs.Where(i => i.ShouldExecute(map)))
			{
				PathingContextList.Add(new SpecialPathingContext(map, new PathGrid(map, config.InitFenceBlocked), config));
			}
		}
	}
	internal static partial class ExploritePatches
	{
		/*
		[HarmonyPostfix] public static void PathingCtorPostfix(Map map, Pathing __instance)
		{
			PathGridOverriderMapComponent comp = map.GetComponent<PathGridOverriderMapComponent>();
		}
		*/

		///<summary>覆盖基础寻路计算。</summary>
		[HarmonyPrefix] public static bool PathGridCalculatedCostAtPrefix(IntVec3 c, bool perceivedStatic, IntVec3 prevCell, Map ___map, bool ___fenceArePassable, PathGrid __instance, ref int __result)
		{
			if (___map.GetComponent<PathGridOverriderMapComponent>() is PathGridOverriderMapComponent comp && comp.PathGridConfig(__instance) is ISpecialPathingConfig config)
			{
				__result = config.CalculatedCostAt(c, perceivedStatic, prevCell, ___map, ___fenceArePassable);
				return false;
			}
			return true;
		}


		///<summary>覆盖基础寻路计算。</summary>
		[HarmonyPrefix] public static bool PathingForTraverseParmsPrefix(ref PathingContext __result, Pathing __instance, TraverseParms parms)
		{
			if (PathingMapComp(__instance) is PathGridOverriderMapComponent comp)
			{
				if(comp.PathingContextList.FirstOrFallback(c => c.config.ShouldFor(parms)) is SpecialPathingContext pathingContext)
				{
					__result = pathingContext;
					return false;
				}
			}
			return true;
		}
		///<summary>覆盖基础寻路计算。</summary>
		[HarmonyPrefix] public static bool PathingForPawnPrefix(ref PathingContext __result, Pathing __instance, Pawn pawn)
		{
			if (PathingMapComp(__instance) is PathGridOverriderMapComponent comp)
			{
				if(comp.PathingContextList.FirstOrFallback(c => c.config.ShouldFor(pawn)) is SpecialPathingContext pathingContext)
				{
					__result = pathingContext;
					return false;
				}
			}
			return true;
		}
		///<summary>覆盖基础寻路计算。</summary>
		[HarmonyPostfix] public static void PathingRecalculateAllPerceivedPathCostsPosfix(Pathing __instance)
		{
			if (PathingMapComp(__instance) is PathGridOverriderMapComponent comp)
			{
				foreach(SpecialPathingContext pathingContext in comp.PathingContextList)
				{
					pathingContext.pathGrid.RecalculateAllPerceivedPathCosts();
				}
			}
		}
		///<summary>覆盖基础寻路计算。</summary>
		[HarmonyPostfix] public static void PathingRecalculatePerceivedPathCostAtPosfix(Pathing __instance, IntVec3 c)
		{
			if (PathingMapComp(__instance) is PathGridOverriderMapComponent comp)
			{
				foreach(SpecialPathingContext pathingContext in comp.PathingContextList)
				{
					bool flag = false;
					pathingContext.pathGrid.RecalculatePerceivedPathCostAt(c, ref flag);
				}
			}
		}
	}
}