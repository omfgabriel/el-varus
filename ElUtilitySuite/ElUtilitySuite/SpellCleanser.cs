namespace ElUtilitySuite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     Casts Cleanse on dangerous spells.
    /// </summary>
    public class SpellCleanser
    {
        #region Static Fields

        /// <summary>
        ///     The Cleanse item
        /// </summary>
        private static Items.Item cleanseItem;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes the <see cref="Cleanse" /> class.
        /// </summary>
        static SpellCleanser()
        {
            #region Spells Init

            Spells = new List<CleanseSpell>()
                         {
                             new CleanseSpell
                                 {
                                     ChampionName = "ashe", SDataName = "enchantedcrystalarrow",
                                     MissileName = "enchantedcrystalarrow", Delay = 250, MissileSpeed = 1600,
                                     CastRange = 20000f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "malzahar", SDataName = "alzaharnethergrasp", MissileName = "",
                                     Delay = 250, MissileSpeed = int.MaxValue, CastRange = 700f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "zed", SDataName = "zedr", MissileName = "", Delay = 250,
                                     MissileSpeed = int.MaxValue, CastRange = 850f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "fiddlesticks", SDataName = "terrify", MissileName = "", Delay = 250,
                                     MissileSpeed = int.MaxValue, CastRange = 575f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "shaco", SDataName = "jackinthebox", MissileName = "", Delay = 250,
                                     MissileSpeed = 1450, CastRange = 0f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "shen", SDataName = "shenshadowdash", MissileName = "shenshadowdash",
                                     Delay = 250, MissileSpeed = 1250, CastRange = 600f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "twistedfate", SDataName = "goldcardpreattack", MissileName = "",
                                     Delay = 250, MissileSpeed = int.MaxValue, CastRange = 600f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "evelynn", SDataName = "evelynnr", MissileName = "evelynnr",
                                     Delay = 250, MissileSpeed = int.MaxValue, CastRange = 900f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "hecarim", SDataName = "hecarimult", MissileName = "", Delay = 50,
                                     MissileSpeed = 1200, CastRange = 1350f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "lissandra", SDataName = "lissandrar", MissileName = "", Delay = 250,
                                     MissileSpeed = int.MaxValue, CastRange = 550f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "lissandra", SDataName = "lissandraw", MissileName = "", Delay = 250,
                                     MissileSpeed = int.MaxValue, CastRange = 450f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "leona", SDataName = "leonasolarflare",
                                     MissileName = "leonasolarflare", Delay = 1200, MissileSpeed = int.MaxValue,
                                     CastRange = 1200f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "leona", SDataName = "leonashieldofdaybreak", MissileName = "",
                                     Delay = 250, MissileSpeed = int.MaxValue, CastRange = 215f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "elise", SDataName = "elisehumane", MissileName = "elisehumane",
                                     Delay = 250, MissileSpeed = 1600, CastRange = 1075f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "cassiopeia", SDataName = "cassiopeiapetrifyinggaze",
                                     MissileName = "cassiopeiapetrifyinggaze", Delay = 350, MissileSpeed = int.MaxValue,
                                     CastRange = 875f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "braum", SDataName = "braumqmissle", MissileName = "", Delay = 250,
                                     MissileSpeed = 1200, CastRange = 1100f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "braum", SDataName = "braumq", MissileName = "braumqmissile",
                                     Delay = 250, MissileSpeed = 1200, CastRange = 1100f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "syndra", SDataName = "syndrar", MissileName = "", Delay = 450,
                                     MissileSpeed = 1250, CastRange = 675f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "ahri", SDataName = "ahriseduce", MissileName = "ahriseducemissile",
                                     Delay = 250, MissileSpeed = 1550, CastRange = 975f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "amumu", SDataName = "curseofthesadmummy", MissileName = "",
                                     Delay = 250, MissileSpeed = int.MaxValue, CastRange = 560f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "amumu", SDataName = "bandagetoss",
                                     MissileName = "sadmummybandagetoss", Delay = 250, MissileSpeed = 2000,
                                     CastRange = 1100f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "skarner", SDataName = "skarnerimpale", MissileName = "", Delay = 350,
                                     MissileSpeed = int.MaxValue, CastRange = 350f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "sejuani", SDataName = "sejuaniglacialprisoncast",
                                     MissileName = "sejuaniglacialprison", Delay = 250, MissileSpeed = 1600,
                                     CastRange = 1200f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "orianna", SDataName = "orianadetonatecommand",
                                     MissileName = "orianadetonatecommand", Delay = 500, MissileSpeed = int.MaxValue,
                                     CastRange = 425f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "nocturne", SDataName = "nocturneunspeakablehorror", MissileName = "",
                                     Delay = 250, MissileSpeed = int.MaxValue, CastRange = 350f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "nami", SDataName = "namiq", MissileName = "namiqmissile", Delay = 250,
                                     MissileSpeed = 1750, CastRange = 875f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "nami", SDataName = "namir", MissileName = "namirmissile", Delay = 250,
                                     MissileSpeed = 1200, CastRange = 2550f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "morgana", SDataName = "darkbindingmissile",
                                     MissileName = "darkbindingmissile", Delay = 250, MissileSpeed = 1200,
                                     CastRange = 1175f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "lux", SDataName = "luxlightbinding",
                                     MissileName = "luxlightbindingmis", Delay = 250, MissileSpeed = 1200,
                                     CastRange = 1300f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "mordekaiser", SDataName = "mordekaiserchildrenofthegrave",
                                     MissileName = "", Delay = 250, MissileSpeed = int.MaxValue, CastRange = 850f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "vladimir", SDataName = "vladimirhemoplague", MissileName = "",
                                     Delay = 250, MissileSpeed = int.MaxValue, CastRange = 875f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "fiora", SDataName = "fiorar", MissileName = "", Delay = 150,
                                     MissileSpeed = int.MaxValue, CastRange = 500f
                                 },
                             new CleanseSpell
                                 {
                                     ChampionName = "fizz", SDataName = "fizzmarinerdoom",
                                     MissileName = "fizzmarinerdoommissile", Delay = 250, MissileSpeed = 1300,
                                     CastRange = 1275f
                                 },
                         };

            #endregion
        }

        #endregion

        #region Public Properties

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

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public static void Init()
        {
            cleanseItem = new Items.Item(3140);

            // 3140 = Quicksilver
            // 3137 = Dervish Blade
            // 3193 = Mercurial Scimitar
            // 3222 = Mikael's Crucible

            GameObject.OnCreate += GameObjectOnCreate;
            Obj_AI_Base.OnProcessSpellCast += ObjAiBaseOnProcessSpellCast;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when a game object is created.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void GameObjectOnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid<MissileClient>() || sender.IsAlly)
            {
                return;
            }

            var missile = (MissileClient)sender;
            var sdata =
                Spells.FirstOrDefault(
                    x =>
                    missile.SData.Name.ToLower().Equals(x.MissileName)
                    || missile.SData.Name.ToLower().Equals(x.SDataName));

            // Not in database
            if (sdata == null)
            {
                return;
            }
        }

        /// <summary>
        ///     Fired when the game processes a spell cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        private static void ObjAiBaseOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsAlly)
            {
                return;
            }

            var spellData =
                Spells.FirstOrDefault(
                    x => x.SDataName == args.SData.Name.ToLower() || x.MissileName == args.SData.Name.ToLower());

            if (spellData == null)
            {
                return;
            }
        }

        #endregion

        public class CleanseSpell
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the cast range.
            /// </summary>
            /// <value>
            ///     The cast range.
            /// </value>
            public float CastRange { get; set; }

            /// <summary>
            ///     Gets or sets the name of the champion.
            /// </summary>
            /// <value>
            ///     The name of the champion.
            /// </value>
            public string ChampionName { get; set; }

            /// <summary>
            ///     Gets or sets the delay.
            /// </summary>
            /// <value>
            ///     The delay.
            /// </value>
            public float Delay { get; set; }

            /// <summary>
            ///     Gets or sets the name of the missile.
            /// </summary>
            /// <value>
            ///     The name of the missile.
            /// </value>
            public string MissileName { get; set; }

            /// <summary>
            ///     Gets or sets the missile speed.
            /// </summary>
            /// <value>
            ///     The missile speed.
            /// </value>
            public int MissileSpeed { get; set; }

            /// <summary>
            ///     Gets or sets the name of the s data.
            /// </summary>
            /// <value>
            ///     The name of the s data.
            /// </value>
            public string SDataName { get; set; }

            #endregion
        }
    }
}