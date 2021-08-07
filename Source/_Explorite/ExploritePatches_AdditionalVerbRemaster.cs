/********************
 * 原始代码: https://github.com/solaris0115/AdditionalVerbMod
 * 由siiftun1857重制
 */
using System;
using System.Text;
using System.Collections.Generic;
using RimWorld;
using HarmonyLib;
using UnityEngine;
using Verse;
using static Explorite.ExploriteCore;
using System.Reflection.Emit;
using System.Reflection;

namespace Explorite
{
	internal static partial class ExploritePatches
	{
		public enum RangeCategory
		{
			Touch,
			Short,
			Medium,
			Long
		}
		public static IEnumerable<Gizmo> GetGizmosPostfix(IEnumerable<Gizmo> __result, Pawn_EquipmentTracker __instance)
		{
			if (__instance.Primary != null && __instance.Primary.def.weaponTags != null && __instance.Primary.def.weaponTags.Contains("AV"))
			{
				foreach (ThingComp tc in __instance.Primary.AllComps)
				{
					if (!(tc is CompForbiddable))
					{
						foreach (Gizmo g in tc.CompGetGizmosExtra())
						{
							yield return g;
						}
					}
				}
			}
			int count = 0;
			foreach (Gizmo g in __result)
			{
				if (g is Command command)
				{
					command.hotKey = count switch
					{
						0 => KeyBindingDefOf.Misc1,
						1 => KeyBindingDefOf.Misc3,
						2 => KeyBindingDefOf.Misc4,
						3 => KeyBindingDefOf.Misc5,
						4 => KeyBindingDefOf.Misc7,
						5 => KeyBindingDefOf.Misc8,
						6 => KeyBindingDefOf.Misc9,
						7 => KeyBindingDefOf.Misc10,
						8 => KeyBindingDefOf.Misc11,
						_ => KeyBindingDefOf.Misc12,
					};
				}
				yield return g;
				count++;
			}
		}
		public static void BeginTargetingPrefix(ITargetingSource source)
		{
			if (source is Verb verb && verb?.verbTracker?.directOwner != null && verb.DirectOwner is CompEquippable equippable)
			{
				if (equippable.parent.GetComp<Comp_VerbSaveable>() is Comp_VerbSaveable compVerbSave)
				{
					compVerbSave.tempVerb = verb;
				}
			}
		}
		public static void OrderPawnForceTargetPostfix(Targeter __instance, ITargetingSource targetingSource)
		{
			if (targetingSource is Verb verb && verb?.verbTracker?.directOwner != null && verb.DirectOwner is CompEquippable equippable)
			{
				if (equippable.parent.GetComp<Comp_VerbSaveable>() is Comp_VerbSaveable compVerbSave)
				{
					if (!Traverse.Create(__instance).Method("CurrentTargetUnderMouse", true).GetValue<LocalTargetInfo>().IsValid)
					{
						return;
					}
					compVerbSave.currentVerb = verb;
				}
			}

		}
		public static void StopTargetingPrefix(Verb ___targetingSource)
		{
			if (___targetingSource is Verb verb && verb?.verbTracker?.directOwner != null)
			{
				if (verb.DirectOwner is CompEquippable compEquip && compEquip.parent != null)
				{
					if (compEquip.parent.GetComp<Comp_VerbSaveable>() is Comp_VerbSaveable compVerbSave)
					{
						compVerbSave.tempVerb = null;
					}
				}
			}
		}

