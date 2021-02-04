/*****************************************
 * Code from 
 *      https://github.com/RimWorld-CCL-Reborn/JecsTools
 *      https://github.com/RimWorld-CCL-Reborn/JecsTools/tree/master/Source/AllModdingComponents/CompOversizedWeapon
 * 
 * remaster by siiftun1857
 */
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
    public class CompProperties_OversizedWeapon : CompProperties
    {
        public CompProperties_OversizedWeapon() : base( typeof(CompOversizedWeapon) )
        { }
    }
    public class CompOversizedWeapon : ThingComp
    {
        public CompProperties_OversizedWeapon Props => props as CompProperties_OversizedWeapon;

        public CompOversizedWeapon()
        {
            if (!(props is CompProperties_OversizedWeapon))
                props = new CompProperties_OversizedWeapon();
        }


        public CompEquippable GetEquippable => parent?.GetComp<CompEquippable>();

        public Pawn GetPawn => GetEquippable?.verbTracker?.PrimaryVerb?.CasterPawn;

        private bool isEquipped = false;
        public bool IsEquipped
        {
            get
            {
                if (Find.TickManager.TicksGame % 60 != 0) return isEquipped;
                isEquipped = GetPawn != null;
                return isEquipped;
            }
        }

        private bool firstAttack = false;
        public bool FirstAttack
        {
            get => firstAttack;
            set => firstAttack = value;
        }
    }

    [StaticConstructorOnStartup]
    internal static class HarmonyCompOversizedWeapon
    {
        static HarmonyCompOversizedWeapon()
        {
            var harmony = new Harmony("CentaurTheMagnuassembly.rimworld.mod.OversizedWeaponGraphicPatch");
            harmony.Patch(typeof(PawnRenderer).GetMethod("DrawEquipmentAiming"),
                new HarmonyMethod(typeof(HarmonyCompOversizedWeapon).GetMethod("DrawEquipmentAimingPreFix")), null);
            harmony.Patch(AccessTools.Method(typeof(Thing), "get_DefaultGraphic"), null,
                new HarmonyMethod(typeof(HarmonyCompOversizedWeapon), nameof(get_Graphic_PostFix)));
        }


        /// <summary>
        ///     Adds another "layer" to the equipment aiming if they have a
        ///     weapon with a CompActivatableEffect.
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="eq"></param>
        /// <param name="drawLoc"></param>
        /// <param name="aimAngle"></param>
        public static bool DrawEquipmentAimingPreFix(PawnRenderer __instance, Thing eq, Vector3 drawLoc, float aimAngle)
        {
            if (eq is ThingWithComps thingWithComps)
            {
                //If the deflector is active, it's already using this code.
                var deflector = thingWithComps.AllComps.FirstOrDefault(y =>
                    y.GetType().ToString() == "CompDeflector.CompDeflector" ||
                    y.GetType().BaseType.ToString() == "CompDeflector.CompDeflector");
                if (deflector != null)
                {
                    var isAnimatingNow = Traverse.Create(deflector).Property("IsAnimatingNow").GetValue<bool>();
                    if (isAnimatingNow)
                        return false;
                }

                var compOversizedWeapon = thingWithComps.TryGetComp<CompOversizedWeapon>();
                if (compOversizedWeapon != null)
                {
                    var flip = false;
                    var num = aimAngle - 90f;
                    var pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
                    if (pawn == null) return true;

                    Mesh mesh;
                    if (aimAngle > 20f && aimAngle < 160f)
                    {
                        mesh = MeshPool.plane10;
                        num += eq.def.equippedAngleOffset;
                    }
                    else if (aimAngle > 200f && aimAngle < 340f)
                    {
                        mesh = MeshPool.plane10Flip;
                        flip = true;
                        num -= 180f;
                        num -= eq.def.equippedAngleOffset;
                    }
                    else
                    {
                        num = AdjustOffsetAtPeace(eq, pawn, compOversizedWeapon, num);
                    }
                    num %= 360f;



                    var graphic_StackCount = eq.Graphic as Graphic_StackCount;
                    Material matSingle;
                    if (graphic_StackCount != null)
                        matSingle = graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingle;
                    else
                        matSingle = eq.Graphic.MatSingle;


                    var s = new Vector3(eq.def.graphicData.drawSize.x, 1f, eq.def.graphicData.drawSize.y);
                    var matrix = default(Matrix4x4);

                    matrix.SetTRS(drawLoc, Quaternion.AngleAxis(num, Vector3.up), s);

                    Graphics.DrawMesh(!flip ? MeshPool.plane10 : MeshPool.plane10Flip, matrix, matSingle, 0);
                    return false;
                }
            }
            //}
            return true;
        }

        private static float AdjustOffsetAtPeace(Thing eq, Pawn pawn, CompOversizedWeapon compOversizedWeapon, float num)
        {
            Mesh mesh;
            mesh = MeshPool.plane10;
            var offsetAtPeace = eq.def.equippedAngleOffset;
            num += offsetAtPeace;
            return num;
        }

        public static void get_Graphic_PostFix(Thing __instance, ref Graphic __result)
        {
            var tempGraphic = Traverse.Create(__instance).Field("graphicInt").GetValue<Graphic>();
            if (tempGraphic != null)
                if (__instance is ThingWithComps thingWithComps)
                {
                    if (thingWithComps.ParentHolder is Pawn)
                        return;
                    var activatableEffect =
                        thingWithComps.AllComps.FirstOrDefault(
                            y => y.GetType().ToString().Contains("ActivatableEffect"));
                    if (activatableEffect != null)
                    {
                        var getPawn = Traverse.Create(activatableEffect).Property("GetPawn").GetValue<Pawn>();
                        if (getPawn != null)
                            return;
                    }
                    var compOversizedWeapon = thingWithComps.TryGetComp<CompOversizedWeapon>();
                    if (compOversizedWeapon != null)
                    {
                        tempGraphic.drawSize = __instance.def.graphicData.drawSize;
                        __result = tempGraphic;
                    }
                }
        }
    }
}