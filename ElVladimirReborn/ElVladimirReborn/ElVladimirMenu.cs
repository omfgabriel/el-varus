namespace ElVladimirReborn
{
    using System;

    using LeagueSharp.Common;

    public class ElVladimirMenu
    {
        #region Static Fields

        public static Menu Menu;

        #endregion

        #region Public Methods and Operators

        public static void Initialize()
        {
            Menu = new Menu("ElVladimir:Reborn", "menu", true);

            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            Vladimir.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            Menu.AddSubMenu(orbwalkerMenu);

            var targetSelector = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelector);

            Menu.AddSubMenu(targetSelector);

            var comboMenu = new Menu("Combo", "Combo");
            {
                comboMenu.AddItem(new MenuItem("ElVladimir.Combo.Q", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElVladimir.Combo.W", "Use W").SetValue(false));
                comboMenu.AddItem(new MenuItem("ElVladimir.Combo.E", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElVladimir.Combo.R", "Use R").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElVladimir.Combo.SmartUlt", "Use Smartult").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElVladimir.Combo.Count.R", "Minimum targets for R"))
                    .SetValue(new Slider(1, 1, 5));
                comboMenu.AddItem(new MenuItem("separator", ""));
                comboMenu.AddItem(
                    new MenuItem("ElVladimir.Combo.R.Killable", "Use R only when killable").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElVladimir.Combo.Ignite", "Use ignite").SetValue(true));
            }

            Menu.AddSubMenu(comboMenu);

            var harassMenu = new Menu("Harass", "Harass");
            {
                harassMenu.AddItem(new MenuItem("ElVladimir.Harass.Q", "Use Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("ElVladimir.Harass.E", "Use E").SetValue(true));

                harassMenu.SubMenu("AutoHarass settings")
                    .AddItem(
                        new MenuItem("ElVladimir.AutoHarass.Health.E", "Minimum Health for E").SetValue(new Slider(20)));
                harassMenu.SubMenu("AutoHarass settings")
                    .AddItem(
                        new MenuItem("ElVladimir.AutoHarass.Activated", "Auto harass", true).SetValue(
                            new KeyBind("L".ToCharArray()[0], KeyBindType.Toggle)));
                harassMenu.SubMenu("AutoHarass settings").AddItem(new MenuItem("spacespacespace", ""));
                harassMenu.SubMenu("AutoHarass settings")
                    .AddItem(new MenuItem("ElVladimir.AutoHarass.Q", "Use Q").SetValue(true));
                harassMenu.SubMenu("AutoHarass settings")
                    .AddItem(new MenuItem("ElVladimir.AutoHarass.E", "Use E").SetValue(true));
            }
            Menu.AddSubMenu(harassMenu);

            var clearMenu = new Menu("Waveclear", "Waveclear");
            {
                clearMenu.AddItem(new MenuItem("ElVladimir.WaveClear.Q", "Use Q").SetValue(true));
                clearMenu.AddItem(new MenuItem("ElVladimir.WaveClear.E", "Use E").SetValue(true));
                clearMenu.AddItem(new MenuItem("ElVladimir.JungleClear.Q", "Use Q in jungle").SetValue(true));
                clearMenu.AddItem(new MenuItem("ElVladimir.JungleClear.E", "Use E in jungle").SetValue(true));
                clearMenu.AddItem(
                    new MenuItem("ElVladimir.WaveClear.Health.E", "Minimum health for E").SetValue(new Slider(20)));
            }

            Menu.AddSubMenu(clearMenu);

            var settingsMenu = new Menu("Settings", "Settings");
            {
                settingsMenu.AddItem(
                    new MenuItem("ElVladimir.Settings.Stack.E", "Automatic stack E").SetValue(
                        new KeyBind("T".ToCharArray()[0], KeyBindType.Toggle)));
                settingsMenu.AddItem(new MenuItem("ElVladimir.Settings.Stack.HP", "Minimum automatic stack HP"))
                    .SetValue(new Slider(20));
                settingsMenu.AddItem(new MenuItem("ElVladimir.Settings.AntiGapCloser.Active", "Anti gapcloser"))
                    .SetValue(true);
            }

            Menu.AddSubMenu(settingsMenu);

            var miscMenu = new Menu("Misc", "Misc");
            {
                miscMenu.AddItem(new MenuItem("ElVladimir.Draw.off", "Turn drawings off").SetValue(false));
                miscMenu.AddItem(new MenuItem("ElVladimir.Draw.Q", "Draw Q").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("ElVladimir.Draw.W", "Draw W").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("ElVladimir.Draw.E", "Draw E").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("ElVladimir.Draw.R", "Draw R").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("ElVladimir.Draw.Text", "Draw Text").SetValue(true));
            }

            Menu.AddSubMenu(miscMenu);

            var credits = new Menu("Credits", "jQuery");
            {
                credits.AddItem(new MenuItem("ElVladimir.Paypal", "if you would like to donate via paypal:"));
                credits.AddItem(new MenuItem("ElVladimir.Email", "info@zavox.nl"));
            }
            Menu.AddSubMenu(credits);

            Menu.AddItem(new MenuItem("422442fsaafs4242f", ""));
            Menu.AddItem(new MenuItem("422442fsaafsf", "Version: 1.0.0.3"));
            Menu.AddItem(new MenuItem("fsasfafsfsafsa", "Made By jQuery"));

            Menu.AddToMainMenu();

            Console.WriteLine("Menu Loaded");
        }

        #endregion
    }
}