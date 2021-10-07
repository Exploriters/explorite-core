/********************
 * 对文化配置器的补丁。
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
		public static bool IsSpecFac(FactionDef factionDef)
		{
			return factionDef == CentaurPlayerColonyDef || factionDef == SayersPlayerColonyDef || factionDef == DeerFoxPlayerColonyDef;
		}
		public static bool SpecPFacInGame()
		{
			return Find.FactionManager.AllFactions.Any(fac => IsSpecFac(fac.def));
		}
	}
}
