/********************
 * 包含多个继承自Hediff的类。
 * --siiftun1857
 */
using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using static Explorite.ExploriteCore;

namespace Explorite
{
	///<summary>物理操作仪的hediff类型。无实际效果。</summary>
	public class Hediff_AddedPart_HManipulator : Hediff_AddedPart
	{
		//T___O_NOMORE__D____O: Prevent remove part surgery targeting HM
		//NOT HERE!
		//Solved somewhere other
	}

	///<summary>子系统HediffDef类，接收数个参数。</summary>
	public class HediffDef_Subsystem : HediffDef
	{
		[MustTranslate]
		public string tip = string.Empty;
		[NoTranslate]
		public string sysTexturePath = "Subsystems/_MissingTexture";

		private Texture2D SubsystemIconInt;
		public Texture2D SubsystemIcon => SubsystemIconInt ??= ContentFinder<Texture2D>.Get(sysTexturePath);
	}
	///<summary>子系统hediff类，不显示部件效率。</summary>
	public class Hediff_AddedPart_Subsystem : Hediff_AddedPart
	{
		public HediffDef_Subsystem Def => def as HediffDef_Subsystem;
		public string Tip => Def.tip ?? string.Empty;
		public virtual bool StageVaild => def.stages.Count >= 2;
		public virtual bool Enabled => !StageVaild || CurStageIndex >= 1;
		public virtual bool Initialzing => StageVaild && CurStageIndex < 1;
		public virtual float InitPercentage => severityInt / def.stages[1].minSeverity;
		public virtual SubsystemState SubsystemState
		{
			get
			{
				if (Initialzing)
					return SubsystemState.Initialzing;
				else if (Enabled)
					return SubsystemState.Enabled;
				return SubsystemState.Unidentified;
			}
		}
		/*public virtual string LabelString(string tip = null)
		{
			if (def.stages.Count >= 2)
			{
				if (CurStageIndex <= 0)
				{
					return "Magnuassembly_SubsystemInitialzingProgressLabel".Translate(severityInt / def.stages[1].minSeverity);
				}
			}
			return tip ?? string.Empty;
		}*/
		public override string SeverityLabel => StageVaild ? InitPercentage.ToStringPercent() : null;
		public virtual float? TicksRemaining
		{
			get
			{
				try
				{
					if (StageVaild && this.TryGetComp<HediffComp_SeverityPerDay>()?.props is HediffCompProperties_SeverityPerDay prop)
					{
						return (def.stages[1].minSeverity - severityInt) / prop.severityPerDay * 60000f;
					}
				}
				catch (Exception)
				{
				}
				return null;
			}
		}

		public override string LabelInBrackets
		{
			get
			{
				List<string> strs = new List<string>();
				if (Initialzing)
				{
					strs.Add("Magnuassembly_SubsystemInitialzingProgressLabel".Translate());
				}
				//stringBuilder.Append(base.LabelInBrackets);
				if (CurStage != null && !CurStage.label.NullOrEmpty())
				{
					strs.Add(CurStage.label);
				}
				if (comps != null)
				{
					for (int i = 0; i < comps.Count; i++)
					{
						string compLabelInBracketsExtra = comps[i].CompLabelInBracketsExtra;
						if (!compLabelInBracketsExtra.NullOrEmpty())
						{
							strs.Add(compLabelInBracketsExtra);
						}
					}
				}
				return string.Join(", ", strs);
			}
		}
		public override string TipStringExtra
		{
			get
			{
				List<string> strs = new List<string>();
				if (Initialzing)
				{
					strs.Add("Magnuassembly_SubsystemInitialzingProgressLabel".Translate() + ": " + ((int)TicksRemaining.Value).ToStringTicksToPeriod());
				}
				else if (!Tip.NullOrEmpty() && Enabled)
				{
					strs.Add(Tip);
				}
				//stringBuilder.Append(base.TipStringExtra);
				foreach (StatDrawEntry item in HediffStatsUtility.SpecialDisplayStats(CurStage, this))
				{
					if (item.ShouldDisplay)
					{
						strs.Add(item.LabelCap + ": " + item.ValueString);
					}
				}
				if (comps != null)
				{
					for (int i = 0; i < comps.Count; i++)
					{
						string compTipStringExtra = comps[i].CompTipStringExtra;
						if (!compTipStringExtra.NullOrEmpty())
						{
							strs.Add(compTipStringExtra);
						}
					}
				}
				return string.Join("\n", strs);
			}
		}
	}
	///<summary>空白子系统的hediff类，控制是否显示这一症状。</summary>
	public class Hediff_AddedPart_BlankSubsystem : Hediff_AddedPart_Subsystem
	{
		public override bool Visible => (pawn?.health?.hediffSet?.hediffs?.Any(hediff => hediff?.def?.tags?.Contains("CentaurTechHediff_AccuSubsystem") ?? false)) ?? false;
		public override string LabelInBrackets => string.Empty;
		public override string TipStringExtra => string.Empty;
		public override SubsystemState SubsystemState => SubsystemState.Blank;
	}

}
