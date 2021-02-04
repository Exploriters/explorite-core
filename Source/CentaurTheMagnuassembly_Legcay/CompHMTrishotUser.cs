using static CentaurTheMagnuassembly.RimCentaurCore;
using Verse;
using RimWorld;
//using AbilityUser;

namespace CentaurTheMagnuassembly
{
    /*public class CompHMTrishotUser : CompAbilityUser
    {
        //A simple check boolean to make sure we don't give abilities twice.
        //Starts false because we haven't given abilities yet.
        private bool gaveAbilities = false;

        private bool firstTick = false;

        /// <summary>
        /// To be psionic, the character must have a psionic brain.
        /// </summary>
        private bool IsHMInstalled
        {
            get
            {
                if (Pawn?.health?.hediffSet == null) return false;
                if (Pawn.health.hediffSet.HasHediff(HyperManipulatorHediffDef)) return true;
                return false;
            }
        }

        /// <summary>
        /// Gives this component class to the character if they are psionic.
        /// </summary>
        public override bool TryTransformPawn() => IsHMInstalled;

        /// <summary>
        /// After getting the component class, checks 200 ticks
        /// after the game starts.
        /// If the character is psionic, give them the abilities in
        /// the function PostInitalizeTick()
        /// </summary>
        public override void CompTick()
        {
            if (Pawn?.Spawned != true) return;
            if (Find.TickManager.TicksGame > 200)
            {
                if (IsHMInstalled)
                {
                    if (!firstTick)
                    {
                        PostInitializeTick();
                    }
                    base.CompTick();
                }
            }
        }

        /// <summary>
        /// Adds the ability "Psionic Blast" to the character.
        /// Sets gaveAbilities to true, because we gave the abilties.
        /// </summary>
        private void PostInitializeTick()
        {
            if (Pawn?.Spawned != true) return;
            if (Pawn?.story == null) return;
            firstTick = true;
            this.Initialize();
            if (!gaveAbilities)
            {
                gaveAbilities = true;
                this.AddPawnAbility(AbilityTrishot_TrishotDef);
                this.AddPawnAbility(AbilityTrishot_IcoshotDef);
                this.AddPawnAbility(AbilityTrishot_OneshotDef);
            }
        }

        //Use this area to store any extra data you want to load
        //with your component.
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this.gaveAbilities, "gaveAbilities", false);
        }
    }*/
}
