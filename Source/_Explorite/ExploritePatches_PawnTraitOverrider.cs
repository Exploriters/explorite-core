/********************
 * 强制特征系统。
 * --siiftun1857
 */
using System;
using RimWorld;
using HarmonyLib;
using Verse;
using static Explorite.ExploriteCore;
using static Explorite.ExploriteTraitOverriderDatabase;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Explorite
{
	/// <summary>强制特征系统。</summary>
	[StaticConstructorOnStartup]
	public static class ExploriteTraitOverriderDatabase
	{
		public static readonly Dictionary<ThingDef, Dictionary<TraitDef, int>> database = new Dictionary<ThingDef, Dictionary<TraitDef, int>>();
		static ExploriteTraitOverriderDatabase()
		{
			AddRecord(AlienCentaurDef, TraitDefOf.Masochist, 0);
			AddRecord(AlienCentaurDef, TraitDefOf.Industriousness, 0);
			AddRecord(AlienCentaurDef, TraitDefOf.Kind, 0);
			AddRecord(AlienCentaurDef, TraitDefOf.Asexual, 0);
			AddRecord(AlienCentaurDef, TraitDefOf.DrugDesire, -1);
			AddRecord(AlienCentaurDef, TraitDefOf.Transhumanist, 2);
			AddRecord(AlienSayersDef, TraitDefOf.Asexual, 0);
		}

		/**
		 * <summary>
		 * 添加特征为强制特征。
		 * </summary>
		 * <param name="races">需要被添加强制特征的种族。</param>
		 * <param name="tDef">需要被添加的特征。</param>
		 * <param name="degree">需要被添加的特征程度。</param>
		 */
		public static void AddRecord(ThingDef races, TraitDef tDef, int degree)
		{
			Dictionary<TraitDef, int> traits;
			if (database.TryGetValue(races, out Dictionary<TraitDef, int> traitsInDatabase))
			{
				traits = traitsInDatabase;
			}
			else
			{
				traits = new Dictionary<TraitDef, int>();
				database.Add(races, traits);
			}
			traits.SetOrAdd(tDef, degree);
		}

		/**
		 * <summary>
		 * 检测特征是否为强制特征。
		 * </summary>
		 * <param name="races">需要被检测特征的种族。</param>
		 * <param name="tDef">需要被检测的特征。</param>
		 * <param name="degree">需要被检测的特征程度。</param>
		 * <returns>该特征是否为强制特征。</returns>
		 */
		public static bool OverrideTraitPredicate(ThingDef races, TraitDef tDef, int? degree = null)
		{
			return OverrideTraitPredicate(races, tDef, out int tergetDegree) && (!degree.HasValue || degree == tergetDegree);
		}
		/**
		 * <summary>
		 * 检测特征是否为强制特征。
		 * </summary>
		 * <param name="races">需要被检测特征的种族。</param>
		 * <param name="tDef">需要被检测的特征。</param>
		 * <param name="degree">需要被检测的特征程度。</param>
		 * <returns>该特征是否为强制特征。</returns>
		 */
		public static bool OverrideTraitPredicate(ThingDef races, TraitDef tDef, out int degree)
		{
			degree = 0;
			return database.TryGetValue(races)?.TryGetValue(tDef, out degree) ?? false;
		}
	}
	internal static partial class ExploritePatches
	{

		///<summary>使强制特征物种始终被视为具有特征等级。</summary>
		[HarmonyPostfix]
		public static void TraitSetDegreeOfTraitPostfix(TraitSet __instance, ref int __result, TraitDef tDef)
		{
			if (__instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn)
			{
				if (OverrideTraitPredicate(pawn.def, tDef, out int degree))
				{
					__result = degree;
				}
			}
		}
		///<summary>使强制特征物种始终被视为具有特征。</summary>
		[HarmonyPostfix]
		public static void TraitSetHasTraitPostfix(TraitSet __instance, ref bool __result, TraitDef tDef)
		{
			if (__result == false
				&& __instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn)
			{
				if (OverrideTraitPredicate(pawn.def, tDef))
				{
					__result = true;
				}
			}
		}
		///<summary>使强制特征物种始终被视为具有特征等级。</summary>
		[HarmonyPostfix]
		public static void TraitSetHasTraitDegreePostfix(TraitSet __instance, ref bool __result, TraitDef tDef, int degree)
		{
			if (__result == false
				&& __instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn)
			{
				if (OverrideTraitPredicate(pawn.def, tDef, degree))
				{
					__result = true;
				}
			}
		}
		///<summary>为强制特征物种特征制作样本。</summary>
		[HarmonyPostfix]
		public static void TraitSetGetTraitPostfix(TraitSet __instance, ref Trait __result, TraitDef tDef)
		{
			if (__result == null
				&& __instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn)
			{
				if (OverrideTraitPredicate(pawn.def, tDef, out int degree))
				{
					__result = new Trait(tDef, degree) { pawn = pawn };
				}
			}
		}
		///<summary>为强制特征物种特征等级制作样本。</summary>
		[HarmonyPostfix]
		public static void TraitSetGetTraitDegreePostfix(TraitSet __instance, ref Trait __result, TraitDef tDef, int degree)
		{
			if (__result == null
				&& __instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn)
			{
				if (OverrideTraitPredicate(pawn.def, tDef, degree))
				{
					__result = new Trait(tDef, degree) { pawn = pawn };
				}
			}
		}
	}
}