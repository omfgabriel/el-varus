namespace ElUtilitySuite.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal class AntiStealth : IPlugin
    {
        #region Delegates

        /// <summary>
        ///     Gets an anti stealth item.
        /// </summary>
        /// <returns></returns>
        private delegate Items.Item GetAntiStealthItemDelegate();

        #endregion

        #region Public Properties

        public Menu Menu { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        private List<AntiStealthRevealItem> Items { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var protectMenu = rootMenu.AddSubMenu(new Menu("Anti-Stealth", "AntiStealth"));
            {
                protectMenu.AddItem(new MenuItem("AntiStealthActive", "Place Pink Ward on Unit Stealth").SetValue(true));
            }

            this.Menu = protectMenu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            this.Items = new List<AntiStealthRevealItem>
                             {
                                 new AntiStealthRevealItem { GetItem = () => ItemData.Vision_Ward.GetItem() },
                                 new AntiStealthRevealItem { GetItem = () => ItemData.Greater_Vision_Totem_Trinket.GetItem() }
                             };

            GameObject.OnIntegerPropertyChange += this.GameObject_OnIntegerPropertyChange;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when an integral property is changed in a GameObject.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectIntegerPropertyChangeEventArgs" /> instance containing the event data.</param>
        private void GameObject_OnIntegerPropertyChange(
            GameObject sender,
            GameObjectIntegerPropertyChangeEventArgs args)
        {
            if (!(sender is Obj_AI_Hero) || args.Property != "ActionState" || !this.Menu.Item("AntiStealthActive").IsActive())
            {
                return;
            }

            var hero = (Obj_AI_Hero)sender;

            if (hero.IsAlly)
            {
                return;
            }

            if (((GameObjectCharacterState)args.OldValue).HasFlag(GameObjectCharacterState.IsStealth)
                || !((GameObjectCharacterState)args.NewValue).HasFlag(GameObjectCharacterState.IsStealth))
            {
                return;
            }

            var item = this.Items.Select(x => x.Item).FirstOrDefault(x => x.IsInRange(hero) && x.IsReady());

            if (item != null)
            {
                item.Cast(hero.Position.Randomize(10, 300));
            }
        }

        #endregion

        /// <summary>
        ///     Represents an item that can reveal stealthed units.
        /// </summary>
        private class AntiStealthRevealItem
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the get item.
            /// </summary>
            /// <value>
            ///     The get item.
            /// </value>
            public GetAntiStealthItemDelegate GetItem { get; set; }

            /// <summary>
            ///     Gets the item.
            /// </summary>
            /// <value>
            ///     The item.
            /// </value>
            public Items.Item Item
            {
                get
                {
                    return this.GetItem();
                }
            }

            #endregion
        }
    }
}