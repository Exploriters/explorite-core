using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Grammar;
using RimWorld;
using RimWorld.QuestGen;
using static Explorite.ExploriteCore;

namespace Explorite
{
    public abstract class Reward_NotImplementedStatic : Reward
    {
        public override IEnumerable<GenUI.AnonymousStackElement> StackElements => throw new NotImplementedException();
        public override IEnumerable<QuestPart> GenerateQuestParts(int index, RewardsGeneratorParams parms, string customLetterLabel, string customLetterText, RulePack customLetterLabelRules, RulePack customLetterTextRules)
        {
            throw new NotImplementedException();
        }
        public override void InitFromValue(float rewardValue, RewardsGeneratorParams parms, out float valueActuallyUsed)
        {
            throw new NotImplementedException();
        }
        public override string GetDescription(RewardsGeneratorParams parms)
        {
            throw new NotImplementedException();
        }
    }

    [StaticConstructorOnStartup]
    public class Reward_FisteverTrishot_Trishot : Reward_NotImplementedStatic
    {
        //private static readonly Texture2D Icon = ContentFinder<Texture2D>.Get(TrishotThingDef.graphicData.texPath);
        public override IEnumerable<GenUI.AnonymousStackElement> StackElements
        {
            get
            {
                yield return
                    QuestPartUtility.GetStandardRewardStackElement(
                        "Reward_ProgressToTrishot_Label".Translate(),
                        TrishotThingDef.uiIcon,
                        () => GetDescription(default).CapitalizeFirst(),
                        () => Find.WindowStack.Add(new Dialog_InfoCard(TrishotThingDef)));
            }
        }
        public override string GetDescription(RewardsGeneratorParams parms)
        {
            return "Reward_ProgressToTrishot".Translate().Resolve();
        }
    }
    [StaticConstructorOnStartup]
    public class Reward_CentaurStoryProgress_Start : Reward_NotImplementedStatic
    {
        private Action<Rect> IconDrawer => delegate (Rect r)
        {
            Faction faction = Find.FactionManager.AllFactions.First(faction => faction.def == (CentaurDummyFactionDef ?? CentaurPlayerColonyDef));
            GUI.color = faction.Color;
            GUI.DrawTexture(r, faction.def.FactionIcon);
            GUI.color = Color.white;
        };
        public override IEnumerable<GenUI.AnonymousStackElement> StackElements
        {
            get
            {
                yield return
                    QuestPartUtility.GetStandardRewardStackElement(
                        "Reward_CentaurStoryProgress_Start_Label".Translate(),
                        IconDrawer,
                        () => GetDescription(default).CapitalizeFirst());
            }
        }
        public override string GetDescription(RewardsGeneratorParams parms)
        {
            return "Reward_CentaurStoryProgress_Start".Translate().Resolve();
        }
    }
}
