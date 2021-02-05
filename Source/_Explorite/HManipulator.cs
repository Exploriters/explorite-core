/**
 * 实现物理操作仪的一系列类。
 * --siiftun1857
 */
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using static Explorite.ExploriteCore;

namespace Explorite
{
    /**
     * <summary>
     * 使物品被使用后为自身安装物理操作仪。
     * </summary>
     */
    public class CompUseEffect_HediffApply_HManipulator : CompUseEffect
    {
        /*public override void CompTick()
        {
            base.CompTick();
        }*/

        private BodyPartRecord FixScapular(Pawn pawn)
        {
            BodyPartRecord bodyPartRecord = null;
            foreach (Hediff_MissingPart hediff_MissingPart in pawn.health.hediffSet.GetMissingPartsCommonAncestors())
            {
                //if (!pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(hediff_MissingPart.Part))
                {
                    if (hediff_MissingPart.Part.def == CentaurScapularDef)
                    {
                        bodyPartRecord = hediff_MissingPart.Part;
                        break;
                    }
                }
            }
            if (bodyPartRecord != null)
            {
                pawn.health.RestorePart(bodyPartRecord, null, true);
            }
            return bodyPartRecord;
        }
        public override bool CanBeUsedBy(Pawn usedBy, out string failReason)
        {

            if (usedBy.health.hediffSet.HasHediff(HyperManipulatorHediffDef))
            {
                if (usedBy?.health?.hediffSet?.GetFirstHediffOfDef(HyperManipulatorHediffDef)?.Part.def == CentaurScapularDef)
                {
                    failReason = "Magnuassembly_CompUseEffect_HediffApply_HManipulator_UseReject_AlreadyInstalled".Translate(usedBy.Name.ToStringShort);
                    return false;
                }
            }

            if (usedBy.def != AlienCentaurDef)
            {
                failReason = "Magnuassembly_CompUseEffect_HediffApply_HManipulator_UseReject_NonCentaur".Translate(usedBy.def.LabelCap);
                return false;
            }

            failReason = null;
            return true;
        }
        public override void DoEffect(Pawn usedBy)
        {
            Hediff hediff = HediffMaker.MakeHediff(HyperManipulatorHediffDef, usedBy, null);

            BodyPartRecord CentaurScapularRecord = new BodyPartRecord();
            //CentaurScapularRecord.body = DefDatabase<BodyDef>.GetNamed("Centaur");
            //CentaurScapularRecord.def = CentaurScapularDef;
            //CentaurScapularRecord.parts.Count = 1;
            bool PartNotFound = true;

            IEnumerable<BodyPartRecord> parts = usedBy.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined);

            if (!CanBeUsedBy(usedBy, out _))
                return;

            //for (int i = 0; i < 1000; i++)
            {
                foreach (BodyPartRecord part in parts)
                {
                    if (part.def == CentaurScapularDef)
                    {
                        CentaurScapularRecord = part;
                        PartNotFound = false;
                        break;
                    }
                }
                if (PartNotFound)
                {
                    CentaurScapularRecord = FixScapular(usedBy);
                    //break;
                }
                else
                {
                    //break;
                }
            }

            if (CentaurScapularRecord == null)
                return;

