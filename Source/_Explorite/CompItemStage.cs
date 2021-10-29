using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Explorite
{
	public class CompProperties_ItemStage : CompProperties
	{
		[NoTranslate]
		public string group;
		[MustTranslate]
		public string groupLabelPrefix;
		[MustTranslate]
		public string statEntryLabel;
		[MustTranslate]
		public string statEntryDesc;
		public List<ItemStage> stages;

		public CompProperties_ItemStage()
		{
			compClass = typeof(CompItemStage);
		}
	}
	public class CompItemStage : ThingComp
	{
		public CompProperties_ItemStage Props => (CompProperties_ItemStage)props;
		public string Group => Props.group;
		public IEnumerable<ItemStage> Stages => Props.stages;
		public Dictionary<string, ItemStage> StagesDic => Stages.ToDictionary(st => st.name);

		private string stageNameInt = null;

		public string StageName => stageNameInt;
		public ItemStage Stage => StagesDic.TryGetValue(StageName);
		public string StageLabelAccu => Stage.label;
		public string StageLabel => Stage.showLabel ? StageLabelAccu : null;
		public IEnumerable<string> StageTags => Stage.tags ?? Enumerable.Empty<string>();

		public bool SetStage(string stage)
		{
			if (stage == null || StagesDic.ContainsKey(stage))
			{
				stageNameInt = stage;
				return true;
			}
			return false;
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref stageNameInt, $"state_{Group}", null, false);
		}

		public override void PostPostMake()
		{
			SetStage(Stages.Any(st => st.weight > 0f) ? Stages.RandomElementByWeight(st => st.weight).name : null);
		}

		public override bool AllowStackWith(Thing other)
		{
			return other.TryGetState(Group, out string stage) && stage == StageName;
		}

		public override void PostSplitOff(Thing piece)
		{
			base.PostSplitOff(piece);
			piece.GetItemStageComps(Group).stageNameInt = stageNameInt;
		}

		public override string CompInspectStringExtra()
		{
			return (StageLabel ?? "") != "" ? $"{Props.groupLabelPrefix}{StageLabel.CapitalizeFirst()}" : null;
		}
		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			foreach (StatDrawEntry statDrawEntry in base.SpecialDisplayStats() ?? Enumerable.Empty<StatDrawEntry>())
			{
				yield return statDrawEntry;
			}
			yield return new StatDrawEntry(
					category: StatCategoryDefOf.Basics,
					label: Props.statEntryLabel,
					valueString: StageLabelAccu,
					reportText: Props.groupLabelPrefix + StageLabelAccu + (Stage.desc?.Any() ?? false ? "\n\n" + Stage.desc : null),
					displayPriorityWithinCategory: 0,
					overrideReportTitle: Props.statEntryDesc + "\n",
					hyperlinks: null
					);
		}

	}
	public class ItemStage
	{
		[NoTranslate]
		public string name;
		[MustTranslate]
		public string label;
		[MustTranslate]
		public string desc;
		public float weight = 0f;
		public bool showLabel = true;
		public List<string> tags;
	}
	public static class ItemStageUtility
	{
		public static IEnumerable<CompItemStage> GetItemStageComps(this Thing thing)
		{
			if (thing is ThingWithComps thingWithComps)
			{
				foreach (ThingComp comp in thingWithComps.AllComps)
				{
					if (comp is CompItemStage stage)
					{
						yield return stage;
					}
				}
			}
			yield break;
		}
		public static CompItemStage GetItemStageComps(this Thing thing, string group)
		{
			try
			{
				return thing.GetItemStageComps().First(comp => comp.Group == group);
			}
			catch (InvalidOperationException)
			{
				return null;
			}
		}
		public static bool AnyItemStageComps(this Thing thing)
		{
			return thing.GetItemStageComps().Any();
		}
		public static bool AnyItemStageLabels(this Thing thing)
		{
			return thing.AllStateLabels().Any();
		}
		public static string AllStateLabels(this Thing thing)
		{
			return string.Join(" ", thing.GetItemStageComps()?.Select(comp => comp.StageLabel?.CapitalizeFirst() ?? "")?.Where(str => str != "") ?? Enumerable.Empty<string>());
			/*if ((result ?? "") != "")
			{
				Log.Message($"Getting thing stages: {thing.LabelShort} with ({result})");
			}
			return result;*/
		}
		public static IEnumerable<string> AllStateTags(this Thing thing)
		{
			foreach (IEnumerable<string> strs in thing.GetItemStageComps()?.Select(comp => comp.StageTags) ?? Enumerable.Empty<IEnumerable<string>>())
			{
				foreach (string str in strs)
				{
					yield return str;
				}
			}
			yield break;
		}
		public static bool TryGetState(this Thing thing, string group, out string stage)
		{
			CompItemStage comp = GetItemStageComps(thing, group);
			if (comp != null)
			{
				stage = comp.StageName;
				return true;
			}
			else
			{
				stage = null;
				return false;
			}
		}
		public static bool TrySetState(this Thing thing, string group, string stage)
		{
			CompItemStage comp = GetItemStageComps(thing, group);
			if (comp != null)
			{
				comp.SetStage(stage);
				return true;
			}
			return false;
		}
	}
}
