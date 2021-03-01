using System.Collections.Generic;
using Verse;
using RimWorld;
using System.Collections;
using System;
using System.Reflection;
using System.Linq;
using HarmonyLib;

namespace Explorite
{
    /*
    public class QuestPart_HyperLinks : QuestPart, IEnumerable<Dialog_InfoCard.Hyperlink>
    {
        private List<Dialog_InfoCard.Hyperlink> hyperlinks = new List<Dialog_InfoCard.Hyperlink>();
        public override IEnumerable<Dialog_InfoCard.Hyperlink> Hyperlinks => hyperlinks;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref hyperlinks, "hyperlinks", LookMode.Deep);
        }

        public IEnumerator<Dialog_InfoCard.Hyperlink> GetEnumerator() => hyperlinks.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => hyperlinks.GetEnumerator();
        public void Add(Dialog_InfoCard.Hyperlink hyperlink)
        {
            hyperlinks.Add(hyperlink);
        }
        public void Add(Dialog_InfoCard infoCard, int statIndex = -1)
        {
            hyperlinks.Add(new Dialog_InfoCard.Hyperlink(infoCard, statIndex));
        }
        public void Add(Def def, int statIndex = -1)
        {
            hyperlinks.Add(new Dialog_InfoCard.Hyperlink(def, statIndex));
        }
        public void Add(Thing thing, int statIndex = -1)
        {
            hyperlinks.Add(new Dialog_InfoCard.Hyperlink(thing, statIndex));
        }
        public void Add(RoyalTitleDef titleDef, Faction faction, int statIndex = -1)
        {
            hyperlinks.Add(new Dialog_InfoCard.Hyperlink(titleDef, faction, statIndex));
        }
    }
    */
    public class QuestPart_HyperLinks : QuestPart, IEnumerable<Dialog_InfoCard.Hyperlink>
    {
        public List<Def> defs = new List<Def>();

        public List<Thing> things = new List<Thing>();

        private IEnumerable<Dialog_InfoCard.Hyperlink> cachedHyperlinks;

        public override IEnumerable<Dialog_InfoCard.Hyperlink> Hyperlinks => cachedHyperlinks ??= GetHyperlinks();

        public IEnumerator<Dialog_InfoCard.Hyperlink> GetEnumerator() => Hyperlinks.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Hyperlinks.GetEnumerator();
        public void Add(Def def)
        {
            defs.Add(def);
        }
        public void Add(Thing thing)
        {
            things.Add(thing);
        }

        private IEnumerable<Dialog_InfoCard.Hyperlink> GetHyperlinks()
        {
            if (defs != null)
            {
                for (int j = 0; j < defs.Count; j++)
                {
                    yield return new Dialog_InfoCard.Hyperlink(defs[j]);
                }
            }
            if (things == null)
            {
                yield break;
            }
            for (int j = 0; j < things.Count; j++)
            {
                if (things[j] is Pawn pawn && pawn.royalty != null && pawn.royalty.AllTitlesForReading.Any())
                {
                    RoyalTitle mostSeniorTitle = pawn.royalty.MostSeniorTitle;
                    if (mostSeniorTitle != null)
                    {
                        yield return new Dialog_InfoCard.Hyperlink(mostSeniorTitle.def, mostSeniorTitle.faction);
                    }
                }
            }
        }

        public struct DefWithType : IExposable
        {
            public Def def;
            public DefWithType(Def def = null)
            {
                this.def = def;
            }

            public static implicit operator DefWithType(Def x)
            {
                return new DefWithType(x);
            }
            public static implicit operator Def(DefWithType x)
            {
                return x.def;
            }
            public static bool operator ==(DefWithType v1, DefWithType v2)
            {
                return v1.def == v2.def;
            }
            public static bool operator !=(DefWithType v1, DefWithType v2)
            {
                return v1.def != v2.def;
            }
            public override bool Equals(object obj)
            {
                return def.Equals(obj);
            }
            public override int GetHashCode()
            {
                return def.GetHashCode();
            }
            public override string ToString()
            {
                return def.ToString();
            }

            public static Def GetDefByType(string defName, Type defClass)
            {
                try
                {
                    /*
                    return typeof(DefDatabase<>).MakeGenericType(defClass)
                        .GetMethod("GetNamed", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                        .Invoke(
                            obj: null,
                            parameters: new object[] { defName, true }
                        ) as Def;
                    */
                    return AccessTools.Method(typeof(DefDatabase<>).MakeGenericType(defClass), "GetNamed").Invoke(
                            obj: null,
                            parameters: new object[] { defName, true }
                        ) as Def;
                }
                catch (Exception)
                {
                    throw;
                    //return null;
                }
            }

            public void ExposeData()
            {
                string defName = string.Empty;
                Type defClass = typeof(Def);
                if (def != null)
                {
                    defName = def.defName;
                    defClass = def.GetType();

                    for (Type type = defClass.BaseType; type != typeof(Def) && type != typeof(object) && type != null; type = type.BaseType)
                    {
                        defClass = type;
                    }
                }

                Scribe_Values.Look(ref defName, "defName", defaultValue: string.Empty, forceSave: true);
                Scribe_Values.Look(ref defClass, "defClass", defaultValue: typeof(Def), forceSave: true);

                //Log.Message($"[Explorite]Exposing defClass {defClass}");
                def = GetDefByType(defName, defClass);
                //Log.Message($"[Explorite]Exposing def {def.defName}");
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            List<DefWithType> defsWithType = defs.Select(d => (DefWithType)d).ToList();
            Scribe_Collections.Look(ref defsWithType, "defs", LookMode.Deep);
            defs = defsWithType.Select(d => (Def)d).ToList();

            Scribe_Collections.Look(ref things, "things", LookMode.Reference);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (defs == null)
                {
                    defs = new List<Def>();
                }
                defs.RemoveAll((Def x) => x == null);
                if (things == null)
                {
                    things = new List<Thing>();
                }
                things.RemoveAll((Thing x) => x == null);
            }
        }

        public override void ReplacePawnReferences(Pawn replace, Pawn with)
        {
            things.Replace(replace, with);
        }
    }
}
