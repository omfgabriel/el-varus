namespace ElUtilitySuite.Items.DefensiveItems
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal class Seraphs : Item
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public Seraphs()
        {
            AttackableUnit.OnDamage += this.AttackableUnit_OnDamage;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public override ItemId Id
        {
            get
            {
                return (ItemId)3040;
            }
        }

        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        /// <value>
        ///     The name of the item.
        /// </value>
        public override string Name
        {
            get
            {
                return "Seraph's embrace";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            this.Menu.AddItem(new MenuItem("UseSeraphsCombo", "Activated").SetValue(true));
            this.Menu.AddItem(new MenuItem("Mode", "Activation mode: ")).SetValue(new StringList(new[] { "Use always", "Use in combo" }, 1));
            this.Menu.AddItem(new MenuItem("Seraphs.HP", "Health percentage").SetValue(new Slider(20, 1)));
            this.Menu.AddItem(new MenuItem("Seraphs.Damage", "Incoming damage percentage").SetValue(new Slider(20, 1)));
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AttackableUnit_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            try
            {
                if (!ItemData.Seraphs_Embrace.GetItem().IsOwned() || !this.Menu.Item("UseSeraphsCombo").IsActive())
                {
                    return;
                }

                if (this.Menu.Item("Mode").GetValue<StringList>().SelectedIndex == 1 && !this.ComboModeActive)
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
                if (hero.IsEnemy)
                {
                    return;
                }

                if (((int)(args.Damage / hero.Health) > this.Menu.Item("Seraphs.Damage").GetValue<Slider>().Value)
                   || (hero.HealthPercent < this.Menu.Item("Seraphs.HP").GetValue<Slider>().Value))
                {
                    Items.UseItem((int)this.Id);
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