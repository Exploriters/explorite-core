/********************
 * 使用反射打破SoS2的内部类访问限制。
 * --siiftun1857
 */
using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace Explorite
{
    ///<summary>对SoS2的内部方法建立反射。</summary>
    [StaticConstructorOnStartup]
    internal static class SoS2Reflection
    {
        public static void GenerateShip(Def shipDef, Map map, TradeShip tradeShip, Faction fac, Lord lord, out Building core)
        {
            core = null;
            if (inaccessible || methodGenerateShip == null)
            {
                return;
            }
            //core = (Building)
            methodGenerateShip.Invoke(null, new object[] {
                shipDef, map, tradeShip, fac, lord, core
            });
        }


        internal static bool inaccessible = false;
        internal static MethodInfo methodGenerateShip;
        internal static Type sos2scm = null;
        static SoS2Reflection()
        {
            try
            {
                sos2scm = AccessTools.TypeByName("RimWorld.ShipCombatManager");
                if (sos2scm == null)
                {
                    throw new MemberAccessException();
                }
                methodGenerateShip =
                    sos2scm.
                    GetMethod("GenerateShip", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

                //ShipCombatManager.GenerateShip(null, null, null, null, null, out _);
                GenerateShip(null, null, null, null, null, out _);
                Log.Message("[Explorite]SoS2 accessible.");

            }
            catch (MemberAccessException)
            {
                Log.Warning("[Explorite]Warning, SoS2 inaccessible. Scenario part StartInSpaceCentaur will be disabled.");
                inaccessible = true;
                methodGenerateShip = null;
            }
            catch (Exception)
            {
            }
        }
    }

}
