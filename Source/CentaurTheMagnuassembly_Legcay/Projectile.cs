using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using static CentaurTheMagnuassembly.RimCentaurCore;

namespace CentaurTheMagnuassembly
{
    public class Projectile_Explosive_Teleshot : Projectile_Explosive_RoofBypass
    {
        //TODO: Fix Teleport
        protected override void Explode()
        {
            if (launcher is Pawn)
            {
                Map map = Map;
                IntVec3 pos = Position;
                if (launcher.Map.uniqueID == map.uniqueID)
                {
                    if (TeleportPawn(launcher as Pawn, pos))
                    {
                        //map.fogGrid.Unfog(launcher.Position);
                        map.fogGrid.Notify_FogBlockerRemoved(launcher.Position);
                    }
                }
            }
            base.Explode();
        }
        /*public override void Tick()
        {
            base.Tick();
            if (!landed && intendedTarget.HasThing && intendedTarget.Thing.Map.uniqueID == Map.uniqueID)
            {
                origin = Position.ToVector3();
                destination = intendedTarget.Thing.Position.ToVector3();
            }
        }*/
        /*private void TickBASEBASE()
        {
            if (AllComps != null)
            {
                int i = 0;
                int count = AllComps.Count;
                while (i < count)
                {
                    AllComps[i].CompTick();
                    i++;
                }
            }
        }*/
    }
    public class Projectile_Explosive_Spotshot : Projectile_Explosive
    {
        protected override void Explode()
        {
            Map.fogGrid.Unfog(Position);
            base.Explode();
        }
    }
    public class Projectile_Explosive_Waggingshot : Projectile_Explosive
    {
        static Random Randy = new Random();
        public override void Tick()
        {
            base.Tick();
            if (!landed)
            {
                ticksToImpact = Math.Max(ticksToImpact + Randy.Next(-1, 1), 0);
            }
        }
    }

}
