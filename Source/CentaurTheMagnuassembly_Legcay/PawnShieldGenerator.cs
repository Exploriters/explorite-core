using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;


namespace CentaurTheMagnuassembly
{
    /*
    public class HediffCompProperties_ShieldGenerator : HediffCompProperties
    {
        public float shield;

        public HediffCompProperties_ShieldGenerator()
        {
            this.compClass = typeof(CompPawnShieldGen);
        }
    }
    [StaticConstructorOnStartup]
    public class CompPawnShieldGen : HediffComp
    {
    }
    */
    /*
    [StaticConstructorOnStartup]
    public class PawnWithShield : Pawn
    {
        private float energy;

        private int ticksToReset = -1;

        private int lastKeepDisplayTick = -9999;

        private Vector3 impactAngleVect;

        private int lastAbsorbDamageTick = -9999;

        private const float MinDrawSize = 1.2f;

        private const float MaxDrawSize = 1.55f;

        private const float MaxDamagedJitterDist = 0.05f;

        private const int JitterDurationTicks = 8;

        private int StartingTicksToReset = 3200;

        private float EnergyOnReset = 0.2f;

        private float EnergyLossPerDamage = 0.033f;

        private int KeepDisplayingTicks = 1000;

        private float ApparelScorePerEnergyMax = 0.25f;

        private static readonly Material BubbleMat = MaterialPool.MatFrom("Other/ShieldBubble", ShaderDatabase.Transparent);

        private float EnergyMax => this.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true);

        private float EnergyGainPerTick => this.GetStatValue(StatDefOf.EnergyShieldRechargeRate, true) / 60f;

        public float Energy => energy;

        public ShieldState ShieldState
        {
            get
            {
                if (ticksToReset > 0)
                {
                    return ShieldState.Resetting;
                }
                return ShieldState.Active;
            }
        }

        private bool ShouldDisplay
        {
            get
            {
                if (this.Spawned && !this.Dead && !this.Downed)
                {
                    if (this.InAggroMentalState)
                    {
                        return true;
                    }
                    if (this.Drafted)
                    {
                        return true;
                    }
                    if (this.Faction.HostileTo(Faction.OfPlayer) && !this.IsPrisoner)
                    {
                        return true;
                    }
                    if (Find.TickManager.TicksGame < lastKeepDisplayTick + KeepDisplayingTicks)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref energy, "energy", 0f, false);
            Scribe_Values.Look(ref ticksToReset, "ticksToReset", -1, false);
            Scribe_Values.Look(ref lastKeepDisplayTick, "lastKeepDisplayTick", 0, false);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            base.GetGizmos();
            if (Find.Selector.SingleSelectedThing != this)
            {
                yield break;
            }
            yield return (Gizmo)new Gizmo_RangedShieldStatus(this);
        }

        / *public override float GetSpecialApparelScoreOffset()
        {
            return EnergyMax * ApparelScorePerEnergyMax;
        }* /

        public override void Tick()
        {
            base.Tick();
            if (this == null)
            {
                energy = 0f;
            }
            else if (ShieldState == ShieldState.Resetting)
            {
                ticksToReset--;
                if (ticksToReset <= 0)
                {
                    Reset();
                }
            }
            else if (ShieldState == ShieldState.Active)
            {
                energy += EnergyGainPerTick;
                if (energy > EnergyMax)
                {
                    energy = EnergyMax;
                }
            }
        }

        public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            base.PreApplyDamage(ref dinfo, out absorbed);
            if (absorbed)
                return;
            if (ShieldState != 0)
            {
                absorbed = false;
                return;
            }
            if (dinfo.Def == DamageDefOf.EMP)
            {
                energy = 0f;
                Break();
                absorbed = false;
                return;
            }
            if (!dinfo.Def.isRanged && !dinfo.Def.isExplosive)
            {
                absorbed = false;
                return;
            }
            energy -= dinfo.Amount * EnergyLossPerDamage;
            if (energy < 0f)
            {
                Break();
            }
            else
            {
                AbsorbedDamage(dinfo);
            }
            absorbed = true;
            return;
        }

        private void KeepDisplaying()
        {
            lastKeepDisplayTick = Find.TickManager.TicksGame;
        }

        private void AbsorbedDamage(DamageInfo dinfo)
        {
            SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(this.Position, this.Map, false));
            impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
            Vector3 loc = this.TrueCenter() + impactAngleVect.RotatedBy(180f) * 0.5f;
            float num = Mathf.Min(10f, 2f + dinfo.Amount / 10f);
            MoteMaker.MakeStaticMote(loc, this.Map, ThingDefOf.Mote_ExplosionFlash, num);
            int num2 = (int)num;
            for (int i = 0; i < num2; i++)
            {
                MoteMaker.ThrowDustPuff(loc, this.Map, Rand.Range(0.8f, 1.2f));
            }
            lastAbsorbDamageTick = Find.TickManager.TicksGame;
            KeepDisplaying();
        }

        private void Break()
        {
            SoundDefOf.EnergyShield_Broken.PlayOneShot(new TargetInfo(this.Position, this.Map, false));
            MoteMaker.MakeStaticMote(this.TrueCenter(), this.Map, ThingDefOf.Mote_ExplosionFlash, 12f);
            for (int i = 0; i < 6; i++)
            {
                Vector3 loc = this.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f);
                MoteMaker.ThrowDustPuff(loc, this.Map, Rand.Range(0.8f, 1.2f));
            }
            energy = 0f;
            ticksToReset = StartingTicksToReset;
        }

        private void Reset()
        {
            if (this.Spawned)
            {
                SoundDefOf.EnergyShield_Reset.PlayOneShot(new TargetInfo(this.Position, this.Map, false));
                MoteMaker.ThrowLightningGlow(this.TrueCenter(), this.Map, 3f);
            }
            ticksToReset = -1;
            energy = EnergyOnReset;
        }

        public override void DrawExtraSelectionOverlays()
        {
            base.DrawExtraSelectionOverlays();
            if (ShieldState == ShieldState.Active && ShouldDisplay)
            {
                float num = Mathf.Lerp(1.2f, 1.55f, energy);
                Vector3 vector = this.Drawer.DrawPos;
                vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();
                int num2 = Find.TickManager.TicksGame - lastAbsorbDamageTick;
                if (num2 < 8)
                {
                    float num3 = (float)(8 - num2) / 8f * 0.05f;
                    vector += impactAngleVect * num3;
                    num -= num3;
                }
                float angle = (float)Rand.Range(0, 360);
                Vector3 s = new Vector3(num, 1f, num);
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, BubbleMat, 0);
            }
        }

        / *public override bool AllowVerbCast(IntVec3 root, Map map, LocalTargetInfo targ, Verb v)
        {
            return true;
        }* /
    }
    [StaticConstructorOnStartup]
    class Gizmo_RangedShieldStatus : Gizmo
    {
        public readonly PawnWithShield shield;

        private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));

        private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

        public Gizmo_RangedShieldStatus()
        {
            base.order = -2000f;
        }
        public Gizmo_RangedShieldStatus(PawnWithShield shield)
        {
            this.shield = shield;
            base.order = -2000f;
        }

        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
            Rect overRect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Find.WindowStack.ImmediateWindow(984688, overRect, WindowLayer.GameUI, delegate
            {
                Rect rect = overRect.AtZero().ContractedBy(6f);
                Rect rect2 = rect;
                rect2.height = overRect.height / 2f;
                Text.Font = GameFont.Tiny;
                Widgets.Label(rect2, shield.Name.ToStringShort);
                Rect rect3 = rect;
                rect3.yMin = overRect.height / 2f;
                float fillPercent = shield.Energy / Mathf.Max(1f, shield.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true));
                Widgets.FillableBar(rect3, fillPercent, FullShieldBarTex, EmptyShieldBarTex, false);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect3, (shield.Energy * 100f).ToString("F0") + " / " + (shield.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true) * 100f).ToString("F0"));
                Text.Anchor = TextAnchor.UpperLeft;
            }, true, false, 1f);
            return new GizmoResult(GizmoState.Clear);
        }
    }
    */
}