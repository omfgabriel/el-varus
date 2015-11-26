using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace ElUtilitySuite
{
    public class Zhonya
    {
        private static Items.Item zhyonyaItem;

        static Zhonya()
        {
            #region Spells Init

            Spells = new List<ZhonyaSpell>
            {
                new ZhonyaSpell
                {
                    ChampionName = "aatrox",
                    SDataName = "aatroxq",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = 2000,
                    CastRange = 650f
                },
                new ZhonyaSpell
                {
                    ChampionName = "ahri",
                    SDataName = "ahriseduce",
                    MissileName = "ahriseducemissile",
                    Delay = 250,
                    MissileSpeed = 1550,
                    CastRange = 975f
                },
                new ZhonyaSpell
                {
                    ChampionName = "amumu",
                    SDataName = "bandagetoss",
                    MissileName = "sadmummybandagetoss",
                    Delay = 250,
                    MissileSpeed = 2000,
                    CastRange = 1100f
                },
                new ZhonyaSpell
                {
                    ChampionName = "amumu",
                    SDataName = "curseofthesadmummy",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 560f
                },
                new ZhonyaSpell
                {
                    ChampionName = "annie",
                    SDataName = "infernalguardian",
                    MissileName = "",
                    Delay = 0,
                    MissileSpeed = int.MaxValue,
                    CastRange = 900f
                },
                new ZhonyaSpell
                {
                    ChampionName = "ashe",
                    SDataName = "enchantedcrystalarrow",
                    MissileName = "enchantedcrystalarrow",
                    Delay = 250,
                    MissileSpeed = 1600,
                    CastRange = 20000f
                },
                new ZhonyaSpell
                {
                    ChampionName = "azir",
                    SDataName = "azirr",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 475f
                },
                new ZhonyaSpell
                {
                    ChampionName = "blitzcrank",
                    SDataName = "rocketgrabmissile",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = 1800,
                    CastRange = 1050f
                },
                new ZhonyaSpell
                {
                    ChampionName = "ziggs",
                    SDataName = "ziggsq",
                    MissileName = "ziggsqspell",
                    Delay = 250,
                    MissileSpeed = 1750,
                    CastRange = 850f
                },
                new ZhonyaSpell
                {
                    ChampionName = "ziggs",
                    SDataName = "ziggsr",
                    MissileName = "ziggsr",
                    Delay = 1800,
                    MissileSpeed = 1750,
                    CastRange = 2250f
                },
                new ZhonyaSpell
                {
                    ChampionName = "zed",
                    SDataName = "zedr",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 850f
                },
                new ZhonyaSpell
                {
                    ChampionName = "syndra",
                    SDataName = "syndrar",
                    MissileName = "",
                    Delay = 450,
                    MissileSpeed = 1250,
                    CastRange = 675f
                },
                new ZhonyaSpell
                {
                    ChampionName = "syndra",
                    SDataName = "syndraq",
                    MissileName = "syndraq",
                    Delay = 250,
                    MissileSpeed = 1750,
                    CastRange = 800f
                },
                new ZhonyaSpell
                {
                    ChampionName = "braum",
                    SDataName = "braumq",
                    MissileName = "braumqmissile",
                    Delay = 250,
                    MissileSpeed = 1200,
                    CastRange = 1100f
                },
                new ZhonyaSpell
                {
                    ChampionName = "braum",
                    SDataName = "braumqmissle",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = 1200,
                    CastRange = 1100f
                },
                new ZhonyaSpell
                {
                    ChampionName = "braum",
                    SDataName = "braumrwrapper",
                    MissileName = "braumrmissile",
                    Delay = 250,
                    MissileSpeed = 1200,
                    CastRange = 1250f
                },
                new ZhonyaSpell
                {
                    ChampionName = "cassiopeia",
                    SDataName = "cassiopeiapetrifyinggaze",
                    MissileName = "cassiopeiapetrifyinggaze",
                    Delay = 350,
                    MissileSpeed = int.MaxValue,
                    CastRange = 875f
                },
                new ZhonyaSpell
                {
                    ChampionName = "chogath",
                    SDataName = "rupture",
                    MissileName = "rupture",
                    Delay = 1000,
                    MissileSpeed = int.MaxValue,
                    CastRange = 950f
                },
                new ZhonyaSpell
                {
                    ChampionName = "darius",
                    SDataName = "dariusaxegrabcone",
                    MissileName = "dariusaxegrabcone",
                    Delay = 150,
                    MissileSpeed = int.MaxValue,
                    CastRange = 555f
                },
                new ZhonyaSpell
                {
                    ChampionName = "diana",
                    SDataName = "dianavortex",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 350f
                },
                new ZhonyaSpell
                {
                    ChampionName = "elise",
                    SDataName = "elisehumane",
                    MissileName = "elisehumane",
                    Delay = 250,
                    MissileSpeed = 1600,
                    CastRange = 1075f
                },
                new ZhonyaSpell
                {
                    ChampionName = "evelynn",
                    SDataName = "evelynnr",
                    MissileName = "evelynnr",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 900f
                },
                new ZhonyaSpell
                {
                    ChampionName = "fiddlesticks",
                    SDataName = "terrify",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 575f
                },
                new ZhonyaSpell
                {
                    ChampionName = "fiddlesticks",
                    SDataName = "crowstorm",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 800f
                },
                new ZhonyaSpell
                {
                    ChampionName = "galio",
                    SDataName = "galioidolofdurand",
                    MissileName = "",
                    Delay = 0,
                    MissileSpeed = int.MaxValue,
                    CastRange = 600f
                },
                new ZhonyaSpell
                {
                    ChampionName = "garen",
                    SDataName = "garenqattack",
                    MissileName = "",
                    Delay = 0,
                    MissileSpeed = int.MaxValue,
                    CastRange = 350f
                },
                new ZhonyaSpell
                {
                    ChampionName = "gnar",
                    SDataName = "gnarult",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 600f
                },
                new ZhonyaSpell
                {
                    ChampionName = "gragas",
                    SDataName = "gragase",
                    MissileName = "gragase",
                    Delay = 200,
                    MissileSpeed = 1200,
                    CastRange = 600f
                },
                new ZhonyaSpell
                {
                    ChampionName = "gragas",
                    SDataName = "gragasr",
                    MissileName = "gragasrboom",
                    Delay = 250,
                    MissileSpeed = 1750,
                    CastRange = 1150f
                },
                new ZhonyaSpell
                {
                    ChampionName = "hecarim",
                    SDataName = "hecarimult",
                    MissileName = "",
                    Delay = 50,
                    MissileSpeed = 1200,
                    CastRange = 1350f
                },
                new ZhonyaSpell
                {
                    ChampionName = "irelia",
                    SDataName = "ireliaequilibriumstrike",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 450f
                },
                new ZhonyaSpell
                {
                    ChampionName = "janna",
                    SDataName = "reapthewhirlwind",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 725f
                },
                new ZhonyaSpell
                {
                    ChampionName = "jarvaniv",
                    SDataName = "jarvanivdragonstrike",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = 2000,
                    CastRange = 700f
                },
                new ZhonyaSpell
                {
                    ChampionName = "jayce",
                    SDataName = "jaycetotheskies",
                    MissileName = "",
                    Delay = 450,
                    MissileSpeed = int.MaxValue,
                    CastRange = 600f
                },
                new ZhonyaSpell
                {
                    ChampionName = "jayce",
                    SDataName = "jayceshockblast",
                    MissileName = "jayceshockblastmis",
                    Delay = 250,
                    MissileSpeed = 2350,
                    CastRange = 1570f
                },
                new ZhonyaSpell
                {
                    ChampionName = "karma",
                    SDataName = "karmaq",
                    MissileName = "karmaqmissile",
                    Delay = 250,
                    MissileSpeed = 1800,
                    CastRange = 1050f
                },
                new ZhonyaSpell
                {
                    ChampionName = "kassadin",
                    SDataName = "nulllance",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = 1900,
                    CastRange = 650f
                },
                new ZhonyaSpell
                {
                    ChampionName = "kassadin",
                    SDataName = "forcepulse",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 700f
                },
                new ZhonyaSpell
                {
                    ChampionName = "leesin",
                    SDataName = "blindmonkrkick",
                    MissileName = "",
                    Delay = 500,
                    MissileSpeed = int.MaxValue,
                    CastRange = 375f
                },
                new ZhonyaSpell
                {
                    ChampionName = "leona",
                    SDataName = "leonashieldofdaybreak",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 215f
                },
                new ZhonyaSpell
                {
                    ChampionName = "leona",
                    SDataName = "leonasolarflare",
                    MissileName = "leonasolarflare",
                    Delay = 1200,
                    MissileSpeed = int.MaxValue,
                    CastRange = 1200f
                },
                new ZhonyaSpell
                {
                    ChampionName = "lissandra",
                    SDataName = "lissandraw",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 450f
                },
                new ZhonyaSpell
                {
                    ChampionName = "lissandra",
                    SDataName = "lissandrar",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 550f
                },
                new ZhonyaSpell
                {
                    ChampionName = "lux",
                    SDataName = "luxlightbinding",
                    MissileName = "luxlightbindingmis",
                    Delay = 250,
                    MissileSpeed = 1200,
                    CastRange = 1300f
                },
                new ZhonyaSpell
                {
                    ChampionName = "malphite",
                    SDataName = "ufslash",
                    MissileName = "ufslash",
                    Delay = 250,
                    MissileSpeed = 2200,
                    CastRange = 1000f
                },
                new ZhonyaSpell
                {
                    ChampionName = "malzahar",
                    SDataName = "alzaharnethergrasp",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 700f
                },
                new ZhonyaSpell
                {
                    ChampionName = "maokai",
                    SDataName = "maokaiunstablegrowth",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 650f
                },
                new ZhonyaSpell
                {
                    ChampionName = "monkeyking",
                    SDataName = "monkeykingspintowin",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 450f
                },
                new ZhonyaSpell
                {
                    ChampionName = "morgana",
                    SDataName = "darkbindingmissile",
                    MissileName = "darkbindingmissile",
                    Delay = 250,
                    MissileSpeed = 1200,
                    CastRange = 1175f
                },
                new ZhonyaSpell
                {
                    ChampionName = "nami",
                    SDataName = "namiq",
                    MissileName = "namiqmissile",
                    Delay = 250,
                    MissileSpeed = 1750,
                    CastRange = 875f
                },
                new ZhonyaSpell
                {
                    ChampionName = "nami",
                    SDataName = "namir",
                    MissileName = "namirmissile",
                    Delay = 250,
                    MissileSpeed = 1200,
                    CastRange = 2550f
                },
                new ZhonyaSpell
                {
                    ChampionName = "nautilus",
                    SDataName = "nautilusanchordrag",
                    MissileName = "nautilusanchordragmissile",
                    Delay = 250,
                    MissileSpeed = 2000,
                    CastRange = 1080f
                },
                new ZhonyaSpell
                {
                    ChampionName = "nocturne",
                    SDataName = "nocturneunspeakablehorror",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 350f
                },
                new ZhonyaSpell
                {
                    ChampionName = "orianna",
                    SDataName = "orianadetonatecommand",
                    MissileName = "orianadetonatecommand",
                    Delay = 500,
                    MissileSpeed = int.MaxValue,
                    CastRange = 425f
                },
                new ZhonyaSpell
                {
                    ChampionName = "riven",
                    SDataName = "rivenmartyr",
                    MissileName = "",
                    Delay = 0,
                    MissileSpeed = int.MaxValue,
                    CastRange = 260f
                },
                new ZhonyaSpell
                {
                    ChampionName = "riven",
                    SDataName = "rivenizunablade",
                    MissileName = "rivenlightsabermissile",
                    Delay = 250,
                    MissileSpeed = 1600,
                    CastRange = 1075
                },
                new ZhonyaSpell
                {
                    ChampionName = "sejuani",
                    SDataName = "sejuaniglacialprisoncast",
                    MissileName = "sejuaniglacialprison",
                    Delay = 250,
                    MissileSpeed = 1600,
                    CastRange = 1200f
                },
                new ZhonyaSpell
                {
                    ChampionName = "shyvana",
                    SDataName = "shyvanatransformcast",
                    MissileName = "shyvanatransformcast",
                    Delay = 100,
                    MissileSpeed = 1100,
                    CastRange = 1000f
                },
                new ZhonyaSpell
                {
                    ChampionName = "skarner",
                    SDataName = "skarnerimpale",
                    MissileName = "",
                    Delay = 350,
                    MissileSpeed = int.MaxValue,
                    CastRange = 350f
                },
                new ZhonyaSpell
                {
                    ChampionName = "sona",
                    SDataName = "sonar",
                    MissileName = "sonar",
                    Delay = 250,
                    MissileSpeed = 2400,
                    CastRange = 1000f
                },
                new ZhonyaSpell
                {
                    ChampionName = "thresh",
                    SDataName = "threshq",
                    MissileName = "threshqmissile",
                    Delay = 500,
                    MissileSpeed = 1900,
                    CastRange = 1175f
                },
                new ZhonyaSpell
                {
                    ChampionName = "twistedfate",
                    SDataName = "goldcardpreattack",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 600f
                },
                new ZhonyaSpell
                {
                    ChampionName = "varus",
                    SDataName = "varusr",
                    MissileName = "varusrmissile",
                    Delay = 250,
                    MissileSpeed = 1950,
                    CastRange = 1300f
                },
                new ZhonyaSpell
                {
                    ChampionName = "vayne",
                    SDataName = "vaynecondemnmissile",
                    MissileName = "",
                    Delay = 500,
                    MissileSpeed = int.MaxValue,
                    CastRange = 450f
                },
                new ZhonyaSpell
                {
                    ChampionName = "velkoz",
                    SDataName = "velkozqplitactive",
                    MissileName = "",
                    Delay = 0,
                    MissileSpeed = 1200,
                    CastRange = 1050f
                },
                new ZhonyaSpell
                {
                    ChampionName = "vi",
                    SDataName = "viq",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = 1500,
                    CastRange = 800f
                },
                new ZhonyaSpell
                {
                    ChampionName = "vi",
                    SDataName = "vir",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = 1400,
                    CastRange = 800f
                },
                new ZhonyaSpell
                {
                    ChampionName = "viktor",
                    SDataName = "viktorchaosstorm",
                    MissileName = "",
                    Delay = 350,
                    MissileSpeed = int.MaxValue,
                    CastRange = 700f
                },
                new ZhonyaSpell
                {
                    ChampionName = "warwick",
                    SDataName = "infiniteduress",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 700f
                },
                new ZhonyaSpell
                {
                    ChampionName = "xerath",
                    SDataName = "xerathmagespear",
                    MissileName = "xerathmagespearmissile",
                    Delay = 250,
                    MissileSpeed = 1600,
                    CastRange = 1050f
                },
                new ZhonyaSpell
                {
                    ChampionName = "xinzhao",
                    SDataName = "xenzhaosweep",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = 1750,
                    CastRange = 600f
                },
                new ZhonyaSpell
                {
                    ChampionName = "xinzhao",
                    SDataName = "xenzhaoparry",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = 1750,
                    CastRange = 375f
                },
                new ZhonyaSpell
                {
                    ChampionName = "zac",
                    SDataName = "zacr",
                    MissileName = "",
                    Delay = 250,
                    MissileSpeed = int.MaxValue,
                    CastRange = 850f
                },
                new ZhonyaSpell
                {
                    ChampionName = "zyra",
                    SDataName = "zyrabramblezone",
                    MissileName = "",
                    Delay = 500,
                    MissileSpeed = int.MaxValue,
                    CastRange = 700f
                },
                new ZhonyaSpell
                {
                    ChampionName = "bard",
                    SDataName = "bardr",
                    MissileName = "bardr",
                    Delay = 450,
                    MissileSpeed = 210,
                    CastRange = 3400f
                }
            };

            #endregion
        }

        public static List<ZhonyaSpell> Spells { get; set; }

        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }

        private static bool ZhonyaLowHp { get { return InitializeMenu.Menu.Item("ZhonyaHP_BETA").IsActive(); } }
        private static int ZhonyaBelowHp { get { return InitializeMenu.Menu.Item("ZhonyaHPSlider").GetValue<Slider>().Value; } }

        public static void Init()
        {
            zhyonyaItem = new Items.Item(Game.MapId == GameMapId.SummonersRift ? 3157 : 3090);

            GameObject.OnCreate += GameObjectOnCreate;
            Obj_AI_Base.OnProcessSpellCast += ObjAiBaseOnProcessSpellCast;
            AttackableUnit.OnDamage += ObjAiBaseOnOnDamage;
        }

        private static void ObjAiBaseOnOnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            var target = ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(args.TargetNetworkId);

            if (!target.IsMe || !ZhonyaLowHp)
            {
                return;
            }

            if (Player.HealthPercent < ZhonyaBelowHp || (Player.Health - args.Damage)/Player.MaxHealth < ZhonyaBelowHp)
            {
                zhyonyaItem.Cast();
            }
        }

        private static void ObjAiBaseOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsAlly)
            {
                return;
            }

            var spellData =
                Spells.FirstOrDefault(
                    x =>
                        x.SDataName == args.SData.Name.ToLower() || x.MissileName == args.SData.Name.ToLower());

            if (spellData == null)
            {
                return;
            }

            if (!InitializeMenu.Menu.Item(string.Format("Zhonya{0}", spellData.SDataName)).IsActive() ||
                !InitializeMenu.Menu.Item("ZhonyaDangerous").IsActive())
            {
                return;
            }

            if (Player.Distance(args.Start) > spellData.CastRange)
            {
                return;
            }

            // Targetted spells
            if (args.SData.TargettingType == SpellDataTargetType.Unit && args.Target.IsMe ||
                args.SData.TargettingType == SpellDataTargetType.SelfAndUnit && args.Target.IsMe ||
                args.SData.TargettingType == SpellDataTargetType.Self ||
                args.SData.TargettingType == SpellDataTargetType.SelfAoe &&
                Player.Distance(sender) < spellData.CastRange)
            {
                zhyonyaItem.Cast();
                return;
            }

            // Anything besides a skillshot return
            if (!args.SData.TargettingType.ToString().Contains("Location") &&
                args.SData.TargettingType != SpellDataTargetType.Cone)
            {
                return;
            }

            // Correct the end position
            var endPosition = args.End;

            if (args.Start.Distance(endPosition) > spellData.CastRange)
            {
                endPosition = args.Start +
                              Vector3.Normalize(endPosition - args.Start)*spellData.CastRange;
            }

            // credits to kurisu
            var isLinear = args.SData.TargettingType == SpellDataTargetType.Cone || args.SData.LineWidth > 0;
            var width = isLinear && args.SData.TargettingType != SpellDataTargetType.Cone
                ? args.SData.LineWidth
                : (args.SData.CastRadius < 1 ? args.SData.CastRadiusSecondary : args.SData.CastRadius);

            if ((isLinear && width + Player.BoundingRadius > Player.ServerPosition.To2D()
                .Distance(
                    Player.ServerPosition.To2D()
                        .ProjectOn(args.Start.To2D(), endPosition.To2D())
                        .SegmentPoint)) ||
                (!isLinear && Player.Distance(endPosition) <= width + Player.BoundingRadius))
            {
                zhyonyaItem.Cast();
            }
        }

        private static void GameObjectOnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid<MissileClient>() || sender.IsAlly)
            {
                return;
            }

            var missile = (MissileClient) sender;
            var sdata =
                Spells.FirstOrDefault(
                    x =>
                        missile.SData.Name.ToLower().Equals(x.MissileName) ||
                        missile.SData.Name.ToLower().Equals(x.SDataName));

            // Not in database
            if (sdata == null)
            {
                return;
            }

            if (!InitializeMenu.Menu.Item(string.Format("Zhonya{0}", sdata.SDataName)).IsActive() ||
                !InitializeMenu.Menu.Item("ZhonyaDangerous").IsActive())
            {
                return;
            }

            // Correct the end position
            var endPosition = missile.EndPosition;

            if (missile.StartPosition.Distance(endPosition) > sdata.CastRange)
            {
                endPosition = missile.StartPosition +
                              Vector3.Normalize(endPosition - missile.StartPosition)*sdata.CastRange;
            }

            if (missile.SData.LineWidth + Player.BoundingRadius >
                Player.ServerPosition.To2D()
                    .Distance(
                        Player.ServerPosition.To2D()
                            .ProjectOn(missile.StartPosition.To2D(), endPosition.To2D())
                            .SegmentPoint))
            {
                zhyonyaItem.Cast();
            }
        }

        public class ZhonyaSpell
        {
            public string ChampionName { get; set; }
            public string SDataName { get; set; }
            public string MissileName { get; set; }
            public float Delay { get; set; }
            public int MissileSpeed { get; set; }
            public float CastRange { get; set; }
        }
    }
}