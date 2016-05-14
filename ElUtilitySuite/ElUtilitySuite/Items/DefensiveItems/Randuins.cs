namespace ElUtilitySuite.Items.DefensiveItems
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Randuins : Item
    {
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
                return ItemId.Randuins_Omen;
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
                return "Randuin's Omen";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            this.Menu.AddItem(new MenuItem("UseRanduinsCombo", "Use on Combo").SetValue(true));
            this.Menu.AddItem(new MenuItem("RanduinsCount", "Use on enemies hit").SetValue(new Slider(3, 1, 5)));
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return this.Menu.Item("UseRanduinsCombo").IsActive() && this.ComboModeActive
                   && this.Player.CountEnemiesInRange(500f) >= this.Menu.Item("RanduinsCount").GetValue<Slider>().Value;
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            Items.UseItem((int)this.Id);
        }

        #endregion
    }
}