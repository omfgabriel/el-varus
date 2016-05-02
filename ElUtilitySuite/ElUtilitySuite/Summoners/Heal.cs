namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    public class Heal : IPlugin
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the heal spell.
        /// </summary>
        /// <value>
        ///     The heal spell.
        /// </value>
        public Spell HealSpell { get; set; }

        /// <summary>
        /// The Menu
        /// </summary>
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
            if (this.Player.GetSpellSlot("summonerheal") == SpellSlot.Unknown)
            {
                return;
            }

            var predicate = new Func<Menu, bool>(x => x.Name == "SummonersMenu");
            var menu = !rootMenu.Children.Any(predicate)
                           ? rootMenu.AddSubMenu(new Menu("Summoners", "SummonersMenu"))
                           : rootMenu.Children.First(predicate);

            var healMenu = menu.AddSubMenu(new Menu("Heal", "Heal"));
            {
                healMenu.AddItem(new MenuItem("Heal.Activated", "Heal").SetValue(true));
                healMenu.AddItem(new MenuItem("PauseHealHotkey", "Don't use heal key").SetValue(new KeyBind('L', KeyBindType.Press)));
                healMenu.AddItem(new MenuItem("Heal.HP", "Health percentage").SetValue(new Slider(20, 1)));
                healMenu.AddItem(new MenuItem("Heal.Damage", "Heal on % incoming damage").SetValue(new Slider(20, 1)));
                healMenu.AddItem(new MenuItem("seperator21", ""));
                foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
                {
                    healMenu.AddItem(new MenuItem("healon" + x.ChampionName, "Use for " + x.ChampionName))
                        .SetValue(true);
                }
            }

            this.Menu = healMenu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            try
            {
                var healSlot = this.Player.GetSpellSlot("summonerheal");

                if (healSlot == SpellSlot.Unknown)
                {
                    return;
                }

                this.HealSpell = new Spell(healSlot, 550);

                AttackableUnit.OnDamage += this.AttackableUnit_OnDamage;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Methods

        private void AttackableUnit_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            try
            {
                if (!this.Menu.Item("Heal.Activated").IsActive())
                {
                    return;
                }

                if (this.Menu.Item("PauseHealHotkey").GetValue<KeyBind>().Active)
                {
                    return;
                }

                var source = ObjectManager.GetUnitByNetworkId<GameObject>(args.SourceNetworkId);
                var obj = ObjectManager.GetUnitByNetworkId<GameObject>(args.TargetNetworkId);

                if (obj.Type != GameObjectType.obj_AI_Hero || source.Type != GameObjectType.obj_AI_Hero)
                {
                    return;
                }

                var hero = (Obj_AI_Hero)obj;

                if (hero.IsEnemy || (!hero.IsMe && !this.HealSpell.IsInRange(obj))
                    || !this.Menu.Item(string.Format("healon{0}", hero.ChampionName)).IsActive())
                {
                    return;
                }

                if (((int)(args.Damage / hero.Health) > this.Menu.Item("Heal.Damage").GetValue<Slider>().Value)
                    || (hero.HealthPercent < this.Menu.Item("Heal.HP").GetValue<Slider>().Value))
                {
                    this.Player.Spellbook.CastSpell(this.HealSpell.Slot);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion
    }
}
