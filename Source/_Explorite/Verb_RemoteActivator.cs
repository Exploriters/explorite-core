/********************
 * 远程效果设备。
 * --siiftun1857
 */
using System.Collections.Generic;
using Verse;
using RimWorld;
using Verse.AI;
using System.Linq;

namespace Explorite
{
	///<summary>为<see cref = "Verb_RemoteActivator" />接收参数。</summary>
	public class VerbProperties_RemoteActivator : VerbProperties
	{
		[NoTranslate]
		public List<string> tags;
		public VerbProperties_RemoteActivator()
		{
			verbClass = typeof(Verb_RemoteActivator);
		}
	}
	///<summary>远程效果激活设备，激活<see cref = "CompRemoteActivationEffect" />。</summary>
	public class Verb_RemoteActivator : Verb_CastBase
	{
		private VerbProperties_RemoteActivator VerbProps => verbProps as VerbProperties_RemoteActivator;
		public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
		{
			if (!base.ValidateTarget(target, showMessages))
			{
				return false;
			}
			if (target.HasThing)
			{
				if (target.Thing.AnyTriggers(VerbProps.tags))
				{
					return true;
				}
				if (showMessages)
				{
					Messages.Message("AbilityCantApplyOnMissingActivationDevice".Translate(), target.Thing, MessageTypeDefOf.RejectInput, false);
				}
			}
			return false;
		}
		protected override bool TryCastShot()
		{
			if (!ValidateTarget(currentTarget))
			{
				return false;
			}
			/*CompReloadable reloadableCompSource = base.ReloadableCompSource;
			if (reloadableCompSource != null)
			{
				reloadableCompSource.UsedOnce();
			}*/
			return RemoteActiveUtility.ActiveTriggers(currentTarget.Thing.RemoteTriggers(VerbProps.tags), VerbProps.tags);
		}
	}

	///<summary>为<see cref = "Verb_RemoteActivator" />接收参数。</summary>
	public class VerbProperties_RemoteActivator_Area : VerbProperties_RemoteActivator
	{
		public float radius = float.PositiveInfinity;
		public bool needLOSToCenter = false;
		public VerbProperties_RemoteActivator_Area()
		{
			verbClass = typeof(Verb_RemoteActivator_Area);
		}
	}
	///<summary>远程效果激活设备，激活<see cref = "CompRemoteActivationEffect" />。</summary>
	public class Verb_RemoteActivator_Area : Verb_CastBase
	{
		private VerbProperties_RemoteActivator_Area VerbProps => verbProps as VerbProperties_RemoteActivator_Area;
		public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
		{
			needLOSToCenter = VerbProps.needLOSToCenter;
			return VerbProps.radius;
		}
		private IEnumerable<IRemoteActivationEffect> ListEffects()
		{
			IntVec3 targetPos = currentTarget.Cell;
			IEnumerable<Thing> things = caster.Map.listerThings.AllThings.Where(thing => thing is ThingWithComps tc && thing.Position.DistanceTo(targetPos) <= VerbProps.radius && (!VerbProps.needLOSToCenter || GenSight.LineOfSight(thing.Position, targetPos, caster.Map)));
			List<IRemoteActivationEffect> effects = new List<IRemoteActivationEffect>();
			foreach (Thing thing in things)
			{
				foreach (IRemoteActivationEffect effect in thing.RemoteTriggers(VerbProps.tags))
				{
					yield return effect;
				}
			}
		}
		protected override bool TryCastShot()
		{
			return RemoteActiveUtility.ActiveTriggers(ListEffects(), VerbProps.tags);
		}
	}
}
