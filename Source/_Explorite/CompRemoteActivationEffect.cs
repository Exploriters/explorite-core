/********************
 * 远程效果设备。
 * --siiftun1857
 */
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Explorite
{
	///<summary>为<see cref = "CompRemoteActivationEffect" />接收参数。</summary>
	public class CompProperties_RemoteActivationEffect : CompProperties
	{
		public CompProperties_RemoteActivationEffect()
		{
			compClass = typeof(CompRemoteActivationEffect);
		}

		[NoTranslate]
		public List<string> tags;
	}
	///<summary>远程效果设备，响应<see cref = "Verb_RemoteActivator" />。</summary>
	public abstract class CompRemoteActivationEffect : ThingComp, IRemoteActivationEffect
	{
		CompProperties_RemoteActivationEffect Props => props as CompProperties_RemoteActivationEffect;
		public virtual bool CanActiveNow(IEnumerable<string> tags)
		{
			foreach (string tag in tags)
			{
				if (Props.tags.Contains(tag))
				{
					return true;
				}
			}
			return false;
		}
		public abstract bool ActiveEffect();
	}

	///<summary>远程效果服装设备，仅在被穿戴时才会响应。</summary>
	public abstract class CompRemoteActivationEffect_Apparel : CompRemoteActivationEffect
	{
		public Pawn Wearer => parent is Apparel apparel ? apparel.Wearer : null;
		public override bool CanActiveNow(IEnumerable<string> tags)
		{
			return base.CanActiveNow(tags) && Wearer != null;
		}
	}

	///<summary>为<see cref = "CompRemoteActivationEffect_Apparel_ApplyHediff" />接收参数。</summary>
	public class CompProperties_RemoteActivationEffect_Apparel_ApplyHediff : CompProperties_RemoteActivationEffect
	{
		public CompProperties_RemoteActivationEffect_Apparel_ApplyHediff()
		{
			compClass = typeof(CompRemoteActivationEffect_Apparel_ApplyHediff);
		}

		public HediffDef hediff;
		public BodyPartDef part;
		public IntRange? count;
	}
	///<summary>响应效果时为穿戴者施加hediff。</summary>
	public class CompRemoteActivationEffect_Apparel_ApplyHediff : CompRemoteActivationEffect_Apparel
	{
		CompProperties_RemoteActivationEffect_Apparel_ApplyHediff Props => props as CompProperties_RemoteActivationEffect_Apparel_ApplyHediff;
		public override bool ActiveEffect()
		{
			if (Props.hediff.HasComp(typeof(HediffComp_DisappearsOnDeath)) && Wearer.Dead)
			{
				return false;
			}
			IEnumerable<BodyPartRecord> partrecs = Props.part == null ? new List<BodyPartRecord>() { null } : Wearer.def.race.body.AllParts.Where(p => p.def == Props.part).InRandomOrder();
			int count = Props.count?.RandomInRange ?? int.MaxValue;
			foreach (BodyPartRecord partrec in partrecs)
			{
				count--;
				if (count < 0)
					break;
				Hediff hediff = HediffMaker.MakeHediff(Props.hediff, Wearer, partrec);
				hediff.TryGetComp<HediffComp_DisappearsOnSourceApparelLost>()?.AddSources(parent as Apparel);
				Wearer.health.AddHediff(hediff);
			}
			return true;
		}
	}
	///<summary>为<see cref = "CompRemoteActivationEffect_Apparel_ApplyDamage" />接收参数。</summary>
	public class CompProperties_RemoteActivationEffect_Apparel_ApplyDamage : CompProperties_RemoteActivationEffect
	{
		public CompProperties_RemoteActivationEffect_Apparel_ApplyDamage()
		{
			compClass = typeof(CompRemoteActivationEffect_Apparel_ApplyDamage);
		}

		public DamageDef damageType;
		public FloatRange? damageAmount;
		public BodyPartDef part;
		public IntRange? count;
	}
	///<summary>响应效果时为穿戴者施加伤害。</summary>
	public class CompRemoteActivationEffect_Apparel_ApplyDamage : CompRemoteActivationEffect_Apparel
	{
		CompProperties_RemoteActivationEffect_Apparel_ApplyDamage Props => props as CompProperties_RemoteActivationEffect_Apparel_ApplyDamage;
		public override bool ActiveEffect()
		{
			IEnumerable<BodyPartRecord> partrecs = Props.part == null ? new List<BodyPartRecord>() { null } : Wearer.def.race.body.AllParts.Where(p => p.def == Props.part).InRandomOrder();
			int count = Props.count?.RandomInRange ?? int.MaxValue;
			foreach (BodyPartRecord partrec in partrecs)
			{
				count--;
				if (count < 0)
					break;
				Wearer.TakeDamage(
				new DamageInfo(
					Props.damageType, Props.damageAmount?.RandomInRange ?? Props.damageType.defaultDamage,
					hitPart: partrec,
					//instigator: 
					weapon: parent.def
					)
				);
			}
			return true;
		}
	}

	///<summary>为<see cref = "CompRemoteActivationEffect_PlayEffects" />接收参数。</summary>
	public class CompProperties_RemoteActivationEffect_PlayEffects : CompProperties_RemoteActivationEffect
	{
		public CompProperties_RemoteActivationEffect_PlayEffects()
		{
			compClass = typeof(CompRemoteActivationEffect_PlayEffects);
		}
		public SoundDef sound;
		public List<SoundDef> sounds;
		public FleckDef fleck;
		public List<FleckDef> flecks;
		public ThingDef mote;
		public List<ThingDef> motes;
		public float scale = 1f;
		public bool forcedLocateionRelated = false;
	}
	///<summary>响应效果时发出声音以及效果。</summary>
	public class CompRemoteActivationEffect_PlayEffects : CompRemoteActivationEffect
	{
		CompProperties_RemoteActivationEffect_PlayEffects Props => props as CompProperties_RemoteActivationEffect_PlayEffects;
		public override bool ActiveEffect()
		{
			Thing finalParent = parent.FinalSpawnedParent();
			if (finalParent == null)
			{
				return false;
			}
			SpawnAll(Props.forcedLocateionRelated ? new TargetInfo(finalParent.Position, finalParent.Map, false) : new TargetInfo(finalParent));
			return true;
		}
		private IEnumerable<T> Combine<T>(T obj, IEnumerable<T> objs)
		{
			return (obj != null ? new T[] { obj } : Enumerable.Empty<T>()).Concat(objs ?? Enumerable.Empty<T>()).Where(obj => obj != null);
		}
		private void SpawnAll(TargetInfo target)
		{
			foreach (SoundDef sound in Combine(Props.sound, Props.sounds))
			{
				sound.PlayOneShot(target);
			}
			foreach (ThingDef mote in Combine(Props.mote, Props.motes))
			{
				SpawnMote(target, mote);
			}
			foreach (FleckDef fleck in Combine(Props.fleck, Props.flecks))
			{
				SpawnFleck(target, fleck);
			}
		}
		private void SpawnFleck(TargetInfo target, FleckDef def)
		{
			if (target.HasThing)
			{
				FleckMaker.AttachedOverlay(target.Thing, def, Vector3.zero, Props.scale, -1f);
				return;
			}
			FleckMaker.Static(target.Cell, target.Map, def, Props.scale);
		}
		private void SpawnMote(TargetInfo target, ThingDef def)
		{
			if (target.HasThing)
			{
				MoteMaker.MakeAttachedOverlay(target.Thing, def, Vector3.zero, Props.scale, -1f);
				return;
			}
			MoteMaker.MakeStaticMote(target.Cell, target.Map, def, Props.scale);
		}
	}
	///<summary>为<see cref = "CompRemoteActivationEffect_Destroy" />接收参数。</summary>
	public class CompProperties_RemoteActivationEffect_Destroy : CompProperties_RemoteActivationEffect
	{
		public CompProperties_RemoteActivationEffect_Destroy()
		{
			compClass = typeof(CompRemoteActivationEffect_Destroy);
		}
		public DestroyMode destroyMode = DestroyMode.Vanish;
	}
	///<summary>响应效果时自毁。</summary>
	public class CompRemoteActivationEffect_Destroy : CompRemoteActivationEffect
	{
		CompProperties_RemoteActivationEffect_Destroy Props => props as CompProperties_RemoteActivationEffect_Destroy;
		public override bool ActiveEffect()
		{
			if (!parent.Destroyed)
				parent.Destroy(Props.destroyMode);
			return true;
		}
	}
	///<summary>为<see cref = "CompRemoteActivationEffect_Kill" />接收参数。</summary>
	public class CompProperties_RemoteActivationEffect_Kill : CompProperties_RemoteActivationEffect
	{
		public CompProperties_RemoteActivationEffect_Kill()
		{
			compClass = typeof(CompRemoteActivationEffect_Kill);
		}
	}
	///<summary>响应效果时自毁。</summary>
	public class CompRemoteActivationEffect_Kill : CompRemoteActivationEffect
	{
		public override bool ActiveEffect()
		{
			if (!parent.Destroyed)
				parent.Kill();
			return true;
		}
	}
	///<summary>为<see cref = "CompRemoteActivationEffect_InvokeExplode" />接收参数。</summary>
	public class CompProperties_RemoteActivationEffect_InvokeExplode : CompProperties_RemoteActivationEffect
	{
		public CompProperties_RemoteActivationEffect_InvokeExplode()
		{
			compClass = typeof(CompRemoteActivationEffect_InvokeExplode);
		}
	}
	///<summary>响应效果时调用爆炸。</summary>
	public class CompRemoteActivationEffect_InvokeExplode : CompRemoteActivationEffect
	{
		public override bool ActiveEffect()
		{
			foreach (ThingComp comp in parent.AllComps)
			{
				if (comp is CompExplosive explosive)
				{
					typeof(CompExplosive).GetMethod("Detonate", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(explosive, new object[] { parent.MapHeld, true });
				}
			}
			return true;
		}
	}
	///<summary>为<see cref = "CompRemoteActivationEffect_Explode" />接收参数。</summary>
	public class CompProperties_RemoteActivationEffect_Explode : CompProperties_RemoteActivationEffect
	{
		public float explosiveRadius = 1.9f;
		public DamageDef explosiveDamageType;
		public int damageAmountBase = -1;
		public float armorPenetrationBase = -1f;
		public ThingDef postExplosionSpawnThingDef;
		public float postExplosionSpawnChance;
		public int postExplosionSpawnThingCount = 1;
		public bool applyDamageToExplosionCellsNeighbors;
		public ThingDef preExplosionSpawnThingDef;
		public float preExplosionSpawnChance;
		public int preExplosionSpawnThingCount = 1;
		public float chanceToStartFire;
		public bool damageFalloff;
		public float explosiveExpandPerStackcount;
		public float explosiveExpandPerFuel;
		public EffecterDef explosionEffect;
		public SoundDef explosionSound;
		public float destroyThingOnExplosionSize = float.PositiveInfinity;
		public bool immuneToExplodeFromSelf;
		public CompProperties_RemoteActivationEffect_Explode()
		{
			compClass = typeof(CompRemoteActivationEffect_Explode);
		}
	}
	///<summary>响应效果时爆炸。</summary>
	public class CompRemoteActivationEffect_Explode : CompRemoteActivationEffect
	{
		CompProperties_RemoteActivationEffect_Explode Props => props as CompProperties_RemoteActivationEffect_Explode;
		public override bool ActiveEffect()
		{
			Detonate();
			return true;
		}
		public float ExplosiveRadius
		{
			get
			{
				float explosiveRadius = Props.explosiveRadius;
				if (parent.stackCount > 1 && Props.explosiveExpandPerStackcount > 0f)
				{
					explosiveRadius += Mathf.Sqrt((float)(parent.stackCount - 1) * Props.explosiveExpandPerStackcount);
				}
				if (Props.explosiveExpandPerFuel > 0f && parent.GetComp<CompRefuelable>() != null)
				{
					explosiveRadius += Mathf.Sqrt(parent.GetComp<CompRefuelable>().Fuel * Props.explosiveExpandPerFuel);
				}
				return explosiveRadius;
			}
		}

		protected void Detonate(bool ignoreUnspawned = false)
		{
			if (!ignoreUnspawned && !parent.SpawnedOrAnyParentSpawned)
			{
				return;
			}
			Map map = parent.MapHeld;
			IntVec3 pos;
			float explosiveRadius = ExplosiveRadius;
			if (Props.explosiveExpandPerFuel > 0f && parent.GetComp<CompRefuelable>() != null)
			{
				parent.GetComp<CompRefuelable>().ConsumeFuel(parent.GetComp<CompRefuelable>().Fuel);
			}
			try
			{
				pos = parent.FinalSpawnedParent()?.Position ?? throw new NullReferenceException("MANUAL");
			}
			catch (NullReferenceException)
			{
				return;
			}
			finally
			{
				if (Props.destroyThingOnExplosionSize <= explosiveRadius && !parent.Destroyed)
				{
					parent.Kill(null, null);
				}
			}
			if (map == null)
			{
				Log.Warning("Tried to detonate CompRemoteActivationEffect_Explode in a null map.");
				return;
			}
			if (Props.explosionEffect != null)
			{
				Effecter effecter = Props.explosionEffect.Spawn();
				effecter.Trigger(new TargetInfo(pos, map, false), new TargetInfo(pos, map, false));
				effecter.Cleanup();
			}
			Thing instigator = null;
			/*if (this.instigator != null && (!this.instigator.HostileTo(this.parent.Faction) || this.parent.Faction == Faction.OfPlayer))
			{
				parent = this.instigator;
			}
			else
			{
				instigator = parent;
			}*/
			GenExplosion.DoExplosion(
				center: pos,
				map: map,
				radius: explosiveRadius,
				damType: Props.explosiveDamageType,
				instigator: instigator,
				damAmount: Props.damageAmountBase,
				armorPenetration: Props.armorPenetrationBase,
				explosionSound: Props.explosionSound,
				weapon: parent.def,
				projectile: null,
				intendedTarget: null,
				postExplosionSpawnThingDef: Props.postExplosionSpawnThingDef,
				postExplosionSpawnChance: Props.postExplosionSpawnChance,
				postExplosionSpawnThingCount: Props.postExplosionSpawnThingCount,
				applyDamageToExplosionCellsNeighbors: Props.applyDamageToExplosionCellsNeighbors,
				preExplosionSpawnThingDef: Props.preExplosionSpawnThingDef,
				preExplosionSpawnChance: Props.preExplosionSpawnChance,
				preExplosionSpawnThingCount: Props.preExplosionSpawnThingCount,
				chanceToStartFire: Props.chanceToStartFire,
				damageFalloff: Props.damageFalloff,
				direction: null,
				ignoredThings: Props.immuneToExplodeFromSelf ? new List<Thing>() { parent } : null//this.thingsIgnoredByExplosion
				);
		}
	}
}
