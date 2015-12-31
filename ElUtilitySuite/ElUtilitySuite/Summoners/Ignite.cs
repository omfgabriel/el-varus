namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    public static class IgniteExtensions
    {
        #region Public Methods and Operators

        public static bool IgniteCheck(this Obj_AI_Base hero)
        {
            if (Ignite.Menu.Item("Ignite.shieldCheck").GetValue<bool>())
            {
                return !hero.HasBuff("summonerdot") || !hero.HasBuff("summonerbarrier") || !hero.HasBuff("BlackShield")
                       || !hero.HasBuff("SivirShield") || !hero.HasBuff("BansheesVeil")
                       || !hero.HasBuff("ShroudofDarkness");
            }

            return true;
        }

        #endregion
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class Ignite : IPlugin
    {
        #region Public Properties

        public static Menu Menu { get; set; }

        #endregion

        #region Public Methods and Operators

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
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The Smitespell
        /// </value>
        public Spell IgniteSpell { get; set; }

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            if (this.Player.GetSpellSlot("summonerdot") == SpellSlot.Unknown)
            {
                return;
            }

            var igniteMenu = rootMenu.AddSubMenu(new Menu("Ignite", "Ignite"));
            {
                igniteMenu.AddItem(new MenuItem("Ignite.Activated", "Ignite").SetValue(true));
                igniteMenu.AddItem(new MenuItem("Ignite.shieldCheck", "Check for shields").SetValue(true));
            }

            Menu = igniteMenu;
        }

        public void Load()
        {
            try
            {
                var igniteSlot = this.Player.GetSpell(SpellSlot.Summoner1).Name.ToLower().Contains("summonerdot")
                                    ? SpellSlot.Summoner1
                                    : this.Player.GetSpell(SpellSlot.Summoner2).Name.ToLower().Contains("summonerdot")
                                          ? SpellSlot.Summoner2
                                          : SpellSlot.Unknown;

                if (igniteSlot == SpellSlot.Unknown)
                {
                    return;
                }

                this.IgniteSpell = new Spell(igniteSlot);

                Game.OnUpdate += this.OnUpdate;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Methods

        private void IgniteKs()
        {
            if (!Menu.Item("Ignite.Activated").GetValue<bool>())
            {
                return;
            }

            var kSableEnemy =
                HeroManager.Enemies.FirstOrDefault(
                    hero =>
                    hero.IsValidTarget(550) && hero.IgniteCheck() && !hero.IsZombie && this.Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite) >= hero.Health);

            if (kSableEnemy != null)
            {
                this.Player.Spellbook.CastSpell(this.IgniteSpell.Slot, kSableEnemy);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.Player.IsDead)
            {
                return;
            }

            try
            {
                this.IgniteKs();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion
    }
}