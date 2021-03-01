/********************
 * 半人马的子系统控制器。
 */
using System;
using Verse;
using UnityEngine;
using System.Collections.Generic;
using static Explorite.ExploriteCore;
using System.Linq;

namespace Explorite
{
    ///<summary>为<see cref = "CompSelfHealOvertime" />接收参数。</summary>
    public class CompProperties_PawnSubsystemManager : CompProperties
    {
        public CompProperties_PawnSubsystemManager() : base(typeof(CompPawnSubsystemManager))
        {
        }
        public CompProperties_PawnSubsystemManager(Type cc) : base(cc)
        {
        }
    }
    ///<summary>半人马的子系统控制器。</summary>
    [StaticConstructorOnStartup]
    public class CompPawnSubsystemManager : ThingComp
    {
        public bool subsystem1_ManualDisabled = false;
        public bool subsystem2_ManualDisabled = false;
        public bool subsystem3_ManualDisabled = false;
        public Pawn Pawn => parent as Pawn;
        public bool Visible => Pawn.SubsystemVisible();

        //private static readonly Texture2D CommandTex = ContentFinder<Texture2D>.Get("UI/Commands/PlaceBlueprints");
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield break;
            /*if (!Visible)
            {
                yield break;
            }

            Command_Toggle command1_Toggle = new Command_Toggle
            {
                defaultLabel = "CommandWarnInBuildingRadius".Translate(),
                defaultDesc = "CommandWarnInBuildingRadiusDesc".Translate(),
                icon = CommandTex,
                isActive = () => subsystem1_ManualDisabled
            };
            command1_Toggle.toggleAction = Delegate.Combine(command1_Toggle.toggleAction, (Action)delegate
            {
                subsystem1_ManualDisabled = !subsystem1_ManualDisabled;
            }) as Action;
            yield return command1_Toggle;

            Command_Toggle command2_Toggle = new Command_Toggle
            {
                defaultLabel = "CommandWarnInBuildingRadius".Translate(),
                defaultDesc = "CommandWarnInBuildingRadiusDesc".Translate(),
                icon = CommandTex,
                isActive = () => subsystem2_ManualDisabled
            };
            command2_Toggle.toggleAction = Delegate.Combine(command2_Toggle.toggleAction, (Action)delegate
            {
                subsystem2_ManualDisabled = !subsystem2_ManualDisabled;
            }) as Action;
            yield return command2_Toggle;

            Command_Toggle command3_Toggle = new Command_Toggle
            {
                defaultLabel = "CommandWarnInBuildingRadius".Translate(),
                defaultDesc = "CommandWarnInBuildingRadiusDesc".Translate(),
                icon = CommandTex,
                isActive = () => subsystem3_ManualDisabled
            };
            command3_Toggle.toggleAction = Delegate.Combine(command3_Toggle.toggleAction, (Action)delegate
            {
                subsystem3_ManualDisabled = !subsystem3_ManualDisabled;
            }) as Action;
            yield return command3_Toggle;

            */
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref subsystem1_ManualDisabled, "subsystem1_ManualDisabled", defaultValue: false, forceSave: true);
            Scribe_Values.Look(ref subsystem2_ManualDisabled, "subsystem2_ManualDisabled", defaultValue: false, forceSave: true);
            Scribe_Values.Look(ref subsystem3_ManualDisabled, "subsystem3_ManualDisabled", defaultValue: false, forceSave: true);
        }
    }
}
