using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using static Explorite.ExploriteCore;
using System;
using System.Text;

namespace Explorite
{
	/// <summary>Sayers在独自一人时会感觉孤独(来自Abrel，她摸索的时候不小心一头撞在了墙壁上)</summary>
	public sealed class ThoughtWorker_SayersWillFeelLonely : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (p.def == AlienSayersDef && !(p?.Map?.mapPawns?.AllPawns?.Any(pawn => pawn != p && (pawn.Faction == null || pawn.Faction == p.Faction) && pawn.def == AlienSayersDef) ?? false))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
