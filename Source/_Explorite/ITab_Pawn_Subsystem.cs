/********************
 * 使物品的生命值随着时间逐渐恢复的Comp类。
 */
using Verse;
using RimWorld;
using UnityEngine;

namespace Explorite
{
    public class ITab_Pawn_Subsystem : ITab
    {
        private static readonly Vector2 WinSize = new Vector2(650f, 350f);
        private Pawn Pawn
        {
            get
            {
                if (SelPawn != null)
                {
                    return SelPawn;
                }
                else if (SelThing is Corpse corpse)
                {
                    return corpse.InnerPawn;
                }
                Log.Error("Character tab found no selected pawn to display.");
                return null;
            }
        }

        private CompPawnSubsystemManager compStash = null;
        public CompPawnSubsystemManager Comp => compStash ??= Pawn?.GetComp<CompPawnSubsystemManager>();
        public override bool IsVisible => Comp != null;

        public ITab_Pawn_Subsystem()
        {
            labelKey = "TabSubsystem";
            tutorTag = "Subsystem";
        }

        protected override void UpdateSize()
        {
            base.UpdateSize();
            //size = CharacterCardUtility.PawnCardSize(PawnToShowInfoAbout) + new Vector2(17f, 17f) * 2f;
            size = WinSize;
        }

        protected override void FillTab()
        {
            if (!IsVisible)
                return;
            Rect rectBase = new Rect(0f, 0f, WinSize.x, WinSize.y).ContractedBy(25f);
            float sectionRange = rectBase.height / 3f;

            Rect rectBase3 = rectBase.ContractedBy(-3f);
            Widgets.DrawRectFast(rectBase3, new Color(0.5f, 0.5f, 0.5f, 0.5f));
            Rect rectBase2 = rectBase.ContractedBy(-2f);
            Widgets.DrawRectFast(rectBase2, new Color(0.5f, 0.5f, 0.5f));

            Rect rect1 = new Rect(rectBase)
            {
                height = sectionRange
            };
            SubsystemUtility.SingleTab(rect1, Pawn, SubsystemSlot.Sys1);

            Rect rect2 = new Rect(rectBase)
            {
                height = sectionRange
            };
            rect2.y += sectionRange;
            SubsystemUtility.SingleTab(rect2, Pawn, SubsystemSlot.Sys2);

            Rect rect3 = new Rect(rectBase)
            {
                height = sectionRange
            };
            rect3.y += 2 * sectionRange;
            SubsystemUtility.SingleTab(rect3, Pawn, SubsystemSlot.Sys3);
        }
    }
}
