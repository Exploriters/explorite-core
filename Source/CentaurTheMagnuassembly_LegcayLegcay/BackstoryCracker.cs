using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RimWorld;
using Verse;
using static CentaurTheMagnuassembly.RimCentaurCore;

namespace CentaurTheMagnuassembly
{
    [StaticConstructorOnStartup]
    internal static class BackstoryCracker
    {
        public static string targetTag = "Civil";
        public static string appTagPrefix = "CentaurAdulthood";
        public static BackstorySlot backstorySlot = BackstorySlot.Adulthood;
        public static Backstory CentaurCivilRetro = BackstoryDatabase.allBackstories.TryGetValue("CentaurCivilRetro");
        static BackstoryCracker()
        {
            THE_ULTRA_FLOOD();
            //TestforIncorrectChildhood();
            //AddExtraPrefixViaTag();
            //ExportBackstoriesToLog();
        }
        public static void THE_ULTRA_FLOOD()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Log.Message("[Magnuassembly]Backstory flooder triggered.");

            uint solvedBackstoryCount = 0;

            string[] BStheFLOOD = {
                /*"Reporter51",
                "InformationBroker77",
                "CoreWorldJeweler59",
                "RocketEngineer31",
                "CombatEngineer4",
                "EnergyResearcher67",
                "ExiledResearcher68",
                "SpaceNavyDoctor72",
                "BountyHunter41",
                "ParticlePhysicist78",
                "StilettoAssassin50",
                "Aromatherapist80",
                "WarshipCaptain67",
                "RunawayDancer35",
                "CasketBuilder76",
                "DeepSpaceSurveyor87",
                "RenownedProfessor51",*/
                "ColonySettler53",
                "UrbworldEntrepreneur14",
                "DeepSpaceMiner3",
                "NavyScientist46",
                "MilitaryCommissar49",
                "MedievalDoctor40",
                "Inventor6",
                "Teacher20",
                "GlitterworldSurgeon15",
                "Bartender55",
                "CaveworldIlluminator23",
                "Herbalist54",
                "CivilServant2",
                "StarshipJanitor33",
                "Paramedic45",
                "Geologist66",
                "Drifter67",
                "MachineCollector55",
                "Sheriff52",
                "Reporter51",
                "EngineeredPilot9",
                "StateEngineer18",
                "SpaceHunter67",
                "Mercenary40",
                "InformationBroker77",
                "MedicalScientist25",
                "CoreWorldJeweler59",
                "SpaceshipSalesman6",
                "RocketEngineer31",
                "CombatEngineer4",
                "Guardian55",
                "EnergyResearcher67",
                "LoneTraveler95",
                "HiredAssassin2",
                "Engineer47",
                "NeuroScientist84",
                "HedgeFundManager85",
                "ExiledResearcher68",
                "SpaceNavyDoctor72",
                "SanitationCaptain28",
                "BountyHunter41",
                "OrbitalReservist22",
                "ParticlePhysicist78",
                "StilettoAssassin50",
                "PoliticalActivist61",
                "AddictionCounsel60",
                "BattleMechanic27",
                "Chemist73",
                "Aromatherapist80",
                "PirateDoctor0",
                "BloodyDentist9",
                "WarshipCaptain67",
                "RunawayDancer35",
                "ArmySergeant1",
                "StellarPirate99",
                "LanguageAnalyst27",
                "CasketBuilder76",
                "Fugitive32",
                "DeepSpaceSurveyor87",
                "RenownedProfessor51",
            };
            
            string solved = "";


            //Backstory pending = null;
            //Backstory patchedbackstory = null;

            foreach (string str in BStheFLOOD)
            {
                BackstoryDatabase.allBackstories.TryGetValue(str).spawnCategories.Add("CentaurAdulthoodCivil");
                /*pending = BackstoryDatabase.allBackstories.TryGetValue(str);
                if (pending != null)
                {
                    patchedbackstory = new Backstory();
                    patchedbackstory.identifier = "Centaur" + pending.identifier;
                    patchedbackstory.descTranslated = pending.descTranslated;
                    patchedbackstory.titleShortFemaleTranslated = pending.titleShortFemaleTranslated;
                    patchedbackstory.titleShortTranslated = pending.titleShortTranslated;
                    patchedbackstory.titleTranslated = pending.titleTranslated;
                    patchedbackstory.untranslatedDesc = pending.untranslatedDesc;
                    patchedbackstory.untranslatedTitleShortFemale = pending.untranslatedTitleShortFemale;
                    patchedbackstory.untranslatedTitleShort = pending.untranslatedTitleShort;
                    patchedbackstory.untranslatedTitleFemale = pending.untranslatedTitleFemale;
                    patchedbackstory.untranslatedTitle = pending.untranslatedTitle;
                    patchedbackstory.shuffleable = pending.shuffleable;
                    patchedbackstory.disallowedTraits = pending.disallowedTraits;
                    patchedbackstory.titleFemaleTranslated = pending.titleFemaleTranslated;
                    patchedbackstory.spawnCategories = pending.spawnCategories;
                    patchedbackstory.requiredWorkTags = pending.requiredWorkTags;
                    patchedbackstory.workDisables = pending.workDisables;
                    patchedbackstory.skillGainsResolved = pending.skillGainsResolved;
                    patchedbackstory.baseDesc = pending.baseDesc;
                    patchedbackstory.titleShortFemale = pending.titleShortFemale;
                    patchedbackstory.titleShort = pending.titleShort;
                    patchedbackstory.forcedTraits = pending.forcedTraits;
                    patchedbackstory.titleFemale = pending.titleFemale;
                    patchedbackstory.title=pending.title;
                    patchedbackstory.slot = pending.slot;

                    BackstoryDatabase.AddBackstory(patchedbackstory);
                    patchedbackstory = null;
                }*/


                solved += $"{str}, ";
                solvedBackstoryCount++;
            }
            Log.Message($"[Magnuassembly]Solved CentaurAdulthoodCivil: {(solved+"&").Replace(", &",".")}");
            stopwatch.Stop();
            Log.Message($"[Magnuassembly]Backstory flood complete, solved total {solvedBackstoryCount} backstories, in {stopwatch.ElapsedMilliseconds}ms.");

        }

