/********************
 * 实现半人马开局在其巡洋舰中。
 * --siiftun1857
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;
using SaveOurShip2;
using static Explorite.ExploriteCore;
using RimworldMod;

namespace Explorite
{
    ///<summary>使半人马开局在其巡洋舰中。</summary>
    public class ScenPart_StartInSpaceCentaur : ScenPart
    {
        private void CentaurAlphaShipPostProcess(Map spaceMap)
        {
            IntVec3 center = spaceMap.Center;
            ThingDef engineDef = DefDatabase<ThingDef>.GetNamed("Ship_Engine_Interplanetary");
            EnemyShipDef shipDef = DefDatabase<EnemyShipDef>.GetNamed("CentaursScenarioRetroCruise");

            /*foreach (Letter letter in Find.LetterStack.LettersListForReading)
            {
                Find.LetterStack.RemoveLetter(letter);
            }*/

            //GenSpawn.Spawn(ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Ship_Engine_Interplanetary")), spaceMap.Center + new IntVec3(shape.x, 0, shape.z), spaceMap, shape.rot);

            /*
            if (sunLampLocation.HasValue)
            {
                IntVec3 leftEngine = new IntVec3(sunLampLocation.Value.ToVector3()) + new IntVec3(-18, 0, -17);
                IntVec3 rightEngine = new IntVec3(sunLampLocation.Value.ToVector3()) + new IntVec3(18, 0, -17);

                Thing InterplanetaryEngineL = ThingMaker.MakeThing(engineDef);
                GenSpawn.Spawn(InterplanetaryEngineL, leftEngine, spaceMap);
                InterplanetaryEngineL.SetFaction(Faction.OfPlayer);
                InterplanetaryEngineL.TryGetComp<CompPowerTrader>().PowerOn = true;

                Thing InterplanetaryEngineR = ThingMaker.MakeThing(engineDef);
                GenSpawn.Spawn(InterplanetaryEngineR, rightEngine, spaceMap);
                InterplanetaryEngineR.SetFaction(Faction.OfPlayer);
                InterplanetaryEngineR.TryGetComp<CompPowerTrader>().PowerOn = true;
            }
            */
            foreach (ShipShape shape in shipDef.parts)
            {
                if (shape.shapeOrDef == "Ship_Engine_Interplanetary")
                {
                    Thing InterplanetaryEngine = ThingMaker.MakeThing(engineDef);
                    GenSpawn.Spawn(InterplanetaryEngine, center + new IntVec3(shape.x,0,shape.z), spaceMap, shape.rot, WipeMode.Vanish);
                    InterplanetaryEngine.SetFaction(Faction.OfPlayer);
                    InterplanetaryEngine.TryGetComp<CompPowerTrader>().PowerOn = true;
                }
            }

            foreach (Pawn pawn in Find.GameInitData.startingAndOptionalPawns)
            {
                foreach (Thing thingInPawn in (pawn?.inventory?.GetDirectlyHeldThings() ?? Enumerable.Empty<Thing>()).Concat(pawn?.apparel?.GetDirectlyHeldThings() ?? Enumerable.Empty<Thing>()).Concat(pawn?.equipment?.GetDirectlyHeldThings() ?? Enumerable.Empty<Thing>()).ToList())
                {
                    if (thingInPawn?.TryGetComp<CompQuality>() is CompQuality compQualityInPawn)
                    {
                        compQualityInPawn.SetQuality(QualityCategory.Legendary, ArtGenerationContext.Colony);
                    }
                    if (thingInPawn?.TryGetComp<CompArt>() is CompArt compArtInPawn)
                    {
                        compArtInPawn.JustCreatedBy(pawn);
                    }
                    if (thingInPawn?.TryGetComp<CompStyleable>() is CompStyleable compStyleableInPawn)
                    {
                        try
                        {
                            compStyleableInPawn.styleDef = Find.FactionManager?.OfPlayer?.ideos?.PrimaryIdeo?.style?.StyleForThingDef(thingInPawn.def)?.styleDef;
                        }
                        catch (NullReferenceException e)
                        {
                        }
                    }
                    if (thingInPawn is Building_TriBattery secretBattery)
                    {
                        secretBattery?.TryGetComp<CompPowerBattery>()?.AddEnergy(float.PositiveInfinity);
                        if (!GameComponentCentaurStory.Any())
                        {
                            secretBattery.SetSecret(true);
                            GameComponentCentaurStory.TryAdd(secretBattery);
                        }
                    }
                }
            }

            //Thing targetTorpedo = null;
            //IntVec3 torpedoToLocation = new IntVec3(0, 0, 0);
            IntVec3? sunLampLocation = null;
            foreach (Thing thing in spaceMap.listerThings.AllThings.ToList())
            {
                try
                {
                    if (thing.def == ThingDefOf.MinifiedThing)
                    {
                        Thing thingInside = ((MinifiedThing)thing).InnerThing;
                        if (thingInside.TryGetComp<CompPowerBattery>() != null)
                        {
                            thingInside?.TryGetComp<CompPowerBattery>()?.AddEnergy(float.PositiveInfinity);
                        }
                    }
                    if (sunLampLocation == null && thing.def == DefDatabase<ThingDef>.GetNamed("SunLamp"))
                    {
                        sunLampLocation = thing.Position;
                    }
                    if (thing?.TryGetComp<CompForbiddable>() is CompForbiddable compForbiddable)
                    {
                        compForbiddable.Forbidden = thing.def == ThingDefOf.ComponentIndustrial;
                    }
                    if (thing?.TryGetComp<CompQuality>() is CompQuality compQuality)
                    {
                        compQuality.SetQuality(QualityCategory.Legendary, ArtGenerationContext.Colony);
                    }
                    if (thing?.TryGetComp<CompArt>() is CompArt compArt)
                    {
                        compArt.JustCreatedBy(Find.GameInitData.startingAndOptionalPawns.RandomElement());
                    }
                    if (thing?.TryGetComp<CompStyleable>() is CompStyleable compStyleable)
                    {
                        try
                        {
                            compStyleable.styleDef = Find.FactionManager.OfPlayer.ideos.PrimaryIdeo.style.StyleForThingDef(thing.def)?.styleDef;
                        }
                        catch (NullReferenceException e)
                        {
                        }
                    }
                    if (thing?.TryGetComp<CompRefuelable>() is CompRefuelable compRefuelable)
                    {
                        //thing.def == DefDatabase<ThingDef>.GetNamed("Ship_Engine_Small") ||
                        //thing.def == DefDatabase<ThingDef>.GetNamed("Ship_Engine") ||
                        //thing.def == DefDatabase<ThingDef>.GetNamed("Ship_Engine_Large") 
                        compRefuelable.Refuel(compRefuelable.Props.fuelCapacity);
                    }
                    if (thing?.TryGetComp<CompTempControl>() is CompTempControl compTempControl)
                    {
                        compTempControl.targetTemperature = 19f;
                    }
                    if (thing?.TryGetComp<CompBreakdownable>() is CompBreakdownable compBreakdownable)
                    {
                    }
                    if (
                        thing?.TryGetComp<CompFlickable>() is CompFlickable compFlickable
                        && (
                            (thing?.def?.building?.buildingTags?.Contains("Production") == true && thing.def != DefDatabase<ThingDef>.GetNamed("HydroponicsBasin"))
                         || thing.def == DefDatabase<ThingDef>.GetNamed("MultiAnalyzer")
                         || thing.def == ThingDefOf.BiosculpterPod
                         || thing.def == ThingDefOf.NeuralSupercharger
                            )
                        )
                    {
                        compFlickable.SwitchIsOn = false;
                        typeof(CompFlickable).GetField("wantSwitchOn").SetValue(compFlickable, false);
                    }
                    if (thing?.TryGetComp<CompRechargeable>() is CompRechargeable compRechargeable)
                    {
                        typeof(CompRechargeable).GetField("ticksUntilCharged").SetValue(compRechargeable, 0);
                    }
                    if (thing?.TryGetComp<CompRoofMe>() is CompRoofMe compRoofMe)
                    {
                        ThingDef oofType = null;
                        if (compRoofMe.Props.archotech)
                        {
                            oofType = DefDatabase<ThingDef>.GetNamed("ShipHullTileArchotech");
                        }
                        else if (compRoofMe.Props.mechanoid)
                        {
                            oofType = DefDatabase<ThingDef>.GetNamed("ShipHullTileMech");
                        }
                        else if (compRoofMe.Props.wreckage)
                        {
                            oofType = DefDatabase<ThingDef>.GetNamed("ShipHullTileWrecked");
                        }
                        else
                        {
                            oofType = DefDatabase<ThingDef>.GetNamed("ShipHullTile");
                        }
                        foreach (IntVec3 cell in thing.OccupiedRect().Cells)
                        {
                            if (!spaceMap.thingGrid.ThingsAt(cell).Any(thi => thi.def == oofType))
                            {
                                Thing hullTile = ThingMaker.MakeThing(oofType);
                                GenSpawn.Spawn(hullTile, cell, spaceMap, WipeMode.Vanish);
                                hullTile.SetFaction(Faction.OfPlayer);
                            }
                        }
                    }
                    if (thing.def == DefDatabase<ThingDef>.GetNamed("HydroponicsBasin"))
                    {
                        ((Building_PlantGrower)thing)?.SetPlantDefToGrow(ThingDefOf.Plant_Potato);
                        thing.TryGetComp<CompForbiddable>().Forbidden = true;
                    }
                    if (thing.def == ThingDefOf.Blight)
                    {
                        ((Blight)thing).Severity = 0.05f;
                    }
                    if (thing.def == DefDatabase<ThingDef>.GetNamed("ShipCombatShieldGenerator"))
                    {
                        thing.TryGetComp<CompFlickable>().SwitchIsOn = false;
                        thing.TryGetComp<CompBreakdownable>()?.DoBreakdown();
                    }
                    if (thing.def == DefDatabase<ThingDef>.GetNamed("ShipTurret_Laser"))
                    {
                        ((Building_ShipTurret)thing).PointDefenseMode = true;
                    }
                    if (thing.def == DefDatabase<ThingDef>.GetNamed("Plant_Potato"))
                    {
                        (thing as Plant).Growth = 0.85f;
                    }
                    /*if (thing.def == DefDatabase<ThingDef>.GetNamed("ShipTorpedoOne"))
                    {
                        foreach (Thing thingInside in ((Building_ShipTurretTorpedo)thing).Contents.innerContainer)
                        {
                            if (thing.def == DefDatabase<ThingDef>.GetNamed("ShipTorpedo_HighExplosive"))
                            {
                                thing.def = DefDatabase<ThingDef>.GetNamed("ShipTorpedo_EMP");
                            }
                        }
                    }*/
                    if (
                            thing.def == DefDatabase<ThingDef>.GetNamed("ComponentIndustrial") ||
                            thing.def == DefDatabase<ThingDef>.GetNamed("ShipTorpedo_HighExplosive") ||
                            thing.def == DefDatabase<ThingDef>.GetNamed("ShipTorpedo_EMP") ||
                            thing.def == DefDatabase<ThingDef>.GetNamed("Chemfuel") ||
                            thing.def == DefDatabase<ThingDef>.GetNamed("ShuttleFuelPods") ||
                            thing.def == DefDatabase<ThingDef>.GetNamed("WoodLog") ||
                            thing.def == DefDatabase<ThingDef>.GetNamed("Shell_EMP")
                        )
                    {
                        //Log.Message("[Explorite]Patching stack.");
                        thing.stackCount = thing.def.stackLimit;
                        foreach (Thing thingInGrid in spaceMap.thingGrid.ThingsAt(thing.Position))
                        {
                            if (thingInGrid.def == DefDatabase<ThingDef>.GetNamed("Shelf"))
                            {
                                (thingInGrid as Building_Storage).settings.filter.SetDisallowAll();
                                (thingInGrid as Building_Storage).settings.filter.SetAllow(thing.def, true);
                            }

                        }
                    }
                    /*if (thing.def == DefDatabase<ThingDef>.GetNamed("ShipTorpedo_HighExplosive"))
                    {
                        //Log.Message("[Explorite]Patching HE.");
                        thing.stackCount = 1;
                        //thing.def = DefDatabase<ThingDef>.GetNamed("ShipTorpedo_EMP");
                    }*/
                    if (thing.def == DefDatabase<ThingDef>.GetNamed("Shelf"))
                    {
                        //torpedoToLocation.z = Math.Max(thing.Position.z, torpedoToLocation.z);
                    }
                    if (thing.def == DefDatabase<ThingDef>.GetNamed("ShipTorpedo_HighExplosive"))
                    {
                        /*
                        torpedoToLocation.x = Math.Max(thing.Position.x, torpedoToLocation.x);
                        if (targetTorpedo == null)
                        {
                            targetTorpedo = thing;
                        }
                        else if (thing.Position.z > targetTorpedo.Position.z)
                        {
                            targetTorpedo = thing;
                        }
                        */
                        if (thing.Spawned)
                            thing.DeSpawn();
                        if (!thing.DestroyedOrNull())
                            thing.Destroy();
                    }
                    if (thing?.TryGetComp<CompPower>() is CompPower compPower)
                    {
                        typeof(CompPower).GetMethod("TryManualReconnect").Invoke(compPower, new object[] { });
                    }
                    foreach (IntVec3 cell in thing.OccupiedRect().Cells)
                    {
                        for (int i = -4; i <= 4; i++)
                        {
                            for (int j = -4; j <= 4; j++)
                            {
                                spaceMap.areaManager.Home[new IntVec3(i, 0, j) + cell] = true;
                            }
                        }
                    }
                }
                catch { }
            }
            //targetTorpedo.Position = torpedoToLocation;


            /*
            Thing InterplanetaryEngineL = ThingMaker.MakeThing(ThingDef.Named("Ship_Engine_Interplanetary"));
            InterplanetaryEngineL.SetFaction(Faction.OfPlayer);
            GenSpawn.Spawn(InterplanetaryEngineL, new IntVec3(-18,0,-28), spaceMap);
            Thing InterplanetaryEngineR = ThingMaker.MakeThing(ThingDef.Named("Ship_Engine_Interplanetary"));
            InterplanetaryEngineR.SetFaction(Faction.OfPlayer);
            GenSpawn.Spawn(InterplanetaryEngineR, new IntVec3(18,0,-28), spaceMap);
                //((Blueprint_Build)InterplanetaryEngineL).;
            */



            /*
            List<Building> thingsRocket = spaceMap.listerBuildings.allBuildingsColonist;
            foreach (Building thing in thingsBatteryIn)
            {
                try
                {
                    if (
                        thing?.TryGetComp<CompRefuelable>() != null
                        //thing.def == DefDatabase<ThingDef>.GetNamed("Ship_Engine_Small") ||
                        //thing.def == DefDatabase<ThingDef>.GetNamed("Ship_Engine") ||
                        //thing.def == DefDatabase<ThingDef>.GetNamed("Ship_Engine_Large")                    
                        )
                    {
                        CompRefuelable fuelTarget = thing?.TryGetComp<CompRefuelable>();
                        fuelTarget.ConsumeFuel(
                            fuelTarget.Fuel -
                            fuelTarget.Props.fuelCapacity
                            );
                    }
                }
                catch { }
            }*/
            if (sunLampLocation != null)
            {
                CameraJumper.TryJump(sunLampLocation.Value, spaceMap);
            }
            spaceMap.fogGrid.ClearAllFog();
            }

        public override void PostGameStart()
        {
            if (SoS2Reflection.inaccessible)
                return;

            if (WorldSwitchUtility.SelectiveWorldGenFlag)
                return;

            EnemyShipDef shipDef = DefDatabase<EnemyShipDef>.GetNamed("CentaursScenarioRetroCruise");
            ShipCombatManager.CanSalvageEnemyShip = false;
            ShipCombatManager.ShouldSalvageEnemyShip = false;
            ShipCombatManager.InCombat = false;
            ShipCombatManager.InEncounter = false;
            List<Pawn> startingPawns = Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer);
            int newTile = -1;
            for (int i = 0; i < 420; i++)
            {
                if (!Find.World.worldObjects.AnyMapParentAt(i))
                {
                    newTile = i;
                    break;
                }
            }
            Map spaceMap = GetOrGenerateMapUtility.GetOrGenerateMap(newTile, DefDatabase<WorldObjectDef>.GetNamed("ShipOrbiting"));
            ((WorldObjectOrbitingShip)spaceMap.Parent).radius = 150;
            ((WorldObjectOrbitingShip)spaceMap.Parent).theta = 2.75f;
            //Building core = null;
            Current.ProgramState = ProgramState.MapInitializing;
            SoS2Reflection.GenerateShip(shipDef, spaceMap, null, Faction.OfPlayer, null, out _);
            Current.ProgramState = ProgramState.Playing;
            IntVec2 secs = (IntVec2)typeof(MapDrawer).GetProperty("SectionCount", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(spaceMap.mapDrawer);
            Section[,] secArray = new Section[secs.x, secs.z];
            typeof(MapDrawer).GetField("sections", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(spaceMap.mapDrawer, secArray);
            for (int i = 0; i < secs.x; i++)
            {
                for (int j = 0; j < secs.z; j++)
                {
                    if (secArray[i, j] == null)
                    {
                        secArray[i, j] = new Section(new IntVec3(i, 0, j), spaceMap);
                    }
                }
            }
            List<IntVec3> cryptoPos = GetAllCryptoCells(spaceMap);
            foreach (Pawn p in startingPawns)
            {
                if (p.InContainerEnclosed)
                {
                    p.ParentHolder.GetDirectlyHeldThings().Remove(p);
                }
                else
                {
                    p.DeSpawn();
                    p.SpawnSetup(spaceMap, true);
                }
            }
            List<List<Thing>> list = new List<List<Thing>>();
            foreach (Pawn startingAndOptionalPawn in Find.GameInitData.startingAndOptionalPawns)
            {
                list.Add(new List<Thing>
                {
                    startingAndOptionalPawn
                });
            }
            List<Thing> list3 = new List<Thing>();
            /*
            foreach (ScenPart allPart in Find.Scenario.AllParts)
            {
                list3.AddRange(allPart.PlayerStartingThings());
            }
            */
            int num = 0;
            foreach (Thing item in list3)
            {
                if (!(item is Pawn))
                {
                    if (item.def.CanHaveFaction)
                    {
                        item.SetFactionDirect(Faction.OfPlayer);
                    }
                    list[num].Add(item);
                    num++;
                    if (num >= list.Count)
                    {
                        num = 0;
                    }
                }
            }
            foreach (List<Thing> thingies in list)
            {
                IntVec3 casketPos = cryptoPos.RandomElement();
                cryptoPos.Remove(casketPos);
                if (cryptoPos.Count() == 0)
                    cryptoPos = GetAllCryptoCells(spaceMap); //Out of caskets, time to start double-dipping
                foreach (Thing thingy in thingies)
                {
                    thingy.SetForbidden(true, false);
                    GenPlace.TryPlaceThing(thingy, casketPos, spaceMap, ThingPlaceMode.Near);
                }
            }
            spaceMap.fogGrid.ClearAllFog();
            //Current.Game.DeinitAndRemoveMap(Find.CurrentMap);
            Find.CurrentMap.Parent.Destroy();
            CameraJumper.TryJump(spaceMap.Center, spaceMap);
            spaceMap.weatherManager.curWeather = WeatherDef.Named("OuterSpaceWeather");
            spaceMap.weatherManager.lastWeather = WeatherDef.Named("OuterSpaceWeather");
            spaceMap.Parent.SetFaction(Faction.OfPlayer);
            Find.MapUI.Notify_SwitchedMap();
            spaceMap.regionAndRoomUpdater.RebuildAllRegionsAndRooms();
            foreach (Room r in spaceMap.regionGrid.allRooms)
                r.Temperature = 19;
            AccessExtensions.Utility.GetType().GetMethod("RecacheSpaceMaps").Invoke(AccessExtensions.Utility, new object[] { });

            CentaurAlphaShipPostProcess(spaceMap);
        }

        List<IntVec3> GetAllCryptoCells(Map spaceMap)
        {
            List<IntVec3> toReturn = new List<IntVec3>();
            foreach (Building b in spaceMap.listerBuildings.allBuildingsColonist.Where(b => b is Building_CryptosleepCasket))
            {
                toReturn.Add(b.InteractionCell);
            }
            return toReturn;
        }
    }

}
