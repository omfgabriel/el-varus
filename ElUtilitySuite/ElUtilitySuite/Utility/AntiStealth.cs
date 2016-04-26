﻿namespace ElUtilitySuite.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal class AntiStealth : IPlugin
    {
        #region Static Fields

        /// <summary>
        ///     The random
        /// </summary>
        private static Random random;

        #endregion

        #region Fields

        /// <summary>
        ///     Rengar
        /// </summary>
        private Obj_AI_Hero rengar;

        #endregion

        #region Constructors and Destructors

        static AntiStealth()
        {
            Spells = new List<AntiStealthSpell>
                         {
                             new AntiStealthSpell { ChampionName = "Akali", SDataName = "akalismokebomb" },
                             new AntiStealthSpell { ChampionName = "Vayne", SDataName = "vayneinquisition" },
                             new AntiStealthSpell { ChampionName = "Twitch", SDataName = "hideinshadows" },
                             new AntiStealthSpell { ChampionName = "Shaco", SDataName = "deceive" },
                             new AntiStealthSpell { ChampionName = "Monkeyking", SDataName = "monkeykingdecoy" },
                             new AntiStealthSpell { ChampionName = "Khazix", SDataName = "khazixrlong" },
                             new AntiStealthSpell { ChampionName = "Khazix", SDataName = "khazixr" }
                         };
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     Gets an anti stealth item.
        /// </summary>
        /// <returns></returns>
        private delegate Items.Item GetAntiStealthItemDelegate();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        public static List<AntiStealthSpell> Spells { get; set; }

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
            var predicate = new Func<Menu, bool>(x => x.Name == "MiscMenu");
            var menu = !rootMenu.Children.Any(predicate)
                           ? rootMenu.AddSubMenu(new Menu("Misc", "MiscMenu"))
                           : rootMenu.Children.First(predicate);

            var protectMenu = menu.AddSubMenu(new Menu("Anti-Stealth", "AntiStealth"));
            {
                protectMenu.AddItem(new MenuItem("AntiStealthActive", "Place Pink Ward on Unit Stealth").SetValue(true));
            }

            this.Menu = protectMenu;
            random = new Random(Environment.TickCount);
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            this.Items = new List<AntiStealthRevealItem>
                             {
                                 new AntiStealthRevealItem { GetItem = () => ItemData.Vision_Ward.GetItem() },
                                 new AntiStealthRevealItem
                                     { GetItem = () => ItemData.Greater_Vision_Totem_Trinket.GetItem() }
                             };

            this.rengar = HeroManager.Enemies.Find(x => x.ChampionName.ToLower() == "rengar");

            GameObject.OnCreate += this.GameObject_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += this.OnProcessSpellCast;
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            try
            {
                if (!sender.IsEnemy || !this.Menu.Item("AntiStealthActive").IsActive())
                {
                    return;
                }

                if (this.rengar != null)
                {
                    if (sender.Name.Contains("Rengar_Base_R_Alert"))
                    {
                        if (this.Player.HasBuff("rengarralertsound") && !this.rengar.IsVisible && !this.rengar.IsDead)
                        {
                            var hero = (Obj_AI_Hero)sender;

                            if (hero.IsAlly)
                            {
                                return;
                            }

                            var item =
                                this.Items.Select(x => x.Item).FirstOrDefault(x => x.IsInRange(hero) && x.IsReady());
                            if (item != null)
                            {
                                Utility.DelayAction.Add(random.Next(100, 1000), () => item.Cast(this.Player.Position));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                var hero = sender as Obj_AI_Hero;
                if (!sender.IsEnemy || hero == null || !this.Menu.Item("AntiStealthActive").IsActive())
                {
                    return;
                }

                var stealthChampion =
                    Spells.FirstOrDefault(x => x.SDataName.Equals(args.SData.Name, StringComparison.OrdinalIgnoreCase));

                if (stealthChampion != null)
                {
                    var item = this.Items.Select(x => x.Item).FirstOrDefault(x => x.IsInRange(hero) && x.IsReady());
                    if (item != null)
                    {
                        var spellCastPosition = this.Player.Distance(args.End) > 600 ? this.Player.Position : args.End;
                        Utility.DelayAction.Add(random.Next(100, 1000), () => item.Cast(spellCastPosition));
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
        ///     Represents a spell that an item should be casted on.
        /// </summary>
        public class AntiStealthSpell
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the name of the champion.
            /// </summary>
            /// <value>
            ///     The name of the champion.
            /// </value>
            public string ChampionName { get; set; }

            /// <summary>
            ///     Gets or sets the name of the s data.
            /// </summary>
            /// <value>
            ///     The name of the s data.
            /// </value>
            public string SDataName { get; set; }

            #endregion
        }

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