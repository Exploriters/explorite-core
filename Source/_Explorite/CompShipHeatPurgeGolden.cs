/********************
 * 黄金排热器。
 * --siiftun1857
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Explorite
{
    [StaticConstructorOnStartup]
    public class CompShipHeatPurgeGolden : CompShipHeatPurge
    {
        private static readonly float HEAT_PURGE_RATIO = 20;
        private static readonly SoundDef HissSound = DefDatabase<SoundDef>.GetNamed("ShipPurgeHiss");
        private static readonly FieldInfo baseHissField = typeof(CompShipHeatPurge).GetField("hiss", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo baseBaseCompTick = ((Func<MethodInfo>)(() =>
        {
            MethodInfo baseBaseCompTickInfo = typeof(CompShipHeatSink).GetMethod(nameof(CompShipHeatSink.CompTick));
            DynamicMethod dynamicMethod = new DynamicMethod("", null, new Type[] { typeof(CompShipHeatSink) });
            ILGenerator iLGenerator = dynamicMethod.GetILGenerator();

            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Call, baseBaseCompTickInfo);
            iLGenerator.Emit(OpCodes.Ret);
            return dynamicMethod;
        }))();
        private bool Hiss
        {
            set
            {
                baseHissField.SetValue(this, value);
            }
            get
            {
                return (bool)baseHissField.GetValue(this);
            }
        }

        //public bool currentlyPurging = false;
        //bool hiss = false;

        public override void PostExposeData()
        {
            base.PostExposeData();
            //Scribe_Values.Look<bool>(ref currentlyPurging, "purging");
        }
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            /*
            List<Gizmo> giz = new List<Gizmo>();
            giz.AddRange(base.CompGetGizmosExtra());
            if (parent.Faction == Faction.OfPlayer)
            {
                Command_Toggle purge = new Command_Toggle
                {
                    toggleAction = delegate
                    {
                        currentlyPurging = !currentlyPurging;
                        notInsideShield = true;
                        if (currentlyPurging && CompShipCombatShield.allShieldsOnMap.ContainsKey(parent.Map))
                        {
                            foreach (CompShipCombatShield shield in CompShipCombatShield.allShieldsOnMap[parent.Map])
                            {
                                shield.parent.TryGetComp<CompFlickable>().SwitchIsOn = false;
                            }
                            hiss = false;
                        }
                    },
                    isActive = delegate { return currentlyPurging; },
                    defaultLabel = TranslatorFormattedStringExtensions.Translate("SoSPurgeHeat"),
                    defaultDesc = TranslatorFormattedStringExtensions.Translate("SoSPurgeHeatDesc"),
                    icon = ContentFinder<UnityEngine.Texture2D>.Get("UI/HeatPurge")
                };
                giz.Add(purge);
            }
            return giz;
            */

            if (Prefs.DevMode)
            {
                foreach (Gizmo gizmo in base.CompGetGizmosExtra())
                {
                    yield return gizmo;
                }
                yield return new Command_Action
                {
                    defaultLabel = "Debug: Set heat to 0",
                    action = delegate ()
                    {
                        heatStored = 0f;
                    }
                };
                yield return new Command_Action
                {
                    defaultLabel = "Debug: Set heat to max",
                    action = delegate ()
                    {
                        heatStored = Props.heatCapacity;
                    }
                };
                /*
                yield return new Command_Action
                {
                    defaultLabel = "Debug: Change heat",
                    action = delegate ()
                    {
                        Dialog_Slider window = new Dialog_Slider((x)=>$"Debug: set heat to {x}", 0, (int)Math.Floor(Props.heatCapacity), delegate (int value)
                        {
                            heatStored = value;
                        }, (int)Math.Floor(heatStored), 1f);
                        Find.WindowStack.Add(window);
                    }
                };
                */
            }
            yield break;
        }
        public override void CompTick()
        {
            baseBaseCompTick.Invoke(null, new object[] { this });
            //(this as CompShipHeatSink).CompTick();
            //base.CompTick();
            if (currentlyPurging)
            {
                float heat = (float)Math.Floor(Math.Min(myNet.StorageUsed / 10 / HEAT_PURGE_RATIO, Props.heatPurge / 10));
                if (notInsideShield && myNet != null && parent.TryGetComp<CompRefuelable>().Fuel > 0 && heat > 0f)
                {
                    parent.TryGetComp<CompRefuelable>().ConsumeFuel(heat);
                    myNet.AddHeat(heat * 10 * HEAT_PURGE_RATIO, remove: true);
                    FleckMaker.ThrowAirPuffUp(parent.DrawPos + new Vector3(0, 0, 1), parent.Map);
                    if (!Hiss)
                    {
                        HissSound.PlayOneShot(parent);
                        Hiss = true;
                    }
                }
                else
                {
                    currentlyPurging = false;
                    Hiss = false;
                }
            }
        }
    }
}
