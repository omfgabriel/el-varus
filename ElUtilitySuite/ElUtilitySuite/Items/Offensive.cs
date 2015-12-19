namespace ElUtilitySuite.Items
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Offensive : IPlugin
    {
        #region Static Fields

        public static bool CanManamune;

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
            this.Menu = rootMenu.AddSubMenu(new Menu("Offensive", "omenu"));

            foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
            {
                this.Menu.SubMenu("Champion Settings")
                    .AddItem(new MenuItem("ouseOn" + x.CharData.BaseSkinName, "Use for " + x.CharData.BaseSkinName))
                    .SetValue(true);
            }

            this.CreateMenuItem("Muramana", "Muramana", 90, 30, true);
            this.CreateMenuItem("Tiamat/Hydra", "Hydra", 90, 30);
            this.CreateMenuItem("Titanic Hydra", "Titanic", 90, 30);
            this.CreateMenuItem("Hextech Gunblade", "Hextech", 90, 30);
            this.CreateMenuItem("Youmuu's Ghostblade", "Youmuus", 90, 30);
            this.CreateMenuItem("Bilgewater's Cutlass", "Cutlass", 90, 30);
            this.CreateMenuItem("Blade of the Ruined King", "Botrk", 70, 70);
            this.CreateMenuItem("Frost Queen's Claim", "Frostclaim", 100, 30);
        }

        public void Load()
        {
            try
            {
                Game.OnUpdate += this.OnUpdate;
                Obj_AI_Base.OnProcessSpellCast += this.Obj_AI_Base_OnProcessSpellCast;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Methods

        private void CreateMenuItem(string displayname, string name, int evalue, int avalue, bool usemana = false)
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

            this.Menu.AddSubMenu(menuName);
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && (Items.HasItem(3042) || Items.HasItem(3043)))
            {
                if (Entry.Player.GetSpellSlot(args.SData.Name) == SpellSlot.Unknown
                    && (this.Menu.Item("usecombo").GetValue<KeyBind>().Active || args.Target.Type == Entry.Player.Type))
                {
                    CanManamune = true;
                }

                else
                {
                    Utility.DelayAction.Add(400, () => CanManamune = false);
                }
            }
        }

        private void OffensiveItemManager()
        {
            if (this.Menu.Item("useMuramana").GetValue<bool>())
            {
                if (CanManamune)
                {
                    if (this.Menu.Item("muraMode").GetValue<StringList>().SelectedIndex != 1
                        || Entry.Menu.Item("usecombo").GetValue<KeyBind>().Active)
                    {
                        var manamune = Entry.Player.GetSpellSlot("Muramana");
                        if (manamune != SpellSlot.Unknown && !Entry.Player.HasBuff("Muramana"))
                        {
                            if (Entry.Player.Mana / Entry.Player.MaxMana * 100
                                > this.Menu.Item("useMuramanaMana").GetValue<Slider>().Value)
                            {
                                Entry.Player.Spellbook.CastSpell(manamune);
                            }

                            Utility.DelayAction.Add(400, () => CanManamune = false);
                        }
                    }
                }

                if (!CanManamune && !Entry.Menu.Item("usecombo").GetValue<KeyBind>().Active)
                {
                    var manamune = Entry.Player.GetSpellSlot("Muramana");
                    if (manamune != SpellSlot.Unknown && Entry.Player.HasBuff("Muramana"))
                    {
                        Entry.Player.Spellbook.CastSpell(manamune);
                    }
                }
            }

            if (Entry.Menu.Item("usecombo").GetValue<KeyBind>().Active)
            {
                this.UseItem("Frostclaim", 3092, 850f, true);
                this.UseItem("Youmuus", 3142, 650f);
                this.UseItem("Hydra", 3077, 250f);
                this.UseItem("Hydra", 3074, 250f);
                this.UseItem("Hextech", 3146, 700f, true);
                this.UseItem("Cutlass", 3144, 450f, true);
                this.UseItem("Botrk", 3153, 450f, true);
                this.UseItem("Titanic", 3748, 450f);
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
                this.OffensiveItemManager();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private void UseItem(string name, int itemId, float range, bool targeted = false)
        {
            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
            {
                return;
            }

            if (!this.Menu.Item("use" + name).GetValue<bool>())
            {
                return;
            }

            Obj_AI_Hero target = null;

            foreach (var targ in
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(hero => hero.IsValidTarget(2000))
                    .OrderByDescending(hero => hero.Distance(Game.CursorPos)))
            {
                target = targ;
            }

            if (target.IsValidTarget(range))
            {
                if (target != null)
                {
                    var eHealthPercent = (int)((target.Health / target.MaxHealth) * 100);
                    var aHealthPercent = (int)((Entry.Player.Health / target.MaxHealth) * 100);

                    if (eHealthPercent <= this.Menu.Item("use" + name + "Pct").GetValue<Slider>().Value
                        && this.Menu.Item("ouseOn" + target.CharData.BaseSkinName).GetValue<bool>())
                    {
                        if (targeted && itemId == 3092)
                        {
                            var pi = new PredictionInput
                                         {
                                             Aoe = true, Collision = false, Delay = 0.0f, From = Entry.Player.Position,
                                             Radius = 250f, Range = 850f, Speed = 1500f, Unit = target,
                                             Type = SkillshotType.SkillshotCircle
                                         };

                            var po = Prediction.GetPrediction(pi);
                            if (po.Hitchance >= HitChance.Medium)
                            {
                                Items.UseItem(itemId, po.CastPosition);
                            }
                        }

                        else if (targeted)
                        {
                            Items.UseItem(itemId, target);
                        }

                        else
                        {
                            Items.UseItem(itemId);
                        }
                    }

                    else if (aHealthPercent <= this.Menu.Item("use" + name + "Me").GetValue<Slider>().Value
                             && this.Menu.Item("ouseOn" + target.CharData.BaseSkinName).GetValue<bool>())
                    {
                        if (targeted)
                        {
                            Items.UseItem(itemId, target);
                        }
                        else
                        {
                            Items.UseItem(itemId);
                        }
                    }
                }
            }
        }

        #endregion
    }
}