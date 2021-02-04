using System.Linq;
using System.Text;
using Verse;
using System.Diagnostics;
using System;
using UnityEngine;
using RimWorld;
//using AbilityUser;

namespace CentaurTheMagnuassembly
{
    /*public class FilthWithComps : ThingWithComps, Filth
    { 
    
    }*/
    public class Trishot_ResearchProjectDef : ResearchProjectDef
    {
        /*public override bool CanStartNow {
            get {
                return false;
            }
        }*/
    }
    public static class RimCentaurCore
    {
        public static readonly HediffDef HyperManipulatorHediffDef = DefDatabase<HediffDef>.GetNamed("HyperManipulator");
        public static readonly BodyPartDef CentaurScapularDef = DefDatabase<BodyPartDef>.GetNamed("CentaurScapular");
        public static readonly ThingDef AlienCentaurDef = DefDatabase<ThingDef>.GetNamed("Alien_Centaur");
        public static readonly PawnKindDef CentaurColonistDef = DefDatabase<PawnKindDef>.GetNamed("CentaurColonist");
        public static readonly Backstory CentaurCivilRetro = BackstoryDatabase.allBackstories.TryGetValue("CentaurCivil_Retro");
        public static readonly Backstory CentaurCivilMayinas = BackstoryDatabase.allBackstories.TryGetValue("Backstory_Mayinas_Exploriter");


        //public static readonly AbilityDef AbilityTrishot_TrishotDef = DefDatabase<AbilityDef>.GetNamed("AbilityTrishot_Trishot");
        //public static readonly AbilityDef AbilityTrishot_IcoshotDef = DefDatabase<AbilityDef>.GetNamed("AbilityTrishot_Icoshot");
        //public static readonly AbilityDef AbilityTrishot_OneshotDef = DefDatabase<AbilityDef>.GetNamed("AbilityTrishot_Oneshot");
        public static int InGameTick { get { return Find.TickManager.TicksGame; } }
        public static int InGameTickAbs { get { return Find.TickManager.TicksAbs; } }
        public static string FormattingTickTimeDiv(double tickNumber, string ToStringFormat = "0.00", string midFix = " /")
        {
            string valueString = "PeriodSeconds".Translate("NaN /");
            if (tickNumber != 0D)
            {
                if (1 / Math.Abs(tickNumber) >= 60000D)
                {
                    valueString = "Period's".Translate((tickNumber * 60000).ToString(ToStringFormat) + midFix);
                }
                else if (1 / Math.Abs(tickNumber) >= 15000D)
                {
                    valueString = "PeriodQuadrums".Translate((tickNumber * 15000).ToString(ToStringFormat) + midFix);
                }
                else if (1 / Math.Abs(tickNumber) >= 1000D)
                {
                    valueString = "PeriodDays".Translate((tickNumber * 1000).ToString(ToStringFormat) + midFix);
                }
                else if (1 / Math.Abs(tickNumber) >= 41.666666666666666666666666666667)
                {
                    valueString = "PeriodHours".Translate((tickNumber * 41.666666666666666666666666666667).ToString(ToStringFormat) + midFix);
                }
                else
                {
                    valueString = "PeriodSeconds".Translate(tickNumber.ToString(ToStringFormat) + midFix);
                }
            }
            return valueString;
        }
        public static string FormattingTickTime(double tickNumber, string ToStringFormat = "0.00")
        {
            string valueString;
            if (Math.Abs(tickNumber) >= 60000D)
            {
                valueString = "PeriodYears".Translate((tickNumber / 60000).ToString(ToStringFormat));
            }
            else if (Math.Abs(tickNumber) >= 15000D)
            {
                valueString = "PeriodQuadrums".Translate((tickNumber / 15000).ToString(ToStringFormat));
            }
            else if (Math.Abs(tickNumber) >= 1000D)
            {
                valueString = "PeriodDays".Translate((tickNumber / 1000).ToString(ToStringFormat));
            }
            /*else if (Math.Abs(tickNumber) > 416.66666666666666666666666666667)
            {
                valueString = "PeriodHours".Translate((tickNumber / 41.666666666666666666666666666667).ToString(ToStringFormat));
            }*/
            else
            {
                valueString = "PeriodSeconds".Translate(tickNumber.ToString(ToStringFormat));
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
        /// <summary>
        /// https://blog.csdn.net/qq_39776199/article/details/81506293
        /// </summary>
        /*public static Texture2D duplicateTexture(Texture2D source)
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
        public static Color hsb2rgb(float h, float s, float v)
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

    }

}
