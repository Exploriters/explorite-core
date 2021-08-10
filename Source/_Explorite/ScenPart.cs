/********************
 * 该文件包含多个剧本部件。
 * --siiftun1857
 */
using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
	///<summary>不具有人物编辑界面的起始人物剧本部件。</summary>
	public class ScenPart_ConfigPage_ConfigureStartingPawns_NoDoEditInterface : ScenPart_ConfigPage_ConfigureStartingPawns
	{
		public override void DoEditInterface(Listing_ScenEdit listing)
		{
		}
	}
	public class ScenPart_ConfigPage_ConfigureStartingPawns_Solo : ScenPart_ConfigPage_ConfigureStartingPawns
	{
		private string pawnCountBuffer;

		private string pawnCountChoiceBuffer;

		public override void DoEditInterface(Listing_ScenEdit listing)
		{
			Rect scenPartRect = listing.GetScenPartRect(this, RowHeight * 2f);
			scenPartRect.height = RowHeight;
			Text.Anchor = TextAnchor.UpperRight;
			Rect rect = new Rect(scenPartRect.x - 200f, scenPartRect.y + RowHeight, 200f, RowHeight);
			rect.xMax -= 4f;
			Widgets.Label(rect, "ScenPart_StartWithPawns_OutOf".Translate());
			Text.Anchor = TextAnchor.UpperLeft;
			Widgets.TextFieldNumeric(scenPartRect, ref pawnCount, ref pawnCountBuffer, 1f, 10f);
			scenPartRect.y += RowHeight;
			Widgets.TextFieldNumeric(scenPartRect, ref pawnChoiceCount, ref pawnCountChoiceBuffer, pawnCount, 10f);
		}
	}

	///<summary>向开局的空降仓内塞入Sayers粘液。</summary>
	public class ScenPart_ScatteredGarbage : ScenPart
	{
		public override void PostGameStart()
		{
			base.PostGameStart();
			List<Thing> Things = Find.CurrentMap.listerThings.AllThings.Where(thing => thing is DropPodIncoming).ToList();
			foreach (Thing thing in Things)
			{
				if (thing is DropPodIncoming droppod)
				{
					ThingDef Filth_SayersMucus = DefDatabase<ThingDef>.GetNamed("Filth_SayersMucus");
					droppod.Contents.innerContainer
						.TryAdd(
						ThingMaker.MakeThing(Filth_SayersMucus)
						, 3
						);
				}
			}
		}
	}

	///<summary>Sayers开局情绪低落(来自Abrel，她正在尝试这个)</summary>
	public class ScenPart_StartThoughtSadness : ScenPart
	{
		public override void PostGameStart()
		{
			base.PostGameStart();
			foreach (List<Thought_Memory> memories in Find.GameInitData.startingAndOptionalPawns.Where(pawn => pawn.def == AlienSayersDef).Select(pawn => pawn.needs.mood.thoughts.memories.Memories))
			{
				memories.RemoveAll(memory => memory.def == ThoughtDefOf.NewColonyOptimism);
				memories.Add((Thought_Memory)ThoughtMaker.MakeThought(SayersAbandonedSadnessThoughtDef));
			}
		}
	}

	///<summary>填满开局的所有电池。</summary>
	[Obsolete]
	public class ScenPart_FillBattery : ScenPart
	{
		//ModContentPack.PatchOperationFindMod
		public override string Summary(Scenario scen) => "Magnuassembly_ScenPart_FillBattery_StaticSummary".Translate();
		private static void ProcessBattery(Thing battery, ref bool once)
		{
			try
			{
				battery?.TryGetComp<CompPowerBattery>()?.AddEnergy(float.PositiveInfinity);
				if (battery is Building_TriBattery secretBattery && secretBattery.CanEverHaveTrishot() && !GameComponentCentaurStory.Any())
				{
					secretBattery.SetSecret(true);
					GameComponentCentaurStory.TryAdd(secretBattery);
					once = true;
				}
			}
			catch
			{ }
		}
		public override void PostGameStart()
		{
			base.PostGameStart();
			List<Thing> things = Find.CurrentMap.listerThings.AllThings.Where(thing => thing is DropPodIncoming || thing is MinifiedThing).ToList();
			foreach (Thing thing in things)
			{
				//不处理世界中散落的电池
				/* if (thing.TryGetComp<CompPowerBattery>() != null)
				{
					thing?.TryGetComp<CompPowerBattery>()?.AddEnergy(float.PositiveInfinity);
				}*/
				bool once = Faction.OfPlayer.def.basicMemberKind.race != AlienCentaurDef;
				if (thing is MinifiedThing minifiedThing)
				{
					Thing thingInside = minifiedThing.InnerThing;
					if (thingInside.TryGetComp<CompPowerBattery>() != null)
					{
						ProcessBattery(thingInside, ref once);
					}
				}
				if (thing is DropPodIncoming droppod)
				{
					foreach (Thing thing3 in droppod.Contents.innerContainer)
					{
						if (thing3.TryGetComp<CompPowerBattery>() != null)
						{
							ProcessBattery(thing3, ref once);
						}
						if (thing3 is MinifiedThing minifiedThing2)
						{
							Thing thingInside = minifiedThing2.InnerThing;
							if (thingInside.TryGetComp<CompPowerBattery>() != null)
							{
								ProcessBattery(thingInside, ref once);
							}
						}
					}
				}
				if (!once)
				{
					Log.Warning("[Explorite]Warning, TriBattery not found.");
				}
			}

			/*foreach (Letter letter in Find.LetterStack.LettersListForReading)
			{
				Find.LetterStack.RemoveLetter(letter);
			}*/
			return;
		}
	}
	///<summary>阻止空投舱产生钢渣块。</summary>
	public class ScenPart_WipeoutChunkSlag : ScenPart
	{
		public override string Summary(Scenario scen) => "Magnuassembly_ScenPart_WipeoutChunkSlag_StaticSummary".Translate();
		public override void PostMapGenerate(Map map)
		{
			base.PostMapGenerate(map);
			if (Find.GameInitData == null)
			{
				return;
			}
			List<Thing> things = map.listerThings.AllThings.Where(thing => thing is DropPodIncoming).ToList();
			foreach (Thing thing in things)
			{
				if (thing is DropPodIncoming droppod)
				{
					droppod.Contents.leaveSlag = false;
				}
			}
			return;
		}
	}
	///<summary>解开空投舱内的打包物品，并且会被直接部署为建筑物。</summary>
	[Obsolete]
	public class ScenPart_UnpackMinified : ScenPart
	{
		public override string Summary(Scenario scen) => "Magnuassembly_ScenPart_UnpackMinified_StaticSummary".Translate();
		public override void PostGameStart()
		{
			base.PostGameStart();
			foreach (Thing thing in Find.CurrentMap.listerThings.AllThings.Where(thing => thing is DropPodIncoming))
			{
				if (thing is DropPodIncoming droppod)
				{
					foreach (Thing thing2 in droppod.Contents.innerContainer.Where(thing => thing is MinifiedThing))
					{
						if (thing2 is MinifiedThing minifiedThing && minifiedThing.InnerThing != null)
						{
							minifiedThing.InnerThing.SetFaction(Faction.OfPlayer);
							minifiedThing.GetDirectlyHeldThings().TryTransferAllToContainer(droppod.Contents.innerContainer);
							minifiedThing.Destroy();
						}
					}
				}
			}
			return;
		}
	}
	///<summary>将物品塞入殖民者的背包中。</summary>
	[Obsolete]
	public class ScenPart_DumpThingsToPawnInv : ScenPart
	{
		public override string Summary(Scenario scen) => "Magnuassembly_ScenPart_DumpThingsToPawnInv_StaticSummary".Translate();
		protected struct ThingAndOwner
		{
			public Thing thing;
			public ThingOwner thingOwner;
			public ThingAndOwner(Thing thing, ThingOwner thingOwner)
			{
				this.thing = thing;
				this.thingOwner = thingOwner;
			}
		}
		public override void PostGameStart()
		{
			base.PostGameStart();
			List<ThingAndOwner> queuedThings = new List<ThingAndOwner>();
			Pawn target = null;

			foreach (Thing thingInWorld in Find.CurrentMap.listerThings.AllThings.Where(thing => thing is DropPodIncoming))
			{
				if (thingInWorld is DropPodIncoming droppod)
				{
					foreach (Thing thingInDroppod in droppod.Contents.innerContainer)
					{
						if (target == null &&
							thingInDroppod is Pawn pawnInDroppod &&
							pawnInDroppod?.def?.race?.Humanlike == true)
						{
							target = pawnInDroppod;
						}
						else if (thingInDroppod?.def?.alwaysHaulable == true)
						{
							queuedThings.Add(new ThingAndOwner(thingInDroppod, droppod.Contents.innerContainer));
						}
					}
				}
			}
			if (target != null)
			{
				bool equipped = target.equipment.Primary != null;
				foreach (ThingAndOwner tno in queuedThings)
				{
					if (tno.thing.TryGetComp<CompForbiddable>() != null)
						tno.thing.TryGetComp<CompForbiddable>().Forbidden = false;

					if (!equipped && tno.thing.TryGetComp<CompEquippable>() != null)
					{
						target.equipment.AddEquipment((ThingWithComps)tno.thingOwner.Take(tno.thing));
					}
					else
						tno.thingOwner.TryTransferToContainer(tno.thing, target.inventory.innerContainer);
				}
			}
			else
			{
				Log.Error("[Explorite]Null target pawn!");
			}
			return;
		}
	}

	///<summary>玩家初始角色开局资源。</summary>
	public class ScenPart_StartingPawnThings : ScenPart_ThingCount
	{
		public const string PlayerStartWithTag = "ExPlayerPawnStartsWith";
		public static string PlayerStartWithIntro => "Magnuassembly_ScenPart_StartingPawnThings_Summary".Translate();

		// Token: 0x060061F1 RID: 25073 RVA: 0x00215E24 File Offset: 0x00214024
		public override string Summary(Scenario scen)
		{
			return ScenSummaryList.SummaryWithList(scen, PlayerStartWithTag, ScenPart_StartingThing_Defined.PlayerStartWithIntro);
		}
		public override IEnumerable<string> GetSummaryListEntries(string tag)
		{
			if (tag == PlayerStartWithTag)
			{
				yield return GenLabel.ThingLabel(thingDef, stuff, count).CapitalizeFirst();
			}
			yield break;
		}

		public override void Notify_PawnGenerated(Pawn pawn, PawnGenerationContext context, bool redressed)
		{
			if (HasNullDefs())
			{
				return;
			}
			if (!PawnGenerationContext.PlayerStarter.Includes(context))
			{
				return;
			}
			Thing thing = ThingMaker.MakeThing(thingDef, stuff);
			if (thingDef.IsIngestible && thingDef.ingestible.IsMeal)
			{
				FoodUtility.GenerateGoodIngredients(thing, Faction.OfPlayer.ideos.PrimaryIdeo);
			}
			if (thing?.TryGetComp<CompPowerBattery>() is CompPowerBattery compPowerBattery)
			{
				compPowerBattery.AddEnergy(float.PositiveInfinity);
			}
			if (thing is Building_TriBattery secretBattery && secretBattery.CanEverHaveTrishot())
			{
				GameComponentCentaurStory.ClearSecret();
				secretBattery.SetSecret(true);
				GameComponentCentaurStory.TryAdd(secretBattery);
			}
			thing.stackCount = count;
			if (thingDef.Minifiable)
			{
				thing = thing.MakeMinified();
			}
			if (pawn.equipment.Primary == null && thing.TryGetComp<CompEquippable>() != null)
			{
				pawn.equipment.AddEquipment(thing as ThingWithComps);
			}
			else
			{
				pawn.inventory.GetDirectlyHeldThings().TryAddOrTransfer(thing);
			}
		}
	}
}
