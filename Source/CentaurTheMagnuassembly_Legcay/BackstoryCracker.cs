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
        public static string appTagPrefix = "Centaur";
        public static BackstorySlot backstorySlot = BackstorySlot.Adulthood;
        static BackstoryCracker()
        {
            //CentaurCivilRetro.spawnCategories.Add("CentaurCivil");
            CentaurCivilRetro.shuffleable = true;
            CentaurCivilMayinas.shuffleable = false;
            //AddExtraPrefixViaTag();
            //ExportBackstoriesToLog();
            //TestforIncorrectChildhood();
            THE_ULTRA_FLOOD();
        }
        public static void THE_ULTRA_FLOOD()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Log.Message("[Magnuassembly]Backstory flooder triggered.");

            uint solvedBackstoryCount = 0;
            /*string[] BStheFLOOD = {
                / *"Reporter51",
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
                "RenownedProfessor51",* /
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
            };*/

            string[] BStheFLOOD = {
                "CorporateDrone10",//, title:奴工/公司奴工, Desc:"[PAWN_nameDef]和很多其他人一样在大型办公楼中过着996的生活。\n\n这种工作充斥着没完没了的重复劳动、毫无意义的各种会议，当然还有绩效考核。".
                "CorporateManager76",//, title:经理/公司经理, Desc:"[PAWN_nameDef]在大型开放式办公室工作，以便于监督[PAWN_possessive]都在认真工作。\n\n在[PAWN_nameDef]细致的安排下，所有人都能出色地完成工作任务。".
                "Mailman2",//, title:邮递员/邮递员, Desc:"[PAWN_nameDef]在[PAWN_possessive]分配路线上收取邮件。有时候[PAWN_pronoun]甚至被派往时髦的公司办公室或者大厦运送特别包裹。\n\n[PAWN_nameDef]非常擅长于躲避愤怒的狗。".
                "Assembler69",//, title:装配工/流水线组装工人, Desc:"[PAWN_nameDef]在工厂流水线上工作，在机器人的帮助下将产品部件组装起来".
                "Builder96",//, title:建筑工/建筑工人, Desc:"[PAWN_nameDef]在建筑工地干活，为[PAWN_possessive]的老板建造他所要求的一切建筑。".
                "CropFarmer17",//, title:农场主/农场主, Desc:"[PAWN_nameDef]经营了一处谷物农场。[PAWN_pronoun]分析了土壤材质、农业设备、天气模式和价格趋势，以优化大规模的农作物种植和收获。".
                "LivestockFarmer8",//, title:农场主/养殖场主, Desc:"[PAWN_nameDef]运营着一家养殖场，[PAWN_pronoun]为了优化自己的利益而学习分析动物的基因，食物种类，接生和屠宰的方式，还有市场价格的波动。".
                "NavyScientist52",//, title:科学家/舰队科学家, Desc:"星际战争依靠科技取胜，帝国舰队在尖端科研上永远处于领先地位。更好的是，他们能最先拿到从外太空搜寻到的超凡世界遗物。\n\n[PAWN_nameDef]曾在舰队实验室工作。".
                "GlitterworldSurgeon15",//, title:外科医生/闪耀世界外科医生, Desc:"[PAWN_nameDef]在一个没有疾病与人间疾苦的闪耀世界当医生.\n\n[PAWN_possessive]绝大多数的工作是精心安排的、富有创造力的整容手术。[PAWN_pronoun]在人体仿生方面有很好地造诣，但从未治疗过癌症或是移除一颗子弹。 ".
                "MedievalDoctor40",//, title:庸医/中世纪医生, Desc:"[PAWN_nameDef]是中世纪星球上的一名医生。[PAWN_pronoun]坚定地认为大多数小病小痛都可以用一个小小的放血疗法来治愈。\n\n[PAWN_pronoun]也是一名麻醉专家，开发出一种特殊的麻醉方式——给脑袋狠狠一击。 ".
                "Herbalist54",//, title:草药师/草药师, Desc:"[PAWN_nameDef]居住在村庄附近的森林中。虽然村民大多都很怕[PAWN_objective]，但是生病时都会向[PAWN_objective]购买[PAWN_pronoun]用种在园子中的药草制成的药膏药剂。[PAWN_pronoun]也乐意帮助村民们——当然，不是免费的。".
                "Policeman45",//, title:警官/警官, Desc:"[PAWN_nameDef]作为一名市警察部队的警员维持治安。\n\n[PAWN_pronoun]接受了治安管理、身体控制、射击等警察技能的培训。".
                "Inventor6",//, title:发明家/发明家, Desc:"[PAWN_nameDef]是[PAWN_possessive]母星上一位比较成功的发明家。 [PAWN_pronoun]开发过一些小型技术。".
                "Teacher20",//, title:教师/受欢迎的教师, Desc:"[PAWN_nameDef]毕业于文学系，并在一所公立学校担任教师。[PAWN_pronoun]非常博学并深受学生们的喜爱。".
                "CivilServant2",//, title:官员/政府文员, Desc:"[PAWN_nameDef]在一个濒临崩溃的政府官僚体系中担任低级行政人员。[PAWN_pronoun]大多数时候都在家写复杂的文件和玩玩办公室政治。".
                "Bartender62",//, title:酒保/太空站的调酒师, Desc:"[PAWN_nameDef]在一个老旧太空站上当酒保。这份工作需要一份调酒，一份社交，以及一份头部猛击。".
                "Evangelist39",//, title:传教士/福音传教士, Desc:"在21岁生日时，[PAWN_nameDef]经历了一次信仰的觉醒。[PAWN_pronoun]决定用余生传播神的福音，神的美丽文化以及非凡的医学传统。".
                "Paramedic45",//, title:军医/急救医生, Desc:"[PAWN_nameDef]的工作是快速响应紧急抢救。[PAWN_pronoun]经常需要在仅有有限的医疗物资的情况下处理严重的伤情。\n\n多年以来[PAWN_pronoun]处理了不计其数的枪伤，以至于如今一看到枪就会感到不适。".
                //"LowWageWorker7",//, title:工人/低薪工人, Desc:"[PAWN_nameDef]接了很多种兼职来支撑[PAWN_possessive]家庭，因此锻炼了一套基本的动手能力。".
                "Geologist66",//, title:地质学家/地质学家, Desc:"[PAWN_nameDef]与矿工和洞穴挖掘机一同工作，识别岩石种类和自然形态。\n\n多年的地下工作也让[PAWN_pronoun]获得了一些钻机等机械设备的维修经验。".
                "Veterinarian99",//, title:兽医/素食主义兽医, Desc:"[PAWN_nameDef]靠治疗生病和受伤的动物为生。动物们饱受折磨的样子影响了[PAWN_pronoun]对吃肉的看法。多年来，[PAWN_nameDef]成为一个素食主义者，抵制母星偏好肉类烹饪的传统。".
                "Drifter67",//, title:流浪者/流浪的小说家, Desc:"[PAWN_nameDef]从未想过[PAWN_possessive]人生需要做什么。[PAWN_pronoun]经常旅行，打零工，随遇而安。\n\n[PAWN_nameDef]也偶尔会拿起笔写一部[PAWN_pronoun]认为一定会畅销的小说，只要能找到一个感兴趣的出版商。".
                "MachineCollector55",//, title:收藏家/机器收藏家, Desc:"[PAWN_nameDef]对老旧的机器和神秘的技术部件非常着迷。[PAWN_pronoun]总是竭尽所能的得到它们，并且喜欢拆开来看它们是怎么运行的。\n\n[PAWN_nameDef]总是喋喋不休的对人讲述[PAWN_possessive]收藏品，久而久之，[PAWN_pronoun]周围的人已经充耳不闻了。".
                "Sheriff52",//, title:城管/城镇治安官, Desc:"[PAWN_nameDef]是一名偏远工业星球城镇上的执法者。[PAWN_pronoun]因其傲慢的态度和卑鄙的性格而出名。除非是为了金钱、酒精或是复仇，[PAWN_nameDef]连一根手指都懒得动。".
                "UrbworldEntrepreneur14",//, title:企业家/下层世界企业家, Desc:"在都市星球上，大部分人都在被压榨着。但是总要有人来管理企业。\n\n\n[PAWN_nameDef]掌握了各类贸易技巧——从贿赂到分析。[PAWN_pronoun]就是一部活生生的社交机器。".
                "CaveworldIlluminator23",//, title:启发者/洞穴星球启发者, Desc:"在隧道居民之中，那些和[PAWN_nameDef]有着同样强大视力的人被尊为圣人。[PAWN_pronoun]会引导众人，用荧光真菌来标记挖掘地点以及警示别人隐藏的危险。".
                "ColonySettler53",//, title:开拓者/殖民地开拓者, Desc:"[PAWN_nameDef]是一名新殖民地的开拓者。\n\n开荒的生活需要一个基础工作方面的多面手。".
                "DeepSpaceMiner3",//, title:矿工/深空矿工, Desc:"[PAWN_nameDef]每天重复干着又脏又累的力气活，使用深空钻井从小行星里开采金属元素。[PAWN_pronoun]每天都要用[PAWN_possessive]行业技能——包括在酒吧打架的时候。".
                "StarshipJanitor33",//, title:维护工/星际飞船维护工, Desc:"当其他乘客在低温休眠舱度过星系间的数年旅途时，[PAWN_nameDef]会被定期唤醒，进行例行检查，检查导航系统并为机器人上油。".
                "SpaceMarine16",//, title:陆战队员/帝国星际陆战队员, Desc:"[PAWN_nameDef]是帝国海军的一名战士。[PAWN_possessive]任务是攻入敌人星舰，射杀所有乘员，然后完整地捕获舰船。[PAWN_pronoun]是这方面的专家。".
            };

            string solved = "";


            //Backstory pending = null;
            //Backstory patchedbackstory = null;

            foreach (string str in BStheFLOOD)
            {
                BackstoryDatabase.allBackstories?.TryGetValue(str)?.spawnCategories?.Add("CentaurCivil");
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
            return $"identifier:{Bs.identifier}, title:{Bs.titleShort}/{Bs.title}, Desc:\"{Bs.baseDesc.Replace("\n", "\\n")}\".\n";
            //return $"identifier:{Bs.identifier}, title:{Bs.titleShort}/{Bs.title}, title:{Bs.spawnCategories}, Desc:\"{Bs.baseDesc.Replace("\n","\\n")}\".\n";
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
                    && Bs.Value.shuffleable == true
                    //&& Bs.Value.workDisables == 0
                    && (
                        Bs.Value.spawnCategories.Contains("Civil")
                        || Bs.Value.spawnCategories.Contains("Offworld")
                        //|| Bs.Value.spawnCategories.Contains("Outlander")
                        //|| Bs.Value.spawnCategories.Contains("Outsider")
                    )
                    )
                {
                    //Log.Message("[Magnuassembly]Scanning backstory in DiBs[" + Bs.Key + "]" + Bs.Value.identifier + " : " + Bs.Value.title);
                    /*if (Bs.Value.identifier.StartsWith(appTagPrefix))
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
                    }*/
                    Bs.Value.spawnCategories.Add("CentaurCivil");
                    solvedBackstoryCount++;
                }
            }
            stopwatch.Stop();
            //Log.Message("[Magnuassembly]Backstory crack complete, solved total " + solvedTagCount + " tags in " + solvedBackstoryCount + " backstories, in " + stopwatch.ElapsedMilliseconds + "ms.");
            Log.Message("[Magnuassembly]Backstory crack complete, solved total " + solvedBackstoryCount + " backstories, in " + stopwatch.ElapsedMilliseconds + "ms.");
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
                if (Bs.Value.spawnCategories.Contains("CentaurCivil"))
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
