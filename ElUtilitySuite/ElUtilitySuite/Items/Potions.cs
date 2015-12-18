﻿namespace ElUtilitySuite.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ElUtilitySuite.Utility;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal class Potions : IPlugin
    {
        #region Delegates

        /// <summary>
        ///     Gets an health item
        /// </summary>
        /// <returns></returns>
        private delegate Items.Item GetHealthItemDelegate();

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private static Obj_AI_Hero Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        /// <summary>
        ///     Gets the set player hp menu value.
        /// </summary>
        /// <value>
        ///     The player hp hp menu value.
        /// </value>
        private static int PlayerHp
        {
            get
            {
                return InitializeMenu.Menu.Item("Potions.Player.Health").GetValue<Slider>().Value;
            }
        }

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        private List<HealthItem> Items { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            this.Items = new List<HealthItem>
                             {
                                 new HealthItem { GetItem = () => ItemData.Health_Potion.GetItem() },
                                 new HealthItem { GetItem = () => ItemData.Total_Biscuit_of_Rejuvenation2.GetItem() },
                                 new HealthItem { GetItem = () => ItemData.Refillable_Potion.GetItem() },
                                 new HealthItem { GetItem = () => ItemData.Hunters_Potion.GetItem() },
                                 new HealthItem { GetItem = () => ItemData.Corrupting_Potion.GetItem() },
                             };

            Game.OnUpdate += this.OnUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the player buffs
        /// </summary>
        /// <value>
        ///     The player buffs
        /// </value>
        private static bool CheckPlayerBuffs()
        {
            return Player.HasBuff("RegenerationPotion") || Player.HasBuff("ItemMiniRegenPotion")
                   || Player.HasBuff("ItemCrystalFlask") || Player.HasBuff("ItemCrystalFlaskJungle")
                   || Player.HasBuff("ItemDarkCrystalFlask");
        }

        private void OnUpdate(EventArgs args)
        {
            try
            {
                if (!InitializeMenu.Menu.Item("Potions.Activated").IsActive() || Entry.Player.InFountain()
                    || Entry.Player.IsRecalling() || Entry.Player.IsDead)
                {
                    return;
                }

                if (Player.HealthPercent < PlayerHp)
                {
                    if (CheckPlayerBuffs())
                    {
                        return;
                    }

                    var item = this.Items.Select(x => x.Item).FirstOrDefault(x => x.IsReady() && x.IsOwned());

                    if (item != null)
                    {
                        item.Cast();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        /// <summary>
        ///     Represents an item that can heal
        /// </summary>
        private class HealthItem
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the get item.
            /// </summary>
            /// <value>
            ///     The get item.
            /// </value>
            public GetHealthItemDelegate GetItem { get; set; }

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