/********************
 * 对人物生成器的补丁。
 * --siiftun1857
 */
using System;
using RimWorld;
using HarmonyLib;
using Verse;
using static Explorite.ExploriteCore;
using static Explorite.ExploritePawnApparelPredicate;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Explorite
{
	/// <summary>自动衣物规划。</summary>
	[StaticConstructorOnStartup]
	public static class ExploritePawnApparelPredicate
	{
		/*
		public static readonly IEnumerable<BodyPartGroupDef> CentaurBodyPartGroups = ((Func<IEnumerable<BodyPartGroupDef>>)(() =>
		{
			List<BodyPartGroupDef> pgs = new List<BodyPartGroupDef>();
			foreach (BodyPartRecord bodyPart in CentaurBodyDef.AllParts)
			{
				foreach (BodyPartGroupDef group in bodyPart.groups)
				{
					if (!pgs.Contains(group))
						pgs.Add(group);
				}
			}
			return pgs;
		}))();
		*/
		public class AutoApparelWhiteListRecord
		{
			public ThingDef targetRace;
			public bool ignoreTorso = true;
			public BodyPartGroupDef alentiveTorsoGroup = null;
			public string directAllowTag = null;

			private IEnumerable<BodyPartGroupDef> bodyGroups;
			private readonly Dictionary<ThingDef, bool> allowingRecord = new Dictionary<ThingDef, bool>();

			public AutoApparelWhiteListRecord(ThingDef thingDef) { targetRace = thingDef; _ = PartGroups; }

			public IEnumerable<BodyPartGroupDef> PartGroups
			{
				get
				{
					if (targetRace != null)
					{
						foreach (BodyPartGroupDef group in bodyGroups ??= bodyGroupsDatabase.TryGetValue(targetRace.race.body, fallback:
							((Func<IEnumerable<BodyPartGroupDef>>)(() =>
							{
								List<BodyPartGroupDef> pgs = new List<BodyPartGroupDef>();
								foreach (BodyPartRecord bodyPart in targetRace.race.body.AllParts)
								{
									foreach (BodyPartGroupDef group in bodyPart.groups)
									{
										if (!pgs.Contains(group))
											pgs.Add(group);
									}
								}
								bodyGroupsDatabase.Add(targetRace.race.body, pgs);
								return pgs;
							}))()))
						{
							yield return group;
						}
					}
					yield break;
				}
			}

			private bool VaildApparelPredicateInternal(ThingDef thing)
			{
				ApparelProperties apparel = thing?.apparel;
				if (apparel == null)
					return false;
				if (!directAllowTag.NullOrEmpty() && apparel?.tags?.Contains(directAllowTag) == true)
					return true;

				if (apparel.bodyPartGroups?.Any() == true)
				{
					bool torsoFound = false;
					bool altTorsoFound = false;
					foreach (BodyPartGroupDef bpg in apparel.bodyPartGroups)
					{
						if (alentiveTorsoGroup != null && bpg == alentiveTorsoGroup)
						{
							altTorsoFound = true;
						}
						if (ignoreTorso && bpg == BodyPartGroupDefOf.Torso)
						{
							torsoFound = true;
						}
						else if (!PartGroups.Contains(bpg))
						{
							return false;
						}
					}
					if (torsoFound && !altTorsoFound)
					{
						return false;
					}
				}
				return true;
			}

			/**
			 * <summary>
			 * 检测物品是否为合法的种族服装。
			 * </summary>
			 * <param name="thing">需要被检测的物品。</param>
			 * <returns>该物品是否为合法的服装。</returns>
			 */
			public bool VaildApparelPredicate(ThingDef thing)
			{
				if (allowingRecord.TryGetValue(thing, out bool result))
				{
					return result;
				}
				result = VaildApparelPredicateInternal(thing);
				allowingRecord.Add(thing, result);
				return result;
			}
		}

		public static readonly Dictionary<BodyDef, IEnumerable<BodyPartGroupDef>> bodyGroupsDatabase = new Dictionary<BodyDef, IEnumerable<BodyPartGroupDef>>();
		public static readonly List<AutoApparelWhiteListRecord> resolvedThingsDatabase = new List<AutoApparelWhiteListRecord>();
		public static Dictionary<ThingDef, AutoApparelWhiteListRecord> Database => resolvedThingsDatabase.ToDictionary(n => n.targetRace);

		/**
		 * <summary>
		 * 检测物品是否为合法的种族服装。
		 * </summary>
		 * <param name="race">需要被检测的种族。</param>
		 * <param name="thing">需要被检测的物品。</param>
		 * <param name="result">该物品是否为合法的服装。</param>
		 * <returns>是否执行了检测。</returns>
		 */
		public static bool VaildApparelPredicate(ThingDef race, ThingDef thing, out bool result)
		{
			if (Database.TryGetValue(race, out AutoApparelWhiteListRecord record))
			{
				result = record.VaildApparelPredicate(thing);
				return true;
			}
			else
			{
				result = false;
				return false;
			}
		}

		static ExploritePawnApparelPredicate()
		{
			foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
			{
				if ((thingDef?.tradeTags?.Contains("ExAutoApparelWhiteList") ?? false) && thingDef?.race?.body is BodyDef body)
				{
					resolvedThingsDatabase.Add(new AutoApparelWhiteListRecord(thingDef));
				}
			}

			if (Database.TryGetValue(AlienCentaurDef) is AutoApparelWhiteListRecord record1)
			{
				record1.directAllowTag = "CentaurOutfit";
				record1.alentiveTorsoGroup = CentaurTorsoGroupDef;
			}
			if (Database.TryGetValue(AlienSayersDef) is AutoApparelWhiteListRecord record2)
			{
				record2.directAllowTag = "SayersOutfit";
			}
		}


	}
	internal static partial class ExploritePatches
	{
		///<summary>添加默认服装方案。</summary>
		[HarmonyPostfix]
		public static void OutfitDatabaseGenerateStartingOutfitsPostfix(OutfitDatabase __instance)
		{
			foreach (AutoApparelWhiteListRecord record in Database.Select(x => x.Value))
			{
				Outfit outfit = __instance.MakeNewOutfit();
				outfit.label = record.targetRace.label;
				outfit.filter.SetDisallowAll();
				outfit.filter.SetAllow(SpecialThingFilterDefOf.AllowDeadmansApparel, allow: false);
				foreach (ThingDef allDef2 in DefDatabase<ThingDef>.AllDefs)
				{
					if (record.VaildApparelPredicate(allDef2))
					{
						outfit.filter.SetAllow(allDef2, allow: true);
					}
				}
			}
		}
		///<summary>自动判断服装是否适合种族。</summary>
		[HarmonyPostfix]
		public static void RaceRestrictionSettingsCanWearPostfix(ThingDef apparel, ThingDef race, ref bool __result)
		{
			if (VaildApparelPredicate(race, apparel, out bool result))
			{
				__result = result;
			}
		}
		///<summary>使半人马服装正确显示覆盖的部位。</summary>
		[HarmonyPrefix]
		public static void ApparelPropertiesGetCoveredOuterPartsStringPostfix(ApparelProperties __instance, ref BodyDef body)
		{
			if (__instance.tags.Contains("CentaurBodyFit"))
			{
				body = CentaurBodyDef;
			}
			else if (__instance.tags.Contains("SayersBodyFit"))
			{
				body = SayersBodyDef;
			}
		}
	}
}