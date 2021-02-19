/********************
 * 该文件包含颜色生成器。
 * --siiftun1857
 */
using UnityEngine;
using Verse;
using System;

namespace Explorite
{
    ///<summary>生成半人马的头发颜色。</summary>
    // TODO: 弃用该方法并改为补丁。
    public class ColorGenerator_CentaurHair : ColorGenerator
    {
        /*public ColorGenerator_CentaurHair()
        {
        }*/
        public override Color ExemplaryColor => new Color(0.75686276f, 0.572549045f, 0.333333343f);

        public override Color NewRandomizedColor()
        {
            //if (Rand.Value < 0.02f)
            if (Rand.Value < 0.05f)
            {
                //return new Color(Rand.Value, Rand.Value, Rand.Value);
                return Color.HSVToRGB(
                    Rand.Value * 1f, 
                    (float)Math.Sin(Math.PI * (0.5 - (Rand.Value * 0.5))), 
                    (float)Math.Sin(Math.PI * (0.5 - (Rand.Value * 0.5))));
            }
            if (/*PawnSkinColors.IsDarkSkin(skinColor) || */Rand.Value < 0.5f)
            {
                float value = Rand.Value;
                if (value < 0.25f)
                {
                    return new Color(0.2f, 0.2f, 0.2f);
                }
                if (value < 0.5f)
                {
                    return new Color(0.31f, 0.28f, 0.26f);
                }
                if (value < 0.75f)
                {
                    return new Color(0.25f, 0.2f, 0.15f);
                }
                return new Color(0.3f, 0.2f, 0.1f);
            }
            else
            {
                float value2 = Rand.Value;
                if (value2 < 0.25f)
                {
                    return new Color(0.3529412f, 0.227450982f, 0.1254902f);
                }
                if (value2 < 0.5f)
                {
                    return new Color(0.5176471f, 0.3254902f, 0.184313729f);
                }
                if (value2 < 0.75f)
                {
                    return new Color(0.75686276f, 0.572549045f, 0.333333343f);
                }
                return new Color(0.929411769f, 0.7921569f, 0.6117647f);
            }
        }
    }

    ///<summary>生成果果人的红色头发颜色。</summary>
    public class ColorGenerator_GuoguoHairRed : ColorGenerator
    {
        public override Color ExemplaryColor => new Color(0.85098039215686274509803921568627f, 0f, 0f);
        public override Color NewRandomizedColor() => Color.HSVToRGB(
            (float)((Math.Pow(Math.Sin(Math.PI * Rand.Value),1D/3D) * 30 - 15 + Rand.Value < 0.05f ? 120f : 0f) / 360),
            ((float)Math.Sin(Math.PI * Rand.Value * 0.5D) * 0.3f) + 0.7f,
            ((float)Math.Sin(Math.PI * Rand.Value) * 0.3f) + 0.7f);
    }
    ///<summary>生成果果人的绿色头发颜色。</summary>
    public class ColorGenerator_GuoguoHairGreen : ColorGenerator
    {
        public override Color ExemplaryColor => new Color(0.07450980392156862745098039215686f, 0.5960784313725490196078431372549f, 0.03137254901960784313725490196078f);
        public override Color NewRandomizedColor() => Color.HSVToRGB(
            (((float)Math.Sin(Math.PI * Rand.Value) * 30f) + Rand.Value >= 0.15f ? 100f : 40f) / 360f,
            ((float)Math.Sin(Math.PI * Rand.Value * 0.5D) * 0.25f) + 0.7f,
            ((float)Math.Sin(Math.PI * Rand.Value) * 0.8f) + 0.2f);
    }
    /*public class ColorGenerator_3E66B5 : ColorGenerator
    {
        public List<ColorOption> options;
        protected ColorGenerator_3E66B5()
        {
        }
        public override Color ExemplaryColor
        {
            get
            {
                return new Color(62 / 255f, 102 / 255f, 181 / 255f);
            }
        }

        public override Color NewRandomizedColor()
        {
            return new Color(62 / 255f, 102 / 255f, 181 / 255f);
        }
    }*/



}
