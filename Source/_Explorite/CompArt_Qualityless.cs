/**
 * 没有品质的艺术品的Comp。
 * 
 * --siiftun1857
 */
using Verse;
using RimWorld;

namespace Explorite
{
    /**
     * <summary>
     * 没有品质的艺术品Comp类。
     * </summary>
     */
    public class CompArt_Qualityless : ThingComp
    {
        private string authorNameInt;

        private string titleInt;

        private TaleReference taleRef;

        public CompArt_Qualityless()
        {
        }

        public string AuthorName
        {
            get
            {
                if (authorNameInt.NullOrEmpty())
                {
                    return "UnknownLower".Translate().CapitalizeFirst();
                }
                return authorNameInt;
            }
        }

        public string Title
        {
            get
            {
                if (titleInt.NullOrEmpty())
                {
                    Log.Error("CompArt got title but it wasn't configured.", false);
                    titleInt = "Error";
                }
                return titleInt;
            }
        }

        public TaleReference TaleRef => taleRef;

        public bool CanShowArt
        {
            get
            {
                if (Props.mustBeFullGrave)
                {
                    if (!(parent is Building_Grave building_Grave) || !building_Grave.HasCorpse)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public bool Active => taleRef != null;

        public CompProperties_Art Props => (CompProperties_Art)props;

        public void InitializeArt(ArtGenerationContext source)
        {
            InitializeArt(null, source);
        }

        public void InitializeArt(Thing relatedThing)
        {
            InitializeArt(relatedThing, ArtGenerationContext.Colony);
        }

        private void InitializeArt(Thing relatedThing, ArtGenerationContext source)
        {
            if (taleRef != null)
            {
                taleRef.ReferenceDestroyed();
                taleRef = null;
            }
            if (CanShowArt)
            {
                if (Current.ProgramState == ProgramState.Playing)
                {
                    if (relatedThing != null)
                    {
                        taleRef = Find.TaleManager.GetRandomTaleReferenceForArtConcerning(relatedThing);
                    }
                    else
                    {
                        taleRef = Find.TaleManager.GetRandomTaleReferenceForArt(source);
                    }
                }
                else
                {
                    taleRef = TaleReference.Taleless;
                }
                titleInt = GenerateTitle();
            }
            else
            {
                titleInt = null;
                taleRef = null;
            }
        }

        public void JustCreatedBy(Pawn pawn)
        {
            if (CanShowArt)
            {
                authorNameInt = pawn.Name.ToStringFull;
            }
        }

        public void Clear()
        {
            authorNameInt = null;
            titleInt = null;
            if (taleRef != null)
            {
                taleRef.ReferenceDestroyed();
                taleRef = null;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref authorNameInt, "authorName", null, false);
            Scribe_Values.Look(ref titleInt, "title", null, false);
            Scribe_Deep.Look(ref taleRef, "taleRef", new object[0]);
        }

        public override string CompInspectStringExtra()
        {
            if (!Active)
            {
                return null;
            }
            string text = "Author".Translate() + ": " + AuthorName;
            string text2 = text;
            return string.Concat(new string[]
            {
                text2,
                "\n",
                "Title".Translate(),
                ": ",
                Title
            });
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            if (taleRef != null)
            {
                taleRef.ReferenceDestroyed();
                taleRef = null;
            }
        }

        public override string GetDescriptionPart()
        {
            if (!Active)
            {
                return null;
            }
            string str = string.Empty;
            str += Title;
            str += "\n\n";
            str += GenerateImageDescription();
            str += "\n\n";
            return str + "Author".Translate() + ": " + AuthorName;
        }

        public override bool AllowStackWith(Thing other)
        {
            return !Active;
        }

        public string GenerateImageDescription()
        {
            if (taleRef == null)
            {
                Log.Error("Did CompArt.GenerateImageDescription without initializing art: " + parent, false);
                InitializeArt(ArtGenerationContext.Outsider);
            }
            return taleRef.GenerateText(TextGenerationPurpose.ArtDescription, Props.descriptionMaker);
        }

        private string GenerateTitle()
        {
            if (taleRef == null)
            {
                Log.Error("Did CompArt.GenerateTitle without initializing art: " + parent, false);
                InitializeArt(ArtGenerationContext.Outsider);
            }
            return GenText.CapitalizeAsTitle(taleRef.GenerateText(TextGenerationPurpose.ArtName, Props.nameMaker));
        }
    }
}
