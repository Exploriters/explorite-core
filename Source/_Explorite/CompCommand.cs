/********************
 * 远程效果设备。
 * --siiftun1857
 */
using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

namespace Explorite
{
    ///<summary>为<see cref = "Verb_RemoteActivator" />接收参数。</summary>
    public class CompProperties_Command : CompProperties
    {
        public bool displayGizmoWhileUndrafted;
        public bool displayGizmoWhileDrafted;
        public KeyBindingDef hotKey;

        public CompProperties_Command()
        {
            compClass = typeof(CompCommand);
        }
    }
    ///<summary>不需要装填的<see cref = "CompReloadable" />代替。</summary>
    public class CompCommand : ThingComp, IVerbOwner
    {
        private VerbTracker verbTracker;
        public CompProperties_Command Props => props as CompProperties_Command;
        public bool CanBeUsed => true;
        public Pawn Wearer
        {
            get
            {
                if (ParentHolder is Pawn_ApparelTracker pawn_ApparelTracker)
                {
                    return pawn_ApparelTracker.pawn;
                }
                return null;
            }
        }
        public List<VerbProperties> VerbProperties => parent.def.Verbs;
        public List<Tool> Tools => parent.def.tools;
        public ImplementOwnerTypeDef ImplementOwnerTypeDef => ImplementOwnerTypeDefOf.NativeVerb;
        public Thing ConstantCaster => Wearer;
        public string UniqueVerbOwnerID()
        {
            return "Reloadable_" + parent.ThingID;
        }
        public bool VerbsStillUsableBy(Pawn p)
        {
            return Wearer == p;
        }
        public VerbTracker VerbTracker
        {
            get
            {
                if (verbTracker == null)
                {
                    verbTracker = new VerbTracker(this);
                }
                return verbTracker;
            }
        }
        public List<Verb> AllVerbs => VerbTracker.AllVerbs;
        public override void PostPostMake()
        {
            base.PostPostMake();
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look(ref verbTracker, "verbTracker", new object[]
            {
                this
            });
        }
        public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetWornGizmosExtra())
            {
                yield return gizmo;
            }
            bool drafted = Wearer.Drafted;
            if ((drafted && !Props.displayGizmoWhileDrafted) || (!drafted && !Props.displayGizmoWhileUndrafted))
            {
                yield break;
            }
            ThingWithComps gear = parent;
            foreach (Verb verb in VerbTracker.AllVerbs)
            {
                if (verb.verbProps.hasStandardCommand)
                {
                    yield return CreateVerbTargetCommand(gear, verb);
                }
            }
            yield break;
        }
        private Command_CompCommand CreateVerbTargetCommand(Thing gear, Verb verb)
        {
            Command_CompCommand command_CompCommand = new Command_CompCommand(this)
            {
                defaultDesc = gear.def.description,
                hotKey = Props.hotKey,
                defaultLabel = verb.verbProps.label,
                verb = verb
            };
            if (verb.verbProps.defaultProjectile != null && verb.verbProps.commandIcon == null)
            {
                command_CompCommand.icon = verb.verbProps.defaultProjectile.uiIcon;
                command_CompCommand.iconAngle = verb.verbProps.defaultProjectile.uiIconAngle;
                command_CompCommand.iconOffset = verb.verbProps.defaultProjectile.uiIconOffset;
                command_CompCommand.overrideColor = new Color?(verb.verbProps.defaultProjectile.graphicData.color);
            }
            else
            {
                command_CompCommand.icon = (verb.UIIcon != BaseContent.BadTex) ? verb.UIIcon : gear.def.uiIcon;
                command_CompCommand.iconAngle = gear.def.uiIconAngle;
                command_CompCommand.iconOffset = gear.def.uiIconOffset;
                command_CompCommand.defaultIconColor = gear.DrawColor;
            }
            if (!Wearer.IsColonistPlayerControlled)
            {
                command_CompCommand.Disable(null);
            }
            else if (verb.verbProps.violent && Wearer.WorkTagIsDisabled(WorkTags.Violent))
            {
                command_CompCommand.Disable("IsIncapableOfViolenceLower".Translate(Wearer.LabelShort, Wearer).CapitalizeFirst() + ".");
            }
            else if (!CanBeUsed)
            {
                //command_CompCommand.Disable(DisabledReason(MinAmmoNeeded(false), MaxAmmoNeeded(false)));
            }
            return command_CompCommand;
        }
    }

    ///<summary>不需要装填的<see cref = "Command_Reloadable" />代替。</summary>
    public class Command_CompCommand : Command_VerbTarget
    {
        private readonly ThingComp comp;
        public Color? overrideColor;
        //public override string TopRightLabel => "∞";
        public Command_CompCommand(ThingComp comp)
        {
            this.comp = comp;
        }
        public override Color IconDrawColor
        {
            get
            {
                Color? color = overrideColor;
                if (color == null)
                {
                    return base.IconDrawColor;
                }
                return color.GetValueOrDefault();
            }
        }
        public override void GizmoUpdateOnMouseover()
        {
            verb.DrawHighlight(LocalTargetInfo.Invalid);
        }
        public override bool GroupsWith(Gizmo other)
        {
            if (!base.GroupsWith(other))
            {
                return false;
            }
            return other is Command_CompCommand command_CompCommand && comp.parent.def == command_CompCommand.comp.parent.def;
        }
    }

    ///<summary>为<see cref = "CompReloadableNotReloadable" />接收参数。</summary>
    public class CompProperties_ReloadableNotReloadable : CompProperties_Reloadable
    {
        public CompProperties_ReloadableNotReloadable()
        {
            maxCharges = int.MaxValue;
            compClass = typeof(CompReloadableNotReloadable);
        }
    }
    ///<summary>不需要装填的<see cref = "CompReloadable" />代替。</summary>
    public class CompReloadableNotReloadable : CompReloadable
    {
        public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetWornGizmosExtra())
            {
                if (gizmo is Command_Reloadable command_Reloadable)
                {
                    yield return new Command_CompCommand(this)
                    {
                        defaultDesc = command_Reloadable.defaultDesc,
                        hotKey = command_Reloadable.hotKey,
                        defaultLabel = command_Reloadable.defaultLabel,
                        verb = command_Reloadable.verb,
                        icon = command_Reloadable.icon,
                        iconAngle = command_Reloadable.iconAngle,
                        iconOffset = command_Reloadable.iconOffset,
                        overrideColor = command_Reloadable.overrideColor,
                        disabled = command_Reloadable.disabled,
                        disabledReason = command_Reloadable.disabledReason,
                    };
                }
                else
                {
                    yield return gizmo;
                }
            }
            yield break;
        }
        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            yield break;
        }
        public override string CompInspectStringExtra()
        {
            return null;
        }
    }
}
