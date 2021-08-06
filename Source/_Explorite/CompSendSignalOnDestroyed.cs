using RimWorld;
using RimWorld.QuestGen;
using System.Linq;
using Verse;

namespace Explorite
{
	public class CompProperties_SendSignalOnDestroyed : CompProperties
	{
		public string signalTag;

		public CompProperties_SendSignalOnDestroyed()
		{
			compClass = typeof(CompSendSignalOnDestroyed);
		}
	}
	public class CompSendSignalOnDestroyed : ThingComp
	{
		private CompProperties_SendSignalOnDestroyed Props => (CompProperties_SendSignalOnDestroyed)props;
		private string SignalTag => Props.signalTag;

		public override void PostDestroy(DestroyMode mode, Map previousMap)
		{
			Find.SignalManager.SendSignal(new Signal(SignalTag));
			foreach (Quest quest in Find.QuestManager.questsInDisplayOrder)
			{
				quest.Notify_SignalReceived(new Signal($"Quest{quest.id}.{SignalTag}"));
			}
			base.PostDestroy(mode, previousMap);
		}
	}
}
