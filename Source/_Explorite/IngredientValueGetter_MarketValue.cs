/********************
 * 该文件包含配方成分获取器。
 * --siiftun1857
 */
using RimWorld;
using Verse;
using Explorite;

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
    /*
    public class IngredientValueGetter_Stage1trishotOnly : IngredientValueGetter_Volume
    {
        public override float ValuePerUnitOf(ThingDef t)
        {
            return !t.TryGetState("Trishot", out string stage) || stage == "stage1";
        }
    }
    */
}
