/********************
 * 选择器建筑物。
 * --siiftun1857
 */
using Verse;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Verse.Sound;
using System.Linq;
using Verse.AI;
using static Explorite.ExploriteCore;

namespace Explorite
{
	///<summary>需要一个目标位置的接口。</summary>
	public interface ITargetWanter
	{
		public IntVec3? WantTargetCell { set; get; }
	}
	///<summary>包含一个可选目标位置参数的建筑物。</summary>
	public class Building_Targeter : Building, INeedMaunalAction, ITargetWanter
	{
		private IntVec3? nowTargetCell = null;
		private IntVec3? wantTargetCell = null;
		//private static readonly Material PortCellMaterial = MaterialPool.MatFrom("UI/Overlays/InteractionCell", ShaderDatabase.Transparent);

		public IntVec3? WantTargetCell { get => wantTargetCell; set => wantTargetCell = value; }
		public IntVec3? NowTargetCell { get => nowTargetCell; set => nowTargetCell = value; }
		public virtual bool NeedRetargeting => wantTargetCell != nowTargetCell;
		public virtual bool NeedManualAction => NeedRetargeting;

		public virtual void DoManualAction()
		{
			nowTargetCell = wantTargetCell;
		}
		public virtual void CancalManualAction()
		{
			wantTargetCell = nowTargetCell;
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			//nowTargetCell = map.AllCells.RandomElement();
			//wantTargetCell = map.AllCells.RandomElement();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref wantTargetCell, "wantTargetCell", null, true);
			Scribe_Values.Look(ref nowTargetCell, "nowTargetCell", null, true);
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
				sb.AppendLine($"[DEV]Want target cell: {wantTargetCell?.ToString() ?? "None"}");
				sb.AppendLine($"[DEV]Now target cell: {nowTargetCell?.ToString() ?? "None"}");
			}
			return sb.ToString().TrimEndNewlines();
		}

		public override void DrawExtraSelectionOverlays()
		{
			base.DrawExtraSelectionOverlays();
			if (wantTargetCell != null && NeedRetargeting)
			{
				GenDraw.DrawLineBetween(this.TrueCenter(), wantTargetCell.Value.ToVector3Shifted(), SimpleColor.Cyan);
				//Graphics.DrawMesh(MeshPool.plane10, wantTargetCall.Value.ToVector3Shifted(), Quaternion.identity, PortCellMaterial, 0);
			}
			if (nowTargetCell != null)
			{
				GenDraw.DrawLineBetween(this.TrueCenter(), nowTargetCell.Value.ToVector3Shifted(), SimpleColor.Green);
				//Graphics.DrawMesh(MeshPool.plane10, nowTargetCall.Value.ToVector3Shifted(), Quaternion.identity, PortCellMaterial, 0);
			}
		}
		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}
			if (Prefs.DevMode)
			{
				yield return new Command_Action
				{
					defaultLabel = "Debug: Take manual action",
					action = delegate ()
					{
						this.DoMaunalActions();
						MaunalActionUtility.UpdateMaunalActionDesignation(this);
					},
					disabled = !NeedManualAction
				};
			}
			if (Faction == Faction.OfPlayer)
			{
				//yield return InstallationDesignatorDatabase.DesignatorFor(this.def);
				Designator_TargetWanterTarget designator_TargetWanterTarget = new Designator_TargetWanterTarget
				{
					targetWanter = this,
					defaultLabel = "CommandSetLocationTarget".Translate(),
					defaultDesc = "CommandSetLocationTargetDesc".Translate(),
					icon = ContentFinder<Texture2D>.Get("UI/Commands/Attack", true),
					hotKey = KeyBindingDefOf.Misc4
				};
				//designator_TargetWanterTarget.Disable("CannotFire".Translate() + ": " + "Roofed".Translate().CapitalizeFirst());
				yield return designator_TargetWanterTarget;
			}
		}
	}

	///<summary>生成目标选择器建筑物的gizmo。</summary>
	public class Designator_TargetWanterTarget : Designator
	{
		public ITargetWanter targetWanter;
		private List<ITargetWanter> groupedTargetWanter;
		private IEnumerable<ITargetWanter> TargetWanters => new[] { targetWanter }.Concat(groupedTargetWanter ?? Enumerable.Empty<ITargetWanter>());
		//public bool drawRadius = true;

		//public override Color IconDrawColor => base.IconDrawColor;
		//icon = ContentFinder<Texture2D>.Get("UI/Commands/Attack", true);

		/*
		public override void GizmoUpdateOnMouseover()
		{
			if (!drawRadius)
			{
				return;
			}
			targeter.verbProps.DrawRadiusRing(targeter.caster.Position);
			if (!groupedTargeter.NullOrEmpty())
			{
				foreach (ITargeterTarget targetor in groupedTargeter)
				{
					targetor.verbProps.DrawRadiusRing(targetor.caster.Position);
				}
			}
		}
		*/
		public override void SelectedUpdate()
		{
			base.SelectedUpdate();
			IntVec3 mouseCell = UI.MouseCell();
			GenDraw.DrawTargetHighlightWithLayer(mouseCell, AltitudeLayer.VisEffects);
			//GenUI.DrawMouseAttachment(icon);
			foreach (ITargetWanter targetWanter in TargetWanters.Distinct())
			{
				Vector3? loc = null;
				if (targetWanter is Thing thing)
				{
					loc = thing.TrueCenter();
				}
				if (targetWanter is ThingComp comp)
				{
					loc = comp.parent.TrueCenter();
				}
				if (loc != null)
				{
					GenDraw.DrawLineBetween(loc.Value, mouseCell.ToVector3Shifted(), SimpleColor.White);
				}
			}
		}

		public override void MergeWith(Gizmo other)
		{
			base.MergeWith(other);
			if (other is Designator_TargetWanterTarget command_VerbTarget)
			{
				if (groupedTargetWanter == null)
				{
					groupedTargetWanter = new List<ITargetWanter>();
				}
				groupedTargetWanter.Add(command_VerbTarget.targetWanter);
				if (command_VerbTarget.groupedTargetWanter != null)
				{
					groupedTargetWanter.AddRange(command_VerbTarget.groupedTargetWanter);
				}
			}
			else
			{
				Log.ErrorOnce("Tried to merge Command_TargeterTarget with unexpected type", 73401283);
				return;
			}
		}

		public override void ProcessInput(Event ev)
		{
			base.ProcessInput(ev);
			if ((Event.current.type == EventType.MouseDown && Event.current.button == 1) || KeyBindingDefOf.Cancel.KeyDownEvent)
			{
				SoundDefOf.CancelMode.PlayOneShotOnCamera(null);
				Find.DesignatorManager.Deselect();
			}
		}

		public override void DesignateSingleCell(IntVec3 c)
		{
			IEnumerable<Thing> things = TargetWanters.Select(x => x as Thing ?? (x is ThingComp comp ? comp.parent : null)).Where(x => x != null);
			if (!c.InBounds(Map))
			{
				Messages.Message("MessageWarningTargetWanterTargetingOutOfBounds".Translate(), new LookTargets(things), MessageTypeDefOf.CautionInput, false);
			}
			foreach (ITargetWanter targetWanter in TargetWanters)
			{
				targetWanter.WantTargetCell = c;
			}
			foreach (Thing thing in things)
			{
				MaunalActionUtility.UpdateMaunalActionDesignation(thing);
			}
			SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
			Find.DesignatorManager.Deselect();
		}

		public override AcceptanceReport CanDesignateCell(IntVec3 loc)
		{
			return AcceptanceReport.WasAccepted;
		}
	}
}

