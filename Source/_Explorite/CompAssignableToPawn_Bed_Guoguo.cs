/********************
 * 果果床的部件类。
 * --siiftun1857
 */
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
	///<summary>果果床的类型，只有果果人可以使用。</summary>
	public class CompAssignableToPawn_Bed_Guoguo : CompAssignableToPawn_Bed
	{
		//public override IEnumerable<Pawn> AssigningCandidates => base.AssigningCandidates.Where(p => p.def == AlienGuoguoDef);
		public override AcceptanceReport CanAssignTo(Pawn pawn)
		{
			if (pawn.def != AlienGuoguoDef)
			{
				return "TooLargeForBed".Translate();
			}
			return base.CanAssignTo(pawn);
		}
	}

}
