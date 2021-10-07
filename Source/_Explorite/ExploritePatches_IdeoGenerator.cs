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
			if (___ideo.memes.Contains(CentaurStructureMemeDef) || ___ideo.memes.Contains(SayersStructureMemeDef))
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
