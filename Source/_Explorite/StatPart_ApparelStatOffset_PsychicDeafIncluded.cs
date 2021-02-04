/**
 * 使心灵敏感度属性受到心灵失聪hediff影响。
 * --siiftun1857
 */
using UnityEngine;
using Verse;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static Explorite.ExploriteCore;

namespace Explorite
{
    /**
     * <summary>
     * 使心灵敏感度属性受到心灵失聪hediff影响。
     * </summary>
     * TODO: 改为补丁，而非XML Patch
     */
    public class StatPart_ApparelStatOffset_PsychicDeafIncluded : StatPart_ApparelStatOffset
    {
        private bool Blocked(StatRequest req)
        {
            try
            {
                if (req.HasThing && (((Pawn)req.Thing)?.health?.hediffSet?.HasHediff(DefDatabase<HediffDef>.GetNamed("PsychicDeafCentaur")))==true)
                {
                    return true;
                }
            }
            catch(NullReferenceException e)
            {
            }
            catch(InvalidCastException e)
            {
            }
            catch
            {
            }
            return false;
        }
        public override void TransformValue(StatRequest req, ref float val)
        {
            if (!Blocked(req))
            {
                base.TransformValue(req, ref val);
                return;
            }
            else
            {
                val = 0f;
                return;
            }
        }
    }



}