		///<summary>更改创建的Verb的Command。</summary>
		public static void CreateVerbTargetCommandPostfix(ref Command_VerbTarget __result, Thing ownerThing, Verb verb)
		{
			if (__result != null && verb.verbProps is IVerbPropertiesCustom verbProps)
			{
				Comp_VerbSaveable comp_VerbSaveable = ownerThing.TryGetComp<Comp_VerbSaveable>();
				Verb_Shoot_Cooldown verb_with_cooldown = verb as Verb_Shoot_Cooldown;

				if (verb.verbProps.label != null)
					__result.defaultLabel = verb.verbProps.label;
				if (verbProps.ExVerbProp().desc != null)
					__result.defaultDesc = verbProps.ExVerbProp().desc;

				if (!__result.disabled || verb.caster.Faction == Faction.OfPlayer)
				{

					if (verbProps.ExVerbProp().disable || !verbProps.ExVerbProp().CheckOut(ownerThing))
					{
						__result.Disable(verbProps.ExVerbProp().disableReason);
					}
					else if (!(verb_with_cooldown?.CanFire() ?? true))
					{
						__result.Disable("Magnuassembly_AdditionalVerbPatch_CooldownDisableReason".Translate(
							FormattingTickTime(verb_with_cooldown.RemainingTickBeforeFire() / 60.0D)
							)
							);
					}
				}

				if (comp_VerbSaveable != null && comp_VerbSaveable.currentVerb == verb)
				{
					if(verbProps.ExVerbProp().textureSelected != null)
						__result.icon = verbProps.ExVerbProp().textureSelected;
				}
				else
				{
					if (verbProps.ExVerbProp().texture != null)
						__result.icon = verbProps.ExVerbProp().texture;
				}
			}
		}

		///<summary>更改武器的主Verb。</summary>
		public static bool PrimaryVerbPrefix(CompEquippable __instance, ref Verb __result)
		{
			//Comp_VerbSaveable comp = ((CompEquippable)(__instance.directOwner)).parent.GetComp<Comp_VerbSaveable>();//VerbTracker
			if (__instance.parent.GetComp<Comp_VerbSaveable>() is Comp_VerbSaveable comp)
			{
				comp.RelocateCooldownVerb();
				__result = comp.currentVerb;
				if (__result != null)
				{
					return false;
				}
			}
			return true;
		}

		public static void AdjustedAccuracyPostfix(VerbProperties __instance, ref float __result, RangeCategory cat)
		{
			if (__instance is VerbProperties_Custom)
			{
				__result += cat switch
				{
					RangeCategory.Touch => __instance.accuracyTouch,
					RangeCategory.Short => __instance.accuracyShort,
					RangeCategory.Medium => __instance.accuracyMedium,
					RangeCategory.Long => __instance.accuracyLong,
					_ => throw new InvalidOperationException(),
				};
			}
		}


		public static bool IsPerfectionProjectile(ThingDef thingDef)
		{
			return typeof(Bullet_Direct).IsAssignableFrom(thingDef?.thingClass);
		}

		public static Verb ReplaceVerbToTempSav(Verb pass, Thing target)
		{
			if (Find.Selector?.SingleSelectedThing is Pawn pawn && pawn != target && pawn?.equipment?.Primary?.GetComp<Comp_VerbSaveable>()?.tempVerb is Verb_LaunchProjectile verbToReplace)
			{
				return verbToReplace;
			}
			return pass;
		}

		///<summary>修正瞄准命中率预览。</summary>
		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> ShotCalculationTipStringTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			byte patchActionStage = 0;
			MethodInfo CompEquippablePrimaryVerbInfo = AccessTools.Method(typeof(CompEquippable), "get_PrimaryVerb");
			MethodInfo VerbCanHitTargetInfo = AccessTools.Method(typeof(Verb), nameof(Verb.CanHitTarget));
			MethodInfo VerbGetProjectileInfo = AccessTools.Method(typeof(VerbUtility), nameof(VerbUtility.GetProjectile));
			MethodInfo StringBuilderAppendInfo = AccessTools.Method(typeof(StringBuilder), nameof(StringBuilder.Append), new Type[] { typeof(string) });

