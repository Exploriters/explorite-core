/********************
 * 常量包以及常用函数。
 * --siiftun1857
 */
using Verse;
using System;
using UnityEngine;
using RimWorld;
using HarmonyLib;
using System.Linq;
using System.Reflection;

namespace Explorite
{
    ///<summary>Explorite Core核心函数集。</summary>
    public static partial class ExploriteCore
    {
        public static class InstelledMods
        {
            public static bool RimCentaurs => ModLister.GetActiveModWithIdentifier("Exploriters.siiftun1857.CentaurTheMagnuassembly") != null;
            public static bool Sayers => ModLister.GetActiveModWithIdentifier("Exploriters.Abrel.Sayers") != null;
            public static bool GuoGuo => ModLister.GetActiveModWithIdentifier("Exploriters.AndoRingo.GuoGuo") != null;
            public static bool Royalty => ModLister.GetActiveModWithIdentifier("Ludeon.RimWorld.Royalty") != null;
            public static bool SoS2 => ModLister.GetActiveModWithIdentifier("kentington.saveourship2") != null;
            public static bool HAR => ModLister.GetActiveModWithIdentifier("erdelf.HumanoidAlienRaces") != null;
        }

        /*
         * <summary>
         * 检测Def名称与目标是否一致。
         * </summary>
         * <param name="def">需要被格式化的刻数。</param>
         * <param name="defName">返回值字符串的格式。</param>
         * <returns>是否相等。</returns>
         */
        //public static bool CheckDef(Def def, string defName) => def?.defName == defName;

        public static int InGameTick => Find.TickManager.TicksGame;
        public static int InGameTickAbs => Find.TickManager.TicksAbs;
        /**
         * <summary>
         * 将一个游戏刻格式化为速率值的字符串。
         * </summary>
         * <param name="tickNumber">需要被格式化的刻数。</param>
         * <param name="toStringFormat">返回值字符串的格式。</param>
         * <param name="midFix">返回值字符串内作为除号的分隔符。</param>
         * <returns>被格式化为可读文本的速率。</returns>
         */
        public static string FormattingTickTimeDiv(double tickNumber, string toStringFormat = "0.00", string midFix = " /")
        {
            string valueString = "PeriodSeconds".Translate("NaN" + midFix);
            if (tickNumber != 0D)
            {
                if (1 / Math.Abs(tickNumber) >= 60000D)
                {
                    valueString = "Period's".Translate((tickNumber * 60000).ToString(toStringFormat) + midFix);
                }
                else if (1 / Math.Abs(tickNumber) >= 15000D)
                {
                    valueString = "PeriodQuadrums".Translate((tickNumber * 15000).ToString(toStringFormat) + midFix);
                }
                else if (1 / Math.Abs(tickNumber) >= 1000D)
                {
                    valueString = "PeriodDays".Translate((tickNumber * 1000).ToString(toStringFormat) + midFix);
                }
                else if (1 / Math.Abs(tickNumber) >= 41.666666666666666666666666666667)
                {
                    valueString = "PeriodHours".Translate((tickNumber * 41.666666666666666666666666666667).ToString(toStringFormat) + midFix);
                }
                else
                {
                    valueString = "PeriodSeconds".Translate(tickNumber.ToString(toStringFormat) + midFix);
                }
            }
            return valueString;
        }
        /**
         * <summary>
         * 将一个游戏刻格式化为时间值的字符串。
         * </summary>
         * <param name="tickNumber">需要被格式化的刻数。</param>
         * <param name="toStringFormat">返回值字符串的格式。</param>
         * <returns>被格式化为可读文本的时间。</returns>
         */
        public static string FormattingTickTime(double tickNumber, string toStringFormat = "0.00")
        {
            string valueString;
            if (Math.Abs(tickNumber) >= 60000D)
            {
                valueString = "PeriodYears".Translate((tickNumber / 60000).ToString(toStringFormat));
            }
            else if (Math.Abs(tickNumber) >= 15000D)
            {
                valueString = "PeriodQuadrums".Translate((tickNumber / 15000).ToString(toStringFormat));
            }
            else if (Math.Abs(tickNumber) >= 1000D)
            {
                valueString = "PeriodDays".Translate((tickNumber / 1000).ToString(toStringFormat));
            }
            /*else if (Math.Abs(tickNumber) > 416.66666666666666666666666666667)
            {
                valueString = "PeriodHours".Translate((tickNumber / 41.666666666666666666666666666667).ToString(ToStringFormat));
            }*/
            else
            {
                valueString = "PeriodSeconds".Translate(tickNumber.ToString(toStringFormat));
            }
            return valueString;
        }
        public static Color CastingPixel(Color color)
        {
            System.Random Randy = new System.Random();
            if (Randy.Next(0, 2) == 1)
                return color;
            color.r = (color.r * (1 - color.a)) + (0.5f * color.a);
            color.g = (color.g * (1 - color.a)) + (0.5f * color.a);
            color.b = (color.b * (1 - color.a)) + (0.5f * color.a);
            color.a = Math.Max(0.5f, color.a);
            return color;
        }
        /*
        /// <summary>
        /// https://blog.csdn.net/qq_39776199/article/details/81506293
        /// </summary>
        public static Texture2D duplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }
        public static Texture2D duplicateTexture(Texture2D source)
        {
            byte[] pix = source.GetRawTextureData();
            Texture2D readableText = new Texture2D(source.width, source.height, source.format, false);
            readableText.LoadRawTextureData(pix);
            readableText.Apply();
            return readableText;
        }
        public static Texture2D FloodingTexture(Texture2D inputtex, float range)
        {
            Texture2D target = duplicateTexture(inputtex);
            //range = new System.Random().Next(0,10000)/10000f;\
            //Log.Message("[Explorite]Casting " + target + " by " + range + ".", true);
            if (range <= 0f)
                return target;
            if (range > 1f)
                range = 1f;
            int width = target.width;
            int height = target.height;
            int heightLevel = (int)(height * range);
            int counter = 0;

            for (int X = 0; X < width; X++)
            {
                for (int Y = heightLevel; Y < height; Y++)
                {
                    target.SetPixel(X, Y, CastingPixel(target.GetPixel(X, Y)));
                    counter++;
                }
            }
            //Log.Message("[Explorite]Casted " + counter + " pixels. ", true);
            return target;
        }*/
        public static Texture2D FloodingTexture(Texture2D inputtex, float range)
        {
            if (range == 0f) { }

            return inputtex;

            /*if (range <= 0f)
                return inputtex;
            if (range > 1f)
                range = 1f;
            int width = inputtex.width;
            int height = inputtex.height;
            int heightLevel = (int)(height * range);
            int counter = 0;*/
        }

