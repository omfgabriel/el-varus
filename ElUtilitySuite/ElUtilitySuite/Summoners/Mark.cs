namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    public class Mark : IPlugin
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the Mark spell
        /// </summary>
        /// <value>
        ///     The Mark spell.
        /// </value>
        public Spell MarkSpell { get; set; }

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private Obj_AI_Hero Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            if (this.Player.GetSpellSlot("summonersnowball") == SpellSlot.Unknown)
            {
                return;
            }

            var snowballMenu = rootMenu.AddSubMenu(new Menu("ARAM Snowball", "Snowball"));
            {
                snowballMenu.AddItem(new MenuItem("Snowball.Activated", "Snowball activated").SetValue(true));
                foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
                {
                    snowballMenu.AddItem(new MenuItem("snowballon" + x.ChampionName, "Use for " + x.ChampionName))
                        .SetValue(true);
                }
            }

            this.Menu = snowballMenu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            if (Game.MapId == GameMapId.HowlingAbyss)
            {
                return;
            }

            var markSlot = this.Player.GetSpellSlot("summonersnowball");

            if (markSlot == SpellSlot.Unknown)
            {
                return;
            }

            this.MarkSpell = new Spell(markSlot, 1400f);
            this.MarkSpell.SetSkillshot(0f, 60f, 1500f, true, SkillshotType.SkillshotLine);

            Game.OnUpdate += this.GameOnUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void GameOnUpdate(EventArgs args)
        {
            if (!this.Menu.Item("Snowball.Activated").IsActive())
            {
                return;
            }

            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy && x.IsValidTarget(1400f)))
            {
                if (this.Menu.Item(string.Format("snowballon{0}", enemy.ChampionName)).IsActive())
                {
                    this.MarkSpell.CastIfHitchanceEquals(enemy, HitChance.High);
                }
            }
        }

        #endregion
    }
}
