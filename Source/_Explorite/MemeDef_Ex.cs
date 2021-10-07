/********************
 * MemeDef增强。
 * --siiftun1857
 */
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Explorite
{
	///<summary><see cref = "MemeDef" />增强。</summary>
	public class MemeDef_Ex : MemeDef
	{
		[NoTranslate]
		public string islocateGroup;
		public bool overrdieIcon;
		public bool countForNonStructureGroup;
		public List<FactionDef> exclusiveTo;
		public bool lockAccuIdeoColor = false;
	}
}