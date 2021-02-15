/**
 * 对人物渲染器的补丁。
 * 
 * 失效。
 * --siiftun1857
 */
using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using HarmonyLib;
using Verse;
using static Explorite.ExploriteCore;
using UnityEngine;

namespace Explorite
{
    internal static partial class ExploritePatches
    {
        ///<summary>对人物渲染器的补丁。</summary>
        [HarmonyPostfix]
        public static void PawnRendererInternalPostfix(PawnRenderer __instance, Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, bool portrait, bool headStump, bool invisible)
        {
            static bool IsHeadwear(ApparelProperties apparelProperties)
            {
                if (apparelProperties.LastLayer == ApparelLayerDefOf.Overhead)
                {
                    return true;
                }
                for (int i = 0; i < apparelProperties.bodyPartGroups.Count; ++i)
                {
                    var group = apparelProperties.bodyPartGroups[i];
                    if (group == BodyPartGroupDefOf.FullHead || group == BodyPartGroupDefOf.UpperHead || group == BodyPartGroupDefOf.Eyes)
                    {
                        return true;
                    }
                }
                return false;
            }

            if (
                __instance.GetType().GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
                && pawn.def == AlienCentaurDef && pawn.apparel.WornApparel.Any(ap => ap.def == CentaurHeaddressDef)
                && __instance.graphics.headGraphic != null)
            {
                Quaternion quad = Quaternion.AngleAxis(angle, Vector3.up);
                Vector3 b = quad * __instance.BaseHeadOffsetAt(headFacing);
                Vector3 loc2 = rootLoc + b;
                float hairLoc = 0;
                bool flag = false;
                List<ApparelGraphicRecord> apparelGraphics = __instance.graphics.apparelGraphics;
                bool offsetApplied = false;
                foreach (ApparelGraphicRecord v in apparelGraphics)
                {
                    Apparel sourceApparel = v.sourceApparel;
                    if (IsHeadwear(sourceApparel.def.apparel))
                    {
                        if (!offsetApplied)
                        {
                            if (!sourceApparel.def.apparel.hatRenderedFrontOfFace)
                            {
                                flag = true;
                                loc2.y += 0.03125f;
                                hairLoc = loc2.y;
                            }
                            else
                            {
                                Vector3 loc3 = rootLoc + b;
                                loc3.y += ((!(bodyFacing == Rot4.North)) ? 0.03515625f : 0.00390625f);
                                hairLoc = loc3.y;
                            }
                            offsetApplied = true;
                        }
                    }
                }
                if (flag || bodyDrawType == RotDrawMode.Dessicated || headStump)
                {
                    if ((true || true) && hairLoc > 0)
                    {
                        if (hairLoc > 0.001f)
                        {
                            loc2.y = hairLoc - 0.001f;

                            Material mat = __instance.graphics.HairMatAt_NewTemp(headFacing, true);
                            if (true)
                            {
                                GenDraw.DrawMeshNowOrLater(__instance.graphics.HairMeshSet.MeshAt(headFacing), loc2, quad, mat, portrait);
                            }
                        }
                    }
                }
            }
        }
    }
}
