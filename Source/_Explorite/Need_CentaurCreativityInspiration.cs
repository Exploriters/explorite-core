/********************
 * 半人马长期幸福的创造灵感奖励需求类。
 * --siiftun1857
 */
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
    ///<summary>长期幸福的创造灵感奖励。</summary>
    public class Need_CentaurCreativityInspiration : Need
    {
        public Need_CentaurCreativityInspiration(Pawn pawn) : base(pawn)
        {
            threshPercents = new List<float>();
        }
        private float lastEffectiveDelta = 0f;
        private float DeltaPerIntervalBase => 0.0025f;
        private float DeltaPerInterval => InHappiness ? DeltaPerIntervalBase : 0f;
        public override int GUIChangeArrow => IsFrozen ? 0 : Math.Sign(lastEffectiveDelta);
        public override float MaxLevel => 2f;
        //protected override bool IsFrozen => false;
        public override bool ShowOnNeedList => false; //!Disabled && pawn.IsColonist;
        public bool ShouldShow => !Disabled;
        public bool InHappiness => pawn.needs.TryGetNeed<Need_Mood>().CurLevelPercentage >= 1f;
        private bool Disabled => pawn.def != AlienCentaurDef;
        public override void SetInitialLevel() => CurLevel = 0;
        public float NextInspirationIn => (CurLevel >= 1f ? MaxLevel - CurLevel : MaxLevel - 1f - CurLevel) / DeltaPerIntervalBase * 2.5f;

        public override string GetTipString()
        {
            string result = $"{LabelCap}: {CurLevel.ToStringPercent()} / {MaxLevel.ToStringPercent()} ({FormattingTickTime(NextInspirationIn, "0.0")})\n{def.description}";
            return result;
        }
        private bool TryStartInspiration()
        {
            InspirationHandler ih = pawn.mindState.inspirationHandler;
            if (ih.Inspired || !pawn.health.capacities.CanBeAwake)
            {
                return false;
            }
            return ih.TryStartInspiration_NewTemp(InspirationDefOf.Inspired_Creativity, "LetterInspirationBeginThanksToHighMoodPart".Translate());
        }

        public override void NeedInterval()
        {
            if (Disabled)
            {
                CurLevel = 0f;
            }
            else if (!IsFrozen)
            {
                float curLevel = CurLevel;
                float targetLevel = CurLevel + DeltaPerInterval;

                if (targetLevel >= 1f && TryStartInspiration())
                    targetLevel -= 1f;

                CurLevel = Mathf.Min(Mathf.Max(targetLevel, 0f), MaxLevel);
                lastEffectiveDelta = CurLevel - curLevel;
            }
        }
    }
}
