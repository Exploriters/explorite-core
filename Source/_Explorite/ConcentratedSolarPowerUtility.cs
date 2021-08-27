/********************
 * 反射太阳能网机制。
 * --siiftun1857
 */

using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace Explorite
{
	///<summary>反射高度。</summary>
	public class SolarRefectionAimingTypeDef : Def
	{
		public float height = 0f;
		public bool willTrace = false;
		[NoTranslate]
		public string gizmoTexturePath;

		private Texture2D gizmoIconInt;
		public Texture2D GizmoIcon => gizmoIconInt ??= ContentFinder<Texture2D>.Get(gizmoTexturePath);
		public float SpinedSize(float distance)
		{
			if (float.IsNaN(height))
			{
				return 1f;
			}
			float asinv = Mathf.Asin(distance / Mathf.Sqrt(Mathf.Pow(height, 2) + Mathf.Pow(distance, 2))) / 2f;
			return height >= 0 ? Mathf.Cos(asinv) : Mathf.Sin(asinv);
		}
	}
	///<summary>反射高度预设。</summary>
	[DefOf]
	public static class SolarRefectionAimingTypeDefOf
	{
		public static SolarRefectionAimingTypeDef SkyHigh;
		public static SolarRefectionAimingTypeDef HighGrid;
		public static SolarRefectionAimingTypeDef LowGrid;
		public static SolarRefectionAimingTypeDef Perpendicular;
	}
	public struct SolarPowerLocation
	{
		public IntVec3 loc;
		public SolarRefectionAimingTypeDef height;

		public SolarPowerLocation(IntVec3 loc, SolarRefectionAimingTypeDef height)
		{
			this.loc = loc;
			this.height = height;
		}

		public override bool Equals(object obj)
		{
			return obj is SolarPowerLocation other && loc == other.loc && height == other.height;
		}
		public static bool operator ==(SolarPowerLocation v1, SolarPowerLocation v2)
		{
			return v1.Equals(v2);
		}
		public static bool operator !=(SolarPowerLocation v1, SolarPowerLocation v2)
		{
			return !(v1 == v2);
		}
		public override int GetHashCode()
		{
			return Gen.HashCombineStruct(Gen.HashCombine(0, height), loc);
		}
	}
	public static class ConcentratedSolarPowerUtility
	{
		public static bool VaildToTrace(this IConcentratedSolarPowerGiver thing)
		{
			return thing.SolarPowerOutputPos is SolarPowerLocation location && location.height.willTrace;
		}
		public static IEnumerable<IConcentratedSolarPowerGiver> GetSolarPower(this Thing thing)
		{
			if (thing is IConcentratedSolarPowerGiver thingI && thingI.VaildToTrace())
			{
				yield return thingI;
			}
			if (thing is ThingWithComps thingWithComps)
			{
				foreach (ThingComp comp in thingWithComps.AllComps)
				{
					if (comp is IConcentratedSolarPowerGiver compI && compI.VaildToTrace())
					{
						yield return compI;
					}
				}
			}
		}
		public static void SolveSolarPower(this Thing thing)
		{
			if (thing.Map is Map map)
			{
				if (map.GetComponent<ConcentratedSolarPowerGrid>() is ConcentratedSolarPowerGrid comp)
				{
					comp.Resolve(thing);
				}
			}
		}
		public static bool CanEverHaveSolarPower(this Thing thing)
		{
			return thing.GetSolarPower().Any();
		}
		public static ConcentratedSolarPowerGrid ConcentratedSolarPowerGrid(this Map map)
		{
			return map.GetComponent<ConcentratedSolarPowerGrid>();
		}
		public static bool AssignToSolarPowerCell(this IConcentratedSolarPowerGiver giver, SolarPowerCell cell)
		{
			giver.AssignedCell.Unassign(giver);
			return cell?.Assign(giver) ?? true;
		}
	}
	public sealed class SolarPowerCell
	{
		public ConcentratedSolarPowerGrid Parent { get; }
		public SolarPowerLocation Location { get; }
		public List<IConcentratedSolarPowerGiver> List { get; } = new List<IConcentratedSolarPowerGiver>();
		public bool Unassign(IConcentratedSolarPowerGiver giver)
		{
			return List.Remove(giver);
		}
		public bool Assign(IConcentratedSolarPowerGiver giver)
		{
			giver.AssignedCell?.Unassign(giver);
			if (giver.SolarPowerOutputPos == Location)
			{
				List.Add(giver);
				return true;
			}
			else
			{
				return false;
			}
		}
		public SolarPowerCell(ConcentratedSolarPowerGrid parent, SolarPowerLocation location)
		{
			Parent = parent;
			Location = location;
		}
		public float OutputPower
		{
			get
			{
				float result = 0f;
				foreach (IConcentratedSolarPowerGiver solarPowerGiver in List.ToArray())
				{
					if (solarPowerGiver.SolarPowerOutputPos == Location)
					{
						result += Math.Max(0, solarPowerGiver.SolarPowerOutputValue);
					}
					else
					{
						List.Remove(solarPowerGiver);
					}
				}
				return result;
			}
		}

	}
	public sealed class ConcentratedSolarPowerGrid : MapComponent
	{
		private Thing autoTarget = null;

		private List<SolarPowerCell> listCells = new List<SolarPowerCell>();
		public Dictionary<SolarPowerLocation, SolarPowerCell> Database => ListCells.ToDictionary(c => c.Location);
		public Thing AutoTarget { get => autoTarget; set => autoTarget = value; }
		public List<SolarPowerCell> ListCells { get => listCells; }

		public IntVec3? AutoTargetCell => !AutoTarget.DestroyedOrNull() && AutoTarget.FinalSpawnedParent() is Thing thing ? (IntVec3?)thing.Position : null;
		public void ResolveAllThings()
		{
			ListCells.Clear();
			foreach (Thing thing in map.listerThings.AllThings)
			{
				Resolve(thing);
			}
		}
		public void Resolve(Thing thing)
		{
			foreach (IConcentratedSolarPowerGiver solarPowerGiver in thing.GetSolarPower())
			{
				Resolve(solarPowerGiver);
			}
		}
		public void Resolve(IConcentratedSolarPowerGiver solarPowerGiver)
		{
			if (solarPowerGiver.VaildToTrace())
			{
				if (!Database.TryGetValue(solarPowerGiver.SolarPowerOutputPos.Value, out SolarPowerCell cell))
				{
					cell = new SolarPowerCell(this, solarPowerGiver.SolarPowerOutputPos.Value);
					ListCells.Add(cell);
				}
				cell.Assign(solarPowerGiver);
			}
			else
			{
				solarPowerGiver.AssignedCell = null;
			}
		}
		public float GetSolarPower(IntVec3 cell, SolarRefectionAimingTypeDef height)
		{
			return GetSolarPower(new SolarPowerLocation(cell, height));
		}
		public float GetSolarPower(SolarPowerLocation loc)
		{
			return Database.TryGetValue(loc, out SolarPowerCell cell) ? cell.OutputPower : 0f;
		}
		public ConcentratedSolarPowerGrid(Map map) : base(map)
		{
			ResolveAllThings();
		}
	}
}

