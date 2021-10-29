/********************
 * 在制作完成时转移全局追踪器。
 * --siiftun1857
 */
using System.Linq;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
	public abstract class SpecialThingFilterWorker_TrishotStage : SpecialThingFilterWorker
	{
		public abstract string StageName { get; }
		public override bool Matches(Thing t)
		{
			return CanEverMatch(t.def) && t.GetItemStageComps().FirstOrFallback(comp => comp.Group == "Trishot")?.StageName == StageName;
		}
		public override bool CanEverMatch(ThingDef def)
		{
			return def == TrishotThingDef;
		}
	}
	public class SpecialThingFilterWorker_TrishotStage1 : SpecialThingFilterWorker_TrishotStage
	{
		public override string StageName => "stage1";
	}
	public class SpecialThingFilterWorker_TrishotStage2 : SpecialThingFilterWorker_TrishotStage
	{
		public override string StageName => "stage2";
	}
	public class SpecialThingFilterWorker_TrishotStage3 : SpecialThingFilterWorker_TrishotStage
	{
		public override string StageName => "stage3";
	}
}
