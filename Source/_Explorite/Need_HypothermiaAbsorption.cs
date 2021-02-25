/********************
 * 半人马免疫冻伤的能力需求类。
 * --siiftun1857
 */
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
    ///<summary>追踪最近20条变化量记录。</summary>
    public class DetlaTracer
    {
        private readonly List<float?> detlas = new List<float?>() { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null };

        public void Insert(float num)
        {
            detlas.RemoveAt(0);
            if (num == 0 && detlas.Count == detlas.Where(n => !n.HasValue || (n.HasValue && n == 0f)).Count())
            {
                detlas.Add(null);
                if (detlas.Where(n => n.HasValue).Any())
                {
                    detlas.Clear();
                    detlas.AddRange(new float?[] { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null });
                }
            }
            else
                detlas.Add(num);
        }
        public float Last()
        {
            float? last = detlas.FindLast(_ => true);
            if (last.HasValue)
                return last.Value;
            return 0f;
        }

        public float Averange()
        {
            float result = 0f;
            int count = 0;
            bool encounted = false;
            foreach (float? num in detlas.Where(de => de.HasValue))
            {
                if (!num.HasValue)
                {
                    result = 0f;
                    count = 0;
                    encounted = false;
                    continue;
                }
                if (encounted || num != 0)
                {
                    encounted = true;
                    result += num.Value;
                    count++;
                }
            }
            return count == 0 ? 0 : result / count;
        }
        public float AverangeP()
        {
            float result = 0f;
            int count = 0;
            bool encounted = false;
            foreach (float? num in detlas.Where(de => de.HasValue && de.Value >= 0))
            {
                if (!num.HasValue)
                {
                    result = 0f;
                    count = 0;
                    encounted = true;
                    continue;
                }
                if (encounted || num > 0)
                {
                    encounted = true;
                    result += num.Value;
                    count++;
                }
            }
            return count == 0 ? 0 : result / count;
        }
        public float AverangeN()
        {
            float result = 0f;
            int count = 0;
            bool encounted = false;
            foreach (float? num in detlas.Where(de => de.HasValue && de.Value <= 0))
            {
                if (!num.HasValue)
                {
                    result = 0f;
                    count = 0;
                    encounted = true;
                    continue;
                }
                if (encounted || num < 0)
                {
                    encounted = true;
                    result += num.Value;
                    count++;
                }
            }
            return count == 0 ? 0 : result / count;
        }
    }
    public class Need_HypothermiaAbsorption : Need
    {

        private const float DeltaPerIntervalBase = 0.0025f;
        private const float AbsorbFactor = 1f;
        private float LastEffectiveDelta => detlaTracer.Last();
        private float lastabsrobAmount = 0f;
        private readonly DetlaTracer detlaTracer = new DetlaTracer();
        public Need_HypothermiaAbsorption(Pawn pawn) : base(pawn)
        {
            threshPercents = new List<float>();
        }

        public override int GUIChangeArrow => IsFrozen ? 0 : Math.Sign(LastEffectiveDelta);
        public override float MaxLevel => 1f;
        protected override bool IsFrozen => false;
        public override bool ShowOnNeedList => !Disabled;// && ExposureState != ExposureStateEnum.None;
        private bool Disabled => pawn.def != AlienCentaurDef;
        public ExposureStateEnum ExposureState
        {
            get
            {
                if (IsFrozen || Disabled)
                    return ExposureStateEnum.None;
                if (Math.Sign(LastEffectiveDelta) < 0 || lastabsrobAmount > 0)
                    return ExposureStateEnum.Exposing;
                else if (MaxLevel > CurLevel)
                    return ExposureStateEnum.Restoring;
                else
                    return ExposureStateEnum.None;
            }
        }

        public bool Depleted => CurLevel <= 0.05;

        public float ReachLimitIn => Math.Sign(LastEffectiveDelta) switch
        {
            -1 => CurLevel / -detlaTracer.AverangeN() * 2.5f,
            1 => (MaxLevel - CurLevel) / detlaTracer.AverangeP() * 2.5f,
            _ => 0,
        };

        public enum ExposureStateEnum : byte
        {
            None = 0,
            Exposing = 1,
            Restoring = 2
        }

        public IEnumerable<HediffDef> TargetHediffs
        {
            get
            {
                yield return HediffDefOf.Hypothermia;
                if ((pawn?.health?.hediffSet?.GetFirstHediffOfDef(HediffCentaurSubsystem_HazardAdaptation_Def)).SubsystemEnabled())
                {
                    yield return HediffDefOf.ToxicBuildup;
                    yield return HediffDefOf.Heatstroke;
                }
                yield break;
            }
        }

        public override void SetInitialLevel()
        {
            CurLevel = MaxLevel;
        }
        public override void DrawOnGUI(Rect rect, int maxThresholdMarkers = 2147483647, float customMargin = -1f, bool drawArrows = true, bool doTooltip = true)
        {
            rect.width /= 0.73f;
            //rect.height = Mathf.Max(rect.height * 0.666f, 30f);
            base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip);
        }
        public override string GetTipString()
        {
            //string result = $"{LabelCap}: {CurLevel.ToStringPercent()} / {MaxLevel.ToStringPercent()} ({CurLevelPercentage.ToStringPercent()})\n";
            string result = $"{LabelCap}: {CurLevelPercentage.ToStringPercent()} ({(detlaTracer.Averange() >= 0f ? "+" : null)}{"PeriodSeconds".Translate((detlaTracer.Averange() * 40f).ToString("0.##") + "% /")})\n";
            result += ExposureState switch
            {
                ExposureStateEnum.Exposing => $"{"Magnuassembly_CriticalExposureIn".Translate() }: {FormattingTickTime(ReachLimitIn)}",
                ExposureStateEnum.Restoring => $"{"Magnuassembly_RestoringIn".Translate()       }: {FormattingTickTime(ReachLimitIn)}",
                _ => "Magnuassembly_RestoringIn".Translate(),
            };
            result += "\n";
            result += "Magnuassembly_HazardAbsorbType".Translate(string.Join(", ", TargetHediffs.Select(hediff => hediff.LabelCap)));
            result += "\n";
            result += def.description;
            return result;
        }

        public override void NeedInterval()
        {
            if (Disabled)
            {
                CurLevel = MaxLevel;
            }
            else if (!IsFrozen)
            {
                float curLevel = CurLevel;
                float targetLevel = CurLevel + DeltaPerIntervalBase;
                float absrobAmount = 0f;

                /*if (pawn.health.hediffSet.HasHediff(HediffDefOf.Hypothermia))
                {
                    float hypothermiaAbsrobAmount;
                    Hediff hediffHypothermia = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Hypothermia);
                    hypothermiaAbsrobAmount = Math.Min(targetLevel, hediffHypothermia.Severity / AbsorbFactor);
                    hediffHypothermia.Severity -= hypothermiaAbsrobAmount / AbsorbFactor;
                    absrobAmount += hypothermiaAbsrobAmount;
                    targetLevel -= hypothermiaAbsrobAmount;
                }*/
                foreach (HediffDef hediffDef in TargetHediffs)
                {
                    Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
                    if (hediff != null)
                    {
                        float hediffAbsrobAmount;
                        hediffAbsrobAmount = Math.Min(targetLevel, hediff.Severity / AbsorbFactor);
                        hediff.Severity -= hediffAbsrobAmount / AbsorbFactor;
                        absrobAmount += hediffAbsrobAmount;
                        targetLevel -= hediffAbsrobAmount;
                    }
                    if (targetLevel <= 0)
                    {
                        break;
                    }
                }

                CurLevel = Mathf.Min(Mathf.Max(targetLevel, 0f), MaxLevel);
                detlaTracer.Insert(CurLevel - curLevel);
                lastabsrobAmount = absrobAmount;
            }
        }
    }
}
