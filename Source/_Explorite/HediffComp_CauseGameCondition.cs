/********************
 * 造成游戏状态的健康状态。
 * --siiftun1857
 */
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Explorite
{
	///<summary>为<see cref = "HediffComp_CauseGameCondition" />接收参数。</summary>
	public class HediffCompProperties_CausesGameCondition : HediffCompProperties
	{
		public HediffCompProperties_CausesGameCondition()
		{
			compClass = typeof(HediffComp_CauseGameCondition);
		}

		public GameConditionDef conditionDef;

		public int worldRange;

		public bool preventConditionStacking = true;
	}
	///<summary>造成游戏状态的健康状态。</summary>
	public class HediffComp_CauseGameCondition : HediffComp
	{
		public HediffCompProperties_CausesGameCondition Props => (HediffCompProperties_CausesGameCondition)props;

		public GameConditionDef ConditionDef => Props.conditionDef;

		public IEnumerable<GameCondition> CausedConditions => causedConditions.Values;

		public bool Active => initiatableComp == null || initiatableComp.Initiated;

		public int MyTile
		{
			get
			{
				if (siteLink != null)
				{
					return siteLink.Tile;
				}
				if (Pawn.SpawnedOrAnyParentSpawned)
				{
					return Pawn.Tile;
				}
				if (Pawn.IsCaravanMember())
				{
					return Pawn.GetCaravan().Tile;
				}
				return -1;
			}
		}

		public void LinkWithSite(Site site)
		{
			siteLink = site;
		}

		public override void CompPostMake()
		{
			base.CompPostMake();
			CacheComps();
		}

		private void CacheComps()
		{
			initiatableComp = Pawn.GetComp<CompInitiatable>();
		}

		public override void CompExposeData()
		{
			base.CompExposeData();
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				causedConditions.RemoveAll((KeyValuePair<Map, GameCondition> x) => !Find.Maps.Contains(x.Key));
			}
			Scribe_References.Look(ref siteLink, "siteLink", false);
			Scribe_Collections.Look(ref causedConditions, "causedConditions", LookMode.Reference, LookMode.Reference);
			if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
			{
				causedConditions.RemoveAll((KeyValuePair<Map, GameCondition> x) => x.Value == null);
				foreach (KeyValuePair<Map, GameCondition> keyValuePair in causedConditions)
				{
					keyValuePair.Value.conditionCauser = Pawn;
				}
			}
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				CacheComps();
			}
		}

		public bool InAoE(int tile)
		{
			return MyTile != -1 && Active && Find.WorldGrid.TraversalDistanceBetween(MyTile, tile, true, Props.worldRange + 1) <= Props.worldRange;
		}

		protected GameCondition GetConditionInstance(Map map)
		{
			if (!causedConditions.TryGetValue(map, out GameCondition activeCondition) && Props.preventConditionStacking)
			{
				activeCondition = map.GameConditionManager.GetActiveCondition(Props.conditionDef);
				if (activeCondition != null)
				{
					causedConditions.Add(map, activeCondition);
					SetupCondition(activeCondition, map);
				}
			}
			return activeCondition;
		}

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (Active)
			{
				foreach (Map map in Find.Maps)
				{
					if (InAoE(map.Tile))
					{
						EnforceConditionOn(map);
					}
				}
			}
			tmpDeadConditionMaps.Clear();
			foreach (KeyValuePair<Map, GameCondition> keyValuePair in causedConditions)
			{
				if (keyValuePair.Value.Expired || !keyValuePair.Key.GameConditionManager.ConditionIsActive(keyValuePair.Value.def))
				{
					tmpDeadConditionMaps.Add(keyValuePair.Key);
				}
			}
			foreach (Map key in tmpDeadConditionMaps)
			{
				causedConditions.Remove(key);
			}
		}

		private GameCondition EnforceConditionOn(Map map)
		{
			GameCondition gameCondition = GetConditionInstance(map);
			if (gameCondition == null)
			{
				gameCondition = CreateConditionOn(map);
			}
			else
			{
				gameCondition.TicksLeft = gameCondition.TransitionTicks;
			}
			return gameCondition;
		}

		protected virtual GameCondition CreateConditionOn(Map map)
		{
			GameCondition gameCondition = GameConditionMaker.MakeCondition(ConditionDef, -1);
			gameCondition.Duration = gameCondition.TransitionTicks;
			gameCondition.conditionCauser = Pawn;
			map.gameConditionManager.RegisterCondition(gameCondition);
			causedConditions.Add(map, gameCondition);
			SetupCondition(gameCondition, map);
			return gameCondition;
		}

		protected virtual void SetupCondition(GameCondition condition, Map map)
		{
			condition.suppressEndMessage = true;
		}

		protected void ReSetupAllConditions()
		{
			foreach (KeyValuePair<Map, GameCondition> keyValuePair in causedConditions)
			{
				SetupCondition(keyValuePair.Value, keyValuePair.Key);
			}
		}

		public override void CompPostPostRemoved()
		{
			base.CompPostPostRemoved();
			Messages.Message("MessagePawnConditionCauserDespawned".Translate(Pawn.LabelShortCap, ConditionDef.label), new TargetInfo(Pawn.Position, Pawn.Map, false), MessageTypeDefOf.NeutralEvent, true);
			foreach (GameCondition gameCondition in CausedConditions)
			{
				gameCondition.End();
			}
		}

		public override string CompDebugString()
		{
			if (!Prefs.DevMode)
			{
				return base.CompDebugString();
			}
			GameCondition gameCondition = Pawn.Map.GameConditionManager.ActiveConditions.Find((GameCondition c) => c.def == Props.conditionDef);
			if (gameCondition == null)
			{
				return base.CompDebugString();
			}
			return string.Concat(new object[]
			{
				"Current map condition\nTicks Passed: ",
				gameCondition.TicksPassed,
				"\nTicks Left: ",
				gameCondition.TicksLeft
			});
		}

		public virtual void RandomizeSettings(Site site)
		{
		}

		protected CompInitiatable initiatableComp;

		protected Site siteLink;

		private Dictionary<Map, GameCondition> causedConditions = new Dictionary<Map, GameCondition>();

		private static readonly List<Map> tmpDeadConditionMaps = new List<Map>();
	}
}