/********************
 * 使心灵敏感度属性受到心灵失聪hediff影响。
 * 
 * 该方案已被弃用。
 * --siiftun1857
 */
using Verse;
using RimWorld;
using System;

namespace Explorite
{
	/*
	///<summary>使心灵敏感度属性受到心灵失聪hediff影响。</summary>
	//改为补丁，而非XML Patch
	public class StatPart_ApparelStatOffset_PsychicDeafIncluded : StatPart_ApparelStatOffset
	{
		private bool Blocked(StatRequest req)
		{
			try
			{
				if (req.HasThing && (((Pawn)req.Thing)?.health?.hediffSet?.HasHediff(DefDatabase<HediffDef>.GetNamed("PsychicDeafCentaur"))) == true)
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
	*/


}
