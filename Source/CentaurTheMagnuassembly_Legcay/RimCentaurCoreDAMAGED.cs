using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace CentaurTheMagnuassembly
{
    public class RimCentaurCore
    {
    }
    class CompUsable_HediffApply_HManipulator : CompUsable
    {
        /*public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn usedBy)
        {
            HediffDef HyperManipulatorHediff = DefDatabase<HediffDef>.GetNamed("HyperManipulator");
            IEnumerable<FloatMenuOption> Imenu = base.CompFloatMenuOptions(usedBy);
            if (usedBy.health.hediffSet.HasHediff(HyperManipulatorHediff))
            {
                foreach (FloatMenuOption target in Imenu)
                {
                    Log.Message("for non-manipulator-user msg");
                    target.Label += "({pawn.def.LabelCap} is already have one manipulator)";
                    target.action = null;
                }
            }
            else if (usedBy.def != DefDatabase<ThingDef>.GetNamed("Alien_Centaur"))
            {
                foreach (FloatMenuOption target in Imenu)
                {
                    Log.Message("for centaurs msg");
                    target.Label += "({pawn.def.LabelCap} can't use manipulator, this only for centaurs)";
                    target.action = null;

                }
            }
            foreach (FloatMenuOption target in Imenu)
            {
                Log.Message("spaming menu");
                target.Label += "({pawn.def.LabelCap}LALALA)";
            }
            return Imenu;
        }*/
        public new void TryStartUseJob(Pawn usedBy)
        {
            HediffDef HyperManipulatorHediff = DefDatabase<HediffDef>.GetNamed("HyperManipulator");
            IEnumerable<FloatMenuOption> Imenu = base.CompFloatMenuOptions(usedBy);
            bool killtask = false;
            if (usedBy.health.hediffSet.HasHediff(HyperManipulatorHediff))
            {
                killtask = true;
                foreach (FloatMenuOption target in Imenu)
                {
                    Log.Message("for non-manipulator-user msg");
                    target.Label += "({pawn.def.LabelCap} is already have one manipulator)";
                    target.action = null;
                }
            }
            else if (usedBy.def != DefDatabase<ThingDef>.GetNamed("Alien_Centaur"))
            {
                killtask = true;
                foreach (FloatMenuOption target in Imenu)
                {
                    Log.Message("for non centaurs msg");
                    target.Label += "({pawn.def.LabelCap} can't use manipulator, this only for centaurs)";
                    target.action = null;
                }
            }
            foreach (FloatMenuOption target in Imenu)
            {
                Log.Message("spaming menu");
                target.Label += "({pawn.def.LabelCap}LALALA)";
                Log.Message("PASTE:"+target.Label);
            }
            if (!killtask)
            {
                Log.Message("task aborted");
                base.TryStartUseJob(usedBy);
            }
        }
    }
    class JobDriver_ApplyHManipulator : JobDriver_UseItem
    {
    }



    class CompUseEffect_HediffApply_HManipulator : CompUseEffect
    {
        public override void DoEffect(Pawn usedBy)
        {
            HediffDef HyperManipulatorHediff = DefDatabase<HediffDef>.GetNamed("HyperManipulator");
            Hediff hediff = HediffMaker.MakeHediff(HyperManipulatorHediff, usedBy, null);

            BodyPartDef CentaurScapular = DefDatabase<BodyPartDef>.GetNamed("CentaurScapular");
            BodyPartRecord CentaurScapularRecord = null;
            bool NoneRecord = true;
            //CentaurScapularRecord.body = DefDatabase<BodyDef>.GetNamed("Centaur");
            //CentaurScapularRecord.def = CentaurScapular;
            //CentaurScapularRecord.parts.Count = 1;

            List<Hediff_MissingPart> misspartsHediff = usedBy.health.hediffSet.GetMissingPartsCommonAncestors();
            IEnumerable<BodyPartRecord> parts = usedBy.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined);

            base.DoEffect(usedBy);

            if (usedBy.def != DefDatabase<ThingDef>.GetNamed("Alien_Centaur"))
            {
                Log.Message("non centaur forbid");
                return;
            }
            if (usedBy.health.hediffSet.HasHediff(HyperManipulatorHediff))
            {
                Log.Message("non non-user forbid");
                return;
            }
            foreach (BodyPartRecord part in parts)
            {
                Log.Message("seeking2");
                if (part.def == CentaurScapular)
                {
                    Log.Message("seeked2");
                    CentaurScapularRecord = part;
                    NoneRecord = false;
                    break;
                }
            }
            if (NoneRecord)
            {
                foreach (Hediff_MissingPart partHediff in misspartsHediff)
                {
                    Log.Message("seeking1");
                    if (partHediff.Part.def == CentaurScapular)
                    {
                        Log.Message("seeked1");
                        usedBy.health.RestorePart(partHediff.Part);
                        CentaurScapularRecord = partHediff.Part;
                        NoneRecord = false;
                        break;
                    }
                }
            }
            if (NoneRecord)
            {
                Log.Message("CentaurScapular == null detected");
                return;
            }
                Log.Message("TRIGGER");
                //   HealthUtility.AdjustSeverity(usedBy, HyperManipulatorHediff, 0.001f);

                usedBy.health.RestorePart(CentaurScapularRecord);
                usedBy.health.AddHediff(hediff, CentaurScapularRecord, null);
                //usedBy.health.AddHediff(hediff, null, null);
                hediff.Severity = 0.001f;

                this.parent.Destroy();



        }
    }

}
