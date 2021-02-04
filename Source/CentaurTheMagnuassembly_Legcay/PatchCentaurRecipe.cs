//using Harmony;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Verse;
using static CentaurTheMagnuassembly.RimCentaurCore;
//[assembly: IgnoresAccessChecksTo("RimWorld.Recipe_RemoveBodyPart")]
/*[assembly: IgnoresAccessChecksTo("RimWorld")]

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class IgnoresAccessChecksToAttribute : Attribute
    {
        public IgnoresAccessChecksToAttribute(string assemblyName)
        {
            AssemblyName = assemblyName;
        }

        public string AssemblyName { get; }
    }
}*/

namespace CentaurTheMagnuassembly
{
    [StaticConstructorOnStartup]
    public static class PatchCentaurRecipe
    {
        static PatchCentaurRecipe()
        {
            //AlienCentaurDef.recipes.Remove(DefDatabase<RecipeDef>.GetNamed("RemoveBodyPart"));
            RecipeDef RemoveBodyPart = DefDatabase<RecipeDef>.GetNamed("RemoveBodyPart");
            RecipeDef RemoveBodyPart_ExcludingScapular = DefDatabase<RecipeDef>.GetNamed("RemoveBodyPart_ExcludingScapular");
            List<RecipeDef> Recipes = AlienCentaurDef.recipes;
            for (int i = 0; i < Recipes.Count; i++)
            {
                if (Recipes[i] == RemoveBodyPart)
                {
                    AlienCentaurDef.recipes[i] = RemoveBodyPart_ExcludingScapular;
                }
            }
        }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("", "CS0122")]
        /*public static IEnumerable<BodyPartRecord> GetPartsToApplyOn(this Recipe_RemoveBodyPart thisclass, Pawn pawn, RecipeDef recipe)
        {
            IEnumerable<BodyPartRecord> preret = thisclass.Recipe_RemoveBodyPart();
            foreach (BodyPartRecord bpr in preret)
            {
                if (bpr.def == CentaurScapularDef)
                    (preret as List<BodyPartRecord>).Remove(bpr);
            }
            return preret;
        }*/
    }


    /*public class Recipe_RemoveBodyPart_ExcludingScapular : Recipe_RemoveBodyPart
    {
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            IEnumerable<BodyPartRecord> preret = base.Recipe_RemoveBodyPart();
            foreach (BodyPartRecord bpr in preret)
            {
                if (bpr.def == CentaurScapularDef)
                    (preret as List<BodyPartRecord>).Remove(bpr);
            }
            return preret;
        }
    }*/

    /*[Verse.StaticConstructorOnStartup]
    static class HarmonyPatchCentaurRecipe
    {
        // this static constructor runs to create a HarmonyInstance and install a patch.
        static HarmonyPatchCentaurRecipe()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("CentaurTheMagnuassembly.rimworld.mod.RemoveBodyPartRecipePatch");

            // find the FillTab method of the class RimWorld.ITab_Pawn_Character
            MethodInfo targetmethod = AccessTools.Method(typeof(RimWorld.Recipe_Surgery), "GetPartsToApplyOn");

            // find the static method to call before (i.e. Prefix) the targetmethod
            HarmonyMethod prefixmethod = new HarmonyMethod(typeof(CentaurTheMagnuassembly.HarmonyPatchCentaurRecipe).GetMethod("GetPartsToApplyOn_Prefix"));

            // patch the targetmethod, by calling prefixmethod before it runs, with no postfixmethod (i.e. null)
            harmony.Patch(targetmethod, prefixmethod, null);
        }
        
        // This method is now always called right before RimWorld.ITab_Pawn_Character.FillTab.
        // So, before the ITab_Pawn_Character is instantiated, reset the height of the dialog window.
        // The class RimWorld.ITab_Pawn_Character is static so there is no this __instance.
        public static void GetPartsToApplyOn_Prefix(RimWorld.Recipe_Surgery __instance, Pawn pawn, RecipeDef recipe, ref IEnumerable<BodyPartRecord> __result)
        {
            __result = __instance.GetPartsToApplyOn(pawn, recipe);
            foreach (BodyPartRecord bpr in __result)
            {
                if (bpr.def == CentaurScapularDef)
                    (__result as List<BodyPartRecord>).Remove(bpr);
            }
        }
    }*/


    public class Recipe_RemoveBodyPart_MX : Recipe_Surgery
    {
        //private const float ViolationGoodwillImpact = 20f;
        [DebuggerHidden]
        /*public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            Recipe_RemoveBodyPart_MX.< GetPartsToApplyOn > c__Iterator8F < GetPartsToApplyOn > c__Iterator8F = new Recipe_RemoveBodyPart_MX.< GetPartsToApplyOn > c__Iterator8F();

            < GetPartsToApplyOn > c__Iterator8F.pawn = pawn;

            < GetPartsToApplyOn > c__Iterator8F.<$> pawn = pawn;
            Recipe_RemoveBodyPart_MX.< GetPartsToApplyOn > c__Iterator8F expr_15 = < GetPartsToApplyOn > c__Iterator8F;
            expr_15.$PC = -2;
            return expr_15;
        }*/
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            IEnumerable<BodyPartRecord> parts = pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null);
            using IEnumerator<BodyPartRecord> enumerator = parts.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BodyPartRecord part = enumerator.Current;
                if (part.def != CentaurScapularDef)
                {
                    if (pawn.health.hediffSet.HasDirectlyAddedPartFor(part))
                    {
                        yield return part;
                    }
                    else if (MedicalRecipesUtility.IsCleanAndDroppable(pawn, part))
                    {
                        yield return part;
                    }
                    else if (part != pawn.RaceProps.body.corePart && part.def.canSuggestAmputation && pawn.health.hediffSet.hediffs.Any((Hediff d) => !(d is Hediff_Injury) && d.def.isBad && d.Visible && d.Part == part))
                    {
                        yield return part;
                    }
                }
            }
        }
        public override string GetLabelWhenUsedOn(Pawn pawn, BodyPartRecord part)
        {
            if (pawn.RaceProps.IsMechanoid || pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(part))
            {
                return RecipeDefOf.RemoveBodyPart.label;
            }
            BodyPartRemovalIntent bodyPartRemovalIntent = HealthUtility.PartRemovalIntent(pawn, part);
            if (bodyPartRemovalIntent != BodyPartRemovalIntent.Amputate)
            {
                if (bodyPartRemovalIntent != BodyPartRemovalIntent.Harvest)
                {
                    throw new InvalidOperationException();
                }
                return "HarvestOrgan".Translate();
            }
            else
            {
                if (part.depth == BodyPartDepth.Inside || part.def.socketed)
                {
                    return "RemoveOrgan".Translate();
                }
                return "Amputate".Translate();
            }
        }
        public override bool IsViolationOnPawn(Pawn pawn, BodyPartRecord part, Faction billDoerFaction)
        {
            return pawn.Faction != billDoerFaction && pawn.Faction != null && HealthUtility.PartRemovalIntent(pawn, part) == BodyPartRemovalIntent.Harvest;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            bool flag = MedicalRecipesUtility.IsClean(pawn, part);
            bool flag2 = this.IsViolationOnPawn(pawn, part, Faction.OfPlayer);
            if (billDoer != null)
            {
                if (base.CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
                {
                    return;
                }
                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
                {
                    billDoer,
                    pawn
                });
                MedicalRecipesUtility.SpawnNaturalPartIfClean(pawn, part, billDoer.Position, billDoer.Map);
                MedicalRecipesUtility.SpawnThingsFromHediffs(pawn, part, billDoer.Position, billDoer.Map);
            }
            DamageDef surgicalCut = DamageDefOf.SurgicalCut;
            float amount = 99999f;
            float armorPenetration = 999f;
            pawn.TakeDamage(new DamageInfo(surgicalCut, amount, armorPenetration, -1f, null, part, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
            if (flag)
            {
                if (pawn.Dead)
                {
                    ThoughtUtility.GiveThoughtsForPawnExecuted(pawn, PawnExecutionKind.OrganHarvesting);
                }
                ThoughtUtility.GiveThoughtsForPawnOrganHarvested(pawn);
            }
            if (flag2 && pawn.Faction != null && billDoer != null && billDoer.Faction != null)
            {
                Faction faction = pawn.Faction;
                Faction faction2 = billDoer.Faction;
                int goodwillChange = -15;
                string reason = "GoodwillChangedReason_RemovedBodyPart".Translate(part.LabelShort);
                GlobalTargetInfo? lookTarget = new GlobalTargetInfo?(pawn);
                faction.TryAffectGoodwillWith(faction2, goodwillChange, true, true, reason, lookTarget);
            }
        }
    }


    /*public class Recipe_RemoveBodyPart : Recipe_Surgery
    {
        public Recipe_RemoveBodyPart()
        {
        }

        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            IEnumerable<BodyPartRecord> parts = pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null);
            using (IEnumerator<BodyPartRecord> enumerator = parts.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    BodyPartRecord part = enumerator.Current;
                    if (pawn.health.hediffSet.HasDirectlyAddedPartFor(part))
                    {
                        yield return part;
                    }
                    else if (MedicalRecipesUtility.IsCleanAndDroppable(pawn, part))
                    {
                        yield return part;
                    }
                    else if (part != pawn.RaceProps.body.corePart && part.def.canSuggestAmputation && pawn.health.hediffSet.hediffs.Any((Hediff d) => !(d is Hediff_Injury) && d.def.isBad && d.Visible && d.Part == part))
                    {
                        yield return part;
                    }
                }
            }
            yield break;
        }

        public override bool IsViolationOnPawn(Pawn pawn, BodyPartRecord part, Faction billDoerFaction)
        {
            return pawn.Faction != billDoerFaction && pawn.Faction != null && HealthUtility.PartRemovalIntent(pawn, part) == BodyPartRemovalIntent.Harvest;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            bool flag = MedicalRecipesUtility.IsClean(pawn, part);
            bool flag2 = this.IsViolationOnPawn(pawn, part, Faction.OfPlayer);
            if (billDoer != null)
            {
                if (base.CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
                {
                    return;
                }
                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
                {
                    billDoer,
                    pawn
                });
                MedicalRecipesUtility.SpawnNaturalPartIfClean(pawn, part, billDoer.Position, billDoer.Map);
                MedicalRecipesUtility.SpawnThingsFromHediffs(pawn, part, billDoer.Position, billDoer.Map);
            }
            DamageDef surgicalCut = DamageDefOf.SurgicalCut;
            float amount = 99999f;
            float armorPenetration = 999f;
            pawn.TakeDamage(new DamageInfo(surgicalCut, amount, armorPenetration, -1f, null, part, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
            if (flag)
            {
                if (pawn.Dead)
                {
                    ThoughtUtility.GiveThoughtsForPawnExecuted(pawn, PawnExecutionKind.OrganHarvesting);
                }
                ThoughtUtility.GiveThoughtsForPawnOrganHarvested(pawn);
            }
            if (flag2 && pawn.Faction != null && billDoer != null && billDoer.Faction != null)
            {
                Faction faction = pawn.Faction;
                Faction faction2 = billDoer.Faction;
                int goodwillChange = -15;
                string reason = "GoodwillChangedReason_RemovedBodyPart".Translate(new object[]
                {
                    part.LabelShort
                });
                GlobalTargetInfo? lookTarget = new GlobalTargetInfo?(pawn);
                faction.TryAffectGoodwillWith(faction2, goodwillChange, true, true, reason, lookTarget);
            }
        }

        public override string GetLabelWhenUsedOn(Pawn pawn, BodyPartRecord part)
        {
            if (pawn.RaceProps.IsMechanoid || pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(part))
            {
                return RecipeDefOf.RemoveBodyPart.label;
            }
            BodyPartRemovalIntent bodyPartRemovalIntent = HealthUtility.PartRemovalIntent(pawn, part);
            if (bodyPartRemovalIntent != BodyPartRemovalIntent.Amputate)
            {
                if (bodyPartRemovalIntent != BodyPartRemovalIntent.Harvest)
                {
                    throw new InvalidOperationException();
                }
                return "HarvestOrgan".Translate();
            }
            else
            {
                if (part.depth == BodyPartDepth.Inside || part.def.socketed)
                {
                    return "RemoveOrgan".Translate();
                }
                return "Amputate".Translate();
            }
        }

        [CompilerGenerated]
        private sealed class <GetPartsToApplyOn>c__Iterator0 : IEnumerable, IEnumerable<BodyPartRecord>, IEnumerator, IDisposable, IEnumerator<BodyPartRecord>
		{
			internal Pawn pawn;

        internal IEnumerable<BodyPartRecord> <parts>__0;

			internal IEnumerator<BodyPartRecord> $locvar0;

			internal BodyPartRecord $current;

			internal bool $disposing;

			internal int $PC;

			private Recipe_RemoveBodyPart.<GetPartsToApplyOn>c__Iterator0.<GetPartsToApplyOn>c__AnonStorey1 $locvar1;

			[DebuggerHidden]
        public <GetPartsToApplyOn>c__Iterator0()
        {
        }

        public bool MoveNext()
        {
            uint num = (uint)this.$PC;
            this.$PC = -1;
            bool flag = false;
            switch (num)
            {
                case 0u:
                    parts = pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null);
                    enumerator = parts.GetEnumerator();
                    num = 4294967293u;
                    break;
                case 1u:
                case 2u:
                case 3u:
                    break;
                default:
                    return false;
            }
            try
            {
                switch (num)
                {
                }
                while (enumerator.MoveNext())
                {
                    BodyPartRecord part = enumerator.Current;
                    if (pawn.health.hediffSet.HasDirectlyAddedPartFor(part))
                    {
                        this.$current = part;
                        if (!this.$disposing)
							{
                            this.$PC = 1;
                        }
                        flag = true;
                        return true;
                    }
                    if (MedicalRecipesUtility.IsCleanAndDroppable(pawn, part))
                    {
                        this.$current = part;
                        if (!this.$disposing)
							{
                            this.$PC = 2;
                        }
                        flag = true;
                        return true;
                    }
                    if (part != pawn.RaceProps.body.corePart && part.def.canSuggestAmputation && pawn.health.hediffSet.hediffs.Any((Hediff d) => !(d is Hediff_Injury) && d.def.isBad && d.Visible && d.Part == part))
                    {
                        this.$current = part;
                        if (!this.$disposing)
							{
                            this.$PC = 3;
                        }
                        flag = true;
                        return true;
                    }
                }
            }
            finally
            {
                if (!flag)
                {
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
            }
            this.$PC = -1;
            return false;
        }

        BodyPartRecord IEnumerator<BodyPartRecord>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        [DebuggerHidden]
        public void Dispose()
        {
            uint num = (uint)this.$PC;
            this.$disposing = true;
            this.$PC = -1;
            switch (num)
            {
                case 1u:
                case 2u:
                case 3u:
                    try
                    {
                    }
                    finally
                    {
                        if (enumerator != null)
                        {
                            enumerator.Dispose();
                        }
                    }
                    break;
            }
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Verse.BodyPartRecord>.GetEnumerator();
        }

        [DebuggerHidden]
        IEnumerator<BodyPartRecord> IEnumerable<BodyPartRecord>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            Recipe_RemoveBodyPart.< GetPartsToApplyOn > c__Iterator0 < GetPartsToApplyOn > c__Iterator = new Recipe_RemoveBodyPart.< GetPartsToApplyOn > c__Iterator0();

                < GetPartsToApplyOn > c__Iterator.pawn = pawn;
            return < GetPartsToApplyOn > c__Iterator;
        }

        private sealed class <GetPartsToApplyOn>c__AnonStorey1
			{
				internal BodyPartRecord part;

        internal Recipe_RemoveBodyPart.<GetPartsToApplyOn>c__Iterator0<> f__ref$0;

        public <GetPartsToApplyOn>c__AnonStorey1()
        {
        }

        internal bool <>m__0(Hediff d)
        {
            return !(d is Hediff_Injury) && d.def.isBad && d.Visible && d.Part == this.part;
        }
    }*/
}
