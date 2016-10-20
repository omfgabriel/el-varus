namespace Elvarus
{
    using System;
    using System.Drawing;

    using LeagueSharp.Common;

    public class ElVarusMenu
    {
        #region Static Fields

        public static Menu Menu;

        #endregion

        #region Public Methods and Operators

        public static void Initialize()
        {
            Menu = new Menu("ElVarus", "menu", true);

            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            Varus.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);

            var targetSelector = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelector);
            Menu.AddSubMenu(targetSelector);
			

            var rMenu = new Menu("R Gapcloser List", "R Gapcloser List");
			foreach (var enemy in HeroManager.Enemies)
            rMenu.AddItem(new MenuItem("GapCloser" + enemy.ChampionName, enemy.ChampionName).SetValue(false));	
            Menu.AddSubMenu(rMenu);			
			
			var r1Menu = new Menu("R AOE", "R AOE");
			r1Menu.AddItem(new MenuItem("rCount", "Auto R if enemies in range (combo mode)", true).SetValue(new Slider(3, 0, 5)));
			Menu.AddSubMenu(r1Menu);	
			
            var r2Menu = new Menu("R 1v1", "R 1v1");
			foreach (var enemy in HeroManager.Enemies)
            r2Menu.AddItem(new MenuItem("RKS123" + enemy.ChampionName, enemy.ChampionName).SetValue(true));	
            Menu.AddSubMenu(r2Menu);			

            var r3Menu = new Menu("R ChainCC", "R ChainCC");
			foreach (var enemy in HeroManager.Enemies)
            r3Menu.AddItem(new MenuItem("chainCC" + enemy.ChampionName, enemy.ChampionName).SetValue(false));	
			Menu.AddSubMenu(r3Menu);	

            var cMenu = new Menu("Combo", "Combo");

            cMenu.AddItem(new MenuItem("ElVarus.Combo.Q", "Use Q").SetValue(true));
            cMenu.AddItem(new MenuItem("ElVarus.omfgabriel", "Mode").SetValue(new StringList(new[] { "AP Mode", "AD Mode" }, 0)));				
            cMenu.AddItem(new MenuItem("ElVarus.Combo.E", "Use E").SetValue(true));
            cMenu.AddItem(new MenuItem("ElVarus.sssss", ""));
            cMenu.AddItem(new MenuItem("ElVarus.Combo.Stack.Count", "Q (AA Range) when stacks >= ")).SetValue(new Slider(3, 1, 3));
            cMenu.AddItem(new MenuItem("ElVarus.ComboE.Stack.Count", "AP Only - E when stacks >= ")).SetValue(new Slider(3, 1, 3));			
            cMenu.AddItem(new MenuItem("ElVarus.sssssssss", ""));
            cMenu.AddItem(
                new MenuItem("ElVarus.SemiR", "Semi-manual cast R key").SetValue(
                    new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));

            cMenu.AddItem(new MenuItem("ElVarus.ssssssssssss", ""));
            cMenu.AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));
            Menu.AddSubMenu(cMenu);

            var hMenu = new Menu("Harass", "Harass");
            hMenu.AddItem(new MenuItem("ElVarus.Harass.Q", "Use Q").SetValue(true));
            hMenu.AddItem(new MenuItem("ElVarus.Harass.E", "Use E").SetValue(true));
            hMenu.AddItem(new MenuItem("ElVarus.Harasssfsass.E", ""));
            hMenu.AddItem(new MenuItem("minmanaharass", "Mana needed to clear ")).SetValue(new Slider(55));

            Menu.AddSubMenu(hMenu);



            

            //ElSinged.Misc
            var miscMenu = new Menu("Misc", "Misc");
            miscMenu.AddItem(new MenuItem("ElVarus.Draw.off", "Turn drawings off").SetValue(true));
            miscMenu.AddItem(new MenuItem("ElVarus.Draw.Q", "Draw Q").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElVarus.Draw.W", "Draw W").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElVarus.Draw.E", "Draw E").SetValue(new Circle()));
			miscMenu.AddItem(new MenuItem("ElVarus.Safety", "Q Safety").SetValue(true));
            miscMenu.AddItem(new MenuItem("ElVarus.KSSS", "Killsteal").SetValue(true));
            miscMenu.AddItem(new MenuItem("InterruptSpells", "Interrupt spells using R").SetValue(true));
 

            Menu.AddSubMenu(miscMenu);

            Menu.AddItem(new MenuItem("422442fsaafs4242f", ""));
            Menu.AddItem(new MenuItem("422442fsaafsf", "Version: 1.3.3.7"));
            Menu.AddItem(new MenuItem("fsasfafsfsafsa", "Made By jQuery"));

            Menu.AddToMainMenu();

            Console.WriteLine("Menu Loaded");
        }

        #endregion
    }
}