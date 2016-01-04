﻿namespace ElUtilitySuite.Summoners
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    public class Exhaust
    {
        /// <summary>
        ///     Gets or sets the exhaust spell.
        /// </summary>
        /// <value>
        ///     The exhaust spell.
        /// </value>
        public Spell ExhaustSpell { get; set; }

        public Menu Menu { get; set; }

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

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            if (this.Player.GetSpellSlot("summonerexhaust") == SpellSlot.Unknown)
            {
                return;
            }

            var exhaustMenu = rootMenu.AddSubMenu(new Menu("Exhaust", "Exhaust"));
            {
                exhaustMenu.AddItem(new MenuItem("Exhaust.Activated", "Exhaust activated").SetValue(true));
                exhaustMenu.AddItem(new MenuItem("blank_line", ""));
                foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
                {
                    exhaustMenu.AddItem(new MenuItem("exhauston" + x.ChampionName, "Use for " + x.ChampionName))
                        .SetValue(true);
                }
            }

            this.Menu = exhaustMenu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            var exhaustSlot = this.Player.GetSpell(SpellSlot.Summoner1).Name == "summonerexhaust"
                                  ? SpellSlot.Summoner1
                                  : this.Player.GetSpell(SpellSlot.Summoner2).Name == "summonerexhaust"
                                        ? SpellSlot.Summoner2
                                        : SpellSlot.Unknown;

            if (exhaustSlot == SpellSlot.Unknown)
            {
                return;
            }

            this.ExhaustSpell = new Spell(exhaustSlot, 550);
        }
    }
}
