/********************
 * 对AlienRace.AlienPartGenerator.BodyAddon的拓展
 * --siiftun1857
 */
using RimWorld;
using UnityEngine;
using Verse;
using AlienRace;
using static Explorite.ExploriteCore;

namespace Explorite
{
	///<summary>不在尸体白骨化情况下显示的部件。</summary>
	public class BodyAddon_MX : AlienPartGenerator.BodyAddon
	{
		public override bool CanDrawAddon(Pawn pawn)
		{
			//如果尸体已经风干了，则不再显示耳朵和尾巴等部位
			if (pawn.IsDessicated())
			{
				return false;
			}
			return base.CanDrawAddon(pawn);
		}
	}

	///<summary>对HAR的方法建立反射。</summary>
	[StaticConstructorOnStartup]
	internal static class HARReflection
	{
		static HARReflection()
		{
			GetAlienRaceCompColorFunc = GetAlienRaceCompColor;
		}
		private static bool GetAlienRaceCompColor(Thing thing, string channel, out Color? first, out Color? second)
		{
			if (thing.TryGetComp<AlienPartGenerator.AlienComp>() is AlienPartGenerator.AlienComp comp
			 && comp.GetChannel(channel) is AlienPartGenerator.ExposableValueTuple<Color, Color> biColor)
			{
				first = biColor.first;
				second = biColor.second;
				return true;
			}
			first = null;
			second = null;
			return true;
		}
	}
}
