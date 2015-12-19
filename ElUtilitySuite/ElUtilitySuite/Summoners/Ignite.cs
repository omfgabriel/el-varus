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
        #region Static Fields

        public static Spell igniteSpell;

        public static SpellSlot summonerDot;

        private static SpellDataInst slot1;

        private static SpellDataInst slot2;

        #endregion

        #region Public Properties

        public static Menu Menu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            if (Entry.Player.GetSpellSlot("summonerdot") == SpellSlot.Unknown)
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
                slot1 = Entry.Player.Spellbook.GetSpell(SpellSlot.Summoner1);
                slot2 = Entry.Player.Spellbook.GetSpell(SpellSlot.Summoner2);

                //Soon riot will introduce multiple ignote, mark my words.
                var igniteNames = new[] { "summonerdot" };

                if (igniteNames.Contains(slot1.Name))
                {
                    igniteSpell = new Spell(SpellSlot.Summoner1);
                    summonerDot = SpellSlot.Summoner1;
                }
                else if (igniteNames.Contains(slot2.Name))
                {
                    igniteSpell = new Spell(SpellSlot.Summoner2);
                    summonerDot = SpellSlot.Summoner2;
                }
                else
                {
                    Console.WriteLine("You don't have ignite faggot");
                    return;
                }

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
                    hero.IsValidTarget(550) && hero.IgniteCheck() && !hero.IsZombie
                    && Entry.Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite) >= hero.Health);

            if (kSableEnemy != null)
            {
                Entry.Player.Spellbook.CastSpell(summonerDot, kSableEnemy);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Entry.Player.IsDead)
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