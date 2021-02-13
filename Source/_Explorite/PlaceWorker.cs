/**
 * 包含多个PlaceWorker的合集文件。
 * --siiftun1857
 */
using RimWorld;
using UnityEngine;
using Verse;

namespace Explorite
{
    ///<summary>为植物反应桌绘制互动图标。</summary>
    [StaticConstructorOnStartup]
    public class PlaceWorker_PlantReactionTable : PlaceWorker
    {
        private static readonly Material PortCellMaterial = MaterialPool.MatFrom("UI/Overlays/InteractionCell", ShaderDatabase.Transparent);

        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            DrawPortCell(PortPosition(def, center, rot));
        }
        public static IntVec3 PortPosition(ThingDef def, IntVec3 center, Rot4 rot)
        {
            if (rot.AsByte == 0)
                return center + new IntVec3(-1 - (def.size.z / 2), 0, 0);
            else if (rot.AsByte == 1)
                return center + new IntVec3(0, 0, 1 + (def.size.z / 2));
            else if (rot.AsByte == 2)
                return center + new IntVec3(1 + (def.size.z / 2), 0, 0);
            else if (rot.AsByte == 3)
                return center + new IntVec3(0, 0, -1 - (def.size.z / 2));
            else return IntVec3.Invalid;
        }
        public static void DrawPortCell(IntVec3 center)
        {
            Map currentMap = Find.CurrentMap;
            if (center.Standable(currentMap))
            {
                Vector3 position = center.ToVector3ShiftedWithAltitude(AltitudeLayer.MetaOverlays);
                Graphics.DrawMesh(MeshPool.plane10, position, Quaternion.identity, PortCellMaterial, 0);
            }
		}
		/*
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thingToPlace = null)
        {
            if (checkingDef is ThingDef thingDef && thingDef.placeWorkers.Contains(typeof(PlaceWorker_PlantReactionTable)))
            {
                IntVec3 intVec = PortPosition(thingDef, loc, rot);
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        IntVec3 c = loc + new IntVec3(i, 0, j);
                        if (!c.InBounds(map))
                        {
                            continue;
                        }
                        foreach (Thing item in map.thingGrid.ThingsListAtFast(c))
                        {
                            if (item != thingToIgnore)
                            {
                                ThingDef thingDef2 = item.def;
                                if (item.def.entityDefToBuild != null)
                                {
                                    thingDef2 = item.def.entityDefToBuild as ThingDef;
                                }
                                if (thingDef2 != null && thingDef2?.placeWorkers?.Contains(typeof(PlaceWorker_PlantReactionTable)) == true && PortPosition(thingDef2, item.Position, item.Rotation) == intVec)
                                {
                                    return new AcceptanceReport(((item.def.entityDefToBuild == null) ? "InteractionSpotOverlaps" : "InteractionSpotWillOverlap").Translate(item.LabelNoCount, item));
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
        */
    }
    
    ///<summary>在目标物体的面前绘制互动图标。</summary>
    [StaticConstructorOnStartup]
    public class PlaceWorker_FacingPort : PlaceWorker
    {
        private static readonly Material PortCellMaterial = MaterialPool.MatFrom("UI/Overlays/InteractionCell", ShaderDatabase.Transparent);

        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            DrawPortCell(PortPosition(def, center, rot));
        }
        public static IntVec3 PortPosition(ThingDef def, IntVec3 center, Rot4 rot)
        {
            if (rot.AsByte == 0)
                return center + new IntVec3(0, 0, -1 - (def.size.z / 2));
            else if (rot.AsByte == 1)
                return center + new IntVec3(-1 - (def.size.z / 2), 0, 0);
            else if (rot.AsByte == 2)
                return center + new IntVec3(0, 0, 1 + (def.size.z / 2));
            else if (rot.AsByte == 3)
                return center + new IntVec3(1 + (def.size.z / 2), 0, 0);
            else return IntVec3.Invalid;
        }
        public static void DrawPortCell(IntVec3 center)
        {
            Map currentMap = Find.CurrentMap;
            if (center.Standable(currentMap))
            {
                Vector3 position = center.ToVector3ShiftedWithAltitude(AltitudeLayer.MetaOverlays);
                Graphics.DrawMesh(MeshPool.plane10, position, Quaternion.identity, PortCellMaterial, 0);
            }
        }
    }
}