        /**
         * <summary>
         * 检测人物是否可以占用目标位置。
         * </summary>
         * <param name="pawn">需要被检测的人物。</param>
         * <param name="c">一个位置。</param>
         * <returns>是否可以占用目标位置。</returns>
         */
        public static bool PawnCanOccupy(Pawn pawn, IntVec3 c)
        {
            if (!c.Walkable(pawn.Map))
            {
                return false;
            }
            Building edifice = c.GetEdifice(pawn.Map);
            if (edifice != null)
            {
                if (edifice is Building_Door building_Door && !building_Door.PawnCanOpen(pawn) && !building_Door.Open)
                {
                    return false;
                }
            }
            return true;
        }
        /**
         * <summary>
         * 扫描目标周围可被占用的位置。
         * </summary>
         * <param name="pawn">需要被检测的人物。</param>
         * <param name="target">一个位置。</param>
         * <returns>最近的可被占用的位置。若无，则返回<c>null</c>。</returns>
         */
        public static IntVec3? ScanOccupiablePosition(Pawn pawn, IntVec3 target)
        {
            IntVec3? flag = null;
            IntVec3 intVec;
            for (int i = 0; i < GenRadial.RadialPattern.Length; i++)
            {
                intVec = target + GenRadial.RadialPattern[i];
                if (PawnCanOccupy(pawn, intVec))
                {
                    if (intVec == target)
                    {
                        return target;
                    }
                    flag = intVec;
                    break;
                }
            }
            return flag;
        }
        /**
         * <summary>
         * 将人物传送到目标位置附近。
         * </summary>
         * <param name="pawn">需要被传送的人物。</param>
         * <param name="target">一个位置。</param>
         * <returns>是否成功地完成了传送。</returns>
         */
        public static bool TeleportPawn(Pawn pawn, IntVec3 target)
        {
            bool flag = false;
            IntVec3? relocated = ScanOccupiablePosition(pawn, target);
            if (relocated != null)
            {
                pawn.SetPositionDirect((IntVec3)relocated);
                pawn.Notify_Teleported();
                flag = true;
            }

            return flag;
        }

