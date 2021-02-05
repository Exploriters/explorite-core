/**
 * 常量包以及常用函数。
 * --siiftun1857
 */
using Verse;
using System;
using UnityEngine;
using RimWorld;
using HarmonyLib;

namespace Explorite
{
    public static partial class ExploriteCore
    {
        public static readonly Harmony harmonyInstance = new Harmony(id: "Explorite.rimworld.mod.HarmonyPatches");

        public static readonly ThingDef AlienSayersDef = DefDatabase<ThingDef>.GetNamed("Alien_Sayers", errorOnFail: false);
        public static readonly ThingDef AlienFlowerBorhAnimalDef = DefDatabase<ThingDef>.GetNamed("Alien_FlowerBorhAnimal", errorOnFail: false);

        public static readonly ThingDef AlienCentaurDef = DefDatabase<ThingDef>.GetNamed("Alien_Centaur", errorOnFail: false);
        public static readonly HediffDef HyperManipulatorHediffDef = DefDatabase<HediffDef>.GetNamed("HyperManipulator", errorOnFail: false);
        public static readonly BodyPartDef CentaurScapularDef = DefDatabase<BodyPartDef>.GetNamed("CentaurScapular", errorOnFail: false);
        public static readonly PawnKindDef CentaurColonistDef = DefDatabase<PawnKindDef>.GetNamed("CentaurColonist", errorOnFail: false);
        public static readonly FactionDef CentaurPlayerColonyDef = DefDatabase<FactionDef>.GetNamed("CentaurPlayerColony", errorOnFail: false);

        //public static readonly AbilityDef AbilityTrishot_TrishotDef = DefDatabase<AbilityDef>.GetNamed("AbilityTrishot_Trishot");
        //public static readonly AbilityDef AbilityTrishot_IcoshotDef = DefDatabase<AbilityDef>.GetNamed("AbilityTrishot_Icoshot");
        //public static readonly AbilityDef AbilityTrishot_OneshotDef = DefDatabase<AbilityDef>.GetNamed("AbilityTrishot_Oneshot");

        public static readonly Backstory CentaurCivilRetro = BackstoryDatabase.allBackstories.TryGetValue("CentaurCivil_Retro");
        public static readonly Backstory CentaurCivilMayinas = BackstoryDatabase.allBackstories.TryGetValue("Backstory_Mayinas_Exploriter");

        public static class InstelledMods
        {
            /*
            public static bool RimCentaurs = false;
            public static bool Sayers = false;
            public static bool GuoGuo = false;

            public static bool SoS2 = false;

            public static void UpdateStatus()
            {
                //ExploriteCore = ModLister.GetActiveModWithIdentifier("Exploriters.ExploriteCore") != null;
                RimCentaurs = ModLister.GetActiveModWithIdentifier("Exploriters.siiftun1857.CentaurTheMagnuassembly") != null;
                Sayers = ModLister.GetActiveModWithIdentifier("Exploriters.Abrel.Sayers") != null;
                GuoGuo = ModLister.GetActiveModWithIdentifier("Exploriters.AndoRingo.GuoGuo") != null;

                SoS2 = ModLister.GetActiveModWithIdentifier("kentington.saveourship2") != null;
            }
            */
            public static bool RimCentaurs => ModLister.GetActiveModWithIdentifier("Exploriters.siiftun1857.CentaurTheMagnuassembly") != null;
            public static bool Sayers => ModLister.GetActiveModWithIdentifier("Exploriters.Abrel.Sayers") != null;
            public static bool GuoGuo => ModLister.GetActiveModWithIdentifier("Exploriters.AndoRingo.GuoGuo") != null;
            public static bool SoS2 => ModLister.GetActiveModWithIdentifier("kentington.saveourship2") != null;
        }

        /**
         * <summary>
         * 检测Def名称与目标是否一致。
         * </summary>
         * <param name="def">需要被格式化的刻数。</param>
         * <param name="defName">返回值字符串的格式。</param>
         * <returns>是否相等。</returns>
         */
        public static bool CheckDef(Def def, string defName) => def?.defName == defName;
        //public static bool operator !=(Def def, string defName) => !CheckDef(def, defName);

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
            if (Randy.Next(0, 1) == 1)
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
            //Log.Message("[Magnuassembly]Casting " + target + " by " + range + ".", true);
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
            //Log.Message("[Magnuassembly]Casted " + counter + " pixels. ", true);
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
                flag = true;
            }

            return flag;
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
    }

}
