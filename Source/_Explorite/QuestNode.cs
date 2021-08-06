using System.Linq;
using Verse;
using RimWorld;
using RimWorld.QuestGen;
using static Explorite.ExploriteCore;

namespace Explorite
{
	public class QuestNode_FisteverTrishot : QuestNode
	{
		[NoTranslate]
		public SlateRef<string> inSignalChoiceUsed;

		protected override bool TestRunInt(Slate slate)
		{
			return Faction.OfPlayer.def == CentaurPlayerColonyDef;
		}

		protected override void RunInt()
		{
			GameComponentCentaurStory.trishotTraceEnabled = true;
			Find.ResearchManager.AddTechprints(CentaurStoryProjectStep1Def, 1);
			Find.ResearchManager.FinishProject(CentaurStoryProjectStep1Def, false, null);

			Slate slate = QuestGen.slate;
			QuestPart_Choice questPart_Choice = new QuestPart_Choice
			{
				inSignalChoiceUsed = QuestGenUtility.HardcodedSignalWithQuestID(inSignalChoiceUsed.GetValue(slate))
			};

			QuestPart_Choice.Choice choice = new QuestPart_Choice.Choice();

			choice.rewards.Add(new Reward_FisteverTrishot_Trishot());
			choice.rewards.Add(new Reward_CentaurStoryProgress_Start());

			questPart_Choice.choices.Add(choice);

			QuestPart_HyperLinks questPart_HyperLinks = new QuestPart_HyperLinks() {
				DefDatabase<ThingDef>.GetNamed("FabricationBench"),
				//DefDatabase<RecipeDef>.GetNamed("Repair_Trishot_1Stage"),
			};

			QuestGen.quest.AddPart(questPart_Choice);
			QuestGen.quest.AddPart(questPart_HyperLinks);
		}
	}
}
