namespace ElUtilitySuite.Summoners
{
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

            var healMenu = rootMenu.AddSubMenu(new Menu("Heal", "Heal"));
            {
                healMenu.AddItem(new MenuItem("Heal.Activated", "Heal").SetValue(true));
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
            var healSlot = this.Player.GetSpell(SpellSlot.Summoner1).Name == "summonerheal"
                               ? SpellSlot.Summoner1
                               : this.Player.GetSpell(SpellSlot.Summoner2).Name == "summonerheal"
                                     ? SpellSlot.Summoner2
                                     : SpellSlot.Unknown;

            if (healSlot == SpellSlot.Unknown)
            {
                return;
            }

            this.HealSpell = new Spell(healSlot, 550);

            AttackableUnit.OnDamage += this.AttackableUnit_OnDamage;
        }

        #endregion

        #region Methods

        private void AttackableUnit_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (!this.Menu.Item("Heal.Activated").IsActive())
            {
                return;
            }

            var obj = ObjectManager.GetUnitByNetworkId<GameObject>(args.TargetNetworkId);

            if (obj.Type != GameObjectType.obj_AI_Hero)
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

        #endregion
    }
}