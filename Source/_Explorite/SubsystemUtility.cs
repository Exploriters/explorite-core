/********************
 * 子系统效用类。
 * 
 * 仅限半人马使用。
 * --siiftun1857
 */
using Verse;
using RimWorld;
using static Explorite.ExploriteCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;

namespace Explorite
{
    ///<summary>指定一个子系统的槽位。</summary>
    public enum SubsystemSlot : byte
    {
        Any = 0,
        Sys1 = 1,
        Sys2 = 2,
        Sys3 = 3
    }
    ///<summary>指定一个子系统的状态。</summary>
    public enum SubsystemState
    {
        Unidentified = 0,
        Blank = 1,
        Initialzing = 2,
        Enabled = 3
    }

    ///<summary>子系统方法集。</summary>
    [StaticConstructorOnStartup]
    public static class SubsystemUtility
    {
        ///<summary>子系统1身体部件。</summary>
        public static readonly BodyPartRecord SubsystemPart1 = AlienCentaurDef?.race?.body?.AllParts?.Find(bpr =>
                bpr?.groups?.Contains(SlotGroup(SubsystemSlot.Sys1)) ?? false);
        ///<summary>子系统2身体部件。</summary>
        public static readonly BodyPartRecord SubsystemPart2 = AlienCentaurDef?.race?.body?.AllParts?.Find(bpr =>
                bpr?.groups?.Contains(SlotGroup(SubsystemSlot.Sys2)) ?? false);
        ///<summary>子系统3身体部件。</summary>
        public static readonly BodyPartRecord SubsystemPart3 = AlienCentaurDef?.race?.body?.AllParts?.Find(bpr =>
                bpr?.groups?.Contains(SlotGroup(SubsystemSlot.Sys3)) ?? false);

