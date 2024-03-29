/********************
 * 该文件包含一个不能打穿屋顶的穿墙弹射物。
 * 
 * 已弃用。
 * --siiftun1857
 */
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Explorite
{
	/*public class Projectile_Explosive_RoofBypass : Projectile_Explosive
	{
		private void ImpactSomething()
		{
			if (def.projectile.flyOverhead)
			{
				RoofDef roofDef = base.Map.roofGrid.RoofAt(base.Position);
				if (roofDef != null)
				{
					Log.Message("AAA!");
				}
			}
		}
		private void ImpactSomething2()
		{
			if (def.projectile.flyOverhead && ExploritePatches.IsProjFliesOverheadOverrider(this))
			{
				RoofDef roofDef = base.Map.roofGrid.RoofAt(base.Position);
				if (roofDef != null)
				{
					Log.Message("AAA2!");
				}
			}
		}
	}*/
	/*public abstract class Projectile_Explosive_RoofBypass : ThingWithComps
	{

		protected Vector3 origin;

		protected Vector3 destination;

		protected LocalTargetInfo usedTarget;

		protected LocalTargetInfo intendedTarget;

		protected ThingDef equipmentDef;

		protected Thing launcher;

		protected ThingDef targetCoverDef;

		private ProjectileHitFlags desiredHitFlags = ProjectileHitFlags.All;

		protected float weaponDamageMultiplier = 1f;

		protected bool landed;

		protected int ticksToImpact;
		
		private Sustainer ambientSustainer;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("", "IDE0044")]
		private static List<IntVec3> checkedCells = new List<IntVec3>();

		private static readonly List<Thing> cellThingsFiltered = new List<Thing>();

		private int ticksToDetonation;

		protected Projectile_Explosive_RoofBypass()
		{
		}

		public ProjectileHitFlags HitFlags
		{
			get
			{
				if (this.def.projectile.alwaysFreeIntercept)
				{
					return ProjectileHitFlags.All;
				}
				if (this.def.projectile.flyOverhead)
				{
					return ProjectileHitFlags.None;
				}
				return this.desiredHitFlags;
			}
			set
			{
				this.desiredHitFlags = value;
			}
		}

		protected int StartingTicksToImpact
		{
			get
			{
				int num = Mathf.RoundToInt((this.origin - this.destination).magnitude / (this.def.projectile.speed / 100f));
				if (num < 1)
				{
					num = 1;
				}
				return num;
			}
		}

		protected IntVec3 DestinationCell
		{
			get
			{
				return new IntVec3(this.destination);
			}
		}

		public virtual Vector3 ExactPosition
		{
			get
			{
				Vector3 b = (this.destination - this.origin) * (1f - (float)this.ticksToImpact / (float)this.StartingTicksToImpact);
				return this.origin + b + Vector3.up * this.def.Altitude;
			}
		}

		public virtual Quaternion ExactRotation
		{
			get
			{
				return Quaternion.LookRotation(this.destination - this.origin);
			}
		}

		public override Vector3 DrawPos
		{
			get
			{
				return this.ExactPosition;
			}
		}

		public int DamageAmount
		{
			get
			{
				return this.def.projectile.GetDamageAmount(this.weaponDamageMultiplier, null);
			}
		}

		public float ArmorPenetration
		{
			get
			{
				return this.def.projectile.GetArmorPenetration(this.weaponDamageMultiplier, null);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<Vector3>(ref this.origin, "origin", default, false);
			Scribe_Values.Look<Vector3>(ref this.destination, "destination", default, false);
			Scribe_Values.Look<int>(ref this.ticksToImpact, "ticksToImpact", 0, false);
			Scribe_TargetInfo.Look(ref this.usedTarget, "usedTarget");
			Scribe_TargetInfo.Look(ref this.intendedTarget, "intendedTarget");
			Scribe_References.Look<Thing>(ref this.launcher, "launcher", false);
			Scribe_Defs.Look<ThingDef>(ref this.equipmentDef, "equipmentDef");
			Scribe_Defs.Look<ThingDef>(ref this.targetCoverDef, "targetCoverDef");
			Scribe_Values.Look<ProjectileHitFlags>(ref this.desiredHitFlags, "desiredHitFlags", ProjectileHitFlags.All, false);
			Scribe_Values.Look<float>(ref this.weaponDamageMultiplier, "weaponDamageMultiplier", 1f, false);
			Scribe_Values.Look<bool>(ref this.landed, "landed", false, false);

			Scribe_Values.Look<int>(ref this.ticksToDetonation, "ticksToDetonation", 0, false);
		}

		public void Launch(Thing launcher, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, Thing equipment = null)
		{
			this.Launch(launcher, Position.ToVector3Shifted(), usedTarget, intendedTarget, hitFlags, equipment, null);
		}

		public void Launch(Thing launcher, Vector3 origin, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, Thing equipment = null, ThingDef targetCoverDef = null)
		{
			this.launcher = launcher;
			this.origin = origin;
			this.usedTarget = usedTarget;
			this.intendedTarget = intendedTarget;
			this.targetCoverDef = targetCoverDef;
			this.HitFlags = hitFlags;
			if (equipment != null)
			{
				this.equipmentDef = equipment.def;
				this.weaponDamageMultiplier = equipment.GetStatValue(StatDefOf.RangedWeapon_DamageMultiplier, true);
			}
			else
			{
				this.equipmentDef = null;
				this.weaponDamageMultiplier = 1f;
			}
			this.destination = usedTarget.Cell.ToVector3Shifted() + Gen.RandomHorizontalVector(0.3f);
			this.ticksToImpact = this.StartingTicksToImpact;
			if (!this.def.projectile.soundAmbient.NullOrUndefined())
			{
				SoundInfo info = SoundInfo.InMap(this, MaintenanceType.PerTick);
				this.ambientSustainer = this.def.projectile.soundAmbient.TrySpawnSustainer(info);
			}
		}

		public void BASETick()
		{
			if (this.landed)
			{
				return;
			}
			Vector3 exactPosition = this.ExactPosition;
			this.ticksToImpact--;
			if (!this.ExactPosition.InBounds(Map))
			{
				this.ticksToImpact++;
				Position = this.ExactPosition.ToIntVec3();
				this.Destroy(DestroyMode.Vanish);
				return;
			}
			Vector3 exactPosition2 = this.ExactPosition;
			if (this.CheckForFreeInterceptBetween(exactPosition, exactPosition2))
			{
				return;
			}
			Position = this.ExactPosition.ToIntVec3();
			if (this.ticksToImpact == 60 && Find.TickManager.CurTimeSpeed == TimeSpeed.Normal && this.def.projectile.soundImpactAnticipate != null)
			{
				this.def.projectile.soundImpactAnticipate.PlayOneShot(this);
			}
			if (this.ticksToImpact <= 0)
			{
				if (this.DestinationCell.InBounds(Map))
				{
					Position = this.DestinationCell;
				}
				this.ImpactSomething();
				return;
			}
			if (this.ambientSustainer != null)
			{
				this.ambientSustainer.Maintain();
			}
		}
		
		private bool CheckForFreeInterceptBetween(Vector3 lastExactPos, Vector3 newExactPos)
		{
			IntVec3 intVec = lastExactPos.ToIntVec3();
			IntVec3 intVec2 = newExactPos.ToIntVec3();
			if (intVec2 == intVec)
			{
				return false;
			}
			if (!intVec.InBounds(Map) || !intVec2.InBounds(Map))
			{
				return false;
			}
			if (intVec2.AdjacentToCardinal(intVec))
			{
				return this.CheckForFreeIntercept(intVec2);
			}
			if (VerbUtility.InterceptChanceFactorFromDistance(this.origin, intVec2) <= 0f)
			{
				return false;
			}
			Vector3 vector = lastExactPos;
			Vector3 v = newExactPos - lastExactPos;
			Vector3 b = v.normalized * 0.2f;
			int num = (int)(v.MagnitudeHorizontal() / 0.2f);
			checkedCells.Clear();
			int num2 = 0;
			for (; ; )
			{
				vector += b;
				IntVec3 intVec3 = vector.ToIntVec3();
				if (!checkedCells.Contains(intVec3))
				{
					if (this.CheckForFreeIntercept(intVec3))
					{
						break;
					}
					checkedCells.Add(intVec3);
				}
				num2++;
				if (num2 > num)
				{
					return false;
				}
				if (intVec3 == intVec2)
				{
					return false;
				}
			}
			return true;
		}

		private bool CheckForFreeIntercept(IntVec3 c)
		{
			if (this.destination.ToIntVec3() == c)
			{
				return false;
			}
			float num = VerbUtility.InterceptChanceFactorFromDistance(this.origin, c);
			if (num <= 0f)
			{
				return false;
			}
			bool flag = false;
			List<Thing> thingList = c.GetThingList(Map);
			for (int i = 0; i < thingList.Count; i++)
			{
				Thing thing = thingList[i];
				if (this.CanHit(thing))
				{
					bool flag2 = false;
					if (thing.def.Fillage == FillCategory.Full)
					{
						if (!(thing is Building_Door building_Door) || !building_Door.Open)
						{
							this.ThrowDebugText("int-wall", c);
							this.Impact(thing);
							return true;
						}
						flag2 = true;
					}
					float num2 = 0f;
					if (thing is Pawn pawn)
					{
						num2 = 0.4f * Mathf.Clamp(pawn.BodySize, 0.1f, 2f);
						if (pawn.GetPosture() != PawnPosture.Standing)
						{
							num2 *= 0.1f;
						}
						if (this.launcher != null && pawn.Faction != null && this.launcher.Faction != null && !pawn.Faction.HostileTo(this.launcher.Faction))
						{
							num2 *= 0.4f;
						}
					}
					else if (thing.def.fillPercent > 0.2f)
					{
						if (flag2)
						{
							num2 = 0.05f;
						}
						else if (this.DestinationCell.AdjacentTo8Way(c))
						{
							num2 = thing.def.fillPercent * 1f;
						}
						else
						{
							num2 = thing.def.fillPercent * 0.15f;
						}
					}
					num2 *= num;
					if (num2 > 1E-05f)
					{
						if (Rand.Chance(num2))
						{
							this.ThrowDebugText("int-" + num2.ToStringPercent(), c);
							this.Impact(thing);
							return true;
						}
						flag = true;
						this.ThrowDebugText(num2.ToStringPercent(), c);
					}
				}
			}
			if (!flag)
			{
				this.ThrowDebugText("o", c);
			}
			return false;
		}

		private void ThrowDebugText(string text, IntVec3 c)
		{
			if (DebugViewSettings.drawShooting)
			{
				MoteMaker.ThrowText(c.ToVector3Shifted(), Map, text, -1f);
			}
		}

		public override void Draw()
		{
			Mesh mesh = MeshPool.GridPlane(this.def.graphicData.drawSize);
			Graphics.DrawMesh(mesh, this.DrawPos, this.ExactRotation, this.def.DrawMatSingle, 0);
			Comps_PostDraw();
		}

		protected bool CanHit(Thing thing)
		{
			if (!thing.Spawned)
			{
				return false;
			}
			if (thing == this.launcher)
			{
				return false;
			}
			bool flag = false;
			CellRect.CellRectIterator iterator = thing.OccupiedRect().GetIterator();
			while (!iterator.Done())
			{
				List<Thing> thingList = iterator.Current.GetThingList(Map);
				bool flag2 = false;
				for (int i = 0; i < thingList.Count; i++)
				{
					if (thingList[i] != thing && thingList[i].def.Fillage == FillCategory.Full && thingList[i].def.Altitude >= thing.def.Altitude)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					flag = true;
					break;
				}
				iterator.MoveNext();
			}
			if (!flag)
			{
				return false;
			}
			ProjectileHitFlags hitFlags = this.HitFlags;
			if (thing == this.intendedTarget && (hitFlags & ProjectileHitFlags.IntendedTarget) != ProjectileHitFlags.None)
			{
				return true;
			}
			if (thing != this.intendedTarget)
			{
				if (thing is Pawn)
				{
					if ((hitFlags & ProjectileHitFlags.NonTargetPawns) != ProjectileHitFlags.None)
					{
						return true;
					}
				}
				else if ((hitFlags & ProjectileHitFlags.NonTargetWorld) != ProjectileHitFlags.None)
				{
					return true;
				}
			}
			return thing == this.intendedTarget && thing.def.Fillage == FillCategory.Full;
		}

		private void ImpactSomething()
		{
			if (this.def.projectile.flyOverhead)
			{
				RoofDef roofDef = Map.roofGrid.RoofAt(Position);
				if (roofDef != null)
				{
					if (roofDef.isThickRoof)
					{
						/*this.ThrowDebugText("hit-thick-roof", Position);
						this.def.projectile.soundHitThickRoof.PlayOneShot(new TargetInfo(Position, Map, false));
						this.Destroy(DestroyMode.Vanish);
						return;* /
					}
					else if (Position.GetEdifice(Map) == null || Position.GetEdifice(Map).def.Fillage != FillCategory.Full)
					{
						RoofCollapserImmediate.DropRoofInCells(Position, Map, null);
					}
				}
			}
			if (!this.usedTarget.HasThing || !this.CanHit(this.usedTarget.Thing))
			{
				cellThingsFiltered.Clear();
				List<Thing> thingList = Position.GetThingList(Map);
				for (int i = 0; i < thingList.Count; i++)
				{
					Thing thing = thingList[i];
					if ((thing.def.category == ThingCategory.Building || thing.def.category == ThingCategory.Pawn || thing.def.category == ThingCategory.Item || thing.def.category == ThingCategory.Plant) && this.CanHit(thing))
					{
						cellThingsFiltered.Add(thing);
					}
				}
				cellThingsFiltered.Shuffle<Thing>();
				for (int j = 0; j < cellThingsFiltered.Count; j++)
				{
					Thing thing2 = cellThingsFiltered[j];
					float num;
					if (thing2 is Pawn pawn)
					{
						num = 0.5f * Mathf.Clamp(pawn.BodySize, 0.1f, 2f);
						if (pawn.GetPosture() != PawnPosture.Standing && (this.origin - this.destination).MagnitudeHorizontalSquared() >= 20.25f)
						{
							num *= 0.2f;
						}
						if (this.launcher != null && pawn.Faction != null && this.launcher.Faction != null && !pawn.Faction.HostileTo(this.launcher.Faction))
						{
							num *= VerbUtility.InterceptChanceFactorFromDistance(this.origin, Position);
						}
					}
					else
					{
						num = 1.5f * thing2.def.fillPercent;
					}
					if (Rand.Chance(num))
					{
						this.ThrowDebugText("hit-" + num.ToStringPercent(), Position);
						this.Impact(cellThingsFiltered.RandomElement<Thing>());
						return;
					}
					this.ThrowDebugText("miss-" + num.ToStringPercent(), Position);
				}
				this.Impact(null);
				return;
			}
			if (this.usedTarget.Thing is Pawn pawn2 && pawn2.GetPosture() != PawnPosture.Standing && (this.origin - this.destination).MagnitudeHorizontalSquared() >= 20.25f && !Rand.Chance(0.2f))
			{
				this.ThrowDebugText("miss-laying", Position);
				this.Impact(null);
				return;
			}
			this.Impact(this.usedTarget.Thing);
		}

		/*protected virtual void Impact(Thing hitThing)
		{
			GenClamor.DoClamor(this, 2.1f, ClamorDefOf.Impact);
			this.Destroy(DestroyMode.Vanish);
		}* /


		public override void Tick()
		{
			if (AllComps != null)
			{
				int i = 0;
				int count = AllComps.Count;
				while (i < count)
				{
					AllComps[i].CompTick();
					i++;
				}
			}
			BASETick();
			if (this.ticksToDetonation > 0)
			{
				this.ticksToDetonation--;
				if (this.ticksToDetonation <= 0)
				{
					this.Explode();
				}
			}
		}

		protected virtual void Impact(Thing hitThing)
		{
			if (this.def.projectile.explosionDelay == 0)
			{
				this.Explode();
				return;
			}
			this.landed = true;
			this.ticksToDetonation = this.def.projectile.explosionDelay;
			GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, this.def.projectile.damageDef, this.launcher.Faction);
		}

		protected virtual void Explode()
		{
			Map map = base.Map;
			this.Destroy(DestroyMode.Vanish);
			if (this.def.projectile.explosionEffect != null)
			{
				Effecter effecter = this.def.projectile.explosionEffect.Spawn();
				effecter.Trigger(new TargetInfo(base.Position, map, false), new TargetInfo(base.Position, map, false));
				effecter.Cleanup();
			}
			IntVec3 position = base.Position;
			Map map2 = map;
			float explosionRadius = this.def.projectile.explosionRadius;
			DamageDef damageDef = this.def.projectile.damageDef;
			Thing launcher = this.launcher;
			int damageAmount = this.DamageAmount;
			float armorPenetration = this.ArmorPenetration;
			SoundDef soundExplode = this.def.projectile.soundExplode;
			ThingDef equipmentDef = this.equipmentDef;
			ThingDef def = this.def;
			Thing thing = this.intendedTarget.Thing;
			ThingDef postExplosionSpawnThingDef = this.def.projectile.postExplosionSpawnThingDef;
			float postExplosionSpawnChance = this.def.projectile.postExplosionSpawnChance;
			int postExplosionSpawnThingCount = this.def.projectile.postExplosionSpawnThingCount;
			ThingDef preExplosionSpawnThingDef = this.def.projectile.preExplosionSpawnThingDef;
			GenExplosion.DoExplosion(position, map2, explosionRadius, damageDef, launcher, damageAmount, armorPenetration, soundExplode, equipmentDef, def, thing, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, this.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, this.def.projectile.preExplosionSpawnChance, this.def.projectile.preExplosionSpawnThingCount, this.def.projectile.explosionChanceToStartFire, this.def.projectile.explosionDamageFalloff);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static Projectile_Explosive_RoofBypass()
		{
		}
	}
	*/
	/*
	[System.Diagnostics.CodeAnalysis.SuppressMessage(null, "IDE0044")]
	public class Projectile_Explosive_RoofBypass : Projectile_Explosive
	{
		private Sustainer ambientSustainer = null;

		private static List<IntVec3> checkedCells = new List<IntVec3>();

		private static List<Thing> cellThingsFiltered = new List<Thing>();

		private int ticksToDetonation;

		protected Projectile_Explosive_RoofBypass()
		{
		}

		public override void ExposeData()
		{
			base.ExposeData();

			Scribe_Values.Look(ref ticksToDetonation, "ticksToDetonation_MXRemaster", 0, false);
		}
		public void BASETick()
		{
			if (landed)
			{
				return;
			}
			Vector3 exactPosition = ExactPosition;
			ticksToImpact--;
			if (!ExactPosition.InBounds(Map))
			{
				ticksToImpact++;
				Position = ExactPosition.ToIntVec3();
				Destroy(DestroyMode.Vanish);
				return;
			}
			Vector3 exactPosition2 = ExactPosition;
			if (CheckForFreeInterceptBetween(exactPosition, exactPosition2))
			{
				return;
			}
			Position = ExactPosition.ToIntVec3();
			if (ticksToImpact == 60 && Find.TickManager.CurTimeSpeed == TimeSpeed.Normal && def.projectile.soundImpactAnticipate != null)
			{
				def.projectile.soundImpactAnticipate.PlayOneShot(this);
			}
			if (ticksToImpact <= 0)
			{
				if (DestinationCell.InBounds(Map))
				{
					Position = DestinationCell;
				}
				ImpactSomething();
				return;
			}
			if (ambientSustainer != null)
			{
				ambientSustainer.Maintain();
			}
		}

		private bool CheckForFreeInterceptBetween(Vector3 lastExactPos, Vector3 newExactPos)
		{
			IntVec3 intVec = lastExactPos.ToIntVec3();
			IntVec3 intVec2 = newExactPos.ToIntVec3();
			if (intVec2 == intVec)
			{
				return false;
			}
			if (!intVec.InBounds(Map) || !intVec2.InBounds(Map))
			{
				return false;
			}
			if (intVec2.AdjacentToCardinal(intVec))
			{
				return CheckForFreeIntercept(intVec2);
			}
			if (VerbUtility.InterceptChanceFactorFromDistance(origin, intVec2) <= 0f)
			{
				return false;
			}
			Vector3 vector = lastExactPos;
			Vector3 v = newExactPos - lastExactPos;
			Vector3 b = v.normalized * 0.2f;
			int num = (int)(v.MagnitudeHorizontal() / 0.2f);
			checkedCells.Clear();
			int num2 = 0;
			for (; ; )
			{
				vector += b;
				IntVec3 intVec3 = vector.ToIntVec3();
				if (!checkedCells.Contains(intVec3))
				{
					if (CheckForFreeIntercept(intVec3))
					{
						break;
					}
					checkedCells.Add(intVec3);
				}
				num2++;
				if (num2 > num)
				{
					return false;
				}
				if (intVec3 == intVec2)
				{
					return false;
				}
			}
			return true;
		}

		private bool CheckForFreeIntercept(IntVec3 c)
		{
			if (destination.ToIntVec3() == c)
			{
				return false;
			}
			float num = VerbUtility.InterceptChanceFactorFromDistance(origin, c);
			if (num <= 0f)
			{
				return false;
			}
			bool flag = false;
			List<Thing> thingList = c.GetThingList(Map);
			for (int i = 0; i < thingList.Count; i++)
			{
				Thing thing = thingList[i];
				if (CanHit(thing))
				{
					bool flag2 = false;
					if (thing.def.Fillage == FillCategory.Full)
					{
						if (!(thing is Building_Door building_Door) || !building_Door.Open)
						{
							ThrowDebugText("int-wall", c);
							Impact(thing);
							return true;
						}
						flag2 = true;
					}
					float num2 = 0f;
					if (thing is Pawn pawn)
					{
						num2 = 0.4f * Mathf.Clamp(pawn.BodySize, 0.1f, 2f);
						if (pawn.GetPosture() != PawnPosture.Standing)
						{
							num2 *= 0.1f;
						}
						if (launcher != null && pawn.Faction != null && launcher.Faction != null && !pawn.Faction.HostileTo(launcher.Faction))
						{
							num2 *= 0.4f;
						}
					}
					else if (thing.def.fillPercent > 0.2f)
					{
						if (flag2)
						{
							num2 = 0.05f;
						}
						else if (DestinationCell.AdjacentTo8Way(c))
						{
							num2 = thing.def.fillPercent * 1f;
						}
						else
						{
							num2 = thing.def.fillPercent * 0.15f;
						}
					}
					num2 *= num;
					if (num2 > 1E-05f)
					{
						if (Rand.Chance(num2))
						{
							ThrowDebugText("int-" + num2.ToStringPercent(), c);
							Impact(thing);
							return true;
						}
						flag = true;
						ThrowDebugText(num2.ToStringPercent(), c);
					}
				}
			}
			if (!flag)
			{
				ThrowDebugText("o", c);
			}
			return false;
		}

		private void ThrowDebugText(string text, IntVec3 c)
		{
			if (DebugViewSettings.drawShooting)
			{
				MoteMaker.ThrowText(c.ToVector3Shifted(), Map, text, -1f);
			}
		}
		private void ImpactSomething()
		{
			if (def.projectile.flyOverhead)
			{
				RoofDef roofDef = Map.roofGrid.RoofAt(Position);
				if (roofDef != null)
				{
					if (roofDef.isThickRoof)
					{
						/ *this.ThrowDebugText("hit-thick-roof", Position);
						this.def.projectile.soundHitThickRoof.PlayOneShot(new TargetInfo(Position, Map, false));
						this.Destroy(DestroyMode.Vanish);
						return;* /
					}
					else if (Position.GetEdifice(Map) == null || Position.GetEdifice(Map).def.Fillage != FillCategory.Full)
					{
						RoofCollapserImmediate.DropRoofInCells(Position, Map, null);
					}
				}
			}
			if (!usedTarget.HasThing || !CanHit(usedTarget.Thing))
			{
				cellThingsFiltered.Clear();
				List<Thing> thingList = Position.GetThingList(Map);
				for (int i = 0; i < thingList.Count; i++)
				{
					Thing thing = thingList[i];
					if ((thing.def.category == ThingCategory.Building || thing.def.category == ThingCategory.Pawn || thing.def.category == ThingCategory.Item || thing.def.category == ThingCategory.Plant) && CanHit(thing))
					{
						cellThingsFiltered.Add(thing);
					}
				}
				cellThingsFiltered.Shuffle();
				for (int j = 0; j < cellThingsFiltered.Count; j++)
				{
					Thing thing2 = cellThingsFiltered[j];
					float num;
					if (thing2 is Pawn pawn)
					{
						num = 0.5f * Mathf.Clamp(pawn.BodySize, 0.1f, 2f);
						if (pawn.GetPosture() != PawnPosture.Standing && (origin - destination).MagnitudeHorizontalSquared() >= 20.25f)
						{
							num *= 0.2f;
						}
						if (launcher != null && pawn.Faction != null && launcher.Faction != null && !pawn.Faction.HostileTo(launcher.Faction))
						{
							num *= VerbUtility.InterceptChanceFactorFromDistance(origin, Position);
						}
					}
					else
					{
						num = 1.5f * thing2.def.fillPercent;
					}
					if (Rand.Chance(num))
					{
						ThrowDebugText("hit-" + num.ToStringPercent(), Position);
						Impact(cellThingsFiltered.RandomElement());
						return;
					}
					ThrowDebugText("miss-" + num.ToStringPercent(), Position);
				}
				Impact(null);
				return;
			}
			if (usedTarget.Thing is Pawn pawn2 && pawn2.GetPosture() != PawnPosture.Standing && (origin - destination).MagnitudeHorizontalSquared() >= 20.25f && !Rand.Chance(0.2f))
			{
				ThrowDebugText("miss-laying", Position);
				Impact(null);
				return;
			}
			Impact(usedTarget.Thing);
		}

		/ *protected virtual void Impact(Thing hitThing)
		{
			GenClamor.DoClamor(this, 2.1f, ClamorDefOf.Impact);
			this.Destroy(DestroyMode.Vanish);
		}* /

		protected override void Impact(Thing hitThing)
		{
			if (def.projectile.explosionDelay == 0)
			{
				Explode();
				return;
			}
			landed = true;
			ticksToDetonation = def.projectile.explosionDelay;
			GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, def.projectile.damageDef, launcher.Faction);
		}

		public override void Tick()
		{
			if (AllComps != null)
			{
				int i = 0;
				int count = AllComps.Count;
				while (i < count)
				{
					AllComps[i].CompTick();
					i++;
				}
			}
			BASETick();
			if (ticksToDetonation > 0)
			{
				ticksToDetonation--;
				if (ticksToDetonation <= 0)
				{
					Explode();
				}
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static Projectile_Explosive_RoofBypass()
		{
		}
	}
	*/
}
