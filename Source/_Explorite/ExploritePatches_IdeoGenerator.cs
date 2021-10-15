/********************
 * 对文化生成器的补丁。
 * --siiftun1857
 */
using System;
using RimWorld;
using HarmonyLib;
using Verse;
using static Explorite.ExploriteCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;

namespace Explorite
{
	internal static partial class ExploritePatches
	{
		///<summary>对文化戒律初始化的补丁。</summary>
		[HarmonyPostfix]public static void IdeoFoundationRandomizePreceptsPostfix(Ideo ___ideo, bool init, IdeoGenerationParms parms)
		{
			if (___ideo.IsCentaursIdeo() || ___ideo.IsSayersIdeo() || ___ideo.IsDeerFoxIdeo())
			{
				foreach (Precept precept in ___ideo.PreceptsListForReading.ToList())
				{
					if ((
						precept is Precept_Ritual precept_Ritual && precept.def.visible
						&& (precept_Ritual.isAnytime || precept_Ritual.obligationTriggers.OfType<RitualObligationTrigger_Date>().FirstOrDefault<RitualObligationTrigger_Date>() != null)
						)
					 || precept is Precept_Apparel
					 || precept is Precept_Relic
					 || precept is Precept_RitualSeat
					 || precept is Precept_Building
					 || precept is Precept_Weapon
					 || precept is Precept_Animal
					 )
					{
						___ideo.RemovePrecept(precept);
					}
					if (precept is Precept_Role preceptRole)
					{
						preceptRole.ApparelRequirements.Clear();
					}
				}
			}
			if (___ideo.IsDeerFoxIdeo())
			{
				List<ThingDef> thingDefs = new List<ThingDef>{ 
					DefDatabase<ThingDef>.GetNamed("Fox_Fennec"),
					DefDatabase<ThingDef>.GetNamed("Fox_Red"),
					DefDatabase<ThingDef>.GetNamed("Fox_Arctic"),
					DefDatabase<ThingDef>.GetNamed("Deer"),
					DefDatabase<ThingDef>.GetNamed("Caribou"),
					DefDatabase<ThingDef>.GetNamed("Elk"),
				};
				foreach (ThingDef thingDef in thingDefs)
				{
					Precept_Animal precept = PreceptMaker.MakePrecept(PreceptDefOf.AnimalVenerated) as Precept_Animal;
					___ideo.AddPrecept(precept);
					precept.ThingDef = thingDef;
				}
			}
		}
		///<summary>对职位服装预设的补丁。</summary>
		[HarmonyPrefix]public static bool PreceptRoleGenerateNewApparelRequirementsPrefix(ref List<PreceptApparelRequirement> __result, FactionDef generatingFor)
		{
			if (generatingFor == CentaurPlayerColonyDef || generatingFor == SayersPlayerColonyDef)
			{
				__result = Enumerable.Empty<PreceptApparelRequirement>().ToList();
				return false;
			}
			return true;
		}
	}
}
