namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Color = SharpDX.Color;
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
                                     Name = "summonerdot", MenuName = "Summoner Ignite", Evade = false, DoT = true,
                                     EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.Unknown,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Name = "summonerexhaustdebuff", MenuName = "Summoner Exhaust", Evade = false,
                                     DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Taric", Name = "taricE", MenuName = "Taric Stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Lulu", Name = "LuluWDebuff", MenuName = "Lulu polymorph", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.W,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Nocturne", Name = "flee", MenuName = "Nocturne Fear", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Pantheon", Name = "stun", MenuName = "Pantheon Stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.W,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Leona", Name = "stun", MenuName = "Leona Stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.Q,
                                     Interval = 1.0
                                 },
                              new CleanseSpell
                                 {
                                     Champion = "Nami", Name = "NamiQtt", MenuName = "Nami Q", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Q,
                                     Interval = 1.0
                                 },
                              new CleanseSpell
                                 {
                                     Champion = "Nami", Name = "NamiESlowtt", MenuName = "Nami E", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 100, Slot = SpellSlot.E,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Quinn", Name = "QuinnQSightReduction", MenuName = "Quinn Blind",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Q, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Teemo", Name = "blind", MenuName = "Teemo Blind", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Q,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Ashe", Name = "stun", MenuName = "Ashe (R)", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "AurelionSol", Name = "Stun", MenuName = "Aurelion Sol stun", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Q
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Brand", Name = "stun", MenuName = "Brand stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Ekko", Name = "stun", MenuName = "Ekko stun", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Unknown,
                                     Interval = 1.0
                                 },
                              new CleanseSpell
                                 {
                                     Champion = "Kayle", Name = "JudicatorReckoning", MenuName = "Kayle slow", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Q,
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
                                     Champion = "Ahri", Name = "ahriseduce", MenuName = "Ahri (E)", Evade = false,
                                     DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.E,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Talon", Name = "talonbleeddebuf", MenuName = "Talon Bleed", Evade = false,
                                     DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.Q,
                                     Interval = .8
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Malzahar", Name = "MalzaharR",
                                     MenuName = "Malzahar Nether Grasp (R)", Evade = false, DoT = true, EvadeTimer = 0,
                                     Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.R, Interval = .8
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Malzahar", Name = "malzaharerecent",
                                     MenuName = "Malzahar Ficvisions (E)", Evade = false, DoT = true, EvadeTimer = 0,
                                     Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.E, Interval = .8
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Soraka", Name = "SorakaESnare", MenuName = "Soraka (E)",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.E, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "FiddleSticks", Name = "flee", MenuName = "Fiddle Fear", Evade = false,
                                     DoT = true, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Q,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "FiddleSticks", Name = "Silence", MenuName = "Fiddle Silence",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.E, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Chogath", Name = "Silence", MenuName = "Chogath (W)", Evade = false,
                                     DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.W,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Galio", Name = "galioidolofdurand", MenuName = "Galio (R)",
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
                                     Champion = "Hecarim", Name = "Flee",
                                     MenuName = "Hecarim (R)", Evade = false, DoT = true, EvadeTimer = 0,
                                     Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.R, Interval = 1.0
                                 },
                              new CleanseSpell
                                 {
                                     Champion = "Swain", Name = "SwainShadowGraspRoot", MenuName = "Swain (W)", Evade = false,
                                     DoT = true, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.W,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Darius", Name = "DariusNoxianTacticsONH", MenuName = "Darius (W)",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.W, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Name = "missfortunescattershotslow", Evade = false, DoT = true, EvadeTimer = 0,
                                     Champion = "MissFortune", Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.E,
                                     Interval = 0.5
                                 },
                             new CleanseSpell
                                 {
                                     Name = "missfortunepassivestack", Evade = false, DoT = true, EvadeTimer = 0,
                                     Champion = "MissFortune", Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.R,
                                     Interval = 0.5
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Wukong", Name = "monkeykingspintowin", Evade = false, DoT = true,
                                     EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.R, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Zyra", Name = "ZyraEHold", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E, Interval = 1.0
                                 },
                              new CleanseSpell
                                 {
                                     Champion = "Jinx", Name = "JinxEMineSnare", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E, Interval = 1.0
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
                                     Champion = "Sejuani", Name = "sejuaniglacialprison",
                                     MenuName = "Sejuani Glacial Prison", Evade = false, DoT = false, EvadeTimer = 0,
                                     Cleanse = true, CleanseTimer = 100, Slot = SpellSlot.R, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Fiora", Name = "Stun", MenuName = "Fiora (W) parries",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 100,
                                     Slot = SpellSlot.W, Interval = 1.0
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
                                     Champion = "Karthus", Name = "KarthusWallOfPain ",  MenuName = "Karthus W", Evade = true, DoT = false,
                                     EvadeTimer = 1600, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.W
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Fizz", Name = "FizzMarinerDoomBomb", MenuName = "Fizz (R)",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Morgana", Name = "SoulShackless", MenuName = "Morgana Soul Shackles",
                                     Evade = true, DoT = false, EvadeTimer = 2600, Cleanse = true, CleanseTimer = 1100,
                                     Slot = SpellSlot.R, Interval = 3.9
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Morgana", Name = "DarkBindingMissile", MenuName = "Morgana Q",
                                     Evade = true, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Q
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Varus", Name = "varusrsecondary", MenuName = "Varus Chains of Corruption",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Caitlyn", Name = "CaitlynYordleTrap", MenuName = "Caitlyn trap",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.W
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
                                     Champion = "Diana", Name = "DianaArc", MenuName = "Diana Q", Evade = true,
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
                                     Champion = "Poppy", Name = "Stun", MenuName = "Poppy wall stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Poppy", Name = "poppyulttargetmark",
                                     MenuName = "Poppy Diplomatic Immunity", Evade = false, DoT = false, EvadeTimer = 0,
                                     Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "LeeSin", Name = "blindmonkqone", MenuName = "Lee Sin Q", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Q
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
                                     Champion = "twistedfate", Name = "stun", MenuName = "Twisted Fate Gold (W)",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown
                                 },
                              new CleanseSpell
                                 {
                                     Champion = "Rengar", Name = "RengarERoot", MenuName = "Rengar EMP (E)", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E
                                 },

                             new CleanseSpell
                                 {
                                     Champion = "amumu", Name = "stun", MenuName = "bandagetoss (Q)", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Q
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "amumu", Name = "curseofthesadmummy", MenuName = "curseofthesadmummy (R)",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Rammus", Name = "taunt", MenuName = "Rammus Taunt", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "shen", Name = "taunt", MenuName = "Shen Taunt", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "jax", Name = "stun", MenuName = "Jax (E)", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                              new CleanseSpell
                                 {
                                     Champion = "Janna", Name = "Slow", MenuName = "Janna (W)", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.W
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "kennen", Name = "stun", MenuName = "Kennen stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "braum", Name = "braumstundebuff", MenuName = "Braum Passive",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "nunu", Name = "IceBlast", MenuName = "Nunu Q", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "riven", Name = "Stun", MenuName = "Riven (W)", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.W
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "annie", Name = "Stun", MenuName = "Annie Stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Warwick", Name = "suppression", MenuName = "Warwick (R)", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "anivia", Name = "Stun", MenuName = "Anivia Stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 100, Slot = SpellSlot.Q
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "heimerdinger", Name = "stun", MenuName = "Heimerdinger Stun",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.W
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "bard", Name = "bardqshackledebuff", MenuName = "Bard Stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "vayne", Name = "stun", MenuName = "Vayne Condemn", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "elise", Name = "buffelisecocoon", MenuName = "Elise E", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "udyr", Name = "UdyrBearStunCheck", MenuName = "Udyr Stun", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "irelia", Name = "Stun", MenuName = "Irelia Stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "veigar", Name = "Stun", MenuName = "Veigar Stun", Evade = true,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "gnar", Name = "GnarStun", MenuName = "Gnar R", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "syndra", Name = "Stun", MenuName = "Syndra stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "sona", Name = "SonaR", MenuName = "Sona (R)", Evade = true, DoT = false,
                                     EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "xerath", Name = "stun", MenuName = "Xerath (E)", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "thresh", Name = "threshq", MenuName = "Thresh (Q)", Evade = true,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Q
                                 },
                              new CleanseSpell
                                 {
                                     Champion = "viktor", Name = "ViktorGravitonStun", MenuName = "Viktor stun", Evade = true,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Q
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Lissandra", Name = "lissandrarenemy2", MenuName = "Lissandra (R)",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 100,
                                     Slot = SpellSlot.R, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "lissandra", Name = "lissandraw", MenuName = "Lissandra (W)",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.W
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "lux", Name = "luxlightbindingmis", MenuName = "Lux (Q)", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Q
                                 },
                              new CleanseSpell
                                 {
                                     Champion = "maokai", Name = "MaokaiUnstableGrowthRoot", MenuName = "Maokai (W)", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.W
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "renekton", Name = "stun", MenuName = "Renekton stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.W
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "tahmkench", Name = "tahmkenchqstun", MenuName = "Tahm Q stun",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Q
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "tahmkench", Name = "tahmkenchwhasdevouredtarget", MenuName = "Tahm stun",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = false, QssIgnore = true,
                                     CleanseTimer = 0, Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "nautilus", Name = "NautilusPassiveRoot", MenuName = "Nautilus Passive",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "nautilus", Name = "stun", MenuName = "Nautilus R", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "zilean", Name = "Stun", MenuName = "Zilean double Q", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 100, Slot = SpellSlot.Q
                                 }
                         };

            Spells =
                Spells.Where(x => !x.QssIgnore)
                    .Where(
                        x =>
                        string.IsNullOrEmpty(x.Champion)
                        || HeroManager.Enemies.Any(
                            y => y.ChampionName.Equals(x.Champion, StringComparison.InvariantCultureIgnoreCase)))
                    .ToList();

            #endregion

            #region Item Data

            Items = new List<CleanseItem>
                        {
                            new CleanseItem
                                {
                                    Slot =
                                        () =>
                                        Player.GetSpellSlot("summonerboost") == SpellSlot.Unknown
                                            ? SpellSlot.Unknown
                                            : Player.GetSpellSlot("summonerboost"),
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
                                    Slot = () =>
                                        {
                                            var slots = ItemData.Quicksilver_Sash.GetItem().Slots;
                                            return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                        },
                                    WorksOn =
                                        new[]
                                            {
                                                BuffType.Blind, BuffType.Charm, BuffType.Flee, BuffType.Slow,
                                                BuffType.Polymorph, BuffType.Silence, BuffType.Snare,
                                                BuffType.Stun, BuffType.Taunt, BuffType.Damage,
                                                BuffType.CombatEnchancer
                                            },
                                    Priority = 0
                                },
                            new CleanseItem
                                {
                                    Slot = () =>
                                        {
                                            var slots = ItemData.Dervish_Blade.GetItem().Slots;
                                            return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                        },
                                    WorksOn =
                                        new[]
                                            {
                                                BuffType.Blind, BuffType.Charm, BuffType.Flee, BuffType.Slow,
                                                BuffType.Polymorph, BuffType.Silence, BuffType.Snare,
                                                BuffType.Stun, BuffType.Taunt, BuffType.Damage,
                                                BuffType.CombatEnchancer
                                            },
                                    Priority = 0
                                },
                            new CleanseItem
                                {
                                    Slot = () =>
                                        {
                                            var slots = ItemData.Mercurial_Scimitar.GetItem().Slots;
                                            return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                        },
                                    WorksOn =
                                        new[]
                                            {
                                                BuffType.Blind, BuffType.Charm, BuffType.Flee, BuffType.Slow,
                                                BuffType.Polymorph, BuffType.Silence, BuffType.Snare,
                                                BuffType.Stun, BuffType.Taunt, BuffType.Damage,
                                                BuffType.CombatEnchancer
                                            },
                                    Priority = 0
                                },
                            new CleanseItem
                                {
                                    Slot = () =>
                                        {
                                            var slots = ItemData.Mikaels_Crucible.GetItem().Slots;
                                            return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                        },
                                    WorksOn =
                                        new[]
                                            {
                                                BuffType.Stun, BuffType.Snare, BuffType.Taunt, BuffType.Silence,
                                                BuffType.Slow, BuffType.CombatEnchancer
                                            },
                                    WorksOnAllies = true,
                                    Priority = 1
                                }
                        };

            #endregion

            Items = Items.OrderBy(x => x.Priority).ToList();

            Random = new Random(Environment.TickCount);
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     A delegate that returns a <see cref="SpellSlot" />
        /// </summary>
        /// <returns>
        ///     <see cref="SpellSlot" />
        /// </returns>
        public delegate SpellSlot GetSlotDelegate();

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
        ///     Gets or sets the random.
        /// </summary>
        /// <value>
        ///     The random.
        /// </value>
        private static Random Random { get; set; }

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
            var predicate = new Func<Menu, bool>(x => x.Name == "SummonersMenu");
            var menu = !rootMenu.Children.Any(predicate)
                           ? rootMenu.AddSubMenu(new Menu("Summoners", "SummonersMenu"))
                           : rootMenu.Children.First(predicate);

            this.Menu = new Menu("Cleanse/QSS", "CleanseV3").SetFontStyle(FontStyle.Regular, Color.Aqua);
            {
                var spellsMenu = new Menu("Spells", "CleanseV3Spells");
                {
                    foreach (var spell in Spells)
                    {
                        spellsMenu.AddItem(
                            new MenuItem(
                                spell.MenuName != null ? spell.MenuName.Replace(" ", string.Empty) : spell.Name,
                                string.IsNullOrEmpty(spell.MenuName) ? spell.Name : spell.MenuName).SetValue(
                                    spell.Cleanse));
                    }
                }

                this.Menu.AddSubMenu(spellsMenu);

                var humanizerDelay = new Menu("Humanizer Delay", "CleanseHumanizer");
                {
                    humanizerDelay.AddItem(
                        new MenuItem("CleanseMinDelay", "Minimum Delay (MS)").SetValue(new Slider(500, 0, 1000)));
                    humanizerDelay.AddItem(
                        new MenuItem("CleanseMaxDelay", "Maximum Delay (MS)").SetValue(new Slider(800, 0, 1500)));

                    humanizerDelay.Item("CleanseMaxDelay").ValueChanged +=
                        delegate(object sender, OnValueChangeEventArgs args)
                            {
                                if (args.GetNewValue<Slider>().Value
                                    < this.Menu.Item("CleanseMinDelay").GetValue<Slider>().Value)
                                {
                                    args.Process = false;
                                }
                            };
                }

                this.Menu.AddSubMenu(humanizerDelay);

                this.Menu.AddItem(new MenuItem("CleanseActivated", "Use Cleanse").SetValue(true));
                this.Menu.AddItem(new MenuItem("seperator211", ""));
                foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
                { 
                    this.Menu.AddItem(new MenuItem("cleanseon" + x.ChampionName, "Use for " + x.ChampionName))
                        .SetValue(true);
                }
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
            foreach (var item in Items.OrderBy(x => x.Priority))
            {
                if (!item.WorksOn.Any(x => buff.Type.HasFlag(x)))
                {
                    continue;
                }

                if (!(ally.IsMe || item.WorksOnAllies))
                {
                    continue;
                }

                if (!item.Spell.IsReady() || !item.Spell.IsInRange(ally) || item.Spell.Slot == SpellSlot.Unknown)
                {
                    continue;
                }

                return item.Spell;
            }

            return null;
        }

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void GameOnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsInvulnerable || Player.HasBuffOfType(BuffType.SpellImmunity)
                || Player.HasBuffOfType(BuffType.Invulnerability))
            {
                return;
            }

            if (!this.Menu.Item("CleanseActivated").IsActive())
            {
                return;
            }

            foreach (var ally in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly && x.IsValidTarget(800f, false)))
            {
                var ally1 = ally;
                foreach (var spell in Spells.Where(x => ally1.HasBuff(x.Name)))
                {
                    if (
                        !this.Menu.Item(spell.MenuName != null ? spell.MenuName.Replace(" ", string.Empty) : spell.Name)
                             .IsActive())
                    {
                        continue;
                    }

                    var buff = ally.GetBuff(spell.Name);

                    if (
                        !((Obj_AI_Hero)buff.Caster).ChampionName.Equals(
                            spell.Champion,
                            StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(spell.Champion))
                    {
                        return;
                    }

                    var item = GetBestCleanseItem(ally, buff);

                    if (item == null)
                    {
                        continue;
                    }

                    var ally2 = ally;
                    Utility.DelayAction.Add(
                        spell.CleanseTimer
                        + Random.Next(
                            this.Menu.Item("CleanseMinDelay").GetValue<Slider>().Value,
                            this.Menu.Item("CleanseMaxDelay").GetValue<Slider>().Value),
                        () =>
                            {
                                if (!ally2.HasBuff(buff.Name) || ally2.IsInvulnerable || !this.Menu.Item(
                                    $"cleanseon{ally2.ChampionName}").IsActive())
                                {
                                    return;
                                }

                                if (item.Slot != SpellSlot.Unknown)
                                {
                                    Player.Spellbook.CastSpell(item.Slot, ally2);
                                }
                            });
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
            public GetSlotDelegate Slot { get; set; }

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
            ///     The buff types the spell works on.
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