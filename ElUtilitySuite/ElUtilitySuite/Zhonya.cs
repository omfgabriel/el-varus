using System;
using System.Collections.Generic;

namespace ElUtilitySuite
{
    public class Zhonya
    {
        public static List<ZhonyaSpell> Spells { get; set; }

        public static void Init()
        {
            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "aatrox",
                SDataName = "aatroxq",
                MissileName = "",
                Delay = 250,
                MissileSpeed = 2000,
                CastRange = 650f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "ahri",
                SDataName = "ahriseduce",
                MissileName = "ahriseducemissile",
                Delay = 250,
                MissileSpeed = 1550,
                CastRange = 975f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "alistar",
                SDataName = "pulverize",
                MissileName = "",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 365f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "amumu",
                SDataName = "bandagetoss",
                MissileName = "sadmummybandagetoss",
                Delay = 250,
                MissileSpeed = 2000,
                CastRange = 1100f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "amumu",
                SDataName = "curseofthesadmummy",
                MissileName = "",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 560f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "annie",
                SDataName = "infernalguardian",
                MissileName = "",
                Delay = 0,
                MissileSpeed = int.MaxValue,
                CastRange = 900f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "ashe",
                SDataName = "enchantedcrystalarrow",
                MissileName = "enchantedcrystalarrow",
                Delay = 250,
                MissileSpeed = 1600,
                CastRange = 20000f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "azir",
                SDataName = "azirr",
                MissileName = "",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 475f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "blitzcrank",
                SDataName = "rocketgrabmissile",
                MissileName = "",
                Delay = 250,
                MissileSpeed = 1800,
                CastRange = 1050f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "ziggs",
                SDataName = "ziggsq",
                MissileName = "ziggsqspell",
                Delay = 250,
                MissileSpeed = 1750,
                CastRange = 850f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "ziggs",
                SDataName = "ziggsr",
                MissileName = "ziggsr",
                Delay = 1800,
                MissileSpeed = 1750,
                CastRange = 2250f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "braum",
                SDataName = "braumq",
                MissileName = "braumqmissile",
                Delay = 250,
                MissileSpeed = 1200,
                CastRange = 1100f
            });


            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "braum",
                SDataName = "braumqmissle",
                MissileName = "",
                Delay = 250,
                MissileSpeed = 1200,
                CastRange = 1100f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "braum",
                SDataName = "braumrwrapper",
                MissileName = "braumrmissile",
                Delay = 250,
                MissileSpeed = 1200,
                CastRange = 1250f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "cassiopeia",
                SDataName = "cassiopeiapetrifyinggaze",
                MissileName = "cassiopeiapetrifyinggaze",
                Delay = 350,
                MissileSpeed = int.MaxValue,
                CastRange = 875f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "chogath",
                SDataName = "rupture",
                MissileName = "rupture",
                Delay = 1000,
                MissileSpeed = int.MaxValue,
                CastRange = 950f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "darius",
                SDataName = "dariusaxegrabcone",
                MissileName = "dariusaxegrabcone",
                Delay = 150,
                MissileSpeed = int.MaxValue,
                CastRange = 555f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "diana",
                SDataName = "dianavortex",
                MissileName = "",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 350f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "elise",
                SDataName = "elisehumane",
                MissileName = "elisehumane",
                Delay = 250,
                MissileSpeed = 1600,
                CastRange = 1075f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "evelynn",
                SDataName = "evelynnr",
                MissileName = "evelynnr",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 900f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "fiddlesticks",
                SDataName = "terrify",
                MissileName = "",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 575f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "galio",
                SDataName = "galioidolofdurand",
                MissileName = "",
                Delay = 0,
                MissileSpeed = int.MaxValue,
                CastRange = 600f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "garen",
                SDataName = "garenqattack",
                MissileName = "",
                Delay = 0,
                MissileSpeed = int.MaxValue,
                CastRange = 350f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "gnar",
                SDataName = "gnarult",
                MissileName = "",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 600f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "gragas",
                SDataName = "gragase",
                MissileName = "gragase",
                Delay = 200,
                MissileSpeed = 1200,
                CastRange = 600f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "gragas",
                SDataName = "gragasr",
                MissileName = "gragasrboom",
                Delay = 250,
                MissileSpeed = 1750,
                CastRange = 1150f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "hecarim",
                SDataName = "hecarimult",
                MissileName = "",
                Delay = 50,
                MissileSpeed = 1200,
                CastRange = 1350f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "irelia",
                SDataName = "ireliaequilibriumstrike",
                MissileName = "",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 450f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "janna",
                SDataName = "reapthewhirlwind",
                MissileName = "",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 725f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "jarvaniv",
                SDataName = "jarvanivdragonstrike",
                MissileName = "",
                Delay = 250,
                MissileSpeed = 2000,
                CastRange = 700f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "jayce",
                SDataName = "jaycetotheskies",
                MissileName = "",
                Delay = 450,
                MissileSpeed = int.MaxValue,
                CastRange = 600f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "jayce",
                SDataName = "jayceshockblast",
                MissileName = "jayceshockblastmis",
                Delay = 250,
                MissileSpeed = 2350,
                CastRange = 1570f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "karma",
                SDataName = "karmaq",
                MissileName = "karmaqmissile",
                Delay = 250,
                MissileSpeed = 1800,
                CastRange = 1050f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "kassadin",
                SDataName = "nulllance",
                MissileName = "",
                Delay = 250,
                MissileSpeed = 1900,
                CastRange = 650f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "kassadin",
                SDataName = "forcepulse",
                MissileName = "",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 700f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "leesin",
                SDataName = "blindmonkrkick",
                MissileName = "",
                Delay = 500,
                MissileSpeed = int.MaxValue,
                CastRange = 375f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "leona",
                SDataName = "leonashieldofdaybreak",
                MissileName = "",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 215f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "leona",
                SDataName = "leonasolarflare",
                MissileName = "leonasolarflare",
                Delay = 1200,
                MissileSpeed = int.MaxValue,
                CastRange = 1200f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "lissandra",
                SDataName = "lissandraw",
                MissileName = "",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 450f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "lissandra",
                SDataName = "lissandrar",
                MissileName = "",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 550f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "lux",
                SDataName = "luxlightbinding",
                MissileName = "luxlightbindingmis",
                Delay = 250,
                MissileSpeed = 1200,
                CastRange = 1300f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "malphite",
                SDataName = "ufslash",
                MissileName = "ufslash",
                Delay = 250,
                MissileSpeed = 2200,
                CastRange = 1000f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "malzahar",
                SDataName = "alzaharnethergrasp",
                MissileName = "",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 700f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "maokai",
                SDataName = "maokaiunstablegrowth",
                MissileName = "",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 650f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "monkeyking",
                SDataName = "monkeykingspintowin",
                MissileName = "",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 450f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "morgana",
                SDataName = "darkbindingmissile",
                MissileName = "darkbindingmissile",
                Delay = 250,
                MissileSpeed = 1200,
                CastRange = 1175f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "nami",
                SDataName = "namiq",
                MissileName = "namiqmissile",
                Delay = 250,
                MissileSpeed = 1750,
                CastRange = 875f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "nami",
                SDataName = "namir",
                MissileName = "namirmissile",
                Delay = 250,
                MissileSpeed = 1200,
                CastRange = 2550f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "nautilus",
                SDataName = "nautilusanchordrag",
                MissileName = "nautilusanchordragmissile",
                Delay = 250,
                MissileSpeed = 2000,
                CastRange = 1080f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "nocturne",
                SDataName = "nocturneunspeakablehorror",
                MissileName = "",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 350f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "orianna",
                SDataName = "orianadetonatecommand",
                MissileName = "orianadetonatecommand",
                Delay = 500,
                MissileSpeed = int.MaxValue,
                CastRange = 425f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "riven",
                SDataName = "rivenmartyr",
                MissileName = "",
                Delay = 0,
                MissileSpeed = int.MaxValue,
                CastRange = 260f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "ryze",
                SDataName = "ryzew",
                MissileName = "",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 600f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "sejuani",
                SDataName = "sejuaniglacialprisoncast",
                MissileName = "sejuaniglacialprison",
                Delay = 250,
                MissileSpeed = 1600,
                CastRange = 1200f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "shyvana",
                SDataName = "shyvanatransformcast",
                MissileName = "shyvanatransformcast",
                Delay = 100,
                MissileSpeed = 1100,
                CastRange = 1000f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "skarner",
                SDataName = "skarnerimpale",
                MissileName = "",
                Delay = 350,
                MissileSpeed = int.MaxValue,
                CastRange = 350f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "sona",
                SDataName = "sonar",
                MissileName = "sonar",
                Delay = 250,
                MissileSpeed = 2400,
                CastRange = 1000f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "thresh",
                SDataName = "threshq",
                MissileName = "threshqmissile",
                Delay = 500,
                MissileSpeed = 1900,
                CastRange = 1175f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "twistedfate",
                SDataName = "goldcardpreattack",
                MissileName = "",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 600f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "varus",
                SDataName = "varusr",
                MissileName = "varusrmissile",
                Delay = 250,
                MissileSpeed = 1950,
                CastRange = 1300f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "vayne",
                SDataName = "vaynecondemnmissile",
                MissileName = "",
                Delay = 500,
                MissileSpeed = int.MaxValue,
                CastRange = 450f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "velkoz",
                SDataName = "velkozqplitactive",
                MissileName = "",
                Delay = 0,
                MissileSpeed = 1200,
                CastRange = 1050f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "vi",
                SDataName = "viq",
                MissileName = "",
                Delay = 250,
                MissileSpeed = 1500,
                CastRange = 800f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "vi",
                SDataName = "vir",
                MissileName = "",
                Delay = 250,
                MissileSpeed = 1400,
                CastRange = 800f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "viktor",
                SDataName = "viktorchaosstorm",
                MissileName = "",
                Delay = 350,
                MissileSpeed = int.MaxValue,
                CastRange = 700f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "warwick",
                SDataName = "infiniteduress",
                MissileName = "",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 700f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "xerath",
                SDataName = "xerathmagespear",
                MissileName = "xerathmagespearmissile",
                Delay = 250,
                MissileSpeed = 1600,
                CastRange = 1050f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "xinzhao",
                SDataName = "xenzhaosweep",
                MissileName = "",
                Delay = 250,
                MissileSpeed = 1750,
                CastRange = 600f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "xinzhao",
                SDataName = "xenzhaoparry",
                MissileName = "",
                Delay = 250,
                MissileSpeed = 1750,
                CastRange = 375f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "zac",
                SDataName = "zacr",
                MissileName = "",
                Delay = 250,
                MissileSpeed = int.MaxValue,
                CastRange = 850f
            });

            Spells.Add(new ZhonyaSpell
            {
                ChampionName = "zyra",
                SDataName = "zyrabramblezone",
                MissileName = "",
                Delay = 500,
                MissileSpeed = int.MaxValue,
                CastRange = 700f
            });
        }

        public class ZhonyaSpell
        {
            public string ChampionName { get; set; }
            public string SDataName { get; set; }
            public string MissileName { get; set; }
            public float Delay { get; set; }
            public Int32 MissileSpeed { get; set; }
            public float CastRange { get; set; }
        }
    }
}