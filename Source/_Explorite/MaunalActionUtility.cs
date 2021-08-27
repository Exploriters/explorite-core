/********************
 * 手动操作建筑物机制。
 * --siiftun1857
 */
using Verse;
using RimWorld;
using System.Collections.Generic;
using Verse.Sound;
using System.Linq;
using static Explorite.ExploriteCore;
using System;
using Verse.AI;
using HarmonyLib;

namespace Explorite
{
	///<summary>可以执行人工操作的物体接口。</summary>
	public interface INeedMaunalAction
	{
		public bool NeedManualAction { get; }
		public void DoManualAction();
		public void CancalManualAction();
	}
	internal static partial class ExploritePatches
	{

		[HarmonyPostfix]public static void DesignationNotifyRemovingPostfix(Designation __instance)
		{
			if (__instance.def == MaunalActionObjectAdjustDesignation && __instance.target.Thing is Thing thing && !thing.DestroyedOrNull())
			{
				thing.CancalMaunalActions();
				return;
			}
		}
	}
	public static class MaunalActionUtility
	{
		public static void UpdateMaunalActionDesignationI(INeedMaunalAction t)
		{
			UpdateMaunalActionDesignation(t.INeedMaunalActionToThing());
		}
		public static void UpdateMaunalActionDesignation(Thing t)
		{
			bool flag = t.AnyMaunalActionsTodo();
			Designation designation = t?.Map?.designationManager?.DesignationOn(t, MaunalActionObjectAdjustDesignation);
			if (flag && designation == null)
			{
				t.Map.designationManager.AddDesignation(new Designation(t, MaunalActionObjectAdjustDesignation));
			}
			else if (!flag && designation != null)
			{
				designation.Delete();
			}
			TutorUtility.DoModalDialogIfNotKnown(ThingdjustingDesignationConcept, Array.Empty<string>());
		}
		public static IEnumerable<INeedMaunalAction> GetMaunalActions(this Thing thing)
		{
			if (thing is INeedMaunalAction thingWithAction)
			{
				yield return thingWithAction;
			}
			if (thing is ThingWithComps thingWithComps)
			{
				foreach (ThingComp comp in thingWithComps.AllComps)
				{
					if (comp is INeedMaunalAction thingCompWithAction)
					{
						yield return thingCompWithAction;
					}
				}
			}
		}
		public static Thing INeedMaunalActionToThing(this INeedMaunalAction act)
		{
			if (act is Thing thing)
			{
				return thing;
			}
			if (act is ThingComp comp)
			{
				return comp.parent;
			}
			return null;
		}
		public static void PawnSolveMaunalActions(this Thing thing)
		{
			SoundDefOf.FlickSwitch.PlayOneShot(new TargetInfo(thing.Position, thing.Map, false));
			thing.DoMaunalActions();
		}
		public static void DoMaunalActions(this Thing thing)
		{
			foreach (INeedMaunalAction maunalActionThing in thing.GetMaunalActions())
			{
				maunalActionThing.DoManualAction();
			}
		}
		public static void CancalMaunalActions(this Thing thing)
		{
			foreach (INeedMaunalAction maunalActionThing in thing.GetMaunalActions())
			{
				maunalActionThing.CancalManualAction();
			}
		}
		public static bool AnyMaunalActions(this Thing thing)
		{
			return thing.GetMaunalActions().Any();
		}
		public static bool AnyMaunalActionsTodo(this Thing thing)
		{
			return thing.GetMaunalActions().Any(t => t.NeedManualAction);
		}
	}
	public class JobDriver_TakeMaunalActionOnWanterThing : JobDriver
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(TargetIndex.A);
			this.FailOn(() => Map.designationManager.DesignationOn(TargetThingA, MaunalActionObjectAdjustDesignation) == null);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			yield return Toils_General.Wait(15, TargetIndex.None).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			Toil finalize = new Toil();
			finalize.initAction = delegate ()
			{
				Pawn actor = finalize.actor;
				actor.CurJob.targetA.Thing.PawnSolveMaunalActions();
				actor.records.Increment(ThingAdjustedRecord);
				Designation designation = Map.designationManager.DesignationOn(actor.CurJob.targetA.Thing, MaunalActionObjectAdjustDesignation);
				if (designation != null)
				{
					designation.Delete();
				}
			};
			finalize.defaultCompleteMode = ToilCompleteMode.Instant;
			yield return finalize;
			yield break;
		}
	}
	public class WorkGiver_TakeMaunalActionOnWanterThing : WorkGiver_Scanner
	{
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.Touch;
			}
		}

		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Deadly;
		}

		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			List<Designation> desList = pawn.Map.designationManager.allDesignations;
			int num;
			for (int i = 0; i < desList.Count; i = num + 1)
			{
				if (desList[i].def == MaunalActionObjectAdjustDesignation)
				{
					yield return desList[i].target.Thing;
				}
				num = i;
			}
			yield break;
		}
		public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			return !pawn.Map.designationManager.AnySpawnedDesignationOfDef(MaunalActionObjectAdjustDesignation);
		}
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return pawn.Map.designationManager.DesignationOn(t, MaunalActionObjectAdjustDesignation) != null && pawn.CanReserve(t, 1, -1, null, forced);
		}
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return JobMaker.MakeJob(MaunalActionObjectAdjustingJob, t);
		}
	}
}

