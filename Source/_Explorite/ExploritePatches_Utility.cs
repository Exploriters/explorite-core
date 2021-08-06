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
			public string prefix;
			public string postfix;
			public string transpiler;
			public string finalizer;
			public ExplortiePatchActionRecordState state;

			public ExplortiePatchActionRecord(MethodBase original, string prefix = null, string postfix = null, string transpiler = null, string finalizer = null)
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
						prefix: prefix == null ? null : new HarmonyMethod(patchType, prefix),
						postfix: postfix == null ? null : new HarmonyMethod(patchType, postfix),
						transpiler: transpiler == null ? null : new HarmonyMethod(patchType, transpiler),
						finalizer: finalizer == null ? null : new HarmonyMethod(patchType, finalizer)
						);
				}
				catch (Exception e)
				{
					Log.Error(string.Concat(
						"[Explorite]Patch sequence failare at ",
						$"{original.FullDescription()}, ",
						prefix != null ? "prefix: " + prefix + ", " : "",
						postfix != null ? "postfix: " + postfix + ", " : "",
						transpiler != null ? "transpiler: " + transpiler + ", " : "",
						finalizer != null ? "finalizer: " + finalizer + ", " : "",
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
		internal static readonly Type patchType = typeof(ExploritePatches);

		static string PrintPatches()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ExplortiePatchActionRecord record in records)
			{
				stringBuilder.AppendLine(string.Concat(
					$"{record.original.FullDescription()}",
					record.prefix != null ? $"\n - prefix: {record.prefix}" : "",
					record.postfix != null ? $"\n - postfix: {record.postfix}" : "",
					record.transpiler != null ? $"\n - transpiler: {record.transpiler}" : "",
					record.finalizer != null ? $"\n - finalizer: {record.finalizer}" : ""
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
		static MethodInfo Patch(MethodBase original, string prefix = null, string postfix = null, string transpiler = null, string finalizer = null)
		{
			ExplortiePatchActionRecord record = new ExplortiePatchActionRecord(original, prefix, postfix, transpiler, finalizer);
			records.Add(record);
			return record.Patch();
		}
	}
}
