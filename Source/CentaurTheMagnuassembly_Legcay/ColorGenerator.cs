using UnityEngine;
using Verse;
using RimWorld;
using System;
using static CentaurTheMagnuassembly.RimCentaurCore;
using System.Collections.Generic;

namespace CentaurTheMagnuassembly
{
    public class ColorGenerator_CentaurHair : ColorGenerator
    {
        public ColorGenerator_CentaurHair()
        {
        }
        public override Color ExemplaryColor
        {
            get
            {
                return new Color(0.75686276f, 0.572549045f, 0.333333343f);
            }
        }

        public override Color NewRandomizedColor()
        {
            //if (Rand.Value < 0.02f)
            if (Rand.Value < 0.05f)
            {
                //return new Color(Rand.Value, Rand.Value, Rand.Value);
                return hsb2rgb(Rand.Value * 360f, (float)Math.Sin(90.0f - Rand.Value * 90.0f), (float)Math.Sin(90.0f - Rand.Value * 90.0f));
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
