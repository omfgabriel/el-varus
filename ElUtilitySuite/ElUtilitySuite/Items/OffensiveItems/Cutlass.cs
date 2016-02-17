namespace ElUtilitySuite.Items.OffensiveItems
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Cutlass : Item
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
                return ItemId.Bilgewater_Cutlass;
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
                return "Bilgewater Cutlass";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            this.Menu.AddItem(new MenuItem("UseCutlassCombo", "Use on Combo").SetValue(true));
            this.Menu.AddItem(new MenuItem("CutlassEnemyHp", "Use on Enemy Hp %").SetValue(new Slider(70)));
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return this.Menu.Item("UseCutlassCombo").IsActive() && this.ComboModeActive
                   && HeroManager.Enemies.Any(
                       x =>
                       x.HealthPercent < this.Menu.Item("CutlassEnemyHp").GetValue<Slider>().Value
                       && x.Distance(this.Player) < 550);
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            Items.UseItem(
                (int)this.Id,
                HeroManager.Enemies.First(
                    x =>
                    x.HealthPercent < this.Menu.Item("CutlassEnemyHp").GetValue<Slider>().Value
                    && x.Distance(this.Player) < 550));
        }

        #endregion
    }
}