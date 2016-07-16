namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    public class Cleanse2
    {
        // : IPlugin
        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private static Obj_AI_Hero Player => ObjectManager.Player;

        /// <summary>
        ///     Gets the buff indexes handled.
        /// </summary>
        /// <value>
        ///     The buff indexes handled.
        /// </value>
        private Dictionary<int, List<int>> BuffIndexesHandled { get; } = new Dictionary<int, List<int>>();

        /// <summary>
        ///     Gets or sets the buffs to cleanse.
        /// </summary>
        /// <value>
        ///     The buffs to cleanse.
        /// </value>
        private IEnumerable<BuffType> BuffsToCleanse { get; set; }

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        private List<CleanseItem> Items { get; set; }

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        private Menu Menu { get; set; }

        /// <summary>
        ///     Gets or sets the random.
        /// </summary>
        /// <value>
        ///     The random.
        /// </value>
        private Random Random { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        public void CreateMenu(Menu rootMenu)
        {
            this.BuffsToCleanse = this.Items.SelectMany(x => x.WorksOn);

            var predicate = new Func<Menu, bool>(x => x.Name == "SummonersMenu");
            var menu = rootMenu.Children.Any(predicate)
                           ? rootMenu.Children.First(predicate)
                           : rootMenu.AddSubMenu(new Menu("Summoners", "SummonersMenu"));

            this.Menu = menu.AddSubMenu(new Menu("Cleanse RELOADED", "BuffTypeStyleCleanser"));

            var humanizerMenu = this.Menu.AddSubMenu(new Menu("Humanizer Settings", "HumanizerSettings"));
            humanizerMenu.AddItem(
                new MenuItem("MinHumanizerDelay", "Min Humanizer Delay (MS)").SetValue(new Slider(100, 0, 500)));
            humanizerMenu.AddItem(
                new MenuItem("MaxHumanizerDelay", "Max Humanizer Delay (MS)").SetValue(new Slider(150, 0, 500)));
            humanizerMenu.AddItem(new MenuItem("HumanizerEnabled", "Enabled").SetValue(false));

            var buffTypeMenu = this.Menu.AddSubMenu(new Menu("Buff Types", "BuffTypeSettings"));
            foreach (var buffType in this.BuffsToCleanse.Select(x => x.ToString()))
            {
                buffTypeMenu.AddItem(new MenuItem($"Cleanse{buffType}", buffType).SetValue(true));
            }

            this.Menu.AddItem(new MenuItem("MinDuration", "Minimum Duration (MS)").SetValue(new Slider(500, 0, 25000)));
            this.Menu.AddItem(new MenuItem("CleanseEnabled", "Enabled").SetValue(true));
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            this.Items = new List<CleanseItem>
                             {
                                 new CleanseItem
                                     {
                                         Slot =
                                             () =>
                                             Player.GetSpellSlot("summonerboost") == SpellSlot.Unknown
                                                 ? SpellSlot.Unknown
                                                 : Player.GetSpellSlot("summonerboost"), 
                                         WorksOn =
                                             new[]
                                                 {
                                                     BuffType.Blind, BuffType.Charm, BuffType.Flee, BuffType.Slow, 
                                                     BuffType.Polymorph, BuffType.Silence, BuffType.Snare, BuffType.Stun, 
                                                     BuffType.Taunt, BuffType.Damage
                                                 }, 
                                         Priority = 2
                                     }, 
                                 new CleanseItem
                                     {
                                         Slot = () =>
                                             {
                                                 var slots = ItemData.Quicksilver_Sash.GetItem().Slots;
                                                 return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                             }, 
                                         WorksOn =
                                             new[]
                                                 {
                                                     BuffType.Blind, BuffType.Charm, BuffType.Flee, 
                                                     BuffType.Slow, BuffType.Polymorph, BuffType.Silence, 
                                                     BuffType.Snare, BuffType.Stun, BuffType.Taunt, 
                                                     BuffType.Damage, BuffType.CombatEnchancer
                                                 }, 
                                         Priority = 0
                                     }, 
                                 new CleanseItem
                                     {
                                         Slot = () =>
                                             {
                                                 var slots = ItemData.Dervish_Blade.GetItem().Slots;
                                                 return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                             }, 
                                         WorksOn =
                                             new[]
                                                 {
                                                     BuffType.Blind, BuffType.Charm, BuffType.Flee, 
                                                     BuffType.Slow, BuffType.Polymorph, BuffType.Silence, 
                                                     BuffType.Snare, BuffType.Stun, BuffType.Taunt, 
                                                     BuffType.Damage, BuffType.CombatEnchancer
                                                 }, 
                                         Priority = 0
                                     }, 
                                 new CleanseItem
                                     {
                                         Slot = () =>
                                             {
                                                 var slots = ItemData.Mercurial_Scimitar.GetItem().Slots;
                                                 return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                             }, 
                                         WorksOn =
                                             new[]
                                                 {
                                                     BuffType.Blind, BuffType.Charm, BuffType.Flee, 
                                                     BuffType.Slow, BuffType.Polymorph, BuffType.Silence, 
                                                     BuffType.Snare, BuffType.Stun, BuffType.Taunt, 
                                                     BuffType.Damage, BuffType.CombatEnchancer
                                                 }, 
                                         Priority = 0
                                     }, 
                                 new CleanseItem
                                     {
                                         Slot = () =>
                                             {
                                                 var slots = ItemData.Mikaels_Crucible.GetItem().Slots;
                                                 return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                             }, 
                                         WorksOn =
                                             new[]
                                                 {
                                                     BuffType.Stun, BuffType.Snare, BuffType.Taunt, 
                                                     BuffType.Silence, BuffType.Slow, BuffType.CombatEnchancer, 
                                                     BuffType.Fear
                                                 }, 
                                         WorksOnAllies = true, Priority = 1
                                     }
                             };

            this.Random = new Random(Environment.TickCount);
            HeroManager.Enemies.ForEach(x => this.BuffIndexesHandled[x.NetworkId] = new List<int>());

            Game.OnUpdate += this.OnUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the best cleanse item.
        /// </summary>
        /// <param name="ally">The ally.</param>
        /// <param name="buff">The buff.</param>
        /// <returns></returns>
        private Spell GetBestCleanseItem(GameObject ally, BuffInstance buff)
        {
            return
                this.Items.OrderBy(x => x.Priority)
                    .Where(
                        x =>
                        x.WorksOn.Any(y => buff.Type.HasFlag(y)) && (ally.IsMe || x.WorksOnAllies) && x.Spell.IsReady()
                        && x.Spell.IsInRange(ally) && x.Spell.Slot != SpellSlot.Unknown)
                    .Select(x => x.Spell)
                    .FirstOrDefault();
        }

        private void OnUpdate(EventArgs args)
        {
            if (!this.Menu.Item("CleanseEnabled").IsActive())
            {
                return;
            }

            foreach (var ally in HeroManager.Allies)
            {
                foreach (var buff in ally.Buffs.Where(x => this.BuffsToCleanse.Contains(x.Type)))
                {
                    if (!this.Menu.Item($"Cleanse{buff.Type}").IsActive()
                        || this.Menu.Item("MinDuration").GetValue<Slider>().Value / 1000f
                        > buff.EndTime - buff.StartTime || this.BuffIndexesHandled[ally.NetworkId].Contains(buff.Index))
                    {
                        continue;
                    }

                    var cleanseItem = this.GetBestCleanseItem(ally, buff);

                    if (cleanseItem == null)
                    {
                        continue;
                    }

                    this.BuffIndexesHandled[ally.NetworkId].Add(buff.Index);

                    if (this.Menu.Item("HumanizerEnabled").IsActive())
                    {
                        Utility.DelayAction.Add(
                            (int)
                            Math.Min(
                                this.Random.Next(
                                    this.Menu.Item("MinHumanizerDelay").GetValue<Slider>().Value, 
                                    this.Menu.Item("MaxHumanizerDelay").GetValue<Slider>().Value), 
                                (buff.StartTime - buff.EndTime) * 1000), 
                            () => cleanseItem.Cast(ally));
                    }
                    else
                    {
                        cleanseItem.Cast(ally);
                    }

                    return;
                }
            }
        }

        #endregion
    }

    /// <summary>
    ///     An item/spell that can be used to cleanse a spell.
    /// </summary>
    public class CleanseItem
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CleanseItem" /> class.
        /// </summary>
        public CleanseItem()
        {
            this.Range = float.MaxValue;
            this.WorksOnAllies = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the priority.
        /// </summary>
        /// <value>
        ///     The priority.
        /// </value>
        public int Priority { get; set; }

        /// <summary>
        ///     Gets or sets the range.
        /// </summary>
        /// <value>
        ///     The range.
        /// </value>
        public float Range { get; set; }

        /// <summary>
        ///     Gets or sets the slot delegate.
        /// </summary>
        /// <value>
        ///     The slot delegate.
        /// </value>
        public Func<SpellSlot> Slot { get; set; }

        /// <summary>
        ///     Gets or sets the spell.
        /// </summary>
        /// <value>
        ///     The spell.
        /// </value>
        public Spell Spell => new Spell(this.Slot(), this.Range);

        /// <summary>
        ///     Gets or sets what the spell works on.
        /// </summary>
        /// <value>
        ///     The buff types the spell works on.
        /// </value>
        public BuffType[] WorksOn { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the spell works on allies.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the spell works on allies; otherwise, <c>false</c>.
        /// </value>
        public bool WorksOnAllies { get; set; }

        #endregion
    }
}