/**
 * 使生物具有该效果时，在死亡后生成物体。
 * 
 * --siiftun1857
 */
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Explorite
{
    /**
     * <summary>为<see cref = "HediffComp_SpawnThingOnDeath" />接收参数。</summary>
     */
    public class HediffCompProperties_SpawnThingOnDeath : HediffCompProperties_DissolveGearOnDeath
    {
        public ThingDef thing;
        public ThingDef thingStuff = null;
        public int thingCount = 1;
        public HediffCompProperties_SpawnThingOnDeath()
        {
            compClass = typeof(HediffComp_SpawnThingOnDeath);
        }
    }
    /**
     * <summary>使生物具有该效果时，在死亡后生成物体。</summary>
     */
    public class HediffComp_SpawnThingOnDeath : HediffComp
    {
        public HediffCompProperties_SpawnThingOnDeath Props => (HediffCompProperties_SpawnThingOnDeath)props;
        public override void Notify_PawnDied()
        {
            base.Notify_PawnDied();
            if (Props.injuryCreatedOnDeath != null)
            {
                List<BodyPartRecord> list = new List<BodyPartRecord>(Pawn.RaceProps.body.AllParts.Where((BodyPartRecord part) => part.coverageAbs > 0f && !Pawn.health.hediffSet.PartIsMissing(part)));
                int num = Mathf.Min(Props.injuryCount.RandomInRange, list.Count);
                for (int i = 0; i < num; i++)
                {
                    int index = Rand.Range(0, list.Count);
                    BodyPartRecord part2 = list[index];
                    list.RemoveAt(index);
                    Pawn.health.AddHediff(Props.injuryCreatedOnDeath, part2);
                }
            }
        }

        public override void Notify_PawnKilled()
        {
            base.Notify_PawnKilled();
            //base.Pawn.equipment.DestroyAllEquipment();
            //base.Pawn.apparel.DestroyAll();
            if (!Pawn.Spawned)
            {
                return;
            }
            if (Props.mote != null)
            {
                Vector3 drawPos = Pawn.DrawPos;
                for (int i = 0; i < Props.moteCount; i++)
                {
                    Vector2 vector = Rand.InsideUnitCircle * Props.moteOffsetRange.RandomInRange * Rand.Sign;
                    MoteMaker.MakeStaticMote(new Vector3(drawPos.x + vector.x, drawPos.y, drawPos.z + vector.y), Pawn.Map, Props.mote);
                }
            }
            if (Props.filth != null)
            {
                FilthMaker.TryMakeFilth(Pawn.Position, Pawn.Map, Props.filth, Props.filthCount);
            }
            if (Props.sound != null)
            {
                Props.sound.PlayOneShot(SoundInfo.InMap(Pawn));
            }
            if (Props.thing != null)
            {
                Thing thing = ThingMaker.MakeThing(Props.thing, Props.thingStuff);
                thing.stackCount = Props.thingCount;
                GenSpawn.Spawn(thing, Pawn.Position, Pawn.Map, WipeMode.FullRefund);
            }


        }
    }

}
