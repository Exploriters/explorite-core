using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using HarmonyLib;
using UnityEngine;
using Verse.AI;
using Verse;
using Verse.Sound;
using static CentaurTheMagnuassembly.RimCentaurCore;
using SaveOurShip2;
using RimworldMod.VacuumIsNotFun;

namespace CentaurTheMagnuassembly
{
    public class ScenPart_StartInSpaceCentaur : ScenPart
    {
        public override void PostGameStart()
        {
            if (SoS2Reflection.inaccessible)
                return;

            //if (!ModLister.HasActiveModWithName("")) return;
            if (WorldSwitchUtility.SelectiveWorldGenFlag)
                return;
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
            SoS2Reflection.GenerateShip(DefDatabase<EnemyShipDef>.GetNamed("CentaursScenarioRetroCruise"), spaceMap, null, Faction.OfPlayer, null, out _);
            Current.ProgramState = ProgramState.Playing;
            IntVec2 secs = (IntVec2)typeof(MapDrawer).GetProperty("SectionCount", System.Reflection.BindingFlags.NonPublic | BindingFlags.Instance).GetValue(spaceMap.mapDrawer);
            Section[,] secArray = new Section[secs.x, secs.z];
            typeof(MapDrawer).GetField("sections", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(spaceMap.mapDrawer, secArray);
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
                List<Thing> list2 = new List<Thing>();
                list2.Add(startingAndOptionalPawn);
                list.Add(list2);
            }
            List<Thing> list3 = new List<Thing>();
            foreach (ScenPart allPart in Find.Scenario.AllParts)
            {
                list3.AddRange(allPart.PlayerStartingThings());
            }
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
                FloodFillerFog.FloodUnfog(casketPos, spaceMap);
            }
            Find.CurrentMap.Parent.Destroy();
            CameraJumper.TryJump(spaceMap.Center, spaceMap);
            spaceMap.weatherManager.curWeather = WeatherDef.Named("OuterSpaceWeather");
            spaceMap.weatherManager.lastWeather = WeatherDef.Named("OuterSpaceWeather");
            H_Vacuum_PathFinder.ResetMap(spaceMap);
            Find.MapUI.Notify_SwitchedMap();

            spaceMap.fogGrid.ClearAllFog();

            /*foreach (Letter letter in Find.LetterStack.LettersListForReading)
            {
                Find.LetterStack.RemoveLetter(letter);
            }*/
            List<Thing> things = spaceMap.listerThings.AllThings;
            Thing targetTorpedo = null;
            IntVec3 torpedoToLocation = new IntVec3(0,0,0);
            foreach (Thing thing in things)
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
                    if (thing?.TryGetComp<CompForbiddable>() != null)
                    {
                        thing.TryGetComp<CompForbiddable>().Forbidden = false;

                    }
                    if (thing?.TryGetComp<CompQuality>() != null)
                    {
                        thing.TryGetComp<CompQuality>().SetQuality(QualityCategory.Legendary, ArtGenerationContext.Colony);
                    }
                    if (thing?.TryGetComp<CompArt>() != null)
                    {
                        thing.TryGetComp<CompArt>().JustCreatedBy(spaceMap.mapPawns.AllPawns.RandomElement());
                    }
                    if (thing?.TryGetComp<CompRefuelable>() != null)
                    {
                    //thing.def == DefDatabase<ThingDef>.GetNamed("Ship_Engine_Small") ||
                    //thing.def == DefDatabase<ThingDef>.GetNamed("Ship_Engine") ||
                    //thing.def == DefDatabase<ThingDef>.GetNamed("Ship_Engine_Large") 
                        CompRefuelable fuelTarget = thing?.TryGetComp<CompRefuelable>();
                        fuelTarget?.Refuel(fuelTarget.Props.fuelCapacity);
                    }
                    if (thing?.TryGetComp<CompTempControl>() != null)
                    {
                        thing.TryGetComp<CompTempControl>().targetTemperature = 19f;
                    }
                    if (thing?.TryGetComp<CompBreakdownable>() != null)
                    {
                    }
                    if (thing?.TryGetComp<CompPower>() != null)
                    {
                    }
                    if (
                        thing?.def?.building?.buildingTags?.Contains("Production") == true
                        && thing?.TryGetComp<CompFlickable>() != null
                        && thing.def != DefDatabase<ThingDef>.GetNamed("HydroponicsBasin")
                        )
                    {
                        thing.TryGetComp<CompFlickable>().SwitchIsOn = false;
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
                        ((Plant)thing).Growth = 0.85f;
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
                            (
                                thing.def == DefDatabase<ThingDef>.GetNamed("ComponentIndustrial") ||
                                thing.def == DefDatabase<ThingDef>.GetNamed("ShipTorpedo_HighExplosive") ||
                                thing.def == DefDatabase<ThingDef>.GetNamed("ShipTorpedo_EMP") ||
                                thing.def == DefDatabase<ThingDef>.GetNamed("Chemfuel") ||
                                thing.def == DefDatabase<ThingDef>.GetNamed("ShuttleFuelPods") ||
                                thing.def == DefDatabase<ThingDef>.GetNamed("WoodLog") ||
                                thing.def == DefDatabase<ThingDef>.GetNamed("Shell_EMP")
                            )
                        )
                    {
                        //Log.Message("[Magnuassembly]Patching stack.");
                        thing.stackCount = thing.def.stackLimit;
                        
                        foreach (Thing thingInGrid in spaceMap.thingGrid.ThingsAt(thing.Position))
                        {
                            if (thingInGrid.def == DefDatabase<ThingDef>.GetNamed("Shelf"))
                            {
                                ((Building_Storage)thingInGrid).settings.filter.SetDisallowAll();
                                ((Building_Storage)thingInGrid).settings.filter.SetAllow(thing.def,true);
                            }

                        }
                    }
                    /*if (thing.def == DefDatabase<ThingDef>.GetNamed("ShipTorpedo_HighExplosive"))
                    {
                        //Log.Message("[Magnuassembly]Patching HE.");
                        thing.stackCount = 1;
                        //thing.def = DefDatabase<ThingDef>.GetNamed("ShipTorpedo_EMP");
                    }*/
                    if (thing.def == DefDatabase<ThingDef>.GetNamed("Shelf"))
                    {
                        torpedoToLocation.z = Math.Max(thing.Position.z, torpedoToLocation.z);
                    }
                    if (thing.def == DefDatabase<ThingDef>.GetNamed("ShipTorpedo_HighExplosive"))
                    {
                        torpedoToLocation.x = Math.Max(thing.Position.x, torpedoToLocation.x);
                        if (targetTorpedo == null)
                        {
                            targetTorpedo = thing;
                        }
                        else if(thing.Position.z > targetTorpedo.Position.z)
                        {
                            targetTorpedo = thing;
                        }
                    }
                    for (int i = -4; i < 5; i++)
                    {
                        for (int j = -4; j < 5; j++)
                        {
                            spaceMap.areaManager.Home[new IntVec3(i, 0, j) + thing.Position] = true;
                        }
                    }
                }
                catch { }
            }
            targetTorpedo.Position = torpedoToLocation;


            /*
            Thing InterplanetaryEngineL = ThingMaker.MakeThing(ThingDef.Named("Ship_Engine_Interplanetary"));
            InterplanetaryEngineL.SetFaction(Faction.OfPlayer);
            GenSpawn.Spawn(InterplanetaryEngineL, new IntVec3(-18,0,-28), spaceMap);
            Thing InterplanetaryEngineR = ThingMaker.MakeThing(ThingDef.Named("Ship_Engine_Interplanetary"));
            InterplanetaryEngineR.SetFaction(Faction.OfPlayer);
            GenSpawn.Spawn(InterplanetaryEngineR, new IntVec3(18,0,-28), spaceMap);
            */

            Thing InterplanetaryEngineL = ThingMaker.MakeThing(ThingDef.Named("Blueprint_Ship_Engine_Interplanetary"));
            InterplanetaryEngineL.SetFaction(Faction.OfPlayer);
            //((Blueprint_Build)InterplanetaryEngineL).;
            GenSpawn.Spawn(InterplanetaryEngineL, new IntVec3(-18,0,-28), spaceMap);

            Thing InterplanetaryEngineR = ThingMaker.MakeThing(ThingDef.Named("Blueprint_Ship_Engine_Interplanetary"));
            InterplanetaryEngineR.SetFaction(Faction.OfPlayer);
            GenSpawn.Spawn(InterplanetaryEngineR, new IntVec3(18,0,-28), spaceMap);
            

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
