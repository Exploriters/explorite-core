/********************
 * 反射镜建筑物。
 * --siiftun1857
 */

using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
	public interface IConcentratedSolarPowerGiver
	{
		public SolarPowerCell AssignedCell { get; set; }
		public SolarPowerLocation? SolarPowerOutputPos { get; }
		public float SolarPowerOutputValue { get; }
	}
	public interface ISolarPowerHeightTargetWanter
	{
		public SolarRefectionAimingTypeDef WantTargetHeight { get; set; }
		public SolarRefectionAimingTypeDef NowTargetHeight { get; }
	}
	public interface ISolarPowerMaunalHeightTargetWanter : ISolarPowerHeightTargetWanter, INeedMaunalAction
	{ }
	public abstract class Building_SolarPowerReflector : Building_Targeter, IConcentratedSolarPowerGiver, ISolarPowerHeightTargetWanter, ISolarPowerMaunalHeightTargetWanter
	{
		private SolarPowerCell assignedCell = null;
		private SolarRefectionAimingTypeDef wantTargetHeight = SolarRefectionAimingTypeDefOf.SkyHigh;
		private SolarRefectionAimingTypeDef nowTargetHeight = SolarRefectionAimingTypeDefOf.SkyHigh;
		public SolarPowerCell AssignedCell { get => assignedCell; set => assignedCell = value; }
		public SolarRefectionAimingTypeDef WantTargetHeight { get => wantTargetHeight ??= SolarRefectionAimingTypeDefOf.SkyHigh; set => wantTargetHeight = value; }
		public SolarRefectionAimingTypeDef NowTargetHeight { get => nowTargetHeight ??= SolarRefectionAimingTypeDefOf.SkyHigh; set => nowTargetHeight = value; }
		public override bool NeedManualAction => base.NeedManualAction || wantTargetHeight != nowTargetHeight;
		public override void DoManualAction()
		{
			base.DoManualAction();
			nowTargetHeight = wantTargetHeight;
			this.SolveSolarPower();
		}
		public override void CancalManualAction()
		{
			base.CancalManualAction();
			wantTargetHeight = nowTargetHeight;
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref wantTargetHeight, "wantTargetHeight");
			Scribe_Defs.Look(ref nowTargetHeight, "nowTargetHeight");
		}

		public virtual SolarPowerLocation? SolarPowerOutputPos => !Destroyed && Spawned && NowTargetCell != null && NowTargetHeight.willTrace
			? (SolarPowerLocation?)new SolarPowerLocation() { loc = NowTargetCell.Value, height = nowTargetHeight }
			: null;

		public virtual float SolarPowerOutputValue => 0;
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			if (map.ConcentratedSolarPowerGrid().AutoTargetCell is IntVec3 cell)
			{
				WantTargetHeight = SolarRefectionAimingTypeDefOf.HighGrid;
				NowTargetHeight = SolarRefectionAimingTypeDefOf.HighGrid;
				WantTargetCell = cell;
				NowTargetCell = cell;
			}
			this.SolveSolarPower();
		}
		public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
		{
			base.DeSpawn(mode);
			this.SolveSolarPower();
		}
		public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			base.Destroy(mode);
			this.SolveSolarPower();
		}
		public override string GetInspectString()
		{
			StringBuilder sb = new StringBuilder();
			string baseInspectString = base.GetInspectString();
			if (!baseInspectString.NullOrEmpty())
			{
				sb.AppendLine(baseInspectString);
			}
			if (Prefs.DevMode)
			{
				foreach (SolarRefectionAimingTypeDef def in DefDatabase<SolarRefectionAimingTypeDef>.AllDefs.Where(d => d.willTrace))
				{
					sb.AppendLine($"[DEV]Cell {def.label}({def.defName}) power: {Map.ConcentratedSolarPowerGrid().GetSolarPower(Position, def)}");
				}
			}
			return sb.ToString().TrimEndNewlines();
		}
		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}
			/*
			yield return new Command_Toggle
			{
				toggleAction = delegate
				{
					wantTargetHeight = wantTargetHeight == SolarRefectionAimingTypeDefOf.HighGrid ? SolarRefectionAimingTypeDefOf.LowGrid : SolarRefectionAimingTypeDefOf.HighGrid;
					MaunalActionUtility.UpdateMaunalActionDesignation(this);
				},
				isActive = delegate { return wantTargetHeight == SolarRefectionAimingTypeDefOf.LowGrid; },
				defaultLabel = "CommandSetReflectHeightToLow".Translate(),
				defaultDesc = "CommandSetReflectHeightToLowDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get("UI/Commands/Attack", true),
				hotKey = KeyBindingDefOf.Misc5
			};
			*/
			yield return new Command_SetSolarRefectionAimingType(this);
			yield break;
		}
	}
	[StaticConstructorOnStartup]
	public class Building_ReflectionMirror : Building_SolarPowerReflector
	{
		private static readonly Material panelFoldMat = InstelledMods.ConcentratedSolarPower ? MaterialPool.MatFrom("Things/Building/SolarPower/PanelFold") : BaseContent.BadMat;
		private static readonly Material panelPlateMat = InstelledMods.ConcentratedSolarPower ? MaterialPool.MatFrom("Things/Building/SolarPower/PanelPlate") : BaseContent.BadMat;
		private static readonly Material panelBlackMat = SolidColorMaterials.SimpleSolidColorMaterial(Color.black, false);


		//public float? solarPowerOutputValue = null;
		public override float SolarPowerOutputValue => SolarPowerOutputValueInternal;//solarPowerOutputValue ??= SolarPowerOutputValueInternal;
		private float SolarPowerOutputValueInternal => !Destroyed && Spawned ? Mathf.Lerp(0f, 100f, Map.skyManager.CurSkyGlow) * UnRoofedCellCount : 0f;
		private int UnRoofedCellCount => this.OccupiedRect().Count(c => !Map.roofGrid.Roofed(c));

		public override void DoManualAction()
		{
			base.DoManualAction();
			distanceToTarget = null;
			panelAngle = null;
			spinedSize = null;
		}

		public override string GetInspectString()
		{
			StringBuilder sb = new StringBuilder();
			string baseInspectString = base.GetInspectString();
			if (!baseInspectString.NullOrEmpty())
			{
				sb.AppendLine(baseInspectString);
			}
			sb.AppendLine("ExSolarPowerOutput".Translate() + ": " + SolarPowerOutputValue.ToString("#####0") + " W");
			return sb.ToString().TrimEndNewlines();
		}

		private float? distanceToTarget = null;
		public float DistanceToTarget => distanceToTarget ??= NowTargetCell.HasValue ? Vector3.Distance(NowTargetCell.Value.ToVector3Shifted(), this.TrueCenter()) : 0f;

		private Quaternion? panelAngle = null;
		public Quaternion PanelAngle => panelAngle ??= (DistanceToTarget >= float.Epsilon ? (NowTargetCell.Value.ToVector3Shifted() - this.TrueCenter()).AngleFlat() : Rot4.North.AsAngle).ToQuat();

		private float? spinedSize = null;
		public float SpinedSize => spinedSize ??= NowTargetHeight.SpinedSize(DistanceToTarget);

		public override void Draw()
		{
			base.Draw();

			Vector3 center = this.TrueCenter();
			Matrix4x4 matrix = default;

			center.y += 0.04054054f;
			matrix.SetTRS(center, PanelAngle, new Vector3(1f, 1f, 1f));
			Graphics.DrawMesh(MeshPool.plane10, matrix, panelFoldMat, 0);

			center.y += 0.04054054f;
			matrix.SetTRS(center, PanelAngle, new Vector3(0.84375f, 1f, SpinedSize * 0.78125f + 0.0625f));
			Graphics.DrawMesh(MeshPool.plane10, matrix, panelBlackMat, 0);

			center.y += 0.04054054f;
			matrix.SetTRS(center, PanelAngle, new Vector3(1f, 1f, SpinedSize));
			Graphics.DrawMesh(MeshPool.plane10, matrix, panelPlateMat, 0);

		}
	}
	public class Command_SetSolarRefectionAimingType : Command
	{
		private readonly ISolarPowerMaunalHeightTargetWanter heightSource;
		private List<ISolarPowerMaunalHeightTargetWanter> groupedheightSource;
		private IEnumerable<ISolarPowerMaunalHeightTargetWanter> HeightSources => new[] { heightSource }.Concat(groupedheightSource ?? Enumerable.Empty<ISolarPowerMaunalHeightTargetWanter>());

		public SolarRefectionAimingTypeDef wantTargetHeight = null;
		public SolarRefectionAimingTypeDef WantTargetHeight => /*wantTargetHeight ??=*/ ((Func<SolarRefectionAimingTypeDef>)(() =>
			{
				SolarRefectionAimingTypeDef buffer = null;
				foreach (ISolarPowerMaunalHeightTargetWanter heightSource in HeightSources)
				{
					if (buffer == null)
					{
						buffer = heightSource.WantTargetHeight;
					}
					else if (buffer != heightSource.WantTargetHeight)
					{
						return null;
					}
				}
				return buffer;
			}))();

		public SolarRefectionAimingTypeDef nowTargetHeight = null;
		public SolarRefectionAimingTypeDef NowTargetHeight => /*nowTargetHeight ??=*/ ((Func<SolarRefectionAimingTypeDef>)(() =>
			{
				SolarRefectionAimingTypeDef buffer = null;
				foreach (ISolarPowerMaunalHeightTargetWanter heightSource in HeightSources)
				{
					if (buffer == null)
					{
						buffer = heightSource.NowTargetHeight;
					}
					else if (buffer != heightSource.NowTargetHeight)
					{
						return null;
					}
				}
				return buffer;
			}))();

		public Command_SetSolarRefectionAimingType(ISolarPowerMaunalHeightTargetWanter heightSource)
		{
			this.heightSource = heightSource;
			defaultLabel = $"{"CommandTargetSolarPowerHeightSelectorLabel".Translate()}{ ": " + WantTargetHeight.LabelCap }";
			defaultDesc = "CommandTargetSolarPowerHeightSelectorDesc".Translate();
			icon = WantTargetHeight?.GizmoIcon ?? SolarRefectionAimingTypeDefOf.SkyHigh.GizmoIcon;
		}

		public override void MergeWith(Gizmo other)
		{
			base.MergeWith(other);
			if (other is Command_SetSolarRefectionAimingType command_VerbTarget)
			{
				if (groupedheightSource == null)
				{
					groupedheightSource = new List<ISolarPowerMaunalHeightTargetWanter>();
				}
				groupedheightSource.Add(command_VerbTarget.heightSource);
				if (command_VerbTarget.groupedheightSource != null)
				{
					groupedheightSource.AddRange(command_VerbTarget.groupedheightSource);
				}
				wantTargetHeight = null;
				nowTargetHeight = null;
			}
			else
			{
				Log.ErrorOnce("Tried to merge Command_SetSolarRefectionAimingType with unexpected type", 73401284);
				return;
			}
		}

		public override void ProcessInput(Event ev)
		{
			base.ProcessInput(ev);
			List<FloatMenuOption> list = new List<FloatMenuOption>();

			foreach (SolarRefectionAimingTypeDef def in DefDatabase<SolarRefectionAimingTypeDef>.AllDefs)
			{
				list.Add(new FloatMenuOption(
					label: def.LabelCap +
						(def == NowTargetHeight ? $"({"CommandTargetSolarPowerHeightSelectorDisabledToNow".Translate()})" :
						(def == WantTargetHeight ? $"({"CommandTargetSolarPowerHeightSelectorDisabledToWant".Translate()})" :
						string.Empty
						)),
					action: delegate ()
					{
						foreach (ISolarPowerMaunalHeightTargetWanter wanter in HeightSources)
						{
							wanter.WantTargetHeight = def;
							MaunalActionUtility.UpdateMaunalActionDesignationI(wanter);
						}
					},
					itemIcon: def.GizmoIcon,
					iconColor: Color.white,
					priority: MenuOptionPriority.Default,
					mouseoverGuiAction: null,
					revalidateClickTarget: null,
					extraPartWidth: 0f,
					extraPartOnGUI: null,
					revalidateWorldClickTarget: null,
					playSelectionSound: true,
					orderInPriority: 0)
				{
					Disabled = def == WantTargetHeight
				});
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}
	}
}

