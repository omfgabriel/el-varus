namespace ElUtilitySuite.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal class AutoLantern 
    {
        #region Public Properties

        public Menu Menu { get; set; }

        #endregion

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var predicate = new Func<Menu, bool>(x => x.Name == "MiscMenu");
            var menu = !rootMenu.Children.Any(predicate) ? rootMenu.AddSubMenu(new Menu("Misc", "MiscMenu")) : rootMenu.Children.First(predicate);

            var autoLanternMenu = menu.AddSubMenu(new Menu("Thresh lantern", "Threshlantern"));
            {
                autoLanternMenu.AddItem(new MenuItem("ThreshLantern", "Auto click Thresh lantern").SetValue(true));
                autoLanternMenu.AddItem(new MenuItem("ThreshLantern", "Hotkey").SetValue(new KeyBind('M', KeyBindType.Press)));
                autoLanternMenu.AddItem(new MenuItem("ThreshLantern.Health", "Click when HP %").SetValue(new Slider(20)));
            }

            this.Menu = autoLanternMenu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            
        }
    }
}
