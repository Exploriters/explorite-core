/********************
 * 三联电池使用的建筑物类。
 * --siiftun1857
 */
using RimWorld;
using System;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;
using Verse.Sound;
using static Explorite.ExploriteCore;

namespace Explorite
{
	///<summary>秘密三射弓物体。</summary>
	public interface ISecretTrishot
	{
		public bool CanEverHaveTrishot();
		public bool GetSecret();
		public bool SetSecret(bool boolen);
		public bool LeaveTrishot();
	}
	///<summary>继承自<seealso cref = "Building_Battery" />但移除其所有独有方法。</summary>
	public abstract class Building_AbstractBattery : Building_Battery
	{
		private static readonly MethodInfo baseBaseExposeData = typeof(Building).GetMethod(nameof(Building.ExposeData));
		public override void ExposeData()
		{
			baseBaseExposeData.InvokeForce(new object[] { this });
		}

		private static readonly MethodInfo baseBaseDraw = typeof(Building).GetMethod(nameof(Building.Draw));
		public override void Draw()
		{
			baseBaseDraw.InvokeForce(new object[] { this });
		}

		private static readonly MethodInfo baseBaseTick = typeof(Building).GetMethod(nameof(Building.Tick));
		public override void Tick()
		{
			baseBaseTick.InvokeForce(new object[] { this });
		}

		private static readonly MethodInfo baseBasePostApplyDamage = typeof(Building).GetMethod(nameof(Building.PostApplyDamage));
		public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
		{
			baseBaseTick.InvokeForce(new object[] { this, dinfo, totalDamageDealt });
		}
	}

	///<summary>三联电池使用的建筑物类，负责处理视觉效果和爆炸性。<br />实现了<seealso cref = "ISecretTrishot" />，可在开局被指定具有三射弓。</summary>
	[StaticConstructorOnStartup]
	public class Building_TriBattery : Building_AbstractBattery, ISecretTrishot
	{
		private int ticksToExplode;

		private Sustainer wickSustainer;

		private static readonly Vector2 BarSize = new Vector2(2.3f, 1.4f);// new Vector2(1.3f, 0.4f);

		private const float MinEnergyToExplode = 1500f;

		private const float EnergyToLoseWhenExplode = 1200f;

		private const float ExplodeChancePerDamage = 0.05f;

		private static readonly Material BatteryBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.9f, 0.85f, 0.2f), false);

		private static readonly Material BatteryBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f), false);

		public Building_TriBattery()
		{
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticksToExplode, "ticksToExplode", 0, false);
			Scribe_Values.Look(ref includingBrokenTrishot, "isSecretBattery", false);
		}

		public override void Draw()
		{
			base.Draw();
			CompPowerBattery comp = GetComp<CompPowerBattery>();
			GenDraw.FillableBarRequest r = default;
			r.center = DrawPos + (Vector3.up * 0.1f);
			r.size = BarSize;
			r.fillPercent = comp.StoredEnergy / comp.Props.storedEnergyMax;
			r.filledMat = BatteryBarFilledMat;
			r.unfilledMat = BatteryBarUnfilledMat;
			r.margin = 0.15f;
			Rot4 rotation = Rotation;
			rotation.Rotate(RotationDirection.Clockwise);
			r.rotation = rotation;
			GenDraw.DrawFillableBar(r);
			if (ticksToExplode > 0 && Spawned)
			{
				Map.overlayDrawer.DrawOverlay(this, OverlayTypes.BurningWick);
			}
		}

		public override void Tick()
		{
			base.Tick();
			//BackstoryCracker.TestforIncorrectChildhood();
			if (ticksToExplode > 0)
			{
				if (wickSustainer == null)
				{
					StartWickSustainer();
				}
				else
				{
					wickSustainer.Maintain();
				}
				ticksToExplode--;
				if (ticksToExplode == 0)
				{
					IntVec3 randomCell = this.OccupiedRect().RandomCell;
					float radius = Rand.Range(0.5f, 1f) * 3f * 1.7320508075688772935274463415059f;
					GenExplosion.DoExplosion(randomCell, Map, radius, DamageDefOf.Flame,
						instigator: null,
						damAmount: -1,
						armorPenetration: -1f,
						explosionSound: null,
						weapon: null,
						projectile: null,
						intendedTarget: null,
						postExplosionSpawnThingDef: null,
						postExplosionSpawnChance: 0f,
						postExplosionSpawnThingCount: 1,
						applyDamageToExplosionCellsNeighbors: false,
						preExplosionSpawnThingDef: null,
						preExplosionSpawnChance: 0f,
						preExplosionSpawnThingCount: 1,
						chanceToStartFire: 0f,
						damageFalloff: false
						);
					GetComp<CompPowerBattery>().DrawPower(EnergyToLoseWhenExplode);
				}
			}
		}

		public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
		{
			base.PostApplyDamage(dinfo, totalDamageDealt);
			if (!Destroyed && ticksToExplode == 0 && dinfo.Def == DamageDefOf.Flame && Rand.Value < ExplodeChancePerDamage && GetComp<CompPowerBattery>().StoredEnergy > MinEnergyToExplode)
			{
				ticksToExplode = Rand.Range(70, 150);
				StartWickSustainer();
			}
		}

		private void StartWickSustainer()
		{
			SoundInfo info = SoundInfo.InMap(this, MaintenanceType.PerTick);
			wickSustainer = SoundDefOf.HissSmall.TrySpawnSustainer(info);
		}

		// Note: this type is marked as 'beforefieldinit'.
		//static Building_TriBattery() { }


		private bool includingBrokenTrishot = false;
		public bool CanEverHaveTrishot()
		{
			return def.defName == "TriBatteryGolden";
		}
		public bool GetSecret()
		{
			if (!CanEverHaveTrishot())
				return false;
			return includingBrokenTrishot;
		}
		public bool SetSecret(bool boolen)
		{
			if (!CanEverHaveTrishot())
				return false;
			return includingBrokenTrishot = boolen;
		}
		public bool LeaveTrishot()
		{
			if (!CanEverHaveTrishot())
				return false;
			Thing final = this.FinalSpawnedParent();
			if (final != null && SpawnedOrAnyParentSpawned && includingBrokenTrishot)
			{
				Thing trishot = ThingMaker.MakeThing(TrishotThingDef);
				trishot.TrySetState("Trishot", "stage1");
				GameComponentCentaurStory.TryAdd(trishot);
				GenSpawn.Spawn(trishot, final.Position, final.Map);
				if (this.TryGetComp<CompForbiddable>()?.Forbidden is bool forbid)
				{
					trishot.TryGetComp<CompForbiddable>().Forbidden = forbid;
				}
				includingBrokenTrishot = false;
				return true;
			}
			return false;
		}
		public override void Destroy(DestroyMode mode)
		{
			LeaveTrishot();
			base.Destroy(mode);
		}
	}
}
