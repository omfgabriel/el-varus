namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal class Cleanse : IPlugin
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes the <see cref="Cleanse" /> class.
        /// </summary>
        static Cleanse()
        {
            #region Spell Data

            Spells = new List<CleanseSpell>
                         {
                             new CleanseSpell
                                 {
                                     Name = "suppression", MenuName = "Suppresion", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.R, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Name = "summonerdot", MenuName = "Summoner Ignite", Evade = false, DoT = true,
                                     EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.Unknown,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Vi", Name = "virknockup", MenuName = "Vi R Knockup", Evade = true,
                                     DoT = false, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Name = "itemsmitechallenge", MenuName = "Challenging Smite", Evade = false,
                                     DoT = true, EvadeTimer = 0, Cleanse = true, CleanseTimer = 100,
                                     Slot = SpellSlot.Unknown, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Gangplank", Name = "gangplankpassiveattackdot",
                                     MenuName = "Gangplank Passive Burn", Evade = false, DoT = true, EvadeTimer = 0,
                                     Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.Unknown, Interval = .8
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Teemo", Name = "bantamtraptarget", MenuName = "Teemo Shroom",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Teemo", Name = "toxicshotparticle", MenuName = "Teemo Toxic Shot",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.E, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Talon", Name = "talonbleeddebuf", MenuName = "Talon Bleed", Evade = false,
                                     DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.Q,
                                     Interval = .8
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Malzahar", Name = "alzaharnethergrasp",
                                     MenuName = "Malzahar Nether Grasp", Evade = false, DoT = true, EvadeTimer = 0,
                                     Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.R, Interval = .8
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Malzahar", Name = "alzaharmaleficvisions", MenuName = "Malzahar Ficvisions",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.E, Interval = .8
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "FiddleSticks", Name = "drainchannel", MenuName = "Fiddle Drain",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.W, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Galio", Name = "galioidolofdurand", MenuName = "Galio Idol of Durand",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.R, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Nasus", Name = "nasusw", MenuName = "Nasus Wither", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.W,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Hecarim", Name = "hecarimdefilelifeleech",
                                     MenuName = "Hecarim Defile Leech", Evade = false, DoT = true, EvadeTimer = 0,
                                     Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.W, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Swain", Name = "swaintorment", MenuName = "Swain Torment", Evade = false,
                                     DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.E,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Brand", Name = "brandablaze", MenuName = "Brand Burn Passive",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown, Interval = 0.5
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Fizz", Name = "fizzseastonetrident", MenuName = "Fizz Burn Passive",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown, Interval = .8
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Tristana", Name = "tristanaechargesound",
                                     MenuName = "Tristana Explosive Charge", Evade = false, DoT = true,
                                     EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.E, Interval = .8
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Darius", Name = "dariushemo", MenuName = "Darius Hemorrhage",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.W, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Nidalee", Name = "bushwackdamage", MenuName = "Nidalee Bushwhack",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.W, Interval = .8
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Nidalee", Name = "nidaleepassivehunted",
                                     MenuName = "Nidalee Passive Mark", Evade = false, DoT = true, EvadeTimer = 0,
                                     Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.Unknown, Interval = .8
                                 },
                             new CleanseSpell
                                 {
                                     Name = "shyvanaimmolationaura", Evade = false, DoT = true, EvadeTimer = 0,
                                     Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.W, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Name = "missfortunescattershotslow", Evade = false, DoT = true, EvadeTimer = 0,
                                     Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.E, Interval = 0.5
                                 },
                             new CleanseSpell
                                 {
                                     Name = "missfortunepassivestack", Evade = false, DoT = true, EvadeTimer = 0,
                                     Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.R, Interval = 0.5
                                 },
                             new CleanseSpell
                                 {
                                     Name = "shyvanaimmolatedragon", Evade = false, DoT = true, EvadeTimer = 0,
                                     Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.W, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Zilean", Name = "zileanqenemybomb", MenuName = "Zilean Bomb",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.Q, Interval = 3.8
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Wukong", Name = "monkeykingspintowin", Evade = false, DoT = true,
                                     EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.R, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Cassiopeia", Name = "cassiopeianoxiousblastpoison",
                                     MenuName = "Cassio Noxious Blast", Evade = false, Cleanse = false, DoT = true,
                                     EvadeTimer = 0, CleanseTimer = 0, Slot = SpellSlot.Q, Interval = 0.4
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Cassiopeia", Name = "cassiopeiamiasmapoison", MenuName = "Cassio Miasma",
                                     Evade = false, Cleanse = false, DoT = true, EvadeTimer = 0, CleanseTimer = 0,
                                     Slot = SpellSlot.Q, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Cassiopeia", Name = "cassiopeiapetrifyinggazestun",
                                     MenuName = "Cassio Petrifying Gaze", Evade = false, DoT = false, EvadeTimer = 0,
                                     Cleanse = true, CleanseTimer = 100, Slot = SpellSlot.R, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Lissandra", Name = "lissandrarenemy2", MenuName = "Lissandra Frozen Tomb",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 100,
                                     Slot = SpellSlot.R, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Sejuani", Name = "sejuaniglacialprison",
                                     MenuName = "Sejuani Glacial Prison", Evade = false, DoT = false, EvadeTimer = 0,
                                     Cleanse = true, CleanseTimer = 100, Slot = SpellSlot.R, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Fiora", Name = "fiorarmark", MenuName = "Fiora Grand Challenge",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 100,
                                     Slot = SpellSlot.R, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Twitch", Name = "twitchdeadlyvenon", MenuName = "Twitch Deadly Venom",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.E, Interval = 0.6
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Urgot", Name = "urgotcorrosivedebuff",
                                     MenuName = "Urgot Corrosive Charge", Evade = false, DoT = true, EvadeTimer = 0,
                                     Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Zac", Name = "zacr", Evade = true, DoT = true, EvadeTimer = 150,
                                     Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.R, Interval = 1.5
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Mordekaiser", Name = "mordekaiserchildrenofthegrave",
                                     MenuName = "Mordekaiser Children of the Grave", Evade = false, DoT = true,
                                     EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Unknown,
                                     Interval = 1.5
                                 },
                             new CleanseSpell
                                 {
                                     Name = "burningagony", Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false,
                                     CleanseTimer = 0, Slot = SpellSlot.Unknown, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Name = "garene", Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false,
                                     CleanseTimer = 0, Slot = SpellSlot.E, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Name = "auraofdespair", Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false,
                                     CleanseTimer = 0, Slot = SpellSlot.W, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Name = "hecarimw", Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false,
                                     CleanseTimer = 0, Slot = SpellSlot.W, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Braum", Name = "braummark", MenuName = "Braum Passive", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 200, Slot = SpellSlot.Q
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Zed", Name = "zedultexecute", MenuName = "Zed Mark", Evade = true,
                                     DoT = false, EvadeTimer = 2500, Cleanse = true, CleanseTimer = 500,
                                     Slot = SpellSlot.R, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Karthus", Name = "fallenonetarget", Evade = true, DoT = false,
                                     EvadeTimer = 2600, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Karthus", Name = "karthusfallenonetarget", Evade = true, DoT = false,
                                     EvadeTimer = 2600, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Fizz", Name = "fizzmarinerdoombomb", MenuName = "Fizz Shark Bait",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Morgana", Name = "soulshackles", MenuName = "Morgana Soul Shackles",
                                     Evade = true, DoT = false, EvadeTimer = 2600, Cleanse = true, CleanseTimer = 1100,
                                     Slot = SpellSlot.R, Interval = 3.9
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Varus", Name = "varusrsecondary", MenuName = "Varus Chains of Corruption",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Caitlyn", Name = "caitlynaceinthehole",
                                     MenuName = "Caitlyn Ace in the Hole", Evade = true, DoT = false, EvadeTimer = 900,
                                     Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Vladimir", Name = "vladimirhemoplague", MenuName = "Vladimir Hemoplague",
                                     Evade = true, DoT = false, EvadeTimer = 4500, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Diana", Name = "dianamoonlight", MenuName = "Diana Moonlight",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Q
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Urgot", Name = "urgotswap2", MenuName = "Urgot Swap", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Skarner", Name = "skarnerimpale", MenuName = "Skarner Impale",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 500,
                                     Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Poppy", Name = "poppyulttargetmark",
                                     MenuName = "Poppy Diplomatic Immunity", Evade = false, DoT = false, EvadeTimer = 0,
                                     Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "LeeSin", Name = "blindmonkqonechaos", MenuName = "Lee Sonic Wave",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Q
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Leblanc", Name = "leblancsoulshackle", MenuName = "Leblanc Shackle",
                                     Evade = false, DoT = false, EvadeTimer = 2000, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Leblanc", Name = "leblancsoulshacklem", MenuName = "Leblanc Shackle (R)",
                                     Evade = true, DoT = false, EvadeTimer = 2000, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Name = "vir", Evade = false, DoT = false, Cleanse = false, CleanseTimer = 0,
                                     EvadeTimer = 0, QssIgnore = true, Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Name = "virknockup", Evade = false, DoT = false, Cleanse = false, CleanseTimer = 0,
                                     EvadeTimer = 0, QssIgnore = true, Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Name = "yasuorknockupcombo", Evade = false, DoT = false, Cleanse = false,
                                     CleanseTimer = 0, EvadeTimer = 0, QssIgnore = true, Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Name = "yasuorknockupcombotar", Evade = false, DoT = false, Cleanse = false,
                                     CleanseTimer = 0, EvadeTimer = 0, QssIgnore = true, Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Name = "zyrabramblezoneknockup", Evade = false, DoT = false, Cleanse = false,
                                     CleanseTimer = 0, EvadeTimer = 0, QssIgnore = true, Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Name = "velkozresearchstack", Evade = false, DoT = true, Cleanse = false,
                                     CleanseTimer = 0, EvadeTimer = 0, QssIgnore = true, Slot = SpellSlot.Unknown,
                                     Interval = 0.3
                                 },
                             new CleanseSpell
                                 {
                                     Name = "frozenheartaura", Evade = false, DoT = false, Cleanse = false,
                                     CleanseTimer = 0, EvadeTimer = 0, QssIgnore = true, Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Name = "dariusaxebrabcone", Evade = false, DoT = false, Cleanse = false,
                                     CleanseTimer = 0, EvadeTimer = 0, QssIgnore = true, Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Name = "frozenheartauracosmetic", Evade = false, DoT = false, Cleanse = false,
                                     CleanseTimer = 0, EvadeTimer = 0, QssIgnore = true, Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Name = "itemsunfirecapeaura", Evade = false, DoT = true, Cleanse = false,
                                     CleanseTimer = 0, EvadeTimer = 0, QssIgnore = true, Slot = SpellSlot.Unknown,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Name = "fizzmoveback", Evade = false, DoT = false, Cleanse = false, CleanseTimer = 0,
                                     EvadeTimer = 0, QssIgnore = true, Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Name = "blessingofthelizardelderslow", Evade = false, DoT = true, Cleanse = false,
                                     CleanseTimer = 0, EvadeTimer = 0, QssIgnore = true, Slot = SpellSlot.Unknown,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Name = "dragonburning", Evade = false, DoT = false, Cleanse = false, CleanseTimer = 0,
                                     EvadeTimer = 0, QssIgnore = true, Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Name = "rocketgrab2", Evade = false, DoT = false, Cleanse = false, CleanseTimer = 0,
                                     EvadeTimer = 0, QssIgnore = true, Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Name = "frostarrow", Evade = false, DoT = false, Cleanse = false, CleanseTimer = 0,
                                     EvadeTimer = 0, QssIgnore = true, Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Name = "Pulverize", Evade = false, DoT = false, Cleanse = false, CleanseTimer = 0,
                                     EvadeTimer = 0, QssIgnore = true, Slot = SpellSlot.Q
                                 },
                             new CleanseSpell
                                 {
                                     Name = "monkeykingspinknockup", Evade = false, DoT = false, Cleanse = false,
                                     CleanseTimer = 0, EvadeTimer = 0, QssIgnore = true, Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Name = "headbutttarget", Evade = false, DoT = false, Cleanse = false,
                                     CleanseTimer = 0, EvadeTimer = 0, QssIgnore = true, Slot = SpellSlot.W
                                 },
                             new CleanseSpell
                                 {
                                     Name = "hecarimrampstuncheck", Evade = false, DoT = false, Cleanse = false,
                                     CleanseTimer = 0, EvadeTimer = 0, QssIgnore = true, Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Name = "hecarimrampattackknockback", Evade = false, DoT = false, Cleanse = false,
                                     CleanseTimer = 0, EvadeTimer = 0, QssIgnore = true, Slot = SpellSlot.Unknown
                                 }
                         };

            Spells =
                Spells.Where(
                    x =>
                    !x.QssIgnore
                    && HeroManager.Enemies.Any(
                        y => y.ChampionName.Equals(x.Champion, StringComparison.InvariantCultureIgnoreCase))).ToList();

            #endregion

            #region Item Data

            Items = new List<CleanseItem>
                        {
                            new CleanseItem()
                                {
                                    Slot =
                                        () =>
                                        Player.GetSpell(SpellSlot.Summoner1).Name == "summonerboost"
                                            ? SpellSlot.Summoner1
                                            : Player.GetSpell(SpellSlot.Summoner2).Name == "summonerboost"
                                                  ? SpellSlot.Summoner2
                                                  : SpellSlot.Unknown,
                                    WorksOn =
                                        new[]
                                            {
                                                BuffType.Blind, BuffType.Charm, BuffType.Flee, BuffType.Slow,
                                                BuffType.Polymorph, BuffType.Silence, BuffType.Snare, BuffType.Stun,
                                                BuffType.Taunt, BuffType.Damage
                                            },
                                    Priority = 2
                                },
                            new CleanseItem
                                {
                                    Slot = () => ItemData.Quicksilver_Sash.GetItem().Slots.FirstOrDefault(),
                                    WorksOn =
                                        new[]
                                            {
                                                BuffType.Blind, BuffType.Charm, BuffType.Flee, BuffType.Slow,
                                                BuffType.Polymorph, BuffType.Silence, BuffType.Snare, BuffType.Stun,
                                                BuffType.Taunt, BuffType.Damage
                                            },
                                    Priority = 0
                                },
                            new CleanseItem
                                {
                                    Slot = () => ItemData.Dervish_Blade.GetItem().Slots.FirstOrDefault(),
                                    WorksOn =
                                        new[]
                                            {
                                                BuffType.Blind, BuffType.Charm, BuffType.Flee, BuffType.Slow,
                                                BuffType.Polymorph, BuffType.Silence, BuffType.Snare, BuffType.Stun,
                                                BuffType.Taunt, BuffType.Damage
                                            },
                                    Priority = 0
                                },
                            new CleanseItem
                                {
                                    Slot = () => ItemData.Mercurial_Scimitar.GetItem().Slots.FirstOrDefault(),
                                    WorksOn =
                                        new[]
                                            {
                                                BuffType.Blind, BuffType.Charm, BuffType.Flee, BuffType.Slow,
                                                BuffType.Polymorph, BuffType.Silence, BuffType.Snare, BuffType.Stun,
                                                BuffType.Taunt, BuffType.Damage
                                            },
                                    Priority = 0
                                },
                            new CleanseItem
                                {
                                    Slot = () => ItemData.Mikaels_Crucible.GetItem().Slots.FirstOrDefault(),
                                    WorksOn =
                                        new[]
                                            {
                                                BuffType.Stun, BuffType.Snare, BuffType.Taunt, BuffType.Silence,
                                                BuffType.Slow
                                            },
                                    WorksOnAllies = true, Priority = 1
                                }
                        };

            #endregion

            Items = Items.OrderBy(x => x.Priority).ToList();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        public static List<CleanseItem> Items { get; set; }

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        public static List<CleanseSpell> Spells { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private static Obj_AI_Hero Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        private Menu Menu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        public void CreateMenu(Menu rootMenu)
        {
            this.Menu = new Menu("Cleanse", "CleanseV3");
            {
                var spellsMenu = new Menu("Spells", "CleanseV3Spells");
                {
                    foreach (
                        var spell in
                            Spells.Where(
                                x =>
                                HeroManager.Enemies.Any(
                                    y => y.ChampionName.Equals(x.Champion, StringComparison.InvariantCultureIgnoreCase)))
                        )
                    {
                        spellsMenu.AddItem(new MenuItem(spell.Name, spell.MenuName).SetValue(spell.Cleanse));
                    }
                }

                this.Menu.AddSubMenu(spellsMenu);

                this.Menu.AddItem(new MenuItem("CleanseActivated", "Use Cleanse").SetValue(true));
            }

            rootMenu.AddSubMenu(this.Menu);
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            Game.OnUpdate += this.GameOnUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the best cleanse item.
        /// </summary>
        /// <param name="ally">The ally.</param>
        /// <param name="buff">The buff.</param>
        /// <returns></returns>
        private static Spell GetBestCleanseItem(GameObject ally, BuffInstance buff)
        {
            return
                Items.Where(item => item.WorksOn.Contains(buff.Type) && (!ally.IsMe && item.WorksOnAllies))
                    .OrderBy(x => x.Priority)
                    .Select(x => x.Spell)
                    .FirstOrDefault();
        }

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void GameOnUpdate(EventArgs args)
        {
            if (!this.Menu.Item("CleanseActivated").IsActive())
            {
                return;
            }

            foreach (var ally in HeroManager.Allies)
            {
                foreach (var buff in
                    ally.Buffs.Where(
                        x => Spells.Any(y => y.Name.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase))))
                {
                    if (!this.Menu.Item(buff.Name).IsActive())
                    {
                        continue;
                    }

                    var item = GetBestCleanseItem(ally, buff);

                    if (item == null)
                    {
                        continue;
                    }

                    Utility.DelayAction.Add(
                        Spells.Find(x => x.Name.Equals(buff.Name, StringComparison.InvariantCultureIgnoreCase))
                            .CleanseTimer,
                        () => item.Cast(ally));
                }
            }
        }

        #endregion

        /// <summary>
        ///     An item/spell that can be used to cleanse a spell.
        /// </summary>
        internal class CleanseItem
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="CleanseSpell" /> class.
            /// </summary>
            public CleanseItem()
            {
                this.Range = float.MaxValue;
                this.WorksOnAllies = false;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets the priority.
            /// </summary>
            /// <value>
            ///     The priority.
            /// </value>
            public int Priority { get; set; }

            /// <summary>
            ///     Gets or sets the range.
            /// </summary>
            /// <value>
            ///     The range.
            /// </value>
            public float Range { get; set; }

            /// <summary>
            ///     Gets or sets the slot delegate.
            /// </summary>
            /// <value>
            ///     The slot delegate.
            /// </value>
            public Func<SpellSlot> Slot { get; set; }

            /// <summary>
            ///     Gets or sets the spell.
            /// </summary>
            /// <value>
            ///     The spell.
            /// </value>
            public Spell Spell
            {
                get
                {
                    return new Spell(this.Slot(), this.Range);
                }
            }

            /// <summary>
            ///     Gets or sets what the spell works on.
            /// </summary>
            /// <value>
            ///     The bugg types the spell works on.
            /// </value>
            public BuffType[] WorksOn { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether the spell works on allies.
            /// </summary>
            /// <value>
            ///     <c>true</c> if the spell works on allies; otherwise, <c>false</c>.
            /// </value>
            public bool WorksOnAllies { get; set; }

            #endregion
        }

        /// <summary>
        ///     Represents a spell that cleanse can be used on.
        /// </summary>
        internal class CleanseSpell
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the champion.
            /// </summary>
            /// <value>
            ///     The champion.
            /// </value>
            public string Champion { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether this <see cref="CleanseSpell" /> is cleanse.
            /// </summary>
            /// <value>
            ///     <c>true</c> if cleanse; otherwise, <c>false</c>.
            /// </value>
            public bool Cleanse { get; set; }

            /// <summary>
            ///     Gets or sets the cleanse timer.
            /// </summary>
            /// <value>
            ///     The cleanse timer.
            /// </value>
            public int CleanseTimer { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether the spell does damage over time.
            /// </summary>
            /// <value>
            ///     <c>true</c> if the spell does damage over time; otherwise, <c>false</c>.
            /// </value>
            public bool DoT { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether this <see cref="CleanseSpell" /> can be evaded.
            /// </summary>
            /// <value>
            ///     <c>true</c> if the spell can be evaded; otherwise, <c>false</c>.
            /// </value>
            public bool Evade { get; set; }

            /// <summary>
            ///     Gets or sets the evade timer.
            /// </summary>
            /// <value>
            ///     The evade timer.
            /// </value>
            public int EvadeTimer { get; set; }

            /// <summary>
            ///     Gets or sets the incoming damage.
            /// </summary>
            /// <value>
            ///     The incoming damage.
            /// </value>
            public int IncomeDamage { get; set; }

            /// <summary>
            ///     Gets or sets the interval.
            /// </summary>
            /// <value>
            ///     The interval.
            /// </value>
            public double Interval { get; set; }

            /// <summary>
            ///     Gets or sets the name of the menu.
            /// </summary>
            /// <value>
            ///     The name of the menu.
            /// </value>
            public string MenuName { get; set; }

            /// <summary>
            ///     Gets or sets the name.
            /// </summary>
            /// <value>
            ///     The name.
            /// </value>
            public string Name { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether QSS can be used on this spell.
            /// </summary>
            /// <value>
            ///     <c>true</c> if QSS can be used on this spell; otherwise, <c>false</c>.
            /// </value>
            public bool QssIgnore { get; set; }

            /// <summary>
            ///     Gets or sets the slot.
            /// </summary>
            /// <value>
            ///     The slot.
            /// </value>
            public SpellSlot Slot { get; set; }

            /// <summary>
            ///     Gets or sets the tick limiter.
            /// </summary>
            /// <value>
            ///     The tick limiter.
            /// </value>
            public int TickLimiter { get; set; }

            #endregion
        }
    }
}