			Label label1 = ilg.DefineLabel();
			Label label2 = ilg.DefineLabel();
			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0 && ins.operand == CompEquippablePrimaryVerbInfo as object)
				{
					patchActionStage++;
					yield return ins;
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, ((Func<Verb, Thing, Verb>)ReplaceVerbToTempSav).GetMethodInfo());
					continue;
				}
				if (patchActionStage == 1 && ins.operand == VerbCanHitTargetInfo as object)
				{
					patchActionStage++;
					yield return ins;
					continue;
				}
				if (patchActionStage == 2 && ins.opcode == OpCodes.Brfalse_S)
				{
					patchActionStage++;
					yield return ins;
					yield return new CodeInstruction(OpCodes.Ldloc_2);
					yield return new CodeInstruction(OpCodes.Call, VerbGetProjectileInfo);
					yield return new CodeInstruction(OpCodes.Call, ((Func<ThingDef, bool>)IsPerfectionProjectile).GetMethodInfo());
					yield return new CodeInstruction(OpCodes.Brfalse_S, label1);
					yield return new CodeInstruction(OpCodes.Ldloc_0);
					yield return new CodeInstruction(OpCodes.Ldstr, " 100%");
					yield return new CodeInstruction(OpCodes.Br_S, label2);
					continue;
				}
				if (patchActionStage == 3)
				{
					patchActionStage++;
					ins.labels.Add(label1);
					yield return ins;
					continue;
				}
				if (patchActionStage == 4 && ins.operand == StringBuilderAppendInfo as object)
				{
					patchActionStage++;
					ins.labels.Add(label2);
					yield return ins;
					continue;
				}
				else
				{
					yield return ins;
					continue;
				}
			}
			yield break;
		}

		public static void VerbLaunchProjectileHighlightFieldRadiusAroundTargetPostfix(ref float __result, Verb_LaunchProjectile __instance, ref bool needLOSToCenter)
		{
			if(__instance.GetDamageDef().ExplosionBypassEverything())
			{
				needLOSToCenter = false;
			}
		}

		/*
		internal static ThingDef perfectionAimPassThingDef = new ThingDef() { defName = "EXperfectionAimPass" };
		internal static ShotReport perfectionAimPass = ((Func<ShotReport>)(() =>
		{
			ShotReport result = new ShotReport();
			typeof(ShotReport).GetField("coveringGas", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(result, perfectionAimPass);
			return result;
		}))();
		public static bool ShotReportHitReportForPrefix(ref ShotReport __result, Thing caster, Verb verb, LocalTargetInfo target)
		{
			if (typeof(Bullet_Direct).IsAssignableFrom(verb?.GetProjectile()?.thingClass))
			{
				__result = perfectionAimPass;
				return false;
			}
			return true;
		}
		public static bool ShotReportGetTextReadoutPrefix(ref string __result, ShotReport __instance)
		{
			if (typeof(ShotReport).GetField("coveringGas", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is ThingDef def && def == perfectionAimPassThingDef)
			{
				__result = " 100%";
				return false;
			}
			return true;
		}

		public static bool ShotCalculationTipStringPrefix(ref string __result, Thing target)
		{
			if (Find.Selector.SingleSelectedThing != null)
			{
				Thing singleSelectedThing = Find.Selector.SingleSelectedThing;
				Verb verb = null;
				if (singleSelectedThing is Pawn pawn && pawn != target && pawn?.equipment?.PrimaryEq?.PrimaryVerb != null)
				{
					verb = pawn.equipment.PrimaryEq.PrimaryVerb;
				}
				else if (singleSelectedThing is Building_TurretGun building_TurretGun && building_TurretGun != target)
				{
					verb = building_TurretGun.AttackVerb;
				}
				if (verb != null && typeof(Bullet_Direct).IsAssignableFrom(verb?.GetProjectile()?.thingClass))
				{
					__result = "ShotBy".Translate(singleSelectedThing.LabelShort, singleSelectedThing) + ": " + (verb.CanHitTarget(target) ? new TaggedString("100%") : "CannotHit".Translate());
					return false;
				}
			}
			return true;
		}

		[Obsolete]public static bool ShotCalculationTipStringPrefix(ref string __result, Thing target)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (Find.Selector.SingleSelectedThing != null)
			{
				Thing singleSelectedThing = Find.Selector.SingleSelectedThing;
				Verb verb = null;
				if (singleSelectedThing is Pawn pawn && pawn != target && pawn.equipment != null && pawn.equipment.Primary != null)
				{
					Comp_VerbSaveable compsav = pawn.equipment.Primary.GetComp<Comp_VerbSaveable>();
					if (compsav != null && compsav.tempVerb != null && compsav.tempVerb is Verb_LaunchProjectile)
					{
						verb = compsav.tempVerb;
					}
					else
					{
						verb = pawn.equipment.PrimaryEq.PrimaryVerb;
					}
				}
				if (singleSelectedThing is Building_TurretGun building_TurretGun && building_TurretGun != target)
				{
					verb = building_TurretGun.AttackVerb;
				}
				if (verb != null)
				{
					stringBuilder.Append("ShotBy".Translate(Find.Selector.SingleSelectedThing.LabelShort, Find.Selector.SingleSelectedThing) + ": ");
					if (verb.CanHitTarget(target))
					{
						stringBuilder.Append(ShotReport.HitReportFor(verb.caster, verb, target).GetTextReadout());
					}
					else
					{
						stringBuilder.AppendLine("CannotHit".Translate());
					}
					if (target is Pawn pawn2 && pawn2.Faction == null && !pawn2.InAggroMentalState)
					{
						float manhunterOnDamageChance;
						if (verb.IsMeleeAttack)
						{
							manhunterOnDamageChance = PawnUtility.GetManhunterOnDamageChance(pawn2, 0f, singleSelectedThing);
						}
						else
						{
							manhunterOnDamageChance = PawnUtility.GetManhunterOnDamageChance(pawn2, singleSelectedThing);
						}
						if (manhunterOnDamageChance > 0f)
						{
							stringBuilder.AppendLine();
							stringBuilder.AppendLine(string.Format("{0}: {1}", "ManhunterPerHit".Translate(), manhunterOnDamageChance.ToStringPercent()));
						}
					}
				}
			}
			__result = stringBuilder.ToString();
			return false;
		}

		[Obsolete]public static bool CreateVerbTargetCommandPrefix(ref Command_VerbTarget __result, Thing ownerThing, Verb verb)
		{
			Command_VerbTarget command_VerbTarget = new Command_VerbTarget();
			VerbProperties_Custom verbProps = verb.verbProps as VerbProperties_Custom;
			Verb_Shoot_Cooldown verb_with_cooldown = verb as Verb_Shoot_Cooldown;
			if (verbProps != null)
			{
				command_VerbTarget.defaultDesc = verbProps.ExVerbProp().desc;
				command_VerbTarget.defaultLabel = verbProps.label;
				Comp_VerbSaveable comp_VerbSaveable = ownerThing.TryGetComp<Comp_VerbSaveable>();
				if (!verbProps.ExVerbProp().disable)
				{
					if (verb_with_cooldown != null)
					{
						if (!verb_with_cooldown.CanFire())
						{
							command_VerbTarget.Disable("Magnuassembly_AdditionalVerbPatch_CooldownDisableReason".Translate(
								FormattingTickTime(verb_with_cooldown.RemainingTickBeforeFire() / 60.0D)
								)
								);
						}
						else
						{
							command_VerbTarget.disabled = false;
						}
					}
				}
				if (comp_VerbSaveable != null && comp_VerbSaveable.currentVerb == verb)
				{
					command_VerbTarget.icon = verb_with_cooldown != null ?
						FloodingTexture(
							verbProps.ExVerbProp().textureSelected,
							verb_with_cooldown.RemainingProgressBeforeFire()
							) : verbProps.ExVerbProp().textureSelected;

					if (command_VerbTarget.disabled && verb_with_cooldown != null)
					{
						comp_VerbSaveable.SwitchVerb(verbProps.ExVerbProp().redirectVerbAfterShoot);
					}
				}
				else
				{
					command_VerbTarget.icon = verb_with_cooldown != null ?
						FloodingTexture(
							verbProps.ExVerbProp().texture,
							verb_with_cooldown.RemainingProgressBeforeFire()
							) : verbProps.ExVerbProp().texture;
				}
			}
			else
			{
				command_VerbTarget.icon = ownerThing.def.uiIcon;
				command_VerbTarget.defaultDesc = ownerThing.LabelCap + ": " + ownerThing.def.description.CapitalizeFirst();
			}
			command_VerbTarget.iconAngle = ownerThing.def.uiIconAngle;
			command_VerbTarget.iconOffset = ownerThing.def.uiIconOffset;
			command_VerbTarget.tutorTag = "VerbTarget";
			command_VerbTarget.verb = verb;
			bool disableReasonOverwrite = false;
			if (verb.caster.Faction == Faction.OfPlayer)
			{
				if (verb.CasterIsPawn)
				{
					if (verb.CasterPawn.WorkTagIsDisabled(WorkTags.Violent))
					{
						command_VerbTarget.Disable("IsIncapableOfViolence".Translate(verb.CasterPawn.LabelShort, verb.CasterPawn));
						disableReasonOverwrite = true;
					}
					else if (!verb.CasterPawn.drafter.Drafted)
					{
						command_VerbTarget.Disable("IsNotDrafted".Translate(verb.CasterPawn.LabelShort, verb.CasterPawn));
						disableReasonOverwrite = true;
					}
				}
			}
			else
			{
				command_VerbTarget.Disable("CannotOrderNonControlled".Translate());
				disableReasonOverwrite = true;
			}
			if (disableReasonOverwrite)
			{
				if (verbProps != null && (verbProps.ExVerbProp().disable || !verbProps.ExVerbProp().CheckOut(ownerThing)))
				{
					command_VerbTarget.disabledReason = verbProps.ExVerbProp().disableReason + "\n" + command_VerbTarget.disabledReason;
				}
				else if (verb_with_cooldown != null && !verb_with_cooldown.CanFire())
				{
					command_VerbTarget.disabledReason =
						"Magnuassembly_AdditionalVerbPatch_CooldownDisableReason".Translate(
							FormattingTickTime(verb_with_cooldown.RemainingTickBeforeFire() / 60.0D)
						) + "\n" + command_VerbTarget.disabledReason;
				}
			}
			else if (verbProps != null && (verbProps.ExVerbProp().disable || !verbProps.ExVerbProp().CheckOut(ownerThing)))
			{
				command_VerbTarget.Disable(verbProps.ExVerbProp().disableReason);
			}
			__result = command_VerbTarget;
			return false;
		}

		[Obsolete]public static void GetGizmosPrefix(ref Pawn_EquipmentTracker __instance, ref IEnumerable<Gizmo> __result)
		{
			if (__instance.Primary != null)
			{
				foreach (ThingComp tc in __instance.Primary.AllComps)
				{
					foreach (Gizmo g in tc.CompGetGizmosExtra())
					{
						__result.AddItem(g);
					}
				}
			}
		}

		[Obsolete]public static bool GetTargetingVerbPrefix(ref Verb __result, Pawn pawn, Verb ___targetingVerb)
		{
			return true;
		}
		
		[Obsolete]public static void GizmoOnGUIPostfix(ref GizmoResult __result, Vector2 topLeft, float maxWidth, Command_VerbTarget __instance)
		{
			Text.Font = GameFont.Tiny;
			Rect rect = new Rect(topLeft.x, topLeft.y, __instance.GetWidth(maxWidth), 75f);

			if (((Verb_Shoot_Cooldown)__instance.verb).RemainingProgressBeforeFire() != 0)
			{
				Widgets.FillableBar(rect,
					((Verb_Shoot_Cooldown)__instance.verb).RemainingProgressBeforeFire(),
					((VerbProperties_Custom)((Verb_Shoot_Cooldown)__instance.verb).verbProps).texture,
					((VerbProperties_Custom)((Verb_Shoot_Cooldown)__instance.verb).verbProps).textureCooldown,
					false
					);
			}

			bool flag = false;
			if (Mouse.IsOver(rect))
			{
				flag = true;
				if (!__instance.disabled)
				{
					GUI.color = GenUI.MouseoverColor;
				}
			}
			Texture2D badTex = __instance.icon;
			if (badTex == null)
			{
				badTex = BaseContent.BadTex;
			}
			Material material = (!__instance.disabled) ? null : TexUI.GrayscaleGUI;
			GenUI.DrawTextureWithMaterial(rect, Command.BGTex, material, default(Rect));
			MouseoverSounds.DoRegion(rect, SoundDefOf.Mouseover_Command);
			Rect outerRect = rect;
			outerRect.position += new Vector2(__instance.iconOffset.x * outerRect.size.x, __instance.iconOffset.y * outerRect.size.y);
			GUI.color = __instance.IconDrawColor;
			Widgets.DrawTextureFitted(outerRect, badTex, __instance.iconDrawScale * 0.85f, __instance.iconProportions, __instance.iconTexCoords, __instance.iconAngle, material);
			GUI.color = Color.white;
			bool flag2 = false;
			KeyCode keyCode = (__instance.hotKey != null) ? __instance.hotKey.MainKey : KeyCode.None;
			if (keyCode != KeyCode.None && !GizmoGridDrawer.drawnHotKeys.Contains(keyCode))
			{
				Rect rect2 = new Rect(rect.x + 5f, rect.y + 5f, rect.width - 10f, 18f);
				Widgets.Label(rect2, keyCode.ToStringReadable());
				GizmoGridDrawer.drawnHotKeys.Add(keyCode);
				if (__instance.hotKey.KeyDownEvent)
				{
					flag2 = true;
					Event.current.Use();
				}
			}
			if (Widgets.ButtonInvisible(rect, false))
			{
				flag2 = true;
			}
			string labelCap = __instance.LabelCap;
			if (!labelCap.NullOrEmpty())
			{
				float num = Text.CalcHeight(labelCap, rect.width);
				Rect rect3 = new Rect(rect.x, rect.yMax - num + 12f, rect.width, num);
				GUI.DrawTexture(rect3, TexUI.GrayTextBG);
				GUI.color = Color.white;
				Text.Anchor = TextAnchor.UpperCenter;
				Widgets.Label(rect3, labelCap);
				Text.Anchor = TextAnchor.UpperLeft;
				GUI.color = Color.white;
			}
			GUI.color = Color.white;
			if (true)
			{
				TipSignal tip = __instance.Desc;
				if (__instance.disabled && !__instance.disabledReason.NullOrEmpty())
				{
					string text = tip.text;
					tip.text = string.Concat(new string[]
					{
						text,
						"\n\n",
						"DisabledCommand".Translate(),
						": ",
						__instance.disabledReason
					});
				}
				TooltipHandler.TipRegion(rect, tip);
			}
			if (!__instance.HighlightTag.NullOrEmpty() && (Find.WindowStack.FloatMenu == null || !Find.WindowStack.FloatMenu.windowRect.Overlaps(rect)))
			{
				UIHighlighter.HighlightOpportunity(rect, __instance.HighlightTag);
			}
			Text.Font = GameFont.Small;
			if (flag2)
			{
				if (__instance.disabled)
				{
					if (!__instance.disabledReason.NullOrEmpty())
					{
						Messages.Message(__instance.disabledReason, MessageTypeDefOf.RejectInput, false);
					}
					__result = new GizmoResult(GizmoState.Mouseover);
					return;
				}
				GizmoResult result;
				if (Event.current.button == 1)
				{
					result = new GizmoResult(GizmoState.OpenedFloatMenu, Event.current);
				}
				else
				{
					if (!TutorSystem.AllowAction(__instance.TutorTagSelect))
					{
						__result = new GizmoResult(GizmoState.Mouseover);
						return;
					}
					result = new GizmoResult(GizmoState.Interacted, Event.current);
					TutorSystem.Notify_Event(__instance.TutorTagSelect);
				}
				__result = result;
				return;
			}
			else
			{
				if (flag)
				{
					__result = new GizmoResult(GizmoState.Mouseover);
					return;
				}
				__result = new GizmoResult(GizmoState.Clear);
				return;
			}
		}

		public static void GizmoOnGUIPostfix(ref GizmoResult __result, Command_VerbTarget __instance, Vector2 topLeft, float maxWidth)
		{
			GizmoOnGUIPostfixPRE(ref __result, __instance, topLeft, maxWidth);

			var rect = new Rect(topLeft.x, topLeft.y, __instance.GetWidth(maxWidth), 75f);
			if (((Verb_Shoot_Cooldown)__instance.verb).RemainingProgressBeforeFire() != 0)
			{
				Widgets.FillableBar(rect,
					((Verb_Shoot_Cooldown)__instance.verb).RemainingProgressBeforeFire(),
					((VerbProperties_Custom)((Verb_Shoot_Cooldown)__instance.verb).verbProps).texture,
					((VerbProperties_Custom)((Verb_Shoot_Cooldown)__instance.verb).verbProps).textureCooldown,
					false
					);
			}
		}
		*/
	}

	public class Comp_VerbSaveable : ThingComp
	{
		public Verb currentVerb;
		public Verb tempVerb;
		public void SwitchVerb(int index = -1)
		{
			CompEquippable comp = parent.GetComp<CompEquippable>();
			List<Verb> verbs = comp.AllVerbs;
			if (index <= verbs.Count && index >= 0)
			{
				currentVerb = verbs[index];
			}
			else
			{
				CurrVerbReset();
			}
		}
		public void RelocateCooldownVerb()
		{
			if (currentVerb is Verb_Shoot_Cooldown verb_with_cooldown && !verb_with_cooldown.CanFire())
			{
				if (currentVerb.verbProps is VerbProperties_Custom verbProps && verbProps.ExVerbProp().redirectVerbAfterShoot >= 0 && parent.GetComp<CompEquippable>().AllVerbs.IndexOf(verb_with_cooldown) != verbProps.ExVerbProp().redirectVerbAfterShoot)
				{
					SwitchVerb(verbProps.ExVerbProp().redirectVerbAfterShoot);
				}
			}
		}
		public void CurrVerbReset()
		{
			CompEquippable comp = parent.GetComp<CompEquippable>();
			List<Verb> verbs = comp.AllVerbs;
			if (comp == null)
			{
				Log.ErrorOnce("[Explorite]CurrVerbReset Error: comp is null.", 41422);
				return;
			}
			if (verbs == null)
			{
				Log.ErrorOnce("[Explorite]CurrVerbReset Error: verbs is null, at comp: " + comp + ".", 41421);
				return;
			}
			if (verbs.Count == 0)
			{
				Log.ErrorOnce("[Explorite]CurrVerbReset Error: verbs contains no verb, at comp: " + comp + ", verbs:" + verbs + ".", 41423);
				return;
			}
			if (currentVerb != null)
			{
				for (int i = 0; i < verbs.Count; i++)
				{
					if (currentVerb == verbs[i])
					{
						return;
					}
				}
			}
			for (int i = 0; i < verbs.Count; i++)
			{
				if (verbs[i].verbProps.isPrimary)
				{
					currentVerb = verbs[i];
					return;
				}
			}
			currentVerb = verbs[0];
		}
		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			/*CompEquippable comp = parent.GetComp<CompEquippable>();
			if (currentVerb == null)
			{
				List<Verb> verbs = comp.AllVerbs;
				for (int i = 0; i < verbs.Count; i++)
				{
					if (verbs[i].verbProps.isPrimary)
					{
						currentVerb = verbs[i];
						return;
					}
				}
			}*/
			CurrVerbReset();
		}
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_References.Look(ref currentVerb, "currentVerb");
			if (Scribe.mode == LoadSaveMode.PostLoadInit
				//&& currentVerb == null
				)
			{
				/*CompEquippable comp = parent.GetComp<CompEquippable>();
				List<Verb> verbs = comp.AllVerbs;
				for (int i = 0; i < verbs.Count; i++)
				{
					if (verbs[i].verbProps.isPrimary)
					{
						currentVerb = verbs[i];
						return;
					}
				}*/
				CurrVerbReset();
			}
		}
	}

	public interface IVerbPropertiesCustom
	{
		public ExploriteVerbProperties ExVerbProp();
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage(null, "IDE0649")]
	public class ExploriteVerbProperties
	{
		public string desc;
		public string texPath;
		public string texPathSelected;
		public string texPathCooldown;
		public bool disable = false;
		public string disableReason = null;
		public Texture2D texture;
		public Texture2D textureSelected;
		public Texture2D textureCooldown;
		public int maxMagazine;
		public int redirectVerbAfterShoot = -1;
		public int cooldownTick = 0;
		public bool ownByPawnEquipment = false;
		public ThingDef secondaryProjectile;
		public CheckOutList<string> enableTagsValidator = new CheckOutList<string>();

		public bool CheckOut(Thing thing)
		{
			return enableTagsValidator.CheckOut(thing.AllStateTags());
		}
		public ExploriteVerbProperties()
		{
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				if (!texPath.NullOrEmpty())
				{
					texture = ContentFinder<Texture2D>.Get(texPath);
				}
				if (!texPathSelected.NullOrEmpty())
				{
					textureSelected = ContentFinder<Texture2D>.Get(texPathSelected);
				}
				else
				{
					textureSelected = texture;
				}
				if (!texPathCooldown.NullOrEmpty())
				{
					textureCooldown = ContentFinder<Texture2D>.Get(texPathCooldown);
				}
				else
				{
					textureCooldown = texture;
				}
				if (cooldownTick < 0)
				{
					Log.Warning($"[Explorite]Detected cooldownTick {cooldownTick}, which is not more than 0.");
				}
			});
		}
	}
	public class VerbProperties_Custom : VerbProperties, IVerbPropertiesCustom
	{
		public ExploriteVerbProperties exProps = new ExploriteVerbProperties();
		public ExploriteVerbProperties ExVerbProp()
		{
			return exProps;
		}
	}
}