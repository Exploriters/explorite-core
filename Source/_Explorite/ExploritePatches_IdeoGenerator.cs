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

		internal static bool GenerateIdeoCentaurPostprocess(ref Ideo ideo, IdeoGenerationParms parms, ref bool matchError)
		{
			if (!InstelledMods.RimCentaurs)
			{
				return false;
			}
			if (matchError)
			{
				return false;
			}
			if (ideo.memes.Contains(CentaurStructureMemeDef))
			{
				if (parms.forFaction != CentaurPlayerColonyDef)
				{
					matchError = true;
					return false;
				}
			}
			else
			{
				return false;
			}
			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			foreach (Precept precept in ideo.PreceptsListForReading.ToList())
			{
				if (precept is Precept_Building || precept is Precept_Weapon || (precept is Precept_Ritual && precept.def.visible && precept.def != PreceptDefOf.Funeral))
				{
					ideo.RemovePrecept(precept);
				}
				if (precept is Precept_Role preceptRole)
				{
					preceptRole.ApparelRequirements.Clear();
				}
			}

			return true;
		}
		internal static bool GenerateIdeoSayersPostprocess(ref Ideo ideo, IdeoGenerationParms parms, ref bool matchError)
		{
			if (!InstelledMods.RimCentaurs)
			{
				return false;
			}
			if (matchError)
			{
				return false;
			}
			if (ideo.memes.Contains(SayersStructureMemeDef))
			{
				if (parms.forFaction != SayersPlayerColonyDef)
				{
					matchError = true;
					return false;
				}
			}
			else
			{
				return false;
			}
			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			foreach (Precept precept in ideo.PreceptsListForReading.ToList())
			{
				if (precept is Precept_Building || precept is Precept_Weapon || (precept is Precept_Ritual && precept.def.visible && precept.def != PreceptDefOf.Funeral))
				{
					ideo.RemovePrecept(precept);
				}
				if (precept is Precept_Role preceptRole)
				{
					preceptRole.ApparelRequirements.Clear();
				}
			}

			return true;
		}
		internal static bool GenerateIdeoGuoguoPostprocess(ref Ideo ideo, IdeoGenerationParms parms, ref bool matchError)
		{
			if (!InstelledMods.GuoGuo)
			{
				return false;
			}
			if (matchError)
			{
				return false;
			}
			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			return false;
		}
		internal static bool GenerateIdeoDeerfoxPostprocess(ref Ideo ideo, IdeoGenerationParms parms, ref bool matchError)
		{
			if (!InstelledMods.GuoGuo)
			{
				return false;
			}
			if (matchError)
			{
				return false;
			}
			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			return false;
		}

		internal static bool GenerateIdeoFinalPostprocess(ref Ideo ideo, IdeoGenerationParms parms)
		{
			if (ideo == null)
			{
				return false;
			}

			try
			{

			}
			catch (NullReferenceException e)
			{
				Log.Message(string.Concat(
					$"[Explorite]an exception ({e.GetType().Name}) occurred during ideo generating.\n",
					$"Message:\n   {e.Message}\n",
					$"Stack Trace:\n{e.StackTrace}\n"
					));
			}
			catch (Exception e)
			{
				Log.Error(string.Concat(
					$"[Explorite]an exception ({e.GetType().Name}) occurred during ideo generating.\n",
					$"Message:\n   {e.Message}\n",
					$"Stack Trace:\n{e.StackTrace}\n"
					));
			}

			return true;
		}


		/**
		  * <summary>
		  * 对人物生成器的补丁。
		  * </summary>
		  */
		[HarmonyPostfix]
		public static void GenerateIdeoPostfix(ref Ideo __result, IdeoGenerationParms parms)
		{
			bool matchError = false;
			GenerateIdeoCentaurPostprocess(ref __result, parms, ref matchError);
			GenerateIdeoSayersPostprocess(ref __result, parms, ref matchError);
			GenerateIdeoGuoguoPostprocess(ref __result, parms, ref matchError);
			GenerateIdeoDeerfoxPostprocess(ref __result, parms, ref matchError);

			if (matchError)
			{
				__result = IdeoGenerator.GenerateTutorialIdeo();
				matchError = false;
			}

			GenerateIdeoFinalPostprocess(ref __result, parms);
		}
	}

	/*
	[HarmonyPatch(typeof(PawnGenerator), "GenerateBodyType")]
	public static class Patch_PawnGenerator_GenerateBodyType
	{
		public static bool GenerateBodyTypePostfix(Pawn pawn)
		{
			BodyTypeDef bodyGlobal;
			BodyTypeDef bodyMale;
			BodyTypeDef bodyFemale;
			float randBodyType;
			if (pawn.story.adulthood != null)
			{
				bodyGlobal = pawn.story.adulthood.BodyTypeFor(Gender.None);
				bodyMale = pawn.story.adulthood.BodyTypeFor(Gender.Male);
				bodyFemale = pawn.story.adulthood.BodyTypeFor(Gender.Female);
			}
			else
			{
				bodyGlobal = pawn.story.childhood.BodyTypeFor(Gender.None);
				bodyMale = pawn.story.childhood.BodyTypeFor(Gender.Male);
				bodyFemale = pawn.story.childhood.BodyTypeFor(Gender.Female);
			}
			if (bodyGlobal != null)
			{
				pawn.story.bodyType = bodyGlobal;
			}
			else if ((bodyMale == BodyTypeDefOf.Male && pawn.gender == Gender.Male)
			  || (bodyFemale == BodyTypeDefOf.Female && pawn.gender == Gender.Female))
			{
				randBodyType = Rand.Value;
				if (randBodyType < 0.05)
				{
					pawn.story.bodyType = BodyTypeDefOf.Hulk;
				}
				else if (randBodyType < 0.1)
				{
					pawn.story.bodyType = BodyTypeDefOf.Fat;
				}
				else if (randBodyType < 0.5)
				{
					pawn.story.bodyType = BodyTypeDefOf.Thin;
				}
				else if (pawn.gender == Gender.Female)
				{
					pawn.story.bodyType = BodyTypeDefOf.Female;
				}
				else
				{
					pawn.story.bodyType = BodyTypeDefOf.Male;
				}
			}
			else if (pawn.gender == Gender.Female)
			{
				pawn.story.bodyType = bodyFemale;
			}
			else
			{
				pawn.story.bodyType = bodyMale;
			}
			return false;
		}
	}
	*/

}
