/**
 * 发射后直接命中目标的子弹。
 * --siiftun1857
 */
using RimWorld;
using Verse;

namespace Explorite
{
    ///<summary>发射后直接命中目标的子弹。</summary>
    public class Bullet_Direct : Bullet
    {
        public override void Tick()
        {
            if (AllComps != null)
            {
                int i = 0;
                for (int count = AllComps.Count; i < count; i++)
                {
                    AllComps[i].CompTick();
                }
            }
            Impact(intendedTarget.Thing as Pawn ?? intendedTarget.Pawn ?? null);
            //if (!Destroyed) Destroy();
        }
    }
}
