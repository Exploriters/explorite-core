using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;
using SaveOurShip2;
using Verse.AI.Group;

namespace CentaurTheMagnuassembly
{
    [StaticConstructorOnStartup]
    public static class SoS2Reflection
    {
        public static void GenerateShip(EnemyShipDef shipDef, Map map, TradeShip tradeShip, Faction fac, Lord lord, out Building core)
        {
            if (inaccessible || methodGenerateShip == null)
            {
                core = null;
                return;
            }
            core = null;
            //core = (Building)
            methodGenerateShip.Invoke(null, new object[] {
                shipDef, map, tradeShip, fac, lord, null
            });
        }


        internal static bool inaccessible = false;
        internal static MethodInfo methodGenerateShip;
        static SoS2Reflection()
        {
            try
            {
                methodGenerateShip = 
                    typeof(ShipCombatManager).
                    GetMethod("GenerateShip", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

                //ShipCombatManager.GenerateShip(null, null, null, null, null, out _);
                GenerateShip(null, null, null, null, null, out _);
                Log.Message("[Magnuassembly]SoS2 accessible.");

            }
            catch (MemberAccessException)
            {
                Log.Warning("[Magnuassembly]Warning, SoS2 inaccessible. Scenario part StartInSpaceCentaur will be disabled.");
                inaccessible = true;
                methodGenerateShip = null;
            }
            catch (Exception)
            {
            }
        }
    }

}
