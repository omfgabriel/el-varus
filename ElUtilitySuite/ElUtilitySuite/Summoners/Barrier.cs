namespace ElUtilitySuite.Summoners
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    public class Barrier : IPlugin
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the barrier spell.
        /// </summary>
        /// <value>
        ///     The barrier spell.
        /// </value>
        public Spell BarrierSpell { get; set; }

        /// <summary>
        /// Gets or sets the menu.
        /// </summary>
        /// <value>
        /// The menu.
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
            if (this.Player.GetSpellSlot("summonerbarrier") == SpellSlot.Unknown)
            {
                return;
            }

            var barrierMenu = rootMenu.AddSubMenu(new Menu("Barrier", "Barrier"));
            {
                barrierMenu.AddItem(new MenuItem("Barrier.Activated", "Barrier activated").SetValue(true));
                barrierMenu.AddItem(new MenuItem("Barrier.HP", "Barrier percentage").SetValue(new Slider(20, 1)));
                barrierMenu.AddItem(
                    new MenuItem("Barrier.Damage", "Barrier on damage dealt %").SetValue(new Slider(20, 1)));
            }

            this.Menu = barrierMenu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            var barrierSlot = this.Player.GetSpellSlot("summonerbarrier");

            if (barrierSlot == SpellSlot.Unknown)
            {
                return;
            }

            this.BarrierSpell = new Spell(barrierSlot, 550);

            AttackableUnit.OnDamage += this.AttackableUnit_OnDamage;
        }

        #endregion

        #region Methods

        private void AttackableUnit_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (!this.Menu.Item("Barrier.Activated").IsActive())
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

            if (!hero.IsMe)
            {
                return;
            }

            if (((int)(args.Damage / this.Player.MaxHealth * 100)
                 > this.Menu.Item("Barrier.Damage").GetValue<Slider>().Value
                 || this.Player.HealthPercent < this.Menu.Item("Barrier.HP").GetValue<Slider>().Value)
                && this.Player.CountEnemiesInRange(1000) >= 1)
            {
                this.BarrierSpell.Cast();
            }
        }

        #endregion
    }
}
