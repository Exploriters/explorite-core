/********************
 * 该文件包含多个剧本部件。
 * --siiftun1857
 */
using RimWorld;
using Verse;

namespace Explorite
{
    public class IngredientValueGetter_MarketValue : IngredientValueGetter
    {
        public override float ValuePerUnitOf(ThingDef t)
        {
            return t.BaseMarketValue;
        }

        public override string BillRequirementsDescription(RecipeDef r, IngredientCount ing)
        {
            return "BillRequiresMarketValue".Translate(ing.GetBaseCount(), ing.filter.Summary);
        }
    }
}
