namespace ElUtilitySuite
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    public class InitializeMenu
    {
        #region Static Fields

        public static Menu Menu;

        private static readonly BuffType[] Bufftype =
            {
                BuffType.Snare, BuffType.Knockback, BuffType.Knockup,
                BuffType.Blind, BuffType.Silence, BuffType.Charm, BuffType.Stun,
                BuffType.Fear, BuffType.Slow, BuffType.Taunt,
                BuffType.Suppression, BuffType.Polymorph, BuffType.Poison,
                BuffType.Flee
            };

        private static Menu mainMenu, defensiveMenu;

        #endregion

        #region Public Methods and Operators

        public static void Load()
        {
            Menu = new Menu("ElUtilitySuite", "ElUtilitySuite", true);

            var smiteMenu = Menu.AddSubMenu(new Menu("Smite", "Smite"));
            {
                smiteMenu.AddItem(
                    new MenuItem("ElSmite.Activated", "Activated").SetValue(
                        new KeyBind("M".ToCharArray()[0], KeyBindType.Toggle, true)));

                if (Entry.IsSummonersRift)
                {
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("SRU_Dragon", "Dragon").SetValue(true));
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("SRU_Baron", "Baron").SetValue(true));
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("SRU_Red", "Red buff").SetValue(true));
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("SRU_Blue", "Blue buff").SetValue(true));

                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("SRU_RiftHerald", "Rift Herald").SetValue(false));

                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("SRU_Gromp", "Gromp").SetValue(false));
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("SRU_Murkwolf", "Wolves").SetValue(false));
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("SRU_Krug", "Krug").SetValue(false));
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("SRU_Razorbeak", "Chicken camp").SetValue(false));
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("Sru_Crab", "Crab").SetValue(false));
                }

                if (Entry.IsTwistedTreeline)
                {
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("TT_Spiderboss", "Vilemaw Enabled").SetValue(true));
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("TT_NGolem", "Golem Enabled").SetValue(true));
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("TT_NWolf", "Wolf Enabled").SetValue(true));
                    smiteMenu.SubMenu("Mobs").AddItem(new MenuItem("TT_NWraith", "Wraith Enabled").SetValue(true));
                }

                //Killsteal submenu
                smiteMenu.SubMenu("Killsteal")
                    .AddItem(new MenuItem("ElSmite.KS.Activated", "Use smite to killsteal").SetValue(true));

                //Drawings
                smiteMenu.SubMenu("Drawings")
                    .AddItem(new MenuItem("ElSmite.Draw.Range", "Draw smite Range").SetValue(new Circle()));
                smiteMenu.SubMenu("Drawings")
                    .AddItem(new MenuItem("ElSmite.Draw.Text", "Draw smite text").SetValue(true));
                smiteMenu.SubMenu("Drawings")
                    .AddItem(new MenuItem("ElSmite.Draw.Damage", "Draw smite Damage").SetValue(true));
            }

            if (Entry.Player.GetSpellSlot("summonerheal") != SpellSlot.Unknown)
            {
                var healMenu = Menu.AddSubMenu(new Menu("Heal", "Heal"));
                {
                    healMenu.AddItem(new MenuItem("Heal.Activated", "Heal").SetValue(true));
                    healMenu.AddItem(new MenuItem("Heal.HP", "Health percentage").SetValue(new Slider(20, 1)));
                    healMenu.AddItem(new MenuItem("Heal.Damage", "Heal on Dmg dealt %").SetValue(new Slider(40, 1)));
                    healMenu.AddItem(new MenuItem("seperator21", ""));
                    foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
                    {
                        healMenu.AddItem(new MenuItem("healon" + x.ChampionName, "Use for " + x.ChampionName))
                            .SetValue(true);
                    }
                }
            }

            if (Entry.Player.GetSpellSlot("summonerdot") != SpellSlot.Unknown)
            {
                var igniteMenu = Menu.AddSubMenu(new Menu("Ignite", "Ignite"));
                {
                    igniteMenu.AddItem(new MenuItem("Ignite.Activated", "Ignite").SetValue(true));
                    igniteMenu.AddItem(new MenuItem("Ignite.shieldCheck", "Check for shields").SetValue(true));
                }
            }

            var potionsMenu = Menu.AddSubMenu(new Menu("Potions", "Potions"));
            {
                potionsMenu.AddItem(new MenuItem("Potions.Activated", "Potions activated").SetValue(true));
                potionsMenu.AddItem(new MenuItem("Potions.Health", "Health potions").SetValue(true));
                potionsMenu.AddItem(new MenuItem("Potions.Biscuit", "Biscuits").SetValue(true));
                potionsMenu.AddItem(new MenuItem("Potions.RefillablePotion", "Refillable Potion").SetValue(true));
                potionsMenu.AddItem(new MenuItem("Potions.HuntersPotion", "Hunters Potion").SetValue(true));
                potionsMenu.AddItem(new MenuItem("Potions.CorruptingPotion", "Corrupting Potion").SetValue(true));

                potionsMenu.AddItem(new MenuItem("seperator.Potions", ""));
                potionsMenu.AddItem(new MenuItem("Potions.Player.Health", "Health percentage").SetValue(new Slider(20)));
            }

            var spellCleanserMenu = new Menu("Cleanse", "Cleanser");
            {
                foreach (var a in ObjectManager.Get<Obj_AI_Hero>().Where(ally => ally.Team == Entry.Player.Team))
                {
                    spellCleanserMenu.SubMenu("Mikaels settings")
                        .AddItem(
                            new MenuItem(
                                "Protect.Cleanse.Kappa" + a.CharData.BaseSkinName,
                                "Use for " + a.CharData.BaseSkinName))
                        .SetValue(true);
                }

                spellCleanserMenu.SubMenu("Mikaels settings")
                    .SubMenu("Activated")
                    .AddItem(new MenuItem("Protect.Cleanse.Mikeals.Activated", "Activate Mikaels").SetValue(true));

                spellCleanserMenu.SubMenu("Mikaels settings")
                    .SubMenu("Buffs")
                    .AddItem(new MenuItem("Protect.Cleanse.Stun.Ally", "Stuns").SetValue(true));
                spellCleanserMenu.SubMenu("Mikaels settings")
                    .SubMenu("Buffs")
                    .AddItem(new MenuItem("Protect.Cleanse.Charm.Ally", "Charms").SetValue(true));
                spellCleanserMenu.SubMenu("Mikaels settings")
                    .SubMenu("Buffs")
                    .AddItem(new MenuItem("Protect.Cleanse.Taunt.Ally", "Taunts").SetValue(false));
                spellCleanserMenu.SubMenu("Mikaels settings")
                    .SubMenu("Buffs")
                    .AddItem(new MenuItem("Protect.Cleanse.Fear.Ally", "Fears").SetValue(false));
                spellCleanserMenu.SubMenu("Mikaels settings")
                    .SubMenu("Buffs")
                    .AddItem(new MenuItem("Protect.Cleanse.Snare.Ally", "Snares").SetValue(false));
                spellCleanserMenu.SubMenu("Mikaels settings")
                    .SubMenu("Buffs")
                    .AddItem(new MenuItem("Protect.Cleanse.Silence.Ally", "Silences").SetValue(true));
                spellCleanserMenu.SubMenu("Mikaels settings")
                    .SubMenu("Buffs")
                    .AddItem(new MenuItem("Protect.Cleanse.Suppression.Ally", "Suppressions").SetValue(true));
                spellCleanserMenu.SubMenu("Mikaels settings")
                    .SubMenu("Buffs")
                    .AddItem(new MenuItem("Protect.Cleanse.Polymorph.Ally", "Polymorphs").SetValue(false));
                spellCleanserMenu.SubMenu("Mikaels settings")
                    .SubMenu("Buffs")
                    .AddItem(new MenuItem("Protect.Cleanse.Blind.Ally", "Blinds").SetValue(false));
                spellCleanserMenu.SubMenu("Mikaels settings")
                    .SubMenu("Buffs")
                    .AddItem(new MenuItem("Protect.Cleanse.Slow.Ally", "Slows").SetValue(false));
                spellCleanserMenu.SubMenu("Mikaels settings")
                    .SubMenu("Buffs")
                    .AddItem(new MenuItem("Protect.Cleanse.Posion.Ally", "Posion").SetValue(false));
                spellCleanserMenu.SubMenu("Mikaels settings")
                    .SubMenu("Buffs")
                    .AddItem(new MenuItem("Protect.Cleanse.Knockup.Ally", "Knockups").SetValue(false));
                spellCleanserMenu.SubMenu("Mikaels settings")
                    .SubMenu("Buffs")
                    .AddItem(new MenuItem("Protect.Cleanse.Knockback.Ally", "Knockbacks").SetValue(false));
                spellCleanserMenu.SubMenu("Mikaels settings")
                    .SubMenu("Buffs")
                    .AddItem(new MenuItem("Protect.Cleanse.Flee.Ally", "Flee").SetValue(false));

                spellCleanserMenu.AddItem(new MenuItem("New.Cleanse.Delay", "Cleanse delay "))
                    .SetValue(new Slider(0, 0, 25));
                spellCleanserMenu.AddItem(new MenuItem("New.cmode", "Mode: "))
                    .SetValue(new StringList(new[] { "Always", "Combo" }, 1));
                var cleanseSpellMenu = new Menu("Spells", "SpellPick");
                {
                    foreach (var spell in
                        SpellCleanser.Spells.Where(
                            x =>
                            ObjectManager.Get<Obj_AI_Hero>()
                                .Where(y => y.IsEnemy)
                                .Any(y => y.ChampionName.ToLower() == x.ChampionName)))
                    {
                        var objAiHero =
                            ObjectManager.Get<Obj_AI_Hero>()
                                .FirstOrDefault(x => x.ChampionName.ToLower() == spell.ChampionName);

                        if (objAiHero == null)
                        {
                            continue;
                        }

                        var firstOrDefault =
                            objAiHero.Spellbook.Spells.FirstOrDefault(x => x.SData.Name.ToLower() == spell.SDataName);

                        if (firstOrDefault != null)
                        {
                            cleanseSpellMenu.AddItem(
                                new MenuItem(
                                    string.Format("Cleanse{0}", spell.SDataName),
                                    string.Format(
                                        "{0} ({1}) - {2}",
                                        char.ToUpper(spell.ChampionName[0]) + spell.ChampionName.Substring(1),
                                        firstOrDefault.Slot,
                                        spell.SDataName)).SetValue(true));
                        }
                    }
                }

                spellCleanserMenu.AddSubMenu(cleanseSpellMenu);
                spellCleanserMenu.AddItem(new MenuItem("CleanseDangerous", "Cleanse on dangerous spell").SetValue(true));

                Menu.AddSubMenu(spellCleanserMenu);
            }

            var protectMenu = Menu.AddSubMenu(new Menu("Stealth", "ProtectYourself"));
            {
                protectMenu.SubMenu("Rengar")
                    .AddItem(new MenuItem("Protect.Rengar2", "Rengar antigapcloser - Beta").SetValue(false));
                protectMenu.SubMenu("Rengar")
                    .AddItem(new MenuItem("Protect.Rengar.Pinkward", "Pinkward").SetValue(true));
                protectMenu.SubMenu("Rengar")
                    .AddItem(new MenuItem("Protect.Rengar.PinkwardTrinket", "Pink trinket").SetValue(true));
                protectMenu.SubMenu("Akali").AddItem(new MenuItem("Protect.Akali", "Autopink Akali W").SetValue(true));
                protectMenu.SubMenu("Akali").AddItem(new MenuItem("Protect.Akali.PinkWard", "Pinkward").SetValue(true));
                protectMenu.SubMenu("Akali").AddItem(new MenuItem("Protect.Akali.Trinket", "Trinket").SetValue(true));
                protectMenu.SubMenu("Akali")
                    .AddItem(new MenuItem("Protect.Akali.HP", "Pink when Akali's HP:").SetValue(new Slider(50)));
            }

            #region Credits to Oracle

            mainMenu = Menu.AddSubMenu(new Menu("Offensive", "omenu"));

            foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
            {
                mainMenu.SubMenu("Champion Settings")
                    .AddItem(new MenuItem("ouseOn" + x.CharData.BaseSkinName, "Use for " + x.CharData.BaseSkinName))
                    .SetValue(true);
            }

            CreateMenuItem("Muramana", "Muramana", 90, 30, true);
            CreateMenuItem("Tiamat/Hydra", "Hydra", 90, 30);
            CreateMenuItem("Titanic Hydra", "Titanic", 90, 30);
            CreateMenuItem("Hextech Gunblade", "Hextech", 90, 30);
            CreateMenuItem("Youmuu's Ghostblade", "Youmuus", 90, 30);
            CreateMenuItem("Bilgewater's Cutlass", "Cutlass", 90, 30);
            CreateMenuItem("Blade of the Ruined King", "Botrk", 70, 70);
            CreateMenuItem("Frost Queen's Claim", "Frostclaim", 100, 30);

            defensiveMenu = Menu.AddSubMenu(new Menu("Defensive", "DefensiveMenu"));

            foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
            {
                defensiveMenu.SubMenu("Champion Settings")
                    .AddItem(new MenuItem("DefenseOn" + x.CharData.BaseSkinName, "Use for " + x.CharData.BaseSkinName))
                    .SetValue(true);
            }

            CreateDefensiveItem("Randuin's Omen", "Randuins", "selfcount", 40, 40);
            CreateDefensiveItem("Face of the Mountain", "Mountain", "allyhealth", 20, 45);
            CreateDefensiveItem("Locket of Iron Solari", "Locket", "allyhealth", 40, 45);
            CreateDefensiveItem("Seraph's Embrace", "Seraphs", "selfhealth", 40, 45);

            defensiveMenu.SubMenu("Talisman")
                .AddItem(new MenuItem("useTalisman", "Use Talisman of Ascension"))
                .SetValue(true);
            defensiveMenu.SubMenu("Talisman")
                .AddItem(new MenuItem("useAllyPct", "Use on ally %"))
                .SetValue(new Slider(50, 1));
            defensiveMenu.SubMenu("Talisman")
                .AddItem(new MenuItem("useEnemyPct", "Use on enemy %"))
                .SetValue(new Slider(50, 1));
            defensiveMenu.SubMenu("Talisman")
                .AddItem(new MenuItem("talismanMode", "Mode: "))
                .SetValue(new StringList(new[] { "Always", "Combo" }));

            #endregion

            var notificationsMenu = Menu.AddSubMenu(new Menu("Recall tracker", "Recall tracker"));
            {
                notificationsMenu.AddItem(new MenuItem("showRecalls", "Show Recalls").SetValue(true));
                notificationsMenu.AddItem(new MenuItem("notifRecFinished", "Recall finished").SetValue(true));
                notificationsMenu.AddItem(new MenuItem("notifRecAborted", "Recall aborted").SetValue(true));
            }

            var zhonyaMenu = new Menu("Zhonya's Hourglass", "zhonya");
            {
                var zhonyaSpellMenu = new Menu("Spells", "SpellPick");
                {
                    foreach (var spell in
                        Zhonya.Spells.Where(
                            x =>
                            ObjectManager.Get<Obj_AI_Hero>()
                                .Where(y => y.IsEnemy)
                                .Any(y => y.ChampionName.ToLower() == x.ChampionName)))
                    {
                        var objAiHero =
                            ObjectManager.Get<Obj_AI_Hero>()
                                .FirstOrDefault(x => x.ChampionName.ToLower() == spell.ChampionName);

                        if (objAiHero == null)
                        {
                            continue;
                        }

                        var firstOrDefault =
                            objAiHero.Spellbook.Spells.FirstOrDefault(x => x.SData.Name.ToLower() == spell.SDataName);

                        if (firstOrDefault != null)
                        {
                            zhonyaSpellMenu.AddItem(
                                new MenuItem(
                                    string.Format("Zhonya{0}", spell.SDataName),
                                    string.Format(
                                        "{0} ({1}) - {2}",
                                        char.ToUpper(spell.ChampionName[0]) + spell.ChampionName.Substring(1),
                                        firstOrDefault.Slot,
                                        spell.SDataName)).SetValue(true));
                        }
                    }
                }

                zhonyaMenu.AddSubMenu(zhonyaSpellMenu);
                zhonyaMenu.AddItem(new MenuItem("ZhonyaDangerous", "Zhonya's on dangerous spell").SetValue(true));
                zhonyaMenu.AddItem(new MenuItem("ZhonyaHP_BETA", "Use Zhonya on low HP (BETA)").SetValue(false));
                zhonyaMenu.AddItem(new MenuItem("ZhonyaHPSlider", "HP Percent").SetValue(new Slider(10, 1, 50)));

                Menu.AddSubMenu(zhonyaMenu);
            }

            Menu.AddItem(new MenuItem("seperator1", ""));

            Menu.AddItem(new MenuItem("usecombo", "Combo (Active)").SetValue(new KeyBind(32, KeyBindType.Press)));

            Menu.AddItem(new MenuItem("seperator", ""));
            Menu.AddItem(new MenuItem("Versionnumber", string.Format("Version: {0}", Entry.ScriptVersion)));
            Menu.AddItem(new MenuItem("by.jQuery", "jQ / ChewyMoon"));

            Menu.AddToMainMenu();
        }

        #endregion

        #region Methods

        private static void CreateDefensiveItem(string displayname, string name, string type, int hpvalue, int dmgvalue)
        {
            var menuName = new Menu(name, name.ToLower());
            menuName.AddItem(new MenuItem("use" + name, "Use " + displayname)).SetValue(true);

            if (!type.Contains("count"))
            {
                menuName.AddItem(new MenuItem("use" + name + "Pct", "Use on HP %")).SetValue(new Slider(hpvalue));
                menuName.AddItem(new MenuItem("use" + name + "Dmg", "Use on damage dealt %"))
                    .SetValue(new Slider(dmgvalue));
            }

            if (type.Contains("count"))
            {
                menuName.AddItem(new MenuItem("use" + name + "Count", "Use on Count")).SetValue(new Slider(3, 1, 5));
            }

            defensiveMenu.AddSubMenu(menuName);
        }

        private static void CreateMenuItem(
            string displayname,
            string name,
            int evalue,
            int avalue,
            bool usemana = false)
        {
            var menuName = new Menu(name, name.ToLower());

            menuName.AddItem(new MenuItem("use" + name, "Use " + displayname)).SetValue(true);
            menuName.AddItem(new MenuItem("use" + name + "Pct", "Use on enemy HP %")).SetValue(new Slider(evalue));

            if (!usemana)
            {
                menuName.AddItem(new MenuItem("use" + name + "Me", "Use on my HP %")).SetValue(new Slider(avalue));
            }

            if (usemana)
            {
                menuName.AddItem(new MenuItem("use" + name + "Mana", "Minimum mana % to use")).SetValue(new Slider(35));
            }

            if (name == "Muramana")
            {
                menuName.AddItem(
                    new MenuItem("muraMode", " Muramana Mode: ").SetValue(
                        new StringList(new[] { "Always", "Combo" }, 1)));
            }

            mainMenu.AddSubMenu(menuName);
        }

        #endregion
    }
}