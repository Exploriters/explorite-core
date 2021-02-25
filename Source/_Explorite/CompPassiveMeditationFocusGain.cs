/********************
 * 使生物持续获取精神力。
 * --siiftun1857
 */
using Verse;

namespace Explorite
{
    ///<summary>为<see cref = "CompPassiveMeditationFocusGain" />接收参数。</summary>
    public class CompProperties_PassiveMeditationFocusGain : CompProperties
    {
        public float focusPerDay = 0f;
        public CompProperties_PassiveMeditationFocusGain()
        {
            compClass = typeof(CompPassiveMeditationFocusGain);
        }
    }

    ///<summary>使生物持续获取精神力。</summary>
    public class CompPassiveMeditationFocusGain : ThingComp
    {
        private float? focusPerTick = null;
        public float FocusPerTick => focusPerTick ??= ((CompProperties_PassiveMeditationFocusGain)props).focusPerDay / 60000;
        public override void CompTick()
        {
            base.CompTick();
            ((Pawn)parent).psychicEntropy.OffsetPsyfocusDirectly(FocusPerTick);
        }
    }
}
