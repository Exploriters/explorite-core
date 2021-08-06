/********************
 * 包含StaticConstructorOnStartup的初始化内容。
 * --siiftun1857
 */
using System.Collections.Generic;
using Verse;

namespace Explorite
{
	[StaticConstructorOnStartup]
	public static partial class ExploriteCore
	{
		static ExploriteCore()
		{
			Log.Message("[Explorite]Is it working? Did you see me? OuO");

			Log.Message($"[Explorite]Acticed mods:"
				+ (InstelledMods.RimCentaurs ? " RimCentaurs" : null)
				+ (InstelledMods.Sayers ? " Sayers" : null)
				+ (InstelledMods.GuoGuo ? " GuoGuo" : null)
				+ (InstelledMods.SoS2 ? " SoS2" : null)
				+ "."
					);

			//LoadedModManager
			//InstelledMods.UpdateStatus();
		}
	}
}
