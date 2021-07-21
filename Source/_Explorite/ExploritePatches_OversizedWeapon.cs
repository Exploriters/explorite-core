/********************
 * 尺寸超出常规大小的武器。
 * 
 * 代码参考自
 *      https://github.com/RimWorld-CCL-Reborn/JecsTools
 *      https://github.com/RimWorld-CCL-Reborn/JecsTools/tree/master/Source/AllModdingComponents/CompOversizedWeapon
 * 
 * 由siiftun1857重制
 */
using System.Linq;
using HarmonyLib;
using UnityEngine;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
    /*
    ///<summary>为<see cref = "CompOversizedWeapon" />接收参数。</summary>
    public class CompProperties_OversizedWeapon : CompProperties
    {
        public CompProperties_OversizedWeapon() : base(typeof(CompOversizedWeapon))
        { }
    }
    ///<summary>使武器尺寸与其物品渲染设置一致。</summary>
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
    */

    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, "IDE1006")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, "IDE0058")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, "IDE0060")]
    internal static partial class ExploritePatches
    {
        public static bool IsOversizedWeapon(ThingDef thingDef)
        {
            return thingDef?.weaponTags?.Contains("ExploriteOversizedWeapon") ?? false;
        }

        /// <summary>
        ///     Adds another "layer" to the equipment aiming if they have a
        ///     weapon with a CompActivatableEffect.
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="eq"></param>
        /// <param name="drawLoc"></param>
        /// <param name="aimAngle"></param>
        public static bool DrawEquipmentAimingPrefix(PawnRenderer __instance, Thing eq, Vector3 drawLoc, float aimAngle)
        {
            if (eq is ThingWithComps thingWithComps)
            {
                //If the deflector is active, it's already using this code.
                ThingComp deflector = thingWithComps.AllComps.FirstOrDefault(y =>
                    y.GetType().ToString() == "CompDeflector.CompDeflector" ||
                    y.GetType().BaseType.ToString() == "CompDeflector.CompDeflector");
                if (deflector != null)
                {
                    bool isAnimatingNow = Traverse.Create(deflector).Property("IsAnimatingNow").GetValue<bool>();
                    if (isAnimatingNow)
                        return false;
                }

                if (IsOversizedWeapon(thingWithComps.def))
                {
                    bool flip = false;
                    float num = aimAngle - 90f;
                    Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
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
                        num = AdjustOffsetAtPeace(eq, num);
                    }
                    num %= 360f;



                    Material matSingle;
                    if (eq.Graphic is Graphic_StackCount graphic_StackCount)
                        matSingle = graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingle;
                    else
                        matSingle = eq.Graphic.MatSingle;


                    Vector3 s = new Vector3(eq.def.graphicData.drawSize.x, 1f, eq.def.graphicData.drawSize.y);
                    Matrix4x4 matrix = default;

                    matrix.SetTRS(drawLoc, Quaternion.AngleAxis(num, Vector3.up), s);

                    Graphics.DrawMesh(!flip ? MeshPool.plane10 : MeshPool.plane10Flip, matrix, matSingle, 0);
                    return false;
                }
            }
            //}
            return true;
        }

        private static float AdjustOffsetAtPeace(Thing eq, float num)
        {
            //Mesh mesh = MeshPool.plane10;
            float offsetAtPeace = eq.def.equippedAngleOffset;
            num += offsetAtPeace;
            return num;
        }

        public static void get_Graphic_PostFix(Thing __instance, ref Graphic __result)
        {
            Graphic tempGraphic = Traverse.Create(__instance).Field("graphicInt").GetValue<Graphic>();
            if (tempGraphic != null)
                if (__instance is ThingWithComps thingWithComps)
                {
                    if (thingWithComps.ParentHolder is Pawn)
                        return;
                    ThingComp activatableEffect =
                        thingWithComps.AllComps.FirstOrDefault(
                            y => y.GetType().ToString().Contains("ActivatableEffect"));
                    if (activatableEffect != null)
                    {
                        Pawn getPawn = Traverse.Create(activatableEffect).Property("GetPawn").GetValue<Pawn>();
                        if (getPawn != null)
                            return;
                    }
                    if (IsOversizedWeapon(thingWithComps.def))
                    {
                        tempGraphic.drawSize = __instance.def.graphicData.drawSize;
                        __result = tempGraphic;
                    }
                }
        }
    }
}