            //if (!usedBy.health.hediffSet.HasHediff(HyperManipulatorHediffDef) && usedBy.def == AlienCentaurDef)
            {
                //   HealthUtility.AdjustSeverity(usedBy, HyperManipulatorHediffDef, 0.001f);

                //usedBy.health.RestorePart(CentaurScapularRecord);
                usedBy.health.RestorePart(CentaurScapularRecord, null, true);
                usedBy.health.AddHediff(hediff, CentaurScapularRecord, null);
                //usedBy.health.AddHediff(hediff, null, null);
                hediff.Severity = 0.001f;

                parent.Destroy();
            }
        }
    }

    /**
     * <summary>
     * 物理操作仪的hediff类型。无实际效果。
     * </summary>
     */
    public class Hediff_AddedPart_HManipulator : Hediff_AddedPart
    {
        //T___O_NOMORE__D____O: Prevent remove part surgery targeting HM
        //NOT HERE!
        //Solved somewhere other
    }

    // TO_NOMORE_DO: Force avaiable Solved
    /**
     * <summary>
     * 继承自<see cref = "CompUsable" />，但即使人物完全丧失操作能力也可用。
     * </summary>
     */
    public class CompUsable_IgnoreManipulation : CompUsable
    {
        private bool CanBeUsedBy(Pawn p, out string failReason)
        {
            List<ThingComp> allComps = parent.AllComps;
            for (int i = 0; i < allComps.Count; i++)
            {
                if (allComps[i] is CompUseEffect compUseEffect && !compUseEffect.CanBeUsedBy(p, out failReason))
                {
                    return false;
                }
            }
            failReason = null;
            return true;
        }
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn myPawn)
        {
            if (!CanBeUsedBy(myPawn, out string failReason))
            {
                yield return new FloatMenuOption(FloatMenuOptionLabel(myPawn) + ((failReason == null) ? string.Empty : (" (" + failReason + ")")), null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            else if (!myPawn.CanReach(parent, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
            {
                yield return new FloatMenuOption(FloatMenuOptionLabel(myPawn) + $" ({"NoPath".Translate()})", null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            else if (!myPawn.CanReserve(parent, 1, -1, null, false))
            {
                yield return new FloatMenuOption(FloatMenuOptionLabel(myPawn) + $" ({"Reserved".Translate()})", null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            /*else if (!myPawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                yield return new FloatMenuOption(this.FloatMenuOptionLabel(myPawn) + $" ({"Incapable".Translate()})", null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }*/
            else
            {
                FloatMenuOption useopt = new FloatMenuOption(FloatMenuOptionLabel(myPawn), delegate ()
                {
                    if (myPawn.CanReserveAndReach(parent, PathEndMode.Touch, Danger.Deadly, 1, -1, null, false))
                    {
                        foreach (CompUseEffect compUseEffect in parent.GetComps<CompUseEffect>())
                        {
                            if (compUseEffect.SelectedUseOption(myPawn))
                            {
                                return;
                            }
                        }
                        TryStartUseJob(myPawn, null);
                    }
                }, MenuOptionPriority.Default, null, null, 0f, null, null);
                yield return useopt;
            }
            yield break;
        }

        /*public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn myPawn)
        {
            IEnumerable<FloatMenuOption> preret = base.CompFloatMenuOptions(myPawn);
            foreach (FloatMenuOption fmo in preret)
            {
                Log.Message("[Magnuassembly]FMO LABEL: " + fmo.Label + ", Disabled: " + fmo.Disabled + ".");
                fmo.Disabled = false;
                Log.Message("[Magnuassembly]FMO LABEL POST: " + fmo.Label + ", Disabled: " + fmo.Disabled + ".");
            }
            return preret;
        }*/
    }
    /**
     * <summary>
     * 继承自<see cref = "JobDriver" />，但即使人物完全丧失操作能力也可用。<br />
     * 由于实现冲突，该类不继承自<see cref = "JobDriver_UseItem" />。
     * </summary>
     */
    public class JobDriver_UseItem_IgnoreManipulation : JobDriver//_UseItem
    {
        private int useDuration = -1;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref useDuration, "useDuration", 0, false);
        }

        public override void Notify_Starting()
        {
            base.Notify_Starting();
            useDuration = job.GetTarget(TargetIndex.A).Thing.TryGetComp<CompUsable>().Props.useDuration;
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo targetA = this.job.targetA;
            Job job = this.job;
            return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            //this.FailOnIncapable(PawnCapacityDefOf.Manipulation);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil prepare = Toils_General.Wait(useDuration, TargetIndex.None);
            prepare.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            prepare.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            prepare.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return prepare;
            Toil use = new Toil();
            use.initAction = delegate ()
            {
                Pawn actor = use.actor;
                CompUsable compUsable = actor.CurJob.targetA.Thing.TryGetComp<CompUsable>();
                compUsable.UsedBy(actor);
            };
            use.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return use;
            yield break;
        }
    }

}
