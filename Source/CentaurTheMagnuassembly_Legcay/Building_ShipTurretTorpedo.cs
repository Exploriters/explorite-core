using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
//using Harmony;
using HarmonyLib;
using UnityEngine;
using Verse.AI;
using Verse;
using Verse.Sound;
using static CentaurTheMagnuassembly.RimCentaurCore;

namespace CentaurTheMagnuassembly
{
    [StaticConstructorOnStartup]
    class Building_ShipTurretTorpedoSpinal : Building_ShipTurret
    {
        public static Graphic torpedoBayDoor = GraphicDatabase.Get(typeof(Graphic_Single), "Things/Building/Ship/SpinalMountBarrel_Missile_Door", ShaderDatabase.Cutout, new Vector2(6, 7), Color.white, Color.white);
        public static Mesh doorOne = MeshMakerPlanes.NewPlaneMesh(new Vector2(5, 7.35f), false, false, false);
        public static Mesh doorTwo = MeshMakerPlanes.NewPlaneMesh(new Vector2(5, 7.35f), true, false, false);
        float ticksSinceOpen = 0;
        float TicksToOpenNow = 60;

        int timesFired = 0;
        static Vector3[] TubePos = { new Vector3(-1, 0, -1.5f), new Vector3(1, 0, -1.5f), new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(-1, 0, 1.5f), new Vector3(1, 0, 1.5f) };

        public static Dictionary<Map, List<Building_ShipTurretTorpedoSpinal>> allTubesOnMap = new Dictionary<Map, List<Building_ShipTurretTorpedoSpinal>>();

        protected override void SetDefaultPowerDistribution(int basePower)
        {
            SegmentPower[0] = basePower;
            SegmentPower[1] = basePower;
            SegmentPower[2] = basePower;
        }

        public override void Draw()
        {
            base.Draw();
            float num = -3.5f * Mathf.Clamp01((float)ticksSinceOpen / (float)TicksToOpenNow);
            float d = 0f + 0f * num;
            for (int i = 0; i < 2; i++)
            {
                Vector3 vector = default(Vector3);
                Mesh mesh;
                if (i == 0)
                {
                    vector = new Vector3(0f, 0f, 0f);
                    mesh = doorOne;
                }
                else
                {
                    vector = new Vector3(0f, 0f, 0f);
                    mesh = doorTwo;
                }
                Rot4 rotation = base.Rotation;
                rotation.Rotate(RotationDirection.Clockwise);
                vector = rotation.AsQuat * vector;
                Vector3 drawPos = DrawPos;
                drawPos.y = AltitudeLayer.MapDataOverlay.AltitudeFor();
                drawPos += vector * d;
                if (i == 0 || def.size.x > 3)
                {
                    Graphics.DrawMesh(mesh, drawPos, base.Rotation.AsQuat, torpedoBayDoor.MatSingle, 0);
                }
            }
        }

        public override void Tick()
        {
            base.Tick();
            if (ShipCombatManager.InCombat)
            {
                if (ticksSinceOpen < TicksToOpenNow)
                    ticksSinceOpen++;
            }
            else
            {
                if (ticksSinceOpen > 0)
                    ticksSinceOpen--;
            }
        }

        public Vector3 TorpedoTubePos()
        {
            Vector3 output;
            output = TubePos[timesFired % 6];
            timesFired++;
            return output;
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!allTubesOnMap.ContainsKey(Map))
            {
                allTubesOnMap.Add(Map, new List<Building_ShipTurretTorpedoSpinal>());
            }
            allTubesOnMap[Map].Add(this);
        }

        public override void DeSpawn(DestroyMode mode)
        {
            allTubesOnMap[Map].Remove(this);
            base.DeSpawn(mode);
        }
    }
}
