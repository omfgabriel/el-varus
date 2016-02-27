namespace ElUtilitySuite.Items.OffensiveItems
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Botrk : Item
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
                return ItemId.Blade_of_the_Ruined_King;
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
                return "Blade of the Ruined King";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            this.Menu.AddItem(new MenuItem("UseBotrkCombo", "Use on Combo").SetValue(true));
            this.Menu.AddItem(new MenuItem("BotrkMyHp", "Use on My Hp %").SetValue(new Slider(100)));
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return this.Menu.Item("UseBotrkCombo").IsActive() && this.ComboModeActive
                   && this.Player.HealthPercent < this.Menu.Item("BotrkMyHp").GetValue<Slider>().Value;
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            Items.UseItem(
                (int)this.Id, TargetSelector.GetTarget(550, TargetSelector.DamageType.Physical));
        }

        #endregion
    }
}
