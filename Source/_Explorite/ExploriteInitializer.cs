/********************
 * 包含StaticConstructorOnStartup的初始化内容。
 * --siiftun1857
 */
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace Explorite
{
	[StaticConstructorOnStartup]
	public static partial class ExploriteCore
	{
		static ExploriteCore()
		{
			Log.Message("[Explorite]Is it working? Did you see me? OuO");

			/*
			Log.Message($"[Explorite]Acticed mods:"
				+ (InstelledMods.RimCentaurs ? " RimCentaurs" : null)
				+ (InstelledMods.Sayers ? " Sayers" : null)
				+ (InstelledMods.GuoGuo ? " GuoGuo" : null)
				+ (InstelledMods.SoS2 ? " SoS2" : null)
				+ "."
					);
			*/

			//LoadedModManager
			//InstelledMods.UpdateStatus();

			try
			{
				List<string> mods = new List<string>();
				foreach (MethodInfo method in typeof(InstelledMods).GetMethods().Where(m => m.DeclaringType == typeof(InstelledMods)))
				{
					if (method.Invoke(null, null) is bool enabled && enabled)
					{
						mods.Add(method.Name.StartsWith("get_") ? method.Name.Substring("get_".Length) : method.Name);
					}
				}
				Log.Message($"[Explorite]Acticed mods: {string.Join(" ", mods)}.");
			}
			catch (Exception e)
			{
				Log.Error(string.Concat(
					$"[Explorite]an exception ({e.GetType().Name}) occurred during pawn mods gethering.\n",
					$"Message:\n   {e.Message}\n",
					$"Stack Trace:\n{e.StackTrace}\n"
					));
			}
		}
	}
}