        static bool inctested = false;
        public static void TestforIncorrectChildhood()
        {
            if (inctested)
                return;
            inctested = true;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Log.Message("[Magnuassembly]Backstory tester triggered.");
            uint solvedBackstoryCount = 0;
            string solved = "";
            Pawn tempPawn;
            List<Backstory> backstories = new List<Backstory>();
            for (int i = 0 ; i < 2000; i++)
            {
                tempPawn = PawnGenerator.GeneratePawn(CentaurColonistDef, Faction.OfPlayer);
                if (tempPawn != null && tempPawn.story.childhood != CentaurCivilRetro && !backstories.Contains(tempPawn.story.adulthood))
                {
                    backstories.Add(tempPawn.story.adulthood);
                    solved += ExportBackstoryAsFormattedString(tempPawn.story.adulthood);
                    Log.Message($"[Magnuassembly]Detected : {ExportBackstoryAsFormattedString(tempPawn.story.adulthood)}");
                    solvedBackstoryCount++;
                }
            }

            Log.Message($"[Magnuassembly]Problem : {(solved+"&").Replace(", &",".")}");
            stopwatch.Stop();
            Log.Message($"[Magnuassembly]Backstory testing complete, detected total {solvedBackstoryCount} backstories, result length {solved.Length}, in {stopwatch.ElapsedMilliseconds}ms.");
        }
        public static string ExportBackstoryAsFormattedString(Backstory Bs)
        {
            return $"identifier:{Bs.identifier}, title:{Bs.titleShort}/{Bs.title}, Desc:\"{Bs.baseDesc}\".\n";
        }
        public static void AddExtraPrefixViaTag()
        {
            uint solvedBackstoryCount = 0;
            uint solvedTagCount = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Log.Message("[Magnuassembly]Backstory cracker triggered for \"" + targetTag + "\" in \"" + backstorySlot + "\" by \"" + appTagPrefix + "\".");

            Dictionary<string, Backstory> DiBs = BackstoryDatabase.allBackstories;
            Log.Message("[Magnuassembly]Scanning backstory total : " + DiBs.Count);
            foreach (KeyValuePair<string, Backstory> Bs in DiBs)
            {
                if (
                    Bs.Value.slot == backstorySlot
                    && Bs.Value.DisabledWorkGivers.Count() == 0
                    && Bs.Value.DisabledWorkTypes.Count() == 0
                    && Bs.Value.workDisables == 0
                    && Bs.Value.spawnCategories.Contains("Civil")
                    )
                {
                    //Log.Message("[Magnuassembly]Scanning backstory in DiBs[" + Bs.Key + "]" + Bs.Value.identifier + " : " + Bs.Value.title);
                    if (Bs.Value.identifier.StartsWith(appTagPrefix))
                    {
                        Bs.Value.requiredWorkTags = (WorkTags)65535;
                    }
                    List<string> posttag = new List<string>();
                    foreach (string tag in Bs.Value.spawnCategories)
                    {
                        if (!Bs.Value.identifier.StartsWith(appTagPrefix))
                        {
                            posttag.Add(appTagPrefix + tag);
                        }
                    }
                    foreach (string tag in posttag)
                    {
                        //Log.Message("[Magnuassembly]Adding \"" + tag + "\" to backstory " + Bs.Value.identifier + " : " + Bs.Value.title);
                        Bs.Value.spawnCategories.Add(tag);
                        solvedTagCount++;
                    }
                    solvedBackstoryCount++;
                }
            }
            stopwatch.Stop();
            Log.Message("[Magnuassembly]Backstory crack complete, solved total " + solvedTagCount + " tags in " + solvedBackstoryCount + " backstories, in " + stopwatch.ElapsedMilliseconds + "ms.");
        }
        public static void ExportBackstoriesToLog()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Dictionary<string, Backstory> DiBs = BackstoryDatabase.allBackstories;
            Log.Message("[Magnuassembly]Scanning backstory total : " + DiBs.Count);
            uint solvedBackstoryCount = 0;

            string result = "[Magnuassembly]Export backstories:\n";

            foreach (KeyValuePair<string, Backstory> Bs in DiBs)
            {
                if (Bs.Value.spawnCategories.Contains("CentaurAdulthoodCivil"))
                {
                    result += ExportBackstoryAsFormattedString(Bs.Value);
                    solvedBackstoryCount++;
                }
            }
            stopwatch.Stop();
            Log.Message(result);
            Log.Message($"[Magnuassembly]Backstory export complete, solved total {solvedBackstoryCount} backstories, in {stopwatch.ElapsedMilliseconds}ms.");

        }
    }
    /*public class BackstoryPrefixDef : Def
    {
        public string targetTag = "Civil";
        public string appTagPrefix = "Centaur";
        public BackstorySlot backstorySlot = BackstorySlot.Adulthood;
    }*/

}
