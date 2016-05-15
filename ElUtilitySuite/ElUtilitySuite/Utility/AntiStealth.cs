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


        /// <summary>
        ///     Vayne
        /// </summary>
        private Obj_AI_Hero vayne;

        #endregion

        #region Constructors and Destructors

        static AntiStealth()
        {
            // add akali health here
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
        ///     A delegate that returns a <see cref="SpellSlot" />
        /// </summary>
        /// <returns>
        ///     <see cref="SpellSlot" />
        /// </returns>
        public delegate SpellSlot GetSlotDelegate();

        /// <summary>
        ///     The Vayne buff stealth end time
        /// </summary>
        public float VayneBuffEndTime = 0;

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        public static List<AntiStealthSpell> Spells { get; set; }

        /// <summary>
        ///     Gets or sets the menu
        /// </summary>
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
                                 new AntiStealthRevealItem
                                     {
                                     Slot = () =>
                                        {
                                            var slots = ItemData.Vision_Ward.GetItem().Slots;
                                            return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                        },
                                         Priority = 0
                                     },
                                 new AntiStealthRevealItem
                                     {
                                     Slot = () =>
                                        {
                                            var slots = ItemData.Greater_Vision_Totem_Trinket.GetItem().Slots;
                                            return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                        },
                                         Priority = 1
                                     }
                             };

            this.Items = this.Items.OrderBy(x => x.Priority).ToList();

            this.rengar = HeroManager.Enemies.Find(x => x.ChampionName.ToLower() == "rengar");

            GameObject.OnCreate += this.GameObject_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += this.OnProcessSpellCast;
            Game.OnUpdate += this.OnUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            try
            {
                this.vayne = HeroManager.Enemies.Find(x => x.ChampionName.ToLower() == "vayne");
                if (this.vayne == null)
                {
                    return;
                }
                    
                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy &&
                    x.ChampionName.ToLower().Contains("vayne") &&
                    x.Buffs.Any(y => y.Name == "VayneInquisition")))
                {
                    this.VayneBuffEndTime = hero.Buffs.First(x => x.Name == "VayneInquisition").EndTime;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        /// <summary>
        ///     Gets the best ward item.
        /// </summary>
        /// <returns></returns>
        private Spell GetBestWardItem()
        {
            foreach (var item in this.Items.OrderBy(x => x.Priority))
            {
                if (!item.Spell.IsReady() || item.Spell.Slot == SpellSlot.Unknown)
                {
                    continue;
                }

                return item.Spell;
            }

            return null;
        }

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
                        if (this.Player.HasBuff("rengarralertsound") && !this.rengar.IsVisible && !this.rengar.IsDead && 
                            this.Player.Distance(sender.Position) < 1700)
                        {
                            var hero = (Obj_AI_Hero)sender;

                            if (hero.IsAlly)
                            {
                                return;
                            }

                            var item = this.GetBestWardItem();
                            if (item != null)
                            {
                                Utility.DelayAction.Add(
                                   random.Next(100, 1000),
                                   () =>
                                   this.Player.Spellbook.CastSpell(item.Slot, this.Player.Position));
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

                if (this.Player.Distance(sender.Position) > 800)
                {
                    return;
                }

                if (args.SData.Name.ToLower().Contains("vaynetumble") && Game.Time > this.VayneBuffEndTime)
                {
                    return;
                }
                   
                var stealthChampion =
                Spells.FirstOrDefault(x => x.SDataName.Equals(args.SData.Name, StringComparison.OrdinalIgnoreCase));

                if (stealthChampion != null)
                { 
                    var item = this.GetBestWardItem();
                    if (item != null)
                    {
                        var spellCastPosition = this.Player.Distance(args.End) > 600 ? this.Player.Position : args.End;
                        Utility.DelayAction.Add(
                            random.Next(100, 1000),
                            () => this.Player.Spellbook.CastSpell(item.Slot, spellCastPosition));
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
            ///     Gets or sets the priority.
            /// </summary>
            /// <value>
            ///     The priority.
            /// </value>
            public int Priority { get; set; }

            /// <summary>
            ///     Gets or sets the spell.
            /// </summary>
            /// <value>
            ///     The spell.
            /// </value>
            public Spell Spell
            {
                get
                {
                    return new Spell(this.Slot());
                }
            }

            /// <summary>
            ///     Gets or sets the slot delegate.
            /// </summary>
            /// <value>
            ///     The slot delegate.
            /// </value>
            public GetSlotDelegate Slot { get; set; }


            #endregion
        }
    }
}