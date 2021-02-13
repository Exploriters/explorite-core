/**
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
        private readonly List<float?> detlas = new List<float?>() { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 0f };

        public void Insert(float num)
        {
            detlas.RemoveAt(0);
            detlas.Add(num);
        }
        public float Last() => detlas.FindLast(de => de.HasValue).Value;
        public float Averange()
        {
            float result = 0f;
            int count = 0;
            foreach (float num in detlas.Where(de => de.HasValue))
            {
                result += num;
                count++;
            }
            return count == 0 ? 0 : result / count;
        }
        public float AverangeP()
        {
            float result = 0f;
            int count = 0;
            bool encounted = false;
            foreach (float num in detlas.Where(de => de.HasValue && de.Value >= 0))
            {
                if (encounted || num > 0)
                {
                    encounted = true;
                    result += num;
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
            foreach (float num in detlas.Where(de => de.HasValue && de.Value <= 0))
            {
                if (encounted || num < 0)
                {
                    encounted = true;
                    result += num;
                    count++;
                }
            }
            return count == 0 ? 0 : result / count;
        }
    }
    ///<summary>持续吸收低温症。</summary>
    public class Need_HypothermiaAbsorption : Need
    {

        private const float DeltaPerIntervalBase = 0.0025f;
        private const float AbsorbHypothermiaFactor = 1f;
        private float LastEffectiveDelta => detlaTracer.Last();
        private readonly DetlaTracer detlaTracer = new DetlaTracer();
        public Need_HypothermiaAbsorption(Pawn pawn) : base(pawn)
        {
            threshPercents = new List<float>();
        }

        public override int GUIChangeArrow => IsFrozen ? 0 : Math.Sign(LastEffectiveDelta);
        public override float MaxLevel => 1f;
        protected override bool IsFrozen => false;
        public override bool ShowOnNeedList => !Disabled;
        private bool Disabled => pawn.def != AlienCentaurDef;
        public ExposureStateEnum ExposureState
        {
            get
            {
                if (IsFrozen || Disabled)
                    return ExposureStateEnum.None;
                if (Math.Sign(LastEffectiveDelta) < 0)
                    return ExposureStateEnum.Exposing;
                else if (MaxLevel > CurLevel)
                    return ExposureStateEnum.Restoring;
                else
                    return ExposureStateEnum.None;
            }
        }

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


        public override void SetInitialLevel()
        {
            CurLevel = MaxLevel;
        }
        public override string GetTipString()
        {
            //string result = $"{LabelCap}: {CurLevel.ToStringPercent()} / {MaxLevel.ToStringPercent()} ({CurLevelPercentage.ToStringPercent()})\n";
            string result = $"{LabelCap}: {CurLevelPercentage.ToStringPercent()} ({(detlaTracer.Averange() >= 0f ? "+" : null)}{"PeriodSeconds".Translate((detlaTracer.Averange() * 100 / 2.5f).ToString("0.##") + "% /")})\n";
            result += ExposureState switch
            {
                ExposureStateEnum.Exposing => $"{"Magnuassembly_CriticalExposureIn".Translate() }: {FormattingTickTime(ReachLimitIn)}\n",
                ExposureStateEnum.Restoring => $"{"Magnuassembly_RestoringIn".Translate()       }: {FormattingTickTime(ReachLimitIn)}\n",
                _ => null,
            };
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
                float absrobAmount;

                if (pawn.health.hediffSet.HasHediff(HediffDefOf.Hypothermia))
                {
                    Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Hypothermia);

                    absrobAmount = Math.Min(targetLevel, hediff.Severity / AbsorbHypothermiaFactor);

                    hediff.Severity -= absrobAmount / AbsorbHypothermiaFactor;
                    targetLevel -= absrobAmount;
                }

                CurLevel = Mathf.Min(Mathf.Max(targetLevel, 0f), MaxLevel);
                detlaTracer.Insert(CurLevel - curLevel);
            }
        }
    }
}