        ///<summary>将子系统槽位枚举转为身体部件组。</summary>
        public static BodyPartGroupDef SlotGroup(SubsystemSlot slot) => slot switch
        {
            SubsystemSlot.Sys1 => CentaurSubsystemGroup1Def,
            SubsystemSlot.Sys2 => CentaurSubsystemGroup2Def,
            SubsystemSlot.Sys3 => CentaurSubsystemGroup3Def,
            _ => CentaurSubsystemGroup0Def,
        };
        ///<summary>将子系统槽位枚举转为身体部件。</summary>
        public static BodyPartRecord SlotPart(SubsystemSlot slot) => slot switch
        {
            SubsystemSlot.Sys1 => SubsystemPart1,
            SubsystemSlot.Sys2 => SubsystemPart2,
            SubsystemSlot.Sys3 => SubsystemPart3,
            _ => throw new ArgumentOutOfRangeException(nameof(slot)),
        };
        ///<summary>检查子系统槽位枚举是否属于三个中的一个。</summary>
        public static void InvalidSlotCheck(SubsystemSlot slot)
        {
            switch (slot)
            {
                case SubsystemSlot.Sys1:
                case SubsystemSlot.Sys2:
                case SubsystemSlot.Sys3:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }
        ///<summary>人物的子系统可见性。</summary>
        public static bool SubsystemVisible(this Pawn pawn)
        {
            return (pawn?.health?.hediffSet?.hediffs?.Any(hediff => hediff?.def?.tags?.Contains("CentaurTechHediff_AccuSubsystem") ?? false)) ?? false;
        }
        ///<summary>检查人物的一个子系统是否已经启用。</summary>
        public static bool SubsystemEnabled(this Pawn pawn, HediffDef hediffDef)
        {
            return (pawn?.health?.hediffSet?.GetFirstHediffOfDef(hediffDef))?.SubsystemEnabled() ?? false;
        }
        ///<summary>检查一个子系统是否已经启用。</summary>
        public static bool SubsystemEnabled(this Hediff hediff)
        {
            if (hediff is Hediff_AddedPart_Subsystem subsystem)
                return subsystem.Enabled;
            return false;
        }
        ///<summary>取得人物对应身体部位的子系统。</summary>
        public static Hediff_AddedPart_Subsystem GetSubsystemHediffOn(this Pawn pawn, SubsystemSlot slot = SubsystemSlot.Any)
        {
            return pawn?.health?.hediffSet?.hediffs.Find(hediff =>
                hediff is Hediff_AddedPart_Subsystem
                && hediff?.Part?.groups?.Contains(SlotGroup(slot)) == true) as Hediff_AddedPart_Subsystem;
        }
        ///<summary>确保子系统存在。</summary>
        public static void EnsureSubsystemExist(this Pawn pawn, HediffDef hediffDef = null)
        {
            hediffDef ??= SubsystemBlankHediffDef;

            List<BodyPartRecord> partsToRestore = new List<BodyPartRecord>();
            IEnumerable<Hediff_MissingPart> missingPartHediffs = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
            foreach (Hediff_MissingPart hediff_MissingPart in missingPartHediffs)
            {
                if (hediff_MissingPart.Part.groups.Contains(CentaurSubsystemGroup0Def))
                {
                    partsToRestore.Add(hediff_MissingPart.Part);
                }
            }
            foreach (BodyPartRecord part in partsToRestore)
            {
                pawn.health.RestorePart(part, null, true);
            }

            IEnumerable<BodyPartRecord> parts = pawn?.health?.hediffSet?.GetNotMissingParts()?.Where(bpr =>
                bpr?.groups?.Contains(CentaurSubsystemGroup0Def) ?? false);
            foreach (BodyPartRecord part in parts)
            {
                if (!(pawn?.health?.hediffSet?.hediffs?.Any(h =>
                        (h?.def?.tags?.Contains("CentaurSubsystem") ?? false) && h?.Part == part
                    ) ?? false))
                {
                    pawn.health.AddHediff(hediffDef, part);
                }
            }
        }


        private static string GetSubsystemTooltip(this Pawn pawn, SubsystemSlot slot)
        {
            InvalidSlotCheck(slot);
            BodyPartRecord part = SlotPart(slot);
            Hediff_AddedPart_Subsystem hediff = pawn.GetSubsystemHediffOn(slot);
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(part.LabelCap + ": ");
            stringBuilder.AppendLine(" " + pawn.health.hediffSet.GetPartHealth(part) + " / " + part.def.GetMaxHealth(pawn));
            float num = PawnCapacityUtility.CalculatePartEfficiency(pawn.health.hediffSet, part);
            if (num != 1f)
            {
                stringBuilder.AppendLine("Efficiency".Translate() + ": " + num.ToStringPercent());
            }
            if (hediff != null)
            {
                stringBuilder.AppendLine("------------------");
                string severityLabel = hediff.SeverityLabel;
                if (!hediff.Label.NullOrEmpty() || !severityLabel.NullOrEmpty() || !hediff.CapMods.NullOrEmpty())
                {
                    stringBuilder.Append(hediff.LabelCap);
                    if (!severityLabel.NullOrEmpty())
                    {
                        stringBuilder.Append(": " + severityLabel);
                    }
                    stringBuilder.AppendLine();
                    string tipStringExtra = hediff.TipStringExtra;
                    if (!tipStringExtra.NullOrEmpty())
                    {
                        stringBuilder.AppendLine(tipStringExtra.TrimEndNewlines().Indented());
                    }
                }
            }
            return stringBuilder.ToString().TrimEnd();
        }

        private static readonly Texture2D SubsystemTabBackground_Normal = ContentFinder<Texture2D>.Get("UI/ExploriteMisc/ITabSubsystemSingle_Normal");
        private static readonly Texture2D SubsystemTabBackground_Blank = ContentFinder<Texture2D>.Get("UI/ExploriteMisc/ITabSubsystemSingle_Blank");
        private static readonly Texture2D SubsystemTabBackground_Error = ContentFinder<Texture2D>.Get("UI/ExploriteMisc/ITabSubsystemSingle_Error");
        private static readonly Texture2D SubsystemHealthBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(51 / 255f, 204 / 255f, 217 / 255f));
        private static readonly Texture2D SubsystemHealthBarRedTex = SolidColorMaterials.NewSolidColorTexture(new Color(217 / 255f, 64 / 255f, 50 / 255f));
        private static readonly Texture2D SubsystemInitialBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(86 / 255f, 86 / 255f, 86 / 255f));
        private static readonly Texture2D SubsystemInitialFilledBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(115 / 255f, 172 / 255f, 230 / 255f));
        private static readonly Texture2D SubsystemEmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
        ///<summary>在ITab内渲染单个子系统UI。</summary>
        public static void SingleTab(Rect rect, Pawn pawn, SubsystemSlot slot)
        {
            try
            {
                InvalidSlotCheck(slot);

                Hediff_AddedPart_Subsystem subsystemHediff = pawn.GetSubsystemHediffOn(slot);
                bool initialzing = false;
                bool noSubsystem = subsystemHediff?.SubsystemState switch
                {
                    SubsystemState.Initialzing => false,
                    SubsystemState.Enabled => false,
                    _ => true,
                };
                if (noSubsystem)
                {
                    subsystemHediff = null;
                }
                else if(subsystemHediff?.SubsystemState == SubsystemState.Initialzing)
                {
                    initialzing = true;
                }

                Texture2D SubsystemTabBackground = noSubsystem ? SubsystemTabBackground_Blank : SubsystemTabBackground_Normal;

                // 渲染背景图
                GUI.DrawTexture(rect, SubsystemTabBackground);

                // 渲染子系统图标
                if (!noSubsystem)
                {
                    Rect rectSysIcon = new Rect(rect)
                    {
                        width = 48f,
                        height = 48f,
                        x = rect.x + 26f,
                        y = rect.y + 26f
                    };
                    GUI.DrawTexture(rectSysIcon, subsystemHediff?.Def?.SubsystemIcon);

                    if (Widgets.ButtonInvisible(rectSysIcon))
                    {
                        Find.WindowStack.Add(new Dialog_InfoCard(subsystemHediff.Def));
                    }
                }

                // 标题文本渲染
                {
                    string label = subsystemHediff?.LabelBaseCap ?? "NoSubsystemLabel".Translate();
                    //string desc = subsystemHediff?.def.description ?? "NoSubsystemLabel".Translate();
                    Rect rectTextBase = new Rect(rect)
                    {
                        width = 424f,
                        height = 31f,
                        x = rect.x + 106f,
                        y = rect.y + 11f
                    };
                    Color textBgColor = new Color(51 / 255f, 48 / 255f, 44 / 255f);
                    if (Widgets.ButtonInvisible(rectTextBase) && !noSubsystem)
                    {
                        Find.WindowStack.Add(new Dialog_InfoCard(subsystemHediff.Def));
                        //Find.MainTabsRoot.SetCurrentTab(MainButtonDefOf.Factions);
                    }
                    if (Mouse.IsOver(rectTextBase))
                    {
                        textBgColor = new Color(45 / 255f, 48 / 255f, 52 / 255f);
                        TaggedString taggedString2 = pawn.GetSubsystemTooltip(slot) + (noSubsystem ? (TaggedString)string.Empty : ("\n\n" + "ClickToLearnMore".Translate()));
                        TipSignal tip6 = new TipSignal(taggedString2, pawn.Faction.loadID * 37);
                        TooltipHandler.TipRegion(rectTextBase, tip6);
                    }
                    Widgets.DrawRectFast(rectTextBase, textBgColor);
                    Rect rectMainText = rectTextBase.ContractedBy(2f);
                    Text.Font = GameFont.Medium;
                    Widgets.Label(rectMainText, label);

                    //Rect rect3 = rectMainText;
                    //rect3.yMin += 50f;
                    //Text.Font = GameFont.Small;
                    //Widgets.Label(rect3, subsystemHediff?.Def?.description ?? "NoSubsystemLabel".Translate());
                }
                // 生命值条渲染
                {
                    float partHealth = pawn.health.hediffSet.GetPartHealth(SlotPart(slot));
                    float partHealthMax = SlotPart(slot).def.GetMaxHealth(pawn);
                    const float scaleFactor = 2f;

                    Text.Font = GameFont.Small;

                    Rect rectFillBarBase = new Rect(rect)
                    {
                        width = 4f + partHealthMax * scaleFactor + Text.CalcSize($"{partHealthMax} / {partHealthMax}").x + 8f,
                        height = 24f,
                        x = rect.x + 106f,
                        y = rect.y + 62f
                    };
                    Widgets.DrawRectFast(rectFillBarBase, Color.black);
                    Rect rectFillBarSpaceBase = rectFillBarBase.ContractedBy(2f);

                    Rect rectFillBarHealth = new Rect(rectFillBarSpaceBase)
                    {
                        width = partHealthMax * scaleFactor
                    };
                    Widgets.FillableBar(rectFillBarHealth, partHealth / partHealthMax, SubsystemHealthBarTex, SubsystemHealthBarRedTex, doBorder: false);

                    Rect rectFillBarHealthTip = new Rect(rectFillBarSpaceBase);
                    rectFillBarHealthTip.xMin += partHealth * scaleFactor + 4;

                    Widgets.Label(rectFillBarHealthTip, $"{partHealth} / {partHealthMax}");
                }
                // 初始化进度渲染
                if(initialzing)
                {
                    const float barScale = 454f;
                    Text.Font = GameFont.Tiny;
                    float percentage = 1 - subsystemHediff.InitPercentage;
                    string reportStr = subsystemHediff.InitPercentage.ToStringPercent();
                    Vector2 textsize = Text.CalcSize(reportStr);

                    Rect rectFillBar = new Rect(rect)
                    {
                        width = barScale,
                        height = 4f,
                        x = rect.x + 98f,
                        y = rect.y + 48f
                    };
                    //Widgets.FillableBar(rectFillBar, subsystemHediff.InitPercentage, SubsystemEmptyBarTex, SubsystemInitialBarTex, doBorder: false);

                    Rect rectFillBarMain = new Rect(rectFillBar);
                    rectFillBarMain.x += rectFillBarMain.width * subsystemHediff.InitPercentage;
                    rectFillBarMain.width *= percentage;
                    GUI.DrawTexture(rectFillBarMain, SubsystemInitialBarTex);

                    Vector2 pos = new Vector2(
                        rectFillBar.x + subsystemHediff.InitPercentage * barScale + 4f,
                        rectFillBar.yMax - (textsize.y / 2f)
                        );
                    Rect rectFillBarTip = new Rect(pos, textsize);
                    if (rectFillBarTip.xMax > rectFillBar.xMax - 4f)
                    {
                        rectFillBarTip.x = rectFillBar.x - textsize.x - 4f;
                    }

                    Widgets.Label(rectFillBarTip, reportStr);
                }

            }
            catch(NullReferenceException e)
            {
                GUI.DrawTexture(rect, SubsystemTabBackground_Error);
                Rect rect2 = new Rect()
                {
                    width = 424f,
                    height = 31f,
                    x = rect.x + 106f,
                    y = rect.y + 11f
                };
                Text.Font = GameFont.Medium;
                Widgets.Label(rect2, "NullReferenceException occurred");

                Log.Error(string.Concat(
                    "[Explorite]Subsystem inspect tab part draw occurred an exception.\n",
                    $"{e.GetType().Name}: {e.Message}\n",
                    $"Stack Trace:\n{e.StackTrace}\n"
                    ));
            }
        }
    }

}
