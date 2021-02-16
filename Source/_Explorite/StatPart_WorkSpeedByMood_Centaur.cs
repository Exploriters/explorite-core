/********************
 * 使全局工作速度属性受到半人马的心情影响。
 * 
 * 未使用的功能。
 * --siiftun1857
 */
using Verse;
using RimWorld;
using System;
using static Explorite.ExploriteCore;

namespace Explorite
{
    public class StatPart_WorkSpeedByMood_Centaur : StatPart
    {
        private bool Vaild(StatRequest req)
        {
            try
            {
                if (req.HasThing && req.Thing.def == AlienCentaurDef)
                {
                    return true;
                }
            }
            catch (NullReferenceException)
            {
            }
            catch (InvalidCastException)
            {
            }
            catch
            {
            }
            return false;
        }
        public override void TransformValue(StatRequest req, ref float val)
        {
            if (!Vaild(req))
            {
                return;
            }
            else if (req.Thing is Pawn pawn)
            {
                float mood = pawn.needs.TryGetNeed<Need_Mood>().CurLevel;
                val *= (2f + (2f * mood)) / 3f;
                return;
            }
        }

        public override string ExplanationPart(StatRequest req)
        {
            throw new NotImplementedException();
        }
    }



}
