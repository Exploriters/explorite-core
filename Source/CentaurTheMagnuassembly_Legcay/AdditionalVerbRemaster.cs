/*****************************************
 * Code from https://github.com/solaris0115/AdditionalVerbMod
 * reworked selected icon
 */
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
//using Harmony;
using HarmonyLib;
using UnityEngine;
using Verse.AI;
using Verse;
using Verse.Sound;
using static CentaurTheMagnuassembly.RimCentaurCore;

namespace CentaurTheMagnuassembly
{
    [StaticConstructorOnStartup]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("", "IDE0060")]
    public static class AdditionalVerbPatch
    {
        private static readonly Type patchType = typeof(AdditionalVerbPatch);
        static AdditionalVerbPatch()
        {
            //HarmonyInstance harmonyInstance = HarmonyInstance.Create("CentaurTheMagnuassembly.rimworld.mod.AdditionalVerbPatch");
            Harmony harmonyInstance = new Harmony("CentaurTheMagnuassembly.rimworld.mod.AdditionalVerbPatch");
            //if (!Harmony.HasAnyPatches("CentaurTheMagnuassembly.rimworld.mod.AdditionalVerbPatch"))
            //{
                /*
                //harmonyInstance.Patch(AccessTools.Method(typeof(Command_VerbTarget), "GizmoOnGUI", null, null), null, new HarmonyMethod(patchType, "GizmoOnGUIPostfix", null));
                //harmonyInstance.Patch(AccessTools.Method(typeof(Targeter), "GetTargetingVerb", null, null), new HarmonyMethod(patchType, "GetTargetingVerbPrefix", null));

                harmonyInstance.Patch(AccessTools.Method(typeof(Pawn_EquipmentTracker), "GetGizmos", null, null), null, new HarmonyMethod(patchType, "GetGizmosPostfix", null));

                harmonyInstance.Patch(AccessTools.Method(typeof(Targeter), "BeginTargeting", new Type[] { typeof(ITargetingSource), typeof(ITargetingSource) }), new HarmonyMethod(patchType, "BeginTargetingPrefix", null));
                harmonyInstance.Patch(AccessTools.Method(typeof(Targeter), "OrderPawnForceTarget"), null, new HarmonyMethod(patchType, "OrderPawnForceTargetPostfix", null));
                harmonyInstance.Patch(AccessTools.Method(typeof(Targeter), "StopTargeting", null, null), new HarmonyMethod(patchType, "StopTargetingPrefix", null));

                harmonyInstance.Patch(AccessTools.Method(typeof(VerbTracker), "CreateVerbTargetCommand", null, null), new HarmonyMethod(patchType, "CreateVerbTargetCommandPrefix", null));
                harmonyInstance.Patch(AccessTools.Property(typeof(VerbTracker), "PrimaryVerb").GetGetMethod(), new HarmonyMethod(patchType, "PrimaryVerbPrefix", null));

                harmonyInstance.Patch(AccessTools.Method(typeof(VerbProperties), "AdjustedAccuracy", null, null), null, new HarmonyMethod(patchType, "AdjustedAccuracyPostfix", null));

                harmonyInstance.Patch(AccessTools.Method(typeof(TooltipUtility), "ShotCalculationTipString", null, null), new HarmonyMethod(patchType, "ShotCalculationTipStringPrefix", null));
                */
                harmonyInstance.Patch(AccessTools.Method(typeof(Pawn_EquipmentTracker), "GetGizmos", null, null), null, new HarmonyMethod(patchType, "GetGizmosPostfix", null));

                harmonyInstance.Patch(AccessTools.Method(typeof(Targeter), "BeginTargeting", new Type[] { typeof(ITargetingSource), typeof(ITargetingSource) }), new HarmonyMethod(patchType, "BeginTargetingPrefix", null));
                harmonyInstance.Patch(AccessTools.Method(typeof(Targeter), "OrderPawnForceTarget"), null, new HarmonyMethod(patchType, "OrderPawnForceTargetPostfix", null));
                harmonyInstance.Patch(AccessTools.Method(typeof(Targeter), "StopTargeting", null, null), new HarmonyMethod(patchType, "StopTargetingPrefix", null));

                harmonyInstance.Patch(AccessTools.Method(typeof(VerbTracker), "CreateVerbTargetCommand", null, null), new HarmonyMethod(patchType, "CreateVerbTargetCommandPrefix", null));
                harmonyInstance.Patch(AccessTools.Property(typeof(CompEquippable), "PrimaryVerb").GetGetMethod(), new HarmonyMethod(patchType, "PrimaryVerbPrefix", null));

                harmonyInstance.Patch(AccessTools.Method(typeof(VerbProperties), "AdjustedAccuracy", null, null), null, new HarmonyMethod(patchType, "AdjustedAccuracyPostfix", null));

                harmonyInstance.Patch(AccessTools.Method(typeof(TooltipUtility), "ShotCalculationTipString", null, null), new HarmonyMethod(patchType, "ShotCalculationTipStringPrefix", null));
                /*LongEventHandler.ExecuteWhenFinished
                (
                    delegate
                    {
                        currentCommandTexture = ContentFinder<Texture2D>.Get("UI/Commands/Select");
                    }
                );*/
            //}
        }
        //public static Texture2D currentCommandTexture;
        public enum RangeCategory
        {
            Touch,
            Short,
            Medium,
            Long
        }
        public static void GetGizmosPrefix(ref Pawn_EquipmentTracker __instance, ref IEnumerable<Gizmo> __result)
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
                if (g is Command)
                {
                    ((Command)g).hotKey = count switch
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
            Verb verb = source as Verb;
            if (verb == null)
            {
                return;
            }
            if (verb.verbTracker != null && verb.verbTracker.directOwner != null && verb.DirectOwner is CompEquippable)
            {
                Comp_VerbSaveable comp = ((CompEquippable)verb.DirectOwner).parent.GetComp<Comp_VerbSaveable>();
                if (comp != null)
                {
                    comp.tempVerb = verb;
                }
            }
        }
        public static void OrderPawnForceTargetPostfix(Targeter __instance, ITargetingSource targetingSource)
        {
            Verb verb = targetingSource as Verb;
            if (verb == null)
            {
                return;
            }
            if (verb.verbTracker != null && verb.verbTracker.directOwner != null && verb.DirectOwner is CompEquippable)
            {
                Comp_VerbSaveable comp = ((CompEquippable)verb.DirectOwner).parent.GetComp<Comp_VerbSaveable>();
                if (comp != null)
                {
                    if (!(Traverse.Create(__instance).Method("CurrentTargetUnderMouse", true).GetValue<LocalTargetInfo>().IsValid))
                    {
                        return;
                    }
                    comp.currentVerb = verb;
                }
            }

        }
        public static bool GetTargetingVerbPrefix(ref Verb __result, Pawn pawn, Verb ___targetingVerb)
        {
            return true;
        }
        public static void StopTargetingPrefix(Verb ___targetingSource)
        {
            Verb verb = ___targetingSource as Verb;
            if (verb == null)
            {
                return;
            }
            if (verb.verbTracker != null && verb.verbTracker.directOwner != null)
            {
                CompEquippable compEquip = verb.DirectOwner as CompEquippable;
                if (compEquip != null && compEquip.parent != null)
                {
                    Comp_VerbSaveable compVerbSave = compEquip.parent.GetComp<Comp_VerbSaveable>();
                    if (compVerbSave != null)
                    {
                        compVerbSave.tempVerb = null;
                    }
                }
            }
        }
        /*public static void GizmoOnGUIPostfix(ref GizmoResult __result, Vector2 topLeft, float maxWidth, Command_VerbTarget __instance)
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
                    __result = new GizmoResult(GizmoState.Mouseover, null);
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
                        __result = new GizmoResult(GizmoState.Mouseover, null);
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
                    __result = new GizmoResult(GizmoState.Mouseover, null);
                    return;
                }
                __result = new GizmoResult(GizmoState.Clear, null);
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
        }*/

        public static bool CreateVerbTargetCommandPrefix(ref Command_VerbTarget __result, Thing ownerThing, Verb verb)
        {
            Command_VerbTarget command_VerbTarget = new Command_VerbTarget();
            VerbProperties_Custom verbProps = verb.verbProps as VerbProperties_Custom;
            Verb_Shoot_Cooldown verb_with_cooldown = verb is Verb_Shoot_Cooldown ? verb as Verb_Shoot_Cooldown : null;
            if (verbProps != null)
            {
                command_VerbTarget.defaultDesc = verbProps.desc;
                command_VerbTarget.defaultLabel = verbProps.label;
                Comp_VerbSaveable comp_VerbSaveable = ownerThing.TryGetComp<Comp_VerbSaveable>();
                if (!verbProps.disable)
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
                            verbProps.textureSelected,
                            verb_with_cooldown.RemainingProgressBeforeFire()
                            ) : verbProps.textureSelected;

                    if (command_VerbTarget.disabled && verb_with_cooldown != null)
                    {
                        comp_VerbSaveable.SwitchVerb(verbProps.redirectVerbAfterShoot);
                    }
                }
                else
                {
                    command_VerbTarget.icon = verb_with_cooldown != null ?
                        FloodingTexture(
                            verbProps.texture,
                            verb_with_cooldown.RemainingProgressBeforeFire()
                            ) : verbProps.texture;
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
                if (verbProps != null && verbProps.disable)
                {
                    command_VerbTarget.disabledReason = verbProps.disableReason + "\n" + command_VerbTarget.disabledReason;
                }
                else if (verb_with_cooldown != null && !verb_with_cooldown.CanFire())
                {
                    command_VerbTarget.disabledReason =
                        "Magnuassembly_AdditionalVerbPatch_CooldownDisableReason".Translate(
                            FormattingTickTime(verb_with_cooldown.RemainingTickBeforeFire() / 60.0D)
                        ) + "\n" + command_VerbTarget.disabledReason;
                }
            }
            else if (verbProps != null && verbProps.disable)
            {
                command_VerbTarget.Disable(verbProps.disableReason);
            }
            __result = command_VerbTarget;
            return false;
        }
        public static bool PrimaryVerbPrefix(CompEquippable __instance, ref Verb __result)
        {
            //Comp_VerbSaveable comp = ((CompEquippable)(__instance.directOwner)).parent.GetComp<Comp_VerbSaveable>();//VerbTracker
            Comp_VerbSaveable comp = __instance.parent.GetComp<Comp_VerbSaveable>();
            if (comp != null)
            {
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

        public static bool ShotCalculationTipStringPrefix(ref string __result, Thing target)
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
        public void CurrVerbReset()
        {
            CompEquippable comp = parent.GetComp<CompEquippable>();
            List<Verb> verbs = comp.AllVerbs;
            if (comp == null)
            {
                Log.ErrorOnce("[Magnuassembly]CurrVerbReset Error: comp is null.", 41422);
                return;
            }
            if (verbs == null)
            {
                Log.ErrorOnce("[Magnuassembly]CurrVerbReset Error: verbs is null, at comp: " + comp + ".", 41421);
                return;
            }
            if (verbs.Count == 0)
            {
                Log.ErrorOnce("[Magnuassembly]CurrVerbReset Error: verbs contains no verb, at comp: " + comp + ", verbs:"+ verbs + ".", 41423);
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

    interface IVerbPropertiesCustom
    {
    }

    [StaticConstructorOnStartup]
    public class VerbProperties_Custom : VerbProperties
    {
        public string desc;
        public string texPath;
        public string texPathSelected = "UI/Commands/Select";
        public string texPathCooldown;
        public bool disable = false;
        public string disableReason = null;
        public Texture2D texture;
        public Texture2D textureSelected;
        public Texture2D textureCooldown;
        public int maxMagazine;
        public int redirectVerbAfterShoot = -1;
        public int cooldownTick = -1;
        public bool ownByPawnEquipment = false;
        public ThingDef secondaryProjectile;

        public VerbProperties_Custom()
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
                if (!texPathCooldown.NullOrEmpty())
                {
                    textureCooldown = ContentFinder<Texture2D>.Get(texPathCooldown);
                }
                if (verbClass == typeof(Verb_Shoot_Cooldown) && cooldownTick <= 0)
                {
                    Log.Warning($"[Magnuassembly]Detected cooldownTick {cooldownTick}, which is not more than 0.");
                }
            });
        }
    }
    public class Verb_Shoot_Cooldown : Verb_Shoot
    {
        public int lastFireTick = -1;
        public int CooldownTick => ((VerbProperties_Custom)verbProps).cooldownTick;
        public bool CooldownTickValid => CooldownTick > 0;
        public int TickSinceLastFire(int currentTime = -1)
        {
            if (!CooldownTickValid || lastFireTick == -1)
                return -1;
            if (currentTime < 0)
            {
                currentTime = InGameTick;
            }
            return currentTime - lastFireTick;
        }
        public bool CanFire(int currentTime = -1)
        {
            if (!CooldownTickValid)
                return true;
            if (currentTime < 0)
            {
                currentTime = InGameTick;
            }
            return lastFireTick == -1 || currentTime - lastFireTick > CooldownTick;
        }
        public int RemainingTickBeforeFire(int currentTime = -1)
        {
            if (currentTime < 0)
            {
                currentTime = InGameTick;
            }
            if (lastFireTick == -1)
                return 0;
            return Math.Max(0, CooldownTick - (currentTime - lastFireTick));
        }
        public float RemainingProgressBeforeFire(int currentTime = -1) => CooldownTickValid ? RemainingTickBeforeFire(currentTime) / ((float)CooldownTick) : 0;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref lastFireTick, "lastFireTick", -1, true);
        }
        protected override bool TryCastShot()
        {
            if (!CooldownTickValid)
                return base.TryCastShot();
            bool result = false;
            int currentTime = InGameTick;
            if (CanFire(currentTime))
            {
                result = base.TryCastShot();
                if (result)
                {
                    lastFireTick = currentTime;
                    /*if (CasterIsPawn)
                    {
                        CasterPawn.jobs.EndCurrentJob(JobCondition.None);
                    }*/
                }
            }
            return result;
        }
        public override void WarmupComplete()
        {
            if (CanFire())
            {
                base.WarmupComplete();
            }
        }
        public override bool Available()
        {
            if (CanFire())
            {
                return base.Available();
            }
            return false;
        }
    }
    public class Verb_Shoot_OvertickShotgun : Verb_Shoot
    {
        protected override int ShotsPerBurst => 1;
        protected int ShotsPerBurstAccture => ((VerbProperties)verbProps).burstShotCount;

        protected override bool TryCastShot()
        {
            bool SuccessedOnce = false;
            for (int i = 0; i < ShotsPerBurstAccture; i++)
            {
                if (base.TryCastShot())
                    SuccessedOnce = true;
            }
            return SuccessedOnce;
        }
    }
    
    public class Verb_Shoot_Rainbow2 : Verb_Shoot
    {
        protected int lastFireTick = -1;
        public ThingDef secondaryProjectile => ((VerbProperties_Custom)verbProps).secondaryProjectile;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref lastFireTick, "lastFireTick", -1);
        }
        public override ThingDef Projectile 
        {
            get
            {
                if (lastFireTick >= 0 && lastFireTick + Math.Max(0, (ShotsPerBurst-1) * verbProps.ticksBetweenBurstShots) >= InGameTick)
                {
                    return secondaryProjectile;
                }
                else
                {
                    return base.Projectile; 
                }
            }
        }
        protected override bool TryCastShot()
        {
            bool result = base.TryCastShot();
            if (lastFireTick < 0 || lastFireTick + Math.Max(0, (ShotsPerBurst - 1) * verbProps.ticksBetweenBurstShots) < InGameTick)
            {
                lastFireTick = InGameTick;
            }
            return result;
        }
    }

    public class Verb_ShootConsumeable : Verb_Shoot
    {
        public int remainBullet = 0;
        //초기화할때 verbproperties랑remainBullet 동기화해야되

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref remainBullet, "remainBullet");
        }
        protected override bool TryCastShot()
        {
            if (base.TryCastShot())
            {
                if (burstShotsLeft <= 1)
                {
                    SelfConsume();
                }
                return true;
            }
            if (burstShotsLeft < verbProps.burstShotCount)
            {
                SelfConsume();
            }
            return false;
        }

        public override void Notify_EquipmentLost()
        {
            base.Notify_EquipmentLost();
            if (state == VerbState.Bursting && burstShotsLeft < verbProps.burstShotCount)
            {
                SelfConsume();
            }
        }

        private void SelfConsume()
        {
            if (remainBullet > 0)
            {
                remainBullet--;
            }
            else
            {
                EquipmentSource.GetComp<CompEquippable>().AllVerbs.Remove(this);
            }
        }
    }
}