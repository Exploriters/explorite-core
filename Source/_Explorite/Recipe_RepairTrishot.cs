/********************
 * 在制作完成时转移全局追踪器。
 * --siiftun1857
 */
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
	public class Recipe_RepairTrishot1Stage : RecipeWorker
	{
		public override void ConsumeIngredient(Thing ingredient, RecipeDef recipe, Map map)
		{
			if (ingredient.def == TrishotThingDef)
			{
				if (ingredient.TryGetComp<CompItemStage>() is CompItemStage comp)
				{
					comp.SetStage("stage2");
				}
			}
			else
			{
				ingredient.Destroy(DestroyMode.Vanish);
			}
		}
	}
	public class Recipe_RepairTrishot2Stage : RecipeWorker
	{
		public override void ConsumeIngredient(Thing ingredient, RecipeDef recipe, Map map)
		{
			if (ingredient.def == TrishotThingDef)
			{
				if (ingredient.TryGetComp<CompItemStage>() is CompItemStage comp)
				{
					comp.SetStage("stage3");
				}
			}
			else
			{
				ingredient.Destroy(DestroyMode.Vanish);
			}
		}
	}
}
