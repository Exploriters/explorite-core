/********************
 * 方法强制调用器。
 * --siiftun1857
 */
using Verse;
using System;
using UnityEngine;
using RimWorld;
using HarmonyLib;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Explorite
{
	///<summary>方法强制调用器。</summary>
	public static class MethodForceInvoker
	{
		private static readonly Dictionary<MethodInfo, MethodInfo> database = new Dictionary<MethodInfo, MethodInfo>();
		public static MethodInfo GetForceInvoker(this MethodInfo method)
		{
			if (database.TryGetValue(method, out MethodInfo invoker))
			{
				return invoker;
			}

			Type[] argTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
			int argc = argTypes.Length + (method.IsStatic ? 0 : 1);
			//Log.Message($"[Explorite] argc {argc}, ret {method.ReturnType} : {method.FullDescription()}");
			DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, method.ReturnType,
				(method.IsStatic ? Array.Empty<Type>() : new Type[] { method.DeclaringType }).Concat(argTypes).ToArray()
				);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			for (int i = 0; i < argc; i++)
			{
				switch (i)
				{
					case 0:
						iLGenerator.Emit(OpCodes.Ldarg_0);
						break;
					case 1:
						iLGenerator.Emit(OpCodes.Ldarg_1);
						break;
					case 2:
						iLGenerator.Emit(OpCodes.Ldarg_2);
						break;
					case 3:
						iLGenerator.Emit(OpCodes.Ldarg_3);
						break;
					default:
						iLGenerator.Emit(i <= byte.MaxValue ? OpCodes.Ldarg_S : OpCodes.Ldarg, i);
						break;
				}
			}
			iLGenerator.Emit(OpCodes.Call, method);
			iLGenerator.Emit(OpCodes.Ret);
			database.Add(method, dynamicMethod);
			return dynamicMethod;
		}
		public static object InvokeForce(this MethodInfo method, object[] parameters)
		{
			return method.GetForceInvoker().Invoke(null, parameters);
		}
	}
}
