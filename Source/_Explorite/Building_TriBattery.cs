/**
 * 三联电池使用的建筑物类。
 * --siiftun1857
 */
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Explorite
{
    /**
     * <summary>三联电池使用的建筑物类，负责处理视觉效果和爆炸性。<br />不继承自<seealso cref = "Building_Battery" />，因该类并未有独有方法，且部分行为不可被覆盖。</summary>
     */
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, "IDE0051")]
    [StaticConstructorOnStartup]
    public class Building_TriBattery : Building//_Battery
    {
        private int ticksToExplode;

        private Sustainer wickSustainer;

        private static readonly Vector2 BarSize = new Vector2(2.3f, 1.4f);// new Vector2(1.3f, 0.4f);

        private const float MinEnergyToExplode = 1500f;

        private const float EnergyToLoseWhenExplode = 1200f;

        private const float ExplodeChancePerDamage = 0.05f;

        private static readonly Material BatteryBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.9f, 0.85f, 0.2f), false);

        private static readonly Material BatteryBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f), false);

        public Building_TriBattery()
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksToExplode, "ticksToExplode", 0, false);
        }

        public override void Draw()
        {
            base.Draw();
            CompPowerBattery comp = GetComp<CompPowerBattery>();
            GenDraw.FillableBarRequest r = default;
            r.center = DrawPos + (Vector3.up * 0.1f);
            r.size = BarSize;
            r.fillPercent = comp.StoredEnergy / comp.Props.storedEnergyMax;
            r.filledMat = BatteryBarFilledMat;
            r.unfilledMat = BatteryBarUnfilledMat;
            r.margin = 0.15f;
            Rot4 rotation = Rotation;
            rotation.Rotate(RotationDirection.Clockwise);
            r.rotation = rotation;
            GenDraw.DrawFillableBar(r);
            if (ticksToExplode > 0 && Spawned)
            {
                Map.overlayDrawer.DrawOverlay(this, OverlayTypes.BurningWick);
            }
        }

        public override void Tick()
        {
            base.Tick();
            //BackstoryCracker.TestforIncorrectChildhood();
            if (ticksToExplode > 0)
            {
                if (wickSustainer == null)
                {
                    StartWickSustainer();
                }
                else
                {
                    wickSustainer.Maintain();
                }
                ticksToExplode--;
                if (ticksToExplode == 0)
                {
                    IntVec3 randomCell = this.OccupiedRect().RandomCell;
                    float radius = Rand.Range(0.5f, 1f) * 3f * 1.7320508075688772935274463415059f;
                    GenExplosion.DoExplosion(randomCell, Map, radius, DamageDefOf.Flame, null, -1, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
                    GetComp<CompPowerBattery>().DrawPower(400f);
                }
            }
        }

        public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostApplyDamage(dinfo, totalDamageDealt);
            if (!Destroyed && ticksToExplode == 0 && dinfo.Def == DamageDefOf.Flame && Rand.Value < 0.05f && GetComp<CompPowerBattery>().StoredEnergy > 500f)
            {
                ticksToExplode = Rand.Range(70, 150);
                StartWickSustainer();
            }
        }

        private void StartWickSustainer()
        {
            SoundInfo info = SoundInfo.InMap(this, MaintenanceType.PerTick);
            wickSustainer = SoundDefOf.HissSmall.TrySpawnSustainer(info);
        }

        // Note: this type is marked as 'beforefieldinit'.
        //static Building_TriBattery() { }
    }
}