        internal static MethodInfo methodFloodUnfogAdjacent = typeof(FogGrid).GetMethod("FloodUnfogAdjacent", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        public static void RevealFogCluster(this FogGrid fogGrid, IntVec3 target)
        {
            methodFloodUnfogAdjacent.Invoke(fogGrid, new object[] { target });
        }

        // 与Color.HSVToRGB重复
        /*
        public static Color hsb2rgb(float h, float s, float v, float a = 1f)
        {
            //if ()
            //{
            //    throw new ArgumentOutOfRangeException();
            //}
            h = Math.Max(Math.Min(h, 360f), 0f);
            s = Math.Max(Math.Min(s, 1f), 0f);
            v = Math.Max(Math.Min(v, 1f), 0f);

            float r = 0, g = 0, b = 0;
            int i = (int)((h / 60) % 6);
            float f = (h / 60) - i;
            float p = v * (1 - s);
            float q = v * (1 - f * s);
            float t = v * (1 - (1 - f) * s);
            switch (i)
            {
                case 0:
                    r = v;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = v;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = v;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = v;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = v;
                    break;
                case 5:
                    r = v;
                    g = p;
                    b = q;
                    break;
                default:
                    break;
            }
            return new Color(r, g, b);
        }
        */
        /*
        BodyPartRecord partRecHead = CentaurBodyDef.AllParts.First(d => d.def == BodyPartDefOf.Head);
        BodyPartRecord partRecWaist = CentaurBodyDef.AllParts.First(d => d?.groups?.Contains(DefDatabase<BodyPartGroupDef>.GetNamed("Waist")) ?? false);
        if (apparel != null &&
                (apparel?.defaultOutfitTags?.Contains("CentaurOutfit") == true
              / *|| apparel?.CoversBodyPart(partRecHead) == true
                || apparel?.CoversBodyPart(partRecWaist) == true
                || allDef2.apparel?.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead) == true
                || allDef2.apparel?.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead) == true
                || allDef2.apparel?.bodyPartGroups.Contains(DefDatabase<BodyPartGroupDef>.GetNamed("Waist")) == true
                || allDef2.apparel?.layers.Contains(ApparelLayerDefOf.Overhead) == true
                || allDef2.apparel?.layers.Contains(ApparelLayerDefOf.Belt) == true* /
                )
            )
            return true;
        */
        /*
        if (allDef2?.apparel != null && 
                (allDef2.apparel?.defaultOutfitTags?.Contains("CentaurOutfit") == true
              || allDef2.apparel?.CoversBodyPart(partRecHead) == true
              || allDef2.apparel?.CoversBodyPart(partRecWaist) == true
                || allDef2.apparel?.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead) == true
              || allDef2.apparel?.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead) == true
              || allDef2.apparel?.bodyPartGroups.Contains(DefDatabase<BodyPartGroupDef>.GetNamed("Waist")) == true
              || allDef2.apparel?.layers.Contains(ApparelLayerDefOf.Overhead) == true
              || allDef2.apparel?.layers.Contains(ApparelLayerDefOf.Belt) == true
                )
            )*/

        /**
         * <summary>
         * 检测物品是否为合法的半人马服装。
         * </summary>
         * <param name="thing">需要被检测的物品。</param>
         * <returns>该物品是否为合法的半人马服装。</returns>
         */
        public static bool VaildCentaurApparelPredicate(ThingDef thing)
        {
            ApparelProperties apparel = thing?.apparel;
            if (apparel == null)
                return false;
            if (apparel?.defaultOutfitTags?.Contains("CentaurOutfit") == true)
                return true;

            if (apparel.bodyPartGroups?.Any() == true)
            {
                byte flag = 0;
                foreach (BodyPartGroupDef bpg in apparel.bodyPartGroups)
                {
                    if (bpg == BodyPartGroupDefOf.Torso)
                    {
                        flag |= 1;
                    }
                    if (bpg == CentaurTorsoGroupDef)
                    {
                        flag |= 2;
                    }
                    else if (!CentaurBodyPartGroups.Contains(bpg))
                    {
                        return false;
                    }
                }
                if ((flag & 1) != 0 && (flag & 2) == 0)
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * <summary>
         * 检测特征是否为半人马特征。
         * </summary>
         * <param name="tDef">需要被检测的特征。</param>
         * <param name="degree">需要被检测的特征程度。</param>
         * <returns>该特征是否为半人马服装。</returns>
         */
        public static bool CentaurTraitPredicate(TraitDef tDef, int? degree = null)
        {
            return CentaurTraitPredicate(tDef, out int tergetDegree) && ( !degree.HasValue || degree == tergetDegree);
        }
        /**
         * <summary>
         * 检测特征是否为半人马特征。
         * </summary>
         * <param name="tDef">需要被检测的特征。</param>
         * <param name="degree">需要被检测的特征程度。</param>
         * <returns>该特征是否为半人马特征。</returns>
         */
        public static bool CentaurTraitPredicate(TraitDef tDef, out int degree)
        {
            if (tDef == DefDatabase<TraitDef>.GetNamed("Masochist")
             || tDef == TraitDefOf.Masochist
             || tDef == TraitDefOf.Industriousness
             || tDef == TraitDefOf.Kind
             || tDef == TraitDefOf.Asexual
             )
            {
                degree = 0;
                return true;
            }
            else if (tDef == TraitDefOf.DrugDesire)
            {
                degree = -1;
                return true;
            }
            else if (tDef == TraitDefOf.Transhumanist)
            {
                degree = 2;
                return true;
            }
            else
            {
                degree = 0;
                return false;
            }
        }
    }
}
