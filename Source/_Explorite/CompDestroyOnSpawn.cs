/********************
 * 生成后直接被摧毁的物品的部件类。
 * --siiftun1857
 */
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
	///<summary>为<see cref = "CompDestroyOnSpawn" />接收参数。</summary>
	public class CompProperties_DestroyOnSpawn : CompProperties
	{
		public CompProperties_DestroyOnSpawn()
		{
			compClass = typeof(CompDestroyOnSpawn);
		}
	}
	///<summary>生成后直接被摧毁的物品。</summary>
	public class CompDestroyOnSpawn : ThingComp
	{
		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			parent.Destroy(DestroyMode.KillFinalize);
		}
		public override void CompTick()
		{
			base.CompTick();
			parent.Destroy(DestroyMode.KillFinalize);
		}
		public override void CompTickRare()
		{
			base.CompTickRare();
			parent.Destroy(DestroyMode.KillFinalize);
		}
		public override void CompTickLong()
		{
			base.CompTickLong();
			parent.Destroy(DestroyMode.KillFinalize);
		}
		//public override void PostDestroy(DestroyMode mode, Map previousMap){ }
	}

}
