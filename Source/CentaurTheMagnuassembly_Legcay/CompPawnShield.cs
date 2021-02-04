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
    class CompProperties_PawnShield : CompProperties
    {
        public CompProperties_PawnShield()
        {
            this.compClass = typeof(CompPawnShield);
        }
    }

    [StaticConstructorOnStartup]
    public class CompPawnShield : ThingComp
    {
        Color currentColor = new Color(0.5f, 0.5f, 0.5f);

        public override void CompTick()
        {
            base.CompTick();
            if (invalid)
            {
                ticksToReset = StartingTicksToReset;
                energy = Math.Min(0f, energy);
                pendingCharge = 0;
                return;
            }
            pendingCharge += EnergyGainPerTick;
            if (pendingCharge >= fragemntPerCharge || pendingCharge > EnergyMax - energy)
            {
                energy = Math.Min(energy + fragemntPerCharge, EnergyMax);
                if (energy > 0 && ShieldState == ShieldState.Resetting)
                {
                    Reset();
                }
                else if (ShieldState == ShieldState.Active)
                {
                }
                pendingCharge -= fragemntPerCharge;
            }
            pendingCharge = Math.Min(pendingCharge, EnergyMax - energy);
        }

        public float energy = 0f;

        public float pendingCharge = 0f;

        public bool overkilled = false;
        public float fragemntPerCharge => EnergyGainPerSec;
        public float chargePrecent => Math.Min(1, Math.Max(0, pendingCharge /EnergyGainPerSec ));
        public float energyPrecent => Math.Min(1, Math.Max(0, energy / EnergyMax));

        public int ticksToReset = -1;

        public int lastKeepDisplayTick = -9999;

        public Vector3 impactAngleVect;

        public int lastAbsorbDamageTick = -9999;

        public int StartingTicksToReset = 3200;

        public float EnergyOnReset => fragemntPerCharge;//0.2f;

        public float EnergyLossPerDamage = 0.03f;//0.033f;

        public int KeepDisplayingTicks = 1000;

        public static readonly Material BubbleMat = MaterialPool.MatFrom("Other/ShieldBubble", ShaderDatabase.Transparent);

        public float EnergyMax => parent.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true);

        public float EnergyGainPerSec => parent.GetStatValue(StatDefOf.EnergyShieldRechargeRate, true);
        public float EnergyGainPerTick => EnergyGainPerSec / 60f;

        public float Energy => energy;

        public bool invalid => EnergyMax <= 0;

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
                if (invalid)
                {
                    return false;
                }
                Pawn wearer = parent as Pawn;
                if (!wearer.Spawned || wearer.Dead || wearer.Downed)
                {
                    return false;
                }
                if (wearer.InAggroMentalState)
                {
                    return true;
                }
                if (wearer.Drafted)
                {
                    return true;
                }
                if (wearer.Faction.HostileTo(Faction.OfPlayer) && !wearer.IsPrisoner)
                {
                    return true;
                }
                if (Find.TickManager.TicksGame < lastKeepDisplayTick + KeepDisplayingTicks)
                {
                    return true;
                }
                if (wearer.playerSettings != null && wearer.playerSettings.RespectedMaster != null && wearer.playerSettings.followDrafted && wearer.playerSettings.RespectedMaster.Drafted)
                {
                    return true;
                }
                return false;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref energy, "energy", 0f);
            Scribe_Values.Look(ref ticksToReset, "ticksToReset", -1);
            Scribe_Values.Look(ref lastKeepDisplayTick, "lastKeepDisplayTick", 0);
            Scribe_Values.Look(ref pendingCharge, "pendingCharge", 0f);
            Scribe_Values.Look(ref overkilled, "overkilled", false);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo wornGizmo in base.CompGetGizmosExtra())
            {
                yield return wornGizmo;
            }
            if (Find.Selector.SingleSelectedThing == parent)
            {
                Gizmo_EnergyShieldStatusPawn gizmo_EnergyShieldStatus = new Gizmo_EnergyShieldStatusPawn();
                gizmo_EnergyShieldStatus.shield = this;
                yield return gizmo_EnergyShieldStatus;
            }
        }

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            if (invalid)
            {
                absorbed = false;
                return;
            }
            if (ShieldState != ShieldState.Active)
            {
                if (dinfo.Def == DamageDefOf.EMP)
                {
                    absorbed = true;
                    return;
                }
                absorbed = false;
                return;
            }
            /*if (dinfo.Def == DamageDefOf.EMP)
            {
                energy = 0f;
                Break();
                absorbed = false;
                return;
            }*/
            if (
                    dinfo.Def.isRanged
                ||  dinfo.Def.isExplosive
                ||  dinfo.Def == DamageDefOf.EMP
                )
            {
                if (dinfo.Def.isRanged || dinfo.Def.isExplosive)
                    energy -= dinfo.Amount * EnergyLossPerDamage;
                else if (dinfo.Def == DamageDefOf.EMP)
                    energy -= dinfo.Amount * EnergyLossPerDamage * 3;
                else
                    energy -= dinfo.Amount * EnergyLossPerDamage * 10;
                if (energy <= 0f)
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
            absorbed = false;
            return;
        }

        public void KeepDisplaying()
        {
            lastKeepDisplayTick = Find.TickManager.TicksGame;
        }

        private void AbsorbedDamage(DamageInfo dinfo)
        {
            SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(parent.Position, parent.Map));
            impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
            Vector3 loc = parent.TrueCenter() + impactAngleVect.RotatedBy(180f) * 0.5f;
            float num = Mathf.Min(10f, 2f + dinfo.Amount / 10f);
            MoteMaker.MakeStaticMote(loc, parent.Map, ThingDefOf.Mote_ExplosionFlash, num);
            int num2 = (int)num;
            for (int i = 0; i < num2; i++)
            {
                MoteMaker.ThrowDustPuff(loc, parent.Map, Rand.Range(0.8f, 1.2f));
            }
            lastAbsorbDamageTick = Find.TickManager.TicksGame;
            KeepDisplaying();
        }

        private void Break()
        {
            float overkill = Math.Min(EnergyMax * 10 / 3, -energy);
            if (overkill >= fragemntPerCharge)
            { 
                SoundDefOf.EnergyShield_Broken.PlayOneShot(new TargetInfo(parent.Position, parent.Map));
                MoteMaker.MakeStaticMote(parent.TrueCenter(), parent.Map, ThingDefOf.Mote_ExplosionFlash, 12f);
                overkilled = true;
            }
            else
                SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(parent.Position, parent.Map));
            for (int i = 0; i < 6; i++)
            {
                MoteMaker.ThrowDustPuff(parent.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle(Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f), parent.Map, Rand.Range(0.8f, 1.2f));
            }
            //energy = 0f;
            energy = -Math.Min(EnergyMax * 10 / 3, overkill * 3);
            pendingCharge = 0f;
            ticksToReset = StartingTicksToReset;
        }

        private void Reset()
        {
            if (parent.Spawned && overkilled)
            {
                SoundDefOf.EnergyShield_Reset.PlayOneShot(new TargetInfo(parent.Position, parent.Map));
                MoteMaker.ThrowLightningGlow(parent.TrueCenter(), parent.Map, 3f);
            }
            ticksToReset = -1;
            overkilled = false;
            //energy = EnergyOnReset;
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (ShieldState == ShieldState.Active && ShouldDisplay)
            {
                float num = Mathf.Lerp(1.2f, 1.55f, energy);
                Vector3 drawPos = parent.DrawPos;
                drawPos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
                int num2 = Find.TickManager.TicksGame - lastAbsorbDamageTick;
                if (num2 < 8)
                {
                    float num3 = (float)(8 - num2) / 8f * 0.05f;
                    drawPos += impactAngleVect * num3;
                    num -= num3;
                }
                float angle = Rand.Range(0, 360);
                Vector3 s = new Vector3(num, 1f, num);
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(drawPos, Quaternion.AngleAxis(angle, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, BubbleMat, 0);
            }
        }
    }
    [StaticConstructorOnStartup]
    public class Gizmo_EnergyShieldStatusPawn : Gizmo
    {
        public CompPawnShield shield;
        private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));
        private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
        public bool valid => !shield.invalid;
        public Gizmo_EnergyShieldStatusPawn() => order = -2000f;
        public override float GetWidth(float maxWidth)=> valid?140f:0f;

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
            if (valid)
            {
                Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
                Rect rect2 = rect.ContractedBy(6f);
                Widgets.DrawWindowBackground(rect);
                Rect rect3 = rect2;
                rect3.height = rect.height / 2f;
                Text.Font = GameFont.Tiny;
                Widgets.Label(rect3, ((Pawn)shield.parent)?.Name?.ToStringShort);
                Rect rect4 = rect2;
                rect4.yMin = rect2.y + rect2.height / 2f;
                Widgets.FillableBar(rect4, shield.energyPrecent, FullShieldBarTex, EmptyShieldBarTex, doBorder: false);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect4, (shield.Energy * 100f).ToString("F0") + " / " + (shield.EnergyMax * 100f).ToString("F0"));
                Text.Anchor = TextAnchor.UpperLeft;
            }
            return new GizmoResult(GizmoState.Clear);
        }
    }
}
