/********************
 * 种族床的部件类。
 * --siiftun1857
 */
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
	public class CompProperties_AssignableToPawn_NoPostLoadSpecial : CompProperties_AssignableToPawn
	{
		public override void PostLoadSpecial(ThingDef parent) { }
	}
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
	///<summary>半人马床的类型，只有半人马可以使用。</summary>
	public class CompAssignableToPawn_Bed_Centaur : CompAssignableToPawn_Bed
	{
		//public override IEnumerable<Pawn> AssigningCandidates => base.AssigningCandidates.Where(p => p.def == AlienGuoguoDef);
		/*
		public override AcceptanceReport CanAssignTo(Pawn pawn)
		{
			if (pawn.def != AlienCentaurDef)
			{
				return "TooLargeForBed".Translate();
			}
			return base.CanAssignTo(pawn);
		}
		*/
	}

}
