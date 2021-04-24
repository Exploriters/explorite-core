/********************
 * 藏着一把三射弓的电池的部件类。
 * --siiftun1857
 */
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
    ///<summary>为<see cref = "CompSecretTrishot" />接收参数。</summary>
    public abstract class CompProperties_SecretTrishot : CompProperties
    {
        public CompProperties_SecretTrishot()
        {
            compClass = typeof(CompSecretTrishot);
        }
    }
    ///<summary>电池里藏着一把三射弓。</summary>
    public abstract class CompSecretTrishot : ThingComp
    {
        public bool includingBrokenTrishot = false;
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref includingBrokenTrishot, "isSecretBattery", false);
        }
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if (includingBrokenTrishot)
            {
                GenSpawn.Spawn(ThingMaker.MakeThing(TrishotThing1Def), parent.Position, previousMap);
            }
        }
        public bool Secret(bool boolen)
        {
            return includingBrokenTrishot = boolen;
        }
    }

}
