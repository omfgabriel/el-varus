namespace ElUtilitySuite.Items.OffensiveItems
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Muramana : Item

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
                return (ItemId)3042;
            }
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        /*public Muramana()
        {
            Obj_AI_Base.OnProcessSpellCast += this.ObjAiBaseOnProcessSpellCast;
        }*/

        /// <summary>
        ///     Fired when the game processes a spell cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        /*private void ObjAiBaseOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsAlly)
            {
                return;
            }
        }*/

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
                return "Muramana";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            this.Menu.AddItem(new MenuItem("UseMuramana", "Use Muramana").SetValue(true));
            this.Menu.AddItem(
                new MenuItem("MuramanaMode", "Muramana Mode").SetValue(new StringList(new[] { "Always", "Combo" }, 1)));
            this.Menu.AddItem(new MenuItem("MuramanaMana", "Min Mana %").SetValue(new Slider(20)));
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            if (this.Menu.Item("UseMuramana").IsActive()
                && (this.Menu.Item("MuramanaMode").GetValue<StringList>().SelectedIndex == 0
                    || (this.Menu.Item("MuramanaMode").GetValue<StringList>().SelectedIndex == 1 && this.ComboModeActive))
                && !(this.Player.ManaPercent < this.Menu.Item("MuramanaMana").GetValue<Slider>().Value))
            {
                return true;
            }

            if (this.Player.HasBuff("Muramana"))
            {
                Items.UseItem((int)this.Id);
            }

            return false;
        }

        #endregion
    }
}