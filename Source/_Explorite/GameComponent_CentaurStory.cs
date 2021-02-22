/********************
 * 游戏部件，用于追踪部分重要物品。
 * --siiftun1857
 */
using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using static Explorite.ExploriteCore;
using System;
using System.Text;

namespace Explorite
{
    public sealed class GameComponent_CentaurStory : GameComponent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060")]
        public GameComponent_CentaurStory(Game game) { }
        /*
        public enum TrishotTraceState : byte
        {
            None = 0,
            TrishotBroken = 1,
            TrishotPrototype = 2,
            TrishotFullPower = 3,
        }
        public TrishotTraceState traceTrishotState = TrishotTraceState.None;
        */
        public bool trishotTraceEnabled = false;
        private List<Thing> tracedTrishots = new List<Thing>();
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref trishotTraceEnabled, "trishotTraceEnabled", false, forceSave: true);

            CleanUp();
            Scribe_Collections.Look(ref tracedTrishots, "tracedTrishots", LookMode.Reference);
            CleanUp();
        }
        public Predicate<Thing> ValidTrishotPredicate => delegate (Thing thing)
        {
            try
            {
                return !thing.DestroyedOrNull()
                // && 
                 && ((thing is ISecretTrishot secretTrishot && secretTrishot.GetSecret())
                    || thing?.def?.weaponTags?.Exists(str => str == "CentaurTracedTrishot") == true);
            }
            catch (NullReferenceException)
            {
                return false;
            }
        };
        public IEnumerable<Thing> CleanUp()
        {
            tracedTrishots.RemoveAll(thing => !ValidTrishotPredicate(thing));
            return tracedTrishots = tracedTrishots.Distinct().ToList();
        }
        public IEnumerable<Thing> ValidTrishots
        {
            get
            {
                return CleanUp();
            }
        }
        public bool TryAdd(Thing thing)
        {
            if (ValidTrishotPredicate(thing))
            {
                if(!tracedTrishots.Contains(thing))
                    tracedTrishots.Add(thing);
                return true;
            }
            return false;
        }
        public bool TryRemove(Thing thing)
        {
            tracedTrishots = tracedTrishots.Distinct().ToList();
            return tracedTrishots.Remove(thing);
        }

        public bool MissingTrishot()
        {
            if (trishotTraceEnabled)
            {
                if (!ValidTrishots.Any())
                {
                    return true;
                }
            }
            return false;
        }
        public void GenerateNewTrishotBroken()
        {
            Map map = Find.Maps.First(map => map.IsPlayerHome && map.mapPawns.FreeColonists.Any(pawn => pawn.def == AlienCentaurDef)) ?? Find.RandomPlayerHomeMap;
            if (map == null)
            {
                Log.Error("[Explorite]Centaur home map not found.");
                return;
            }
            IntVec3 dropCenter = DropCellFinder.RandomDropSpot(map);
            Thing copy = ThingMaker.MakeThing(TrishotThing1Def);
            tracedTrishots.Add(copy);
            DropPodUtility.DropThingsNear(
                dropCenter,
                map,
                Gen.YieldSingle(copy),
                11,
                canInstaDropDuringInit: false,
                leaveSlag: false,
                canRoofPunch: true,
                forbid: false
                );
            Find.LetterStack.ReceiveLetter(
                LetterMaker.MakeLetter(
                    "Magnuassembly_NewTrishotArrived_Label".Translate(),
                    "Magnuassembly_NewTrishotArrived_Text".Translate(),
                    LetterDefOf.NeutralEvent,
                    lookTargets: copy
                    )
                );
        }
        public override void GameComponentTick()
        {
            if (Current.ProgramState != ProgramState.Playing)
                return;
            if (MissingTrishot())
            {
                //Log.Message("[Explorite]Trishot lost detected.");
                GenerateNewTrishotBroken();
            }
        }
        ///<summary>显示追踪器状态。</summary>
        [DebugAction(category: "Explorite", name: "Print trishot tracer state", allowedGameStates = AllowedGameStates.Playing,
            actionType = DebugActionType.Action)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(null, "IDE0051")]
        private static void PrintTrishotTracerState()
        {
            StringBuilder str = new StringBuilder();
            GameComponent_CentaurStory component = GameComponentCentaurStory;

            str.AppendLine("[Explorite]Printing trishot tracer state.");
            str.AppendLine($"trishotTraceEnabled: {component.trishotTraceEnabled}");
            str.AppendLine("Traced Trishots:");
            foreach (Thing thing in component.tracedTrishots)
            {
                str.AppendLine($"   {thing.GetUniqueLoadID()}");
            }
            str.AppendLine("Traced Valid Trishots:");
            foreach (Thing thing in component.ValidTrishots)
            {
                str.AppendLine($"   {thing.GetUniqueLoadID()}");
            }
            str.AppendLine("Traced Trishots After Predicate:");
            foreach (Thing thing in component.tracedTrishots)
            {
                str.AppendLine($"   {thing.GetUniqueLoadID()}");
            }
            Log.Message(str.ToString());
            Log.TryOpenLogWindow();
        }
    }
}
