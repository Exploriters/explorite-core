/********************
 * 该文件包含多个弹射物。
 * --siiftun1857
 */
using System;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
    ///<summary>该弹射物会将目标传送至爆炸位置。</summary>
	 // TODO: 需要修复传送失败问题。
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
    ///<summary>该弹射物会探查目的地区域。</summary>
    public class Projectile_Explosive_Spotshot : Projectile_Explosive
    {
        protected override void Explode()
        {
            Map.fogGrid.Unfog(Position);
            base.Explode();
        }
    }
    ///<summary>该弹射物会在空中随机加速或减速，大量发射时之间会错开。</summary>
    public class Projectile_Explosive_Waggingshot : Projectile_Explosive
    {
        static readonly Random Randy = new Random();
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
