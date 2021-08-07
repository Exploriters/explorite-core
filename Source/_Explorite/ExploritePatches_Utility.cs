/********************
 * 补丁模块功能。
 * --siiftun1857
 */
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using static Explorite.ExploriteCore;
using static Verse.DamageInfo;
using static Verse.PawnCapacityUtility;

namespace Explorite
{
	internal static partial class ExploritePatches
	{
		public enum ExplortiePatchActionRecordState : byte
		{
			Unresolved = 0,
			Success = 1,
			Failed = 2,
		}
		public class ExplortiePatchActionRecord
		{
			public MethodBase original;
			public HarmonyMethod prefix;
			public HarmonyMethod postfix;
			public HarmonyMethod transpiler;
			public HarmonyMethod finalizer;
			public ExplortiePatchActionRecordState state;

			public ExplortiePatchActionRecord(MethodBase original, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null, HarmonyMethod finalizer = null)
			{
				this.original = original;
				this.prefix = prefix;
				this.postfix = postfix;
				this.transpiler = transpiler;
				this.finalizer = finalizer;
				state = ExplortiePatchActionRecordState.Unresolved;
			}
			public MethodInfo Patch()
			{
				state = ExplortiePatchActionRecordState.Success;
				try
				{
					return harmonyInstance.Patch(
						original: original,
						prefix: prefix,
						postfix: postfix,
						transpiler: transpiler,
						finalizer: finalizer
						);
				}
				catch (Exception e)
				{
					Log.Error(string.Concat(
						"[Explorite]Patch sequence failare at ",
						$"{original.FullDescription()}, ",
						prefix != null ? "prefix: " + prefix.method.FullDescription() + ", " : "",
						postfix != null ? "postfix: " + postfix.method.FullDescription() + ", " : "",
						transpiler != null ? "transpiler: " + transpiler.method.FullDescription() + ", " : "",
						finalizer != null ? "finalizer: " + finalizer.method.FullDescription() + ", " : "",
						", ",
						$"an exception ({e.GetType().Name}) occurred.\n",
						$"Message:\n   {e.Message}\n",
						$"Stack Trace:\n{e.StackTrace}\n"
						));
				}
				state = ExplortiePatchActionRecordState.Failed;
				return null;
			}
		}
		internal static readonly List<ExplortiePatchActionRecord> records = new List<ExplortiePatchActionRecord>();
		//internal static readonly Type patchType = typeof(ExploritePatches);

		static string PrintPatches()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ExplortiePatchActionRecord record in records)
			{
				stringBuilder.AppendLine(string.Concat(
					$"{record.original.FullDescription()}",
					record.prefix != null ? $"\n - prefix: {record.prefix.method.FullDescription()}" : "",
					record.postfix != null ? $"\n - postfix: {record.postfix.method.FullDescription()}" : "",
					record.transpiler != null ? $"\n - transpiler: {record.transpiler.method.FullDescription()}" : "",
					record.finalizer != null ? $"\n - finalizer: {record.finalizer.method.FullDescription()}" : ""
					));
			}
			return stringBuilder.ToString();
		}
		private static string App(this string str, ref string target)
		{
			return target = str;
		}
		private static T Appsb<T>(this T obj, StringBuilder stringBuilder)
		{
			stringBuilder.AppendLine(obj.ToString());
			return obj;
		}
		//Patch(AccessTools.Method(typeof(DamageWorker), nameof(DamageWorker.ExplosionCellsToHit), new Type[] { typeof(IntVec3), typeof(Map), typeof(float), typeof(IntVec3?), typeof(IntVec3?) }),
		//    transpiler: nameof(PrinterTranspiler));
		//Patch(AccessTools.Method(typeof(DamageWorker_Tes1), nameof(DamageWorker_Tes1.ExplosionCellsToHit), new Type[] { typeof(IntVec3), typeof(Map), typeof(float), typeof(IntVec3?), typeof(IntVec3?) }),
		//    transpiler: nameof(PrinterTranspiler));
		//Patch(AccessTools.Method(typeof(DamageWorker_Tes2), nameof(DamageWorker_Tes2.ExplosionCellsToHit), new Type[] { typeof(IntVec3), typeof(Map), typeof(float), typeof(IntVec3?), typeof(IntVec3?) }),
		//    transpiler: nameof(PrinterTranspiler));
		//Patch(AccessTools.Method(typeof(ExploritePatches), nameof(LocalTest1)),
		//    transpiler: nameof(PrinterTranspiler));
		//Patch(AccessTools.Method(typeof(ExploritePatches), nameof(LocalTest2)),
		//    transpiler: nameof(PrinterTranspiler));
		//Patch(AccessTools.Method(typeof(ExploritePatches), nameof(Fun6)),
		//    transpiler: nameof(PrinterTranspiler));

		//Patch(AccessTools.Method(typeof(ThoughtWorker_WearingColor), "CurrentStateInternal"),
		//    transpiler: nameof(PrinterTranspiler));
		//Patch(AccessTools.Method(typeof(ThoughtWorker_WearingColorX2), "CurrentStateInternal"),
		//    transpiler: nameof(PrinterTranspiler));
		///<summary>打印函数的构造。</summary>
		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> PrinterTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			StringBuilder stringBuilder = new StringBuilder();

			foreach (CodeInstruction ins in instr)
			{
				stringBuilder.AppendLine(ins.ToString());
				yield return ins;
			}
			Log.Message("[Explorite]instr result:\n" + stringBuilder.ToString());
			yield break;
		}
		static MethodInfo Patch(MethodBase original, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null, HarmonyMethod finalizer = null)
		{
			ExplortiePatchActionRecord record = new ExplortiePatchActionRecord(original, prefix, postfix, transpiler, finalizer);
			records.Add(record);
			return record.Patch();
		}
	}
}
