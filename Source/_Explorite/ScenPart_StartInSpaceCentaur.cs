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
			EnemyShipDef shipDef = DefDatabase<EnemyShipDef>.GetNamed("CentaursScenarioRetroCruise");

			ThingDef Ship_Engine_Interplanetary = DefDatabase<ThingDef>.GetNamed("Ship_Engine_Interplanetary");
			ThingDef ShipHullTile = DefDatabase<ThingDef>.GetNamed("ShipHullTile");
			ThingDef ShipHullTileArchotech = DefDatabase<ThingDef>.GetNamed("ShipHullTileArchotech");
			ThingDef ShipHullTileMech = DefDatabase<ThingDef>.GetNamed("ShipHullTileMech");
			ThingDef ShipHullTileWrecked = DefDatabase<ThingDef>.GetNamed("ShipHullTileWrecked");
			ThingDef ShipCombatShieldGenerator = DefDatabase<ThingDef>.GetNamed("ShipCombatShieldGenerator");
			ThingDef MultiAnalyzer = DefDatabase<ThingDef>.GetNamed("MultiAnalyzer");
			ThingDef HydroponicsBasin = DefDatabase<ThingDef>.GetNamed("HydroponicsBasin");
			ThingDef ShipPilotSeatMini = DefDatabase<ThingDef>.GetNamed("ShipPilotSeatMini");
			ThingDef ShipShuttleBay = DefDatabase<ThingDef>.GetNamed("ShipShuttleBay");
			ThingDef ShipHeatManifoldLarge_Ex = DefDatabase<ThingDef>.GetNamed("ShipHeatManifoldLarge_Ex");
			ThingDef ShipTurret_Laser = DefDatabase<ThingDef>.GetNamed("ShipTurret_Laser");
			ThingDef ShipCapacitor = DefDatabase<ThingDef>.GetNamed("ShipCapacitor");
			ThingDef SunLamp = DefDatabase<ThingDef>.GetNamed("SunLamp");
			ThingDef ComponentIndustrial = DefDatabase<ThingDef>.GetNamed("ComponentIndustrial");
			ThingDef ShipTorpedo_HighExplosive = DefDatabase<ThingDef>.GetNamed("ShipTorpedo_HighExplosive");
			ThingDef ShipTorpedo_EMP = DefDatabase<ThingDef>.GetNamed("ShipTorpedo_EMP");
			ThingDef Chemfuel = DefDatabase<ThingDef>.GetNamed("Chemfuel");
			ThingDef ShuttleFuelPods = DefDatabase<ThingDef>.GetNamed("ShuttleFuelPods");
			ThingDef WoodLog = DefDatabase<ThingDef>.GetNamed("WoodLog");
			ThingDef Shell_EMP = DefDatabase<ThingDef>.GetNamed("Shell_EMP");
			ThingDef Shelf = DefDatabase<ThingDef>.GetNamed("Shelf");
			ThingDef Ship_LifeSupport = DefDatabase<ThingDef>.GetNamed("Ship_LifeSupport");

			/*foreach (Letter letter in Find.LetterStack.LettersListForReading)
			{
				Find.LetterStack.RemoveLetter(letter);
			}*/

			foreach (ShipShape shape in shipDef.parts)
			{
				if (shape.shapeOrDef == "Ship_Engine_Interplanetary")
				{
					Thing InterplanetaryEngine = ThingMaker.MakeThing(Ship_Engine_Interplanetary);
					GenSpawn.Spawn(InterplanetaryEngine, center + new IntVec3(shape.x, 0, shape.z), spaceMap, shape.rot, WipeMode.Vanish);
					InterplanetaryEngine.SetFaction(Faction.OfPlayer);
					InterplanetaryEngine.TryGetComp<CompPowerTrader>().PowerOn = true;
				}
			}

			/*
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
						catch (NullReferenceException)
						{
						}
					}
					if (thingInPawn is Building_TriBattery_SecretTrishot secretBattery)
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
			*/

			foreach (Thing thing in spaceMap.listerThings.AllThings.ToList())
			{
				try
				{
					thing.stackCount = thing.def.stackLimit;
					if (thing.def == ThingDefOf.MinifiedThing)
					{
						Thing thingInside = ((MinifiedThing)thing).InnerThing;
						if (thingInside.TryGetComp<CompPowerBattery>() != null)
						{
							thingInside?.TryGetComp<CompPowerBattery>()?.AddEnergy(float.PositiveInfinity);
						}
					}
					if (thing.def == SunLamp)
					{
						CameraJumper.TryJump(thing.Position, spaceMap);
					}
					if (thing.TryGetComp<CompForbiddable>() is CompForbiddable compForbiddable)
					{
						compForbiddable.Forbidden = (
							thing.def == ThingDefOf.ComponentIndustrial
						 || thing.def == ThingDefOf.GroundPenetratingScanner
						 || thing.def == ThingDefOf.LongRangeMineralScanner
						 || thing.def == HydroponicsBasin
							);
					}
					if (thing.TryGetComp<CompQuality>() is CompQuality compQuality)
					{
						compQuality.SetQuality(QualityCategory.Legendary, ArtGenerationContext.Colony);
					}
					if (thing.TryGetComp<CompArt>() is CompArt compArt)
					{
						if (thing.def == CentaurBedDef)
						{
							compArt.JustCreatedBy(Find.GameInitData.startingAndOptionalPawns.RandomElement());
						}
						else
						{
							compArt.Clear();
						}
					}
					if (thing.TryGetComp<CompStyleable>() is CompStyleable compStyleable)
					{
						try
						{
							compStyleable.styleDef = Find.FactionManager.OfPlayer.ideos.PrimaryIdeo.style.StyleForThingDef(thing.def)?.styleDef;
						}
						catch (NullReferenceException)
						{
						}
					}
					if (thing.TryGetComp<CompRefuelable>() is CompRefuelable compRefuelable)
					{
						if (thing.TryGetComp<CompShipHeatPurgeGolden>() != null)
							compRefuelable.ConsumeFuel(compRefuelable.Props.fuelCapacity);
						else
							compRefuelable.Refuel(compRefuelable.Props.fuelCapacity);
					}
					if (thing.TryGetComp<CompTempControl>() is CompTempControl compTempControl)
					{
						compTempControl.targetTemperature = 19f;
					}
					if (thing is Building_ShipVent shipVent)
					{
						shipVent.heatWithPower = false;
					}
					if (thing.TryGetComp<CompBreakdownable>() is CompBreakdownable compBreakdownable
						&& (thing.def == ThingDefOf.GroundPenetratingScanner
						 || thing.def == ThingDefOf.LongRangeMineralScanner
						 || thing.def == ShipCombatShieldGenerator
						 ))
					{
						compBreakdownable.DoBreakdown();
					}
					if (thing.TryGetComp<CompFlickable>() is CompFlickable compFlickable
						&& (
							(thing?.def?.building?.buildingTags?.Contains("Production") == true && thing.def != HydroponicsBasin)
						 || thing.def == MultiAnalyzer
						 || thing.def == ThingDefOf.BiosculpterPod
						 || thing.def == ThingDefOf.NeuralSupercharger
						 || thing.def == ThingDefOf.GroundPenetratingScanner
						 || thing.def == ThingDefOf.LongRangeMineralScanner
						 || thing.def == ShipCombatShieldGenerator
							)
						)
					{
						compFlickable.SwitchIsOn = false;
						typeof(CompFlickable).GetField("wantSwitchOn", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).SetValue(compFlickable, false);
					}
					if (thing.TryGetComp<CompRechargeable>() is CompRechargeable compRechargeable)
					{
						typeof(CompRechargeable).GetField("ticksUntilCharged", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).SetValue(compRechargeable, 1);
					}
					if (thing.def != ShipHullTile
					 && thing.def != ShipHullTileArchotech
					 && thing.def != ShipHullTileMech
					 && thing.def != ShipHullTileWrecked
					 && thing.TryGetComp<CompRoofMe>() is CompRoofMe compRoofMe)
					{
						ThingDef oofType = null;
						if (compRoofMe.Props.archotech)
						{
							oofType = ShipHullTileArchotech;
						}
						else if (compRoofMe.Props.mechanoid)
						{
							oofType = ShipHullTileMech;
						}
						else if (compRoofMe.Props.wreckage)
						{
							oofType = ShipHullTileWrecked;
						}
						else
						{
							oofType = ShipHullTile;
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
					if (thing.def == HydroponicsBasin && thing is Building_PlantGrower plantGrower)
					{
						plantGrower.SetPlantDefToGrow(ThingDefOf.Plant_Potato);
						foreach (IntVec3 cell in plantGrower.OccupiedRect())
						{
							Plant potato = ThingMaker.MakeThing(ThingDefOf.Plant_Potato) as Plant;
							potato.Growth = 0.85f;
							GenSpawn.Spawn(potato, cell, plantGrower.Map, WipeMode.Vanish);

							Blight blight = ThingMaker.MakeThing(ThingDefOf.Blight) as Blight;
							blight.Severity = 0.05f;
							GenSpawn.Spawn(blight, cell, plantGrower.Map, WipeMode.Vanish);
						}
					}
					/*
					if (thing.def == ThingDefOf.Plant_Potato && thing is Plant plant)
					{
						plant.Growth = 0.85f;
					}
					if (thing.def == ThingDefOf.Blight && thing is Blight blight)
					{
						blight.Severity = 0.05f;
					}
					*/
					if (thing.def == ShipTurret_Laser && thing is Building_ShipTurret shipTurret)
					{
						shipTurret.PointDefenseMode = true;
					}
					/*if (thing.def == ShipTorpedoOne)
					{
						foreach (Thing thingInside in ((Building_ShipTurretTorpedo)thing).Contents.innerContainer)
						{
							if (thing.def == ShipTorpedo_HighExplosive)
							{
								thing.def = ShipTorpedo_EMP;
							}
						}
					}*/
					/*
					if (thing.def == ComponentIndustrial
					 || thing.def == ShipTorpedo_HighExplosive
					 || thing.def == ShipTorpedo_EMP
					 || thing.def == Chemfuel
					 || thing.def == ShuttleFuelPods
					 || thing.def == WoodLog
					 || thing.def == Shell_EMP
						)
					{
						thing.stackCount = thing.def.stackLimit;
						foreach (Thing thingInGrid in spaceMap.thingGrid.ThingsAt(thing.Position))
						{
							if (thingInGrid.def == Shelf && thingInGrid is Building_Storage storge)
							{
								if (storge.settings.filter.AllowedDefCount > 1)
								{
								}
							}
						}
					}
					*/
					if (thing.def == Shelf && thing is Building_Storage storge)
					{
						storge.settings.filter.SetDisallowAll();
						foreach (ThingDef thingDef in storge.OccupiedRect().SelectMany(x => spaceMap.thingGrid.ThingsAt(x).Select(d => d.def)).Distinct().Where(x => x.category == ThingCategory.Item))
						{
							storge.settings.filter.SetAllow(thingDef, true);
						}
					}
					if (thing.def == ShipTorpedo_HighExplosive)
					{
						if (thing.Spawned)
							thing.DeSpawn();
						if (!thing.DestroyedOrNull())
							thing.Destroy();
					}
					if (thing.TryGetComp<CompPower>() is CompPower compPower && !compPower.Props.transmitsPower)
					{
						typeof(CompPower).GetMethod("TryManualReconnect", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(compPower, new object[] { });
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
			foreach (Room room in spaceMap.regionGrid.allRooms)
			{
				float ctrlTemp = -50;
				bool heatUp = false;

				if (room.ContainsThing(ThingDefOf.OrbitalTradeBeacon))
				{
					room.Temperature = -50;
					Designator des = DesignatorUtility.FindAllowedDesignator<Designator_ZoneAddStockpile_Resources>();
					des.DesignateMultiCell(room.Cells);
					ctrlTemp = -50;
					heatUp = false;
				}
				else if (
					room.ContainedAndAdjacentThings.Any(thing =>
					thing.def == ThingDefOf.Drape
				 || thing.def == ThingDefOf.BiosculpterPod
				 || thing.def == HydroponicsBasin
				 || thing.def == ShipPilotSeatMini
				 || thing.def == Ship_LifeSupport
					)
					)
				{
					room.Temperature = 19;
					ctrlTemp = 19;
					heatUp = true;
				}
				else if (
					room.ContainedAndAdjacentThings.Any(thing =>
					thing.def == ShipHeatManifoldLarge_Ex
				 || thing.def == ShipShuttleBay
				 || thing.def == ShipTurret_Laser
				 || thing.def == ShipCapacitor
					)
					)
				{
					room.Temperature = 19;
					ctrlTemp = 19;
					heatUp = false;
				}
				else
				{
					room.Temperature = -50;
				}

				foreach (Thing thing in room.ContainedAndAdjacentThings.Where(thing => !(thing is Building_ShipVent shipVent) || room.Cells.Contains(thing.Position + IntVec3.North.RotatedBy(thing.Rotation))))
				{
					if (thing is Building_ShipVent shipVent)
					{
						shipVent.heatWithPower = heatUp;
					}
					if (thing.TryGetComp<CompTempControl>() is CompTempControl compTempControl)
					{
						compTempControl.targetTemperature = ctrlTemp;
					}
				}
			}
			spaceMap.autoBuildRoofAreaSetter.AutoBuildRoofAreaSetterTick_First();

			foreach (IntVec3 cell in spaceMap.AllCells)
			{
				spaceMap.areaManager.BuildRoof[cell] = false;
				spaceMap.areaManager.NoRoof[cell] = false;
			}
			Find.Selector.ClearSelection();
			spaceMap.fogGrid.ClearAllFog();
		}

		public override void PostGameStart()
		{
			if (SoS2Reflection.inaccessible)
				return;

			if (WorldSwitchUtility.SelectiveWorldGenFlag)
				return;

			EnemyShipDef shipDef = DefDatabase<EnemyShipDef>.GetNamed("CentaursScenarioRetroCruise");
			SoS2Reflection.sos2scm.GetField("CanSalvageEnemyShip", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, false);
			SoS2Reflection.sos2scm.GetField("ShouldSalvageEnemyShip", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, false);
			SoS2Reflection.sos2scm.GetField("InCombat", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, false);
			SoS2Reflection.sos2scm.GetField("InEncounter", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, false);
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
			foreach (Room room in spaceMap.regionGrid.allRooms)
			{
				room.Temperature = 19;
			}
			AccessExtensions.Utility.GetType().GetMethod("RecacheSpaceMaps", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(AccessExtensions.Utility, new object[] { });

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