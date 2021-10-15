/********************
 * 使生物具有该效果时，在死亡后生成血肉之树。
 * 
 * --siiftun1857
 */
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
using static Explorite.ExploriteCore;

namespace Explorite
{
	///<summary>为<see cref = "HediffComp_FleshSeedParasitized" />接收参数。</summary>
	public class HediffCompProperties_FleshSeedParasitized : HediffCompProperties_DissolveGearOnDeath
	{
		public HediffCompProperties_FleshSeedParasitized()
		{
			compClass = typeof(HediffComp_FleshSeedParasitized);
		}
	}
	///<summary>使生物具有该效果时，在死亡后生成血肉之树。</summary>
	public class HediffComp_FleshSeedParasitized : HediffComp
	{
		public HediffCompProperties_FleshSeedParasitized Props => (HediffCompProperties_FleshSeedParasitized)props;
		public override void Notify_PawnDied()
		{
			base.Notify_PawnDied();
			Corpse corpse = Pawn?.Corpse;
			CompRottable rott = corpse.TryGetComp<CompRottable>();
			//float corpseNutrition = corpse.GetStatValue(StatDefOf.Nutrition);

			corpse.Strip();

			List<BodyPartRecord> parts =
				Pawn.RaceProps.body.corePart.parts.Where((BodyPartRecord part) => part.coverageAbs > 0f && !Pawn.health.hediffSet.PartIsMissing(part)).ToList();
			//parts.Add(Pawn.RaceProps.body.corePart);
			//foreach (BodyPartRecord part in Pawn.RaceProps.body.AllParts.Where((BodyPartRecord part) => !part.IsCorePart && part.coverageAbs > 0f && !Pawn.health.hediffSet.PartIsMissing(part)))
			foreach (BodyPartRecord part in parts)
			{
				Hediff hediff = HediffMaker.MakeHediff(InjectionHediffDef, Pawn, part);

				hediff.Severity = part.def.hitPoints * 20;
				Pawn.health.AddHediff(hediff, part, new DamageInfo(
						InjectionDamageDef, part.def.hitPoints * 20,
						armorPenetration: 2f,
						instigator: Pawn,
						weapon: Pawn.def,
						hitPart: part
						));
				/*Pawn.TakeDamage(
					new DamageInfo(
						InjectionDamageDef, part.def.hitPoints * 20,
						armorPenetration: 2f,
						instigator: Pawn,
						weapon: Pawn.def,
						hitPart: part
						)
				);*/
			}
			foreach (Hediff hediff in Pawn.health.hediffSet.hediffs)
			{
				if (hediff is Hediff_MissingPart hediffMissingPart)
				{
					hediffMissingPart.lastInjury = InjectionHediffDef;
				}
			}
			if (rott != null &&
				rott.RotProgress < rott.PropsRot.TicksToDessicated)
			{
				rott.RotProgress = rott.PropsRot.TicksToDessicated;
			}
			Plant thingTree = (Plant)ThingMaker.MakeThing(FleshTreeDef);
			GenSpawn.Spawn(thingTree, corpse.Position, corpse.Map, WipeMode.FullRefund);

			corpse.DeSpawn();
			thingTree.TryGetComp<CompThingHolder>().innerContainer.TryAdd(corpse);
		}

		public override void Notify_PawnKilled()
		{
			base.Notify_PawnKilled();
			//base.Pawn.equipment.DestroyAllEquipment();
			//base.Pawn.apparel.DestroyAll();

			Pawn.Strip();
			if (!Pawn.Spawned)
			{
				return;
			}
			if (Props.mote != null)
			{
				Vector3 drawPos = Pawn.DrawPos;
				for (int i = 0; i < Props.moteCount; i++)
				{
					Vector2 vector = Rand.InsideUnitCircle * Props.moteOffsetRange.RandomInRange * Rand.Sign;
					MoteMaker.MakeStaticMote(new Vector3(drawPos.x + vector.x, drawPos.y, drawPos.z + vector.y), Pawn.Map, Props.mote);
				}
			}
			if (Props.fleck != null)
			{
				Vector3 drawPos = Pawn.DrawPos;
				for (int i = 0; i < Props.moteCount; i++)
				{
					Vector2 vector = Rand.InsideUnitCircle * Props.moteOffsetRange.RandomInRange * Rand.Sign;
					FleckMaker.Static(new Vector3(drawPos.x + vector.x, drawPos.y, drawPos.z + vector.y), Pawn.Map, Props.fleck);
				}
			}
			if (Props.filth != null)
			{
				FilthMaker.TryMakeFilth(Pawn.Position, Pawn.Map, Props.filth, Props.filthCount);
			}
			if (Props.sound != null)
			{
				Props.sound.PlayOneShot(SoundInfo.InMap(Pawn));
			}
		}
	}

}
