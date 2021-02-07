/**
 * 使生物具有该效果时，在死亡后生成血肉之树。
 * 
 * --siiftun1857
 */
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
using static Explorite.ExploriteCore;

namespace Explorite
{
    /**
     * <summary>为<see cref = "HediffComp_FleshSeedParasitized" />接收参数。</summary>
     */
    public class HediffCompProperties_FleshSeedParasitized : HediffCompProperties_DissolveGearOnDeath
    {
        public HediffCompProperties_FleshSeedParasitized()
        {
            compClass = typeof(HediffComp_FleshSeedParasitized);
        }
    }
    /**
     * <summary>使生物具有该效果时，在死亡后生成血肉之树。</summary>
     */
    public class HediffComp_FleshSeedParasitized : HediffComp
    {
        public HediffCompProperties_FleshSeedParasitized Props => (HediffCompProperties_FleshSeedParasitized)props;
        public override void Notify_PawnDied()
        {
            base.Notify_PawnDied();
            if (Props.injuryCreatedOnDeath != null)
            {
                List<BodyPartRecord> list = new List<BodyPartRecord>(base.Pawn.RaceProps.body.AllParts.Where((BodyPartRecord part) => part.coverageAbs > 0f && !base.Pawn.health.hediffSet.PartIsMissing(part)));
                int num = Mathf.Min(Props.injuryCount.RandomInRange, list.Count);
                for (int i = 0; i < num; i++)
                {
                    int index = Rand.Range(0, list.Count);
                    BodyPartRecord part2 = list[index];
                    list.RemoveAt(index);
                    base.Pawn.health.AddHediff(Props.injuryCreatedOnDeath, part2);
                }
            }
        }

        public override void Notify_PawnKilled()
        {
            base.Notify_PawnKilled();
            //base.Pawn.equipment.DestroyAllEquipment();
            //base.Pawn.apparel.DestroyAll();
            if (!base.Pawn.Spawned)
            {
                return;
            }
            if (Props.mote != null)
            {
                Vector3 drawPos = base.Pawn.DrawPos;
                for (int i = 0; i < Props.moteCount; i++)
                {
                    Vector2 vector = Rand.InsideUnitCircle * Props.moteOffsetRange.RandomInRange * Rand.Sign;
                    MoteMaker.MakeStaticMote(new Vector3(drawPos.x + vector.x, drawPos.y, drawPos.z + vector.y), base.Pawn.Map, Props.mote);
                }
            }
            if (Props.filth != null)
            {
                FilthMaker.TryMakeFilth(base.Pawn.Position, base.Pawn.Map, Props.filth, Props.filthCount);
            }
            if (Props.sound != null)
            {
                Props.sound.PlayOneShot(SoundInfo.InMap(base.Pawn));
            }

            Thing thingTree = ThingMaker.MakeThing(FleshTreeDef);
            GenSpawn.Spawn(thingTree, Pawn.Position, Pawn.Map, WipeMode.FullRefund);
        }
    }

}
