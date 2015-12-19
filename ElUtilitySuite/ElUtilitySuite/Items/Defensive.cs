namespace ElUtilitySuite.Items
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal static class DefensiveExtensions
    {
        #region Public Methods and Operators

        public static int CountHerosInRange(this Obj_AI_Hero target, bool checkteam, float range = 1200f)
        {
            var objListTeam = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(range, false));

            return objListTeam.Count(hero => checkteam ? hero.Team != target.Team : hero.Team == target.Team);
        }

        public static bool IsValidState(this Obj_AI_Hero target)
        {
            return !target.HasBuffOfType(BuffType.SpellShield) && !target.HasBuffOfType(BuffType.SpellImmunity)
                   && !target.HasBuffOfType(BuffType.Invulnerability);
        }

        #endregion
    }

    internal class Defensive : IPlugin
    {
        #region Static Fields

        public static Obj_AI_Hero AggroTarget;

        private static float incomingDamage, minionDamage;

        #endregion

        #region Public Properties

        public Menu Menu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            this.Menu = rootMenu.AddSubMenu(new Menu("Defensive", "DefensiveMenu"));

            foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly))
            {
                this.Menu.SubMenu("Champion Settings")
                    .AddItem(new MenuItem("DefenseOn" + x.CharData.BaseSkinName, "Use for " + x.CharData.BaseSkinName))
                    .SetValue(true);
            }

            this.CreateDefensiveItem("Randuin's Omen", "Randuins", "selfcount", 40, 40);
            this.CreateDefensiveItem("Face of the Mountain", "Mountain", "allyhealth", 20, 45);
            this.CreateDefensiveItem("Locket of Iron Solari", "Locket", "allyhealth", 40, 45);
            this.CreateDefensiveItem("Seraph's Embrace", "Seraphs", "selfhealth", 40, 45);

            this.Menu.SubMenu("Talisman")
                .AddItem(new MenuItem("useTalisman", "Use Talisman of Ascension"))
                .SetValue(true);
            this.Menu.SubMenu("Talisman")
                .AddItem(new MenuItem("useAllyPct", "Use on ally %"))
                .SetValue(new Slider(50, 1));
            this.Menu.SubMenu("Talisman")
                .AddItem(new MenuItem("useEnemyPct", "Use on enemy %"))
                .SetValue(new Slider(50, 1));
            this.Menu.SubMenu("Talisman")
                .AddItem(new MenuItem("talismanMode", "Mode: "))
                .SetValue(new StringList(new[] { "Always", "Combo" }));
        }

        public void Load()
        {
            try
            {
                Game.OnUpdate += this.OnUpdate;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Methods

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.Type == GameObjectType.obj_AI_Hero && sender.IsEnemy)
            {
                var heroSender = ObjectManager.Get<Obj_AI_Hero>().First(x => x.NetworkId == sender.NetworkId);
                if (heroSender.GetSpellSlot(args.SData.Name) == SpellSlot.Unknown
                    && args.Target.Type == Entry.Player.Type)
                {
                    AggroTarget = ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(args.Target.NetworkId);
                    incomingDamage = (float)heroSender.GetAutoAttackDamage(AggroTarget);
                }

                if (heroSender.ChampionName == "Jinx" && args.SData.Name.Contains("JinxQAttack")
                    && args.Target.Type == Entry.Player.Type)
                {
                    AggroTarget = ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(args.Target.NetworkId);
                    incomingDamage = (float)heroSender.GetAutoAttackDamage(AggroTarget);
                }
            }

            if (sender.Type == GameObjectType.obj_AI_Minion && sender.IsEnemy)
            {
                if (args.Target.NetworkId == Entry.Player.NetworkId)
                {
                    AggroTarget = ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(args.Target.NetworkId);

                    minionDamage =
                        (float)
                        sender.CalcDamage(
                            AggroTarget,
                            Damage.DamageType.Physical,
                            sender.BaseAttackDamage + sender.FlatPhysicalDamageMod);
                }
            }

            if (sender.Type == GameObjectType.obj_AI_Turret && sender.IsEnemy)
            {
                if (args.Target.Type == Entry.Player.Type)
                {
                    if (sender.Distance(Entry.Allies().ServerPosition, true) <= 900 * 900)
                    {
                        AggroTarget = ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(args.Target.NetworkId);

                        incomingDamage =
                            (float)
                            sender.CalcDamage(
                                AggroTarget,
                                Damage.DamageType.Physical,
                                sender.BaseAttackDamage + sender.FlatPhysicalDamageMod);
                    }
                }
            }
        }

        private void CreateDefensiveItem(string displayname, string name, string type, int hpvalue, int dmgvalue)
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

            this.Menu.AddSubMenu(menuName);
        }

        private void DefensiveItemManager()
        {
            if (Entry.Menu.Item("usecombo").GetValue<KeyBind>().Active)
            {
                this.UseItemCount("Randuins", 3143, 450f);
            }

            this.UseItem("allyshieldlocket", "Locket", 3190, 600f);
            this.UseItem("allyshieldmountain", "Mountain", 3401, 700f);
            this.UseItem("selfshieldseraph", "Seraphs", 3040);

            if (Items.HasItem(3069) && Items.CanUseItem(3069) && this.Menu.Item("useTalisman").GetValue<bool>())
            {
                if (!Entry.Menu.Item("usecombo").GetValue<KeyBind>().Active
                    && this.Menu.Item("talismanMode").GetValue<StringList>().SelectedIndex == 1)
                {
                    return;
                }

                var target = Entry.Allies();
                if (target.Distance(Entry.Player.ServerPosition, true) > 600 * 600)
                {
                    return;
                }

                var lowTarget =
                    ObjectManager.Get<Obj_AI_Hero>()
                        .OrderBy(ex => ex.Health / ex.MaxHealth * 100)
                        .First(x => x.IsValidTarget(1000));

                var aHealthPercent = target.Health / target.MaxHealth * 100;
                var eHealthPercent = lowTarget.Health / lowTarget.MaxHealth * 100;

                if (lowTarget.Distance(target.ServerPosition, true) <= 900 * 900
                    && (target.CountHerosInRange(false) > target.CountHerosInRange(true)
                        && eHealthPercent <= this.Menu.Item("useEnemyPct").GetValue<Slider>().Value))
                {
                    Items.UseItem(3069);
                }

                if (target.CountHerosInRange(false) > target.CountHerosInRange(true)
                    && aHealthPercent <= this.Menu.Item("useAllyPct").GetValue<Slider>().Value)
                {
                    Items.UseItem(3069);
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Entry.Player.IsDead)
            {
                return;
            }

            try
            {
                this.DefensiveItemManager();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private void UseItem(string menuvar, string name, int itemId, float itemRange = float.MaxValue)
        {
            if (Entry.Player.InFountain())
            {
                return;
            }

            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
            {
                return;
            }

            if (!this.Menu.Item("use" + name).GetValue<bool>())
            {
                return;
            }

            var target = itemRange > 5000 ? Entry.Player : Entry.Allies();
            if (target.Distance(Entry.Player.ServerPosition, true) > itemRange * itemRange || !target.IsValidState())
            {
                return;
            }

            var aHealthPercent = (int)((target.Health / target.MaxHealth) * 100);
            var iDamagePercent = (int)(incomingDamage / target.MaxHealth * 100);

            if (!this.Menu.Item("DefenseOn" + target.CharData.BaseSkinName).GetValue<bool>())
            {
                return;
            }

            if (menuvar.Contains("shield"))
            {
                if (aHealthPercent <= this.Menu.Item("use" + name + "Pct").GetValue<Slider>().Value)
                {
                    if ((iDamagePercent >= 1 || incomingDamage >= target.Health))
                    {
                        if (AggroTarget.NetworkId == target.NetworkId)
                        {
                            Items.UseItem(itemId, target);
                        }
                    }

                    if (iDamagePercent >= this.Menu.Item("use" + name + "Dmg").GetValue<Slider>().Value)
                    {
                        if (AggroTarget.NetworkId == target.NetworkId)
                        {
                            Items.UseItem(itemId, target);
                        }
                    }
                }
            }
        }

        private void UseItemCount(string name, int itemId, float itemRange)
        {
            if (Entry.Player.InFountain())
            {
                return;
            }

            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
            {
                return;
            }

            if (this.Menu.Item("use" + name).GetValue<bool>())
            {
                if (Entry.Player.CountHerosInRange(true, itemRange)
                    >= this.Menu.Item("use" + name + "Count").GetValue<Slider>().Value)
                {
                    Items.UseItem(itemId);
                }
            }
        }

        #endregion
    }
}