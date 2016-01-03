namespace ElEasy.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Cassiopeia : IPlugin
    {
        #region Static Fields

        private static readonly Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                                       {
                                                                           { Spells.Q, new Spell(SpellSlot.Q, 850) },
                                                                           { Spells.W, new Spell(SpellSlot.W, 850) },
                                                                           { Spells.E, new Spell(SpellSlot.E, 700) },
                                                                           { Spells.R, new Spell(SpellSlot.R, 825) }
                                                                       };

        private static SpellSlot Ignite;

        private static int lastE;

        private static int lastQ;

        private static Orbwalking.Orbwalker Orbwalker;

        #endregion

        #region Enums

        public enum Spells
        {
            Q,

            W,

            E,

            R
        }

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
        ///     Gets the hitchance
        /// </summary>
        /// <value>
        ///     The hitchance
        /// </value>
        private HitChance CustomHitChance
        {
            get
            {
                return this.GetHitchance();
            }
        }

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        private Menu Menu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            this.Menu = new Menu("ElCassiopeia", "ElCassiopeia");
            {
                var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
                Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
                this.Menu.AddSubMenu(orbwalkerMenu);

                var targetSelector = new Menu("Target Selector", "TargetSelector");
                TargetSelector.AddToMenu(targetSelector);
                this.Menu.AddSubMenu(targetSelector);

                var comboMenu = new Menu("Combo", "Combo");
                {
                    comboMenu.AddItem(new MenuItem("ElEasy.Cassio.Combo.Q", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Cassio.Combo.W", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Cassio.Combo.E", "Use E").SetValue(true));
                    comboMenu.SubMenu("R").AddItem(new MenuItem("ElEasy.Cassio.Combo.R", "Use R").SetValue(true));
                    comboMenu.SubMenu("R")
                        .AddItem(
                            new MenuItem("ElEasy.Cassio.Combo.R.Count", "Enemies for R").SetValue(new Slider(2, 1, 5)));
                    comboMenu.AddItem(new MenuItem("ElEasy.Cassio.Combo.Ignite", "Use Ignite").SetValue(true));
                    comboMenu.SubMenu("E").AddItem(new MenuItem("ElEasy.Cassio.E.Legit", "Legit E").SetValue(false));
                    comboMenu.SubMenu("E")
                        .AddItem(new MenuItem("ElEasy.Cassio.E.Delay", "E Delay").SetValue(new Slider(1000, 0, 2000)));
                }

                this.Menu.AddSubMenu(comboMenu);

                var harassMenu = new Menu("Harass", "Harass");
                {
                    harassMenu.AddItem(new MenuItem("ElEasy.Cassio.Harass.Q", "Use Q").SetValue(true));
                    harassMenu.AddItem(new MenuItem("ElEasy.Cassio.Harass.W", "Use W").SetValue(true));
                    harassMenu.AddItem(new MenuItem("ElEasy.Cassio.Harass.E", "Use E").SetValue(true));
                    harassMenu.AddItem(
                        new MenuItem("ElEasy.Cassio.Harass.Mana", "Minimum Mana").SetValue(new Slider(55)));

                    harassMenu.SubMenu("Harass")
                        .SubMenu("AutoHarass settings")
                        .AddItem(
                            new MenuItem("ElEasy.Cassio.AutoHarass.Activated", "Auto harass", true).SetValue(
                                new KeyBind("L".ToCharArray()[0], KeyBindType.Toggle)));
                    harassMenu.SubMenu("Harass")
                        .SubMenu("AutoHarass settings")
                        .AddItem(new MenuItem("ElEasy.Cassio.AutoHarass.Q", "Use Q").SetValue(true));
                    harassMenu.SubMenu("Harass")
                        .SubMenu("AutoHarass settings")
                        .AddItem(new MenuItem("ElEasy.Cassio.AutoHarass.W", "Use W").SetValue(true));
                    harassMenu.SubMenu("Harass")
                        .SubMenu("AutoHarass settings")
                        .AddItem(new MenuItem("ElEasy.Cassio.AutoHarass.Mana", "Minimum mana").SetValue(new Slider(55)));
                }

                this.Menu.AddSubMenu(harassMenu);

                var clearMenu = new Menu("Clear", "Clear");
                {
                    clearMenu.SubMenu("Lasthit")
                        .AddItem(new MenuItem("ElEasy.Cassio.LastHit.E", "Use E").SetValue(true));

                    clearMenu.SubMenu("Lane clear")
                        .AddItem(new MenuItem("ElEasy.Cassio.LaneClear.Q", "Use Q").SetValue(true));
                    clearMenu.SubMenu("Lane clear")
                        .AddItem(new MenuItem("ElEasy.Cassio.LaneClear.W", "Use W").SetValue(true));
                    clearMenu.SubMenu("Lane clear")
                        .AddItem(new MenuItem("ElEasy.Cassio.LaneClear.E", "Use E").SetValue(true));
                    clearMenu.SubMenu("Lane clear")
                        .AddItem(
                            new MenuItem("ElEasy.Cassio.LaneClear.MinionsHit", "W minions hit").SetValue(
                                new Slider(2, 1, 5)));

                    clearMenu.SubMenu("Jungle clear")
                        .AddItem(new MenuItem("ElEasy.Cassio.JungleClear.Q", "Use Q").SetValue(true));
                    clearMenu.SubMenu("Jungle clear")
                        .AddItem(new MenuItem("ElEasy.Cassio.JungleClear.W", "Use W").SetValue(true));
                    clearMenu.SubMenu("Jungle clear")
                        .AddItem(new MenuItem("ElEasy.Cassio.JungleClear.E", "Use E").SetValue(true));
                    clearMenu.AddItem(
                        new MenuItem("ElEasy.Cassio.LaneClear.Mana", "Minimum mana").SetValue(new Slider(55)));
                }

                this.Menu.AddSubMenu(clearMenu);

                var hitchanceMenu = new Menu("Settings", "Settings");
                {
                    hitchanceMenu.AddItem(new MenuItem("ElEasy.Cassio.Killsteal", "Killsteal").SetValue(true));
                    hitchanceMenu.AddItem(
                        new MenuItem("ElEasy.Cassio.Hitchance", "Hitchance").SetValue(
                            new StringList(new[] { "Low", "Medium", "High", "Very High" }, 3)));
                }

                this.Menu.AddSubMenu(hitchanceMenu);

                var miscellaneousMenu = new Menu("Miscellaneous", "Miscellaneous");
                {
                    miscellaneousMenu.AddItem(
                        new MenuItem("ElEasy.Cassio.Draw.off", "Turn drawings off").SetValue(true));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Cassio.Draw.Q", "Draw Q").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Cassio.Draw.W", "Draw W").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Cassio.Draw.E", "Draw E").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Cassio.Draw.R", "Draw R").SetValue(new Circle()));

                    var dmgAfterE = new MenuItem("ElEasy.Cassio.DrawComboDamage", "Draw combo damage").SetValue(true);
                    var drawFill =
                        new MenuItem("ElEasy.Cassio.DrawColour", "Fill colour", true).SetValue(
                            new Circle(true, Color.FromArgb(0xcc, 0xcc, 0x0, 0x0)));
                    miscellaneousMenu.AddItem(drawFill);
                    miscellaneousMenu.AddItem(dmgAfterE);

                    DrawDamage.DamageToUnit = this.GetComboDamage;
                    DrawDamage.Enabled = dmgAfterE.GetValue<bool>();
                    DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
                    DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;

                    dmgAfterE.ValueChanged +=
                        delegate(object sender, OnValueChangeEventArgs eventArgs)
                            {
                                DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
                            };

                    drawFill.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
                        {
                            DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                            DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
                        };

                    miscellaneousMenu.AddItem(
                        new MenuItem("ElEasy.Cassio.GapCloser.Activated", "Anti gapcloser").SetValue(false));
                    miscellaneousMenu.AddItem(
                        new MenuItem("ElEasy.Cassio.Interrupt.Activated", "Interupt spells").SetValue(false));
                }

                this.Menu.AddSubMenu(miscellaneousMenu);
            }
            rootMenu.AddSubMenu(this.Menu);
        }

        public void Load()
        {
            Console.WriteLine("Loaded Cassiopeia");
            Ignite = Player.GetSpellSlot("summonerdot");

            spells[Spells.Q].SetSkillshot(0.6f, 75f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            spells[Spells.W].SetSkillshot(0.5f, 90f, 2500, false, SkillshotType.SkillshotCircle);
            spells[Spells.R].SetSkillshot(
                0.3f,
                (float)(80 * Math.PI / 180),
                float.MaxValue,
                false,
                SkillshotType.SkillshotCone);
            spells[Spells.E].SetTargetted(0.25f, float.MaxValue);

            Game.OnUpdate += this.OnUpdate;
            Drawing.OnDraw += this.OnDraw;
            Orbwalking.BeforeAttack += this.OrbwalkingBeforeAttack;
            AntiGapcloser.OnEnemyGapcloser += this.AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += this.Interrupter2_OnInterruptableTarget;
        }

        #endregion

        #region Methods

        private float IgniteDamage(Obj_AI_Hero target)
        {
            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var gapCloserActive = this.Menu.Item("ElEasy.Cassio.GapCloser.Activated").GetValue<bool>();

            if (gapCloserActive && spells[Spells.R].IsReady()
                && gapcloser.Sender.Distance(Player) < spells[Spells.R].Range)
            {
                spells[Spells.W].Cast(gapcloser.End);
            }
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (spells[Spells.Q].IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);
            }

            if (spells[Spells.W].IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);
            }

            if (spells[Spells.E].IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);
            }

            if (spells[Spells.R].IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.R);
            }

            return (float)damage;
        }

        private HitChance GetHitchance()
        {
            switch (this.Menu.Item("ElEasy.Cassio.Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.Medium;
            }
        }

        private void Interrupter2_OnInterruptableTarget(
            Obj_AI_Hero sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            var gapCloserActive = this.Menu.Item("ElEasy.Cassio.Interrupt.Activated").GetValue<bool>();
            if (!gapCloserActive)
            {
                return;
            }

            if (args.DangerLevel != Interrupter2.DangerLevel.High || sender.Distance(Player) > spells[Spells.R].Range)
            {
                return;
            }

            if (sender.IsValidTarget(spells[Spells.R].Range) && args.DangerLevel == Interrupter2.DangerLevel.High
                && spells[Spells.R].IsReady())
            {
                spells[Spells.R].Cast(sender);
            }
        }

        private void KillSteal()
        {
            if (this.Menu.Item("ElEasy.Cassio.Killsteal").IsActive())
            {
                foreach (var target in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(
                            hero =>
                            Player.Distance(hero.ServerPosition) <= spells[Spells.Q].Range && !hero.IsMe
                            && hero.IsValidTarget() && hero.IsEnemy && !hero.IsInvulnerable))
                {
                    var qDamage = spells[Spells.Q].GetDamage(target);
                    var wDamage = spells[Spells.W].GetDamage(target);

                    if (target.IsValidTarget(600) && this.IgniteDamage(target) >= target.Health)
                    {
                        Player.Spellbook.CastSpell(Ignite, target);
                    }

                    if (target.Health - qDamage < 0 && spells[Spells.Q].IsReady())
                    {
                        var prediction = spells[Spells.Q].GetPrediction(target);
                        if (prediction.Hitchance >= this.CustomHitChance
                            && (Player.ServerPosition.Distance(
                                spells[Spells.Q].GetPrediction(target, true).CastPosition) < spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }
                    }

                    if (target.Health - wDamage < 0 && spells[Spells.W].IsReady() && spells[Spells.W].IsReady())
                    {
                        var prediction = spells[Spells.W].GetPrediction(target);
                        if (prediction.Hitchance >= this.CustomHitChance
                            && (Player.ServerPosition.Distance(
                                spells[Spells.W].GetPrediction(target, true).CastPosition) < spells[Spells.W].Range))
                        {
                            spells[Spells.W].Cast(target);
                        }
                    }
                }
            }
        }

        private void OnAutoHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            var useQ = this.Menu.Item("ElEasy.Cassio.AutoHarass.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Cassio.AutoHarass.W").IsActive();
            var mana = this.Menu.Item("ElEasy.Cassio.AutoHarass.Mana").GetValue<Slider>().Value;

            if (Player.Mana < mana)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady())
            {
                var prediction = spells[Spells.Q].GetPrediction(target);
                if (prediction.Hitchance >= this.CustomHitChance
                    && (Player.ServerPosition.Distance(spells[Spells.Q].GetPrediction(target, true).CastPosition)
                        < spells[Spells.Q].Range))
                {
                    spells[Spells.Q].Cast(target);
                }
            }

            if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
            {
                if (target.HasBuffOfType(BuffType.Poison))
                {
                    return;
                }

                var prediction = spells[Spells.W].GetPrediction(target);
                if (prediction.Hitchance >= this.CustomHitChance
                    && (Player.ServerPosition.Distance(spells[Spells.W].GetPrediction(target, true).CastPosition)
                        < spells[Spells.W].Range))
                {
                    spells[Spells.W].Cast(target);
                }
            }
        }

        private void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            var rtarget = TargetSelector.GetTarget(spells[Spells.R].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid || rtarget == null || !rtarget.IsValid)
            {
                return;
            }

            var useQ = this.Menu.Item("ElEasy.Cassio.Combo.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Cassio.Combo.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Cassio.Combo.E").IsActive();
            var useR = this.Menu.Item("ElEasy.Cassio.Combo.R").IsActive();
            var useI = this.Menu.Item("ElEasy.Cassio.Combo.Ignite").IsActive();
            var countEnemies = this.Menu.Item("ElEasy.Cassio.Combo.R.Count").GetValue<Slider>().Value;

            if (useQ && spells[Spells.Q].IsReady())
            {
                var prediction = spells[Spells.Q].GetPrediction(target);
                if ((Player.ServerPosition.Distance(prediction.CastPosition) < spells[Spells.Q].Range)
                    && target.IsVisible && !target.IsDead)
                {
                    spells[Spells.Q].CastIfHitchanceEquals(target, this.CustomHitChance);
                    lastQ = Environment.TickCount;
                }
            }

            if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
            {
                if (!target.HasBuffOfType(BuffType.Poison))
                {
                    return;
                }

                var playLegit = this.Menu.Item("ElEasy.Cassio.E.Legit").IsActive();
                var legitCastDelay = this.Menu.Item("ElEasy.Cassio.E.Delay").GetValue<Slider>().Value;

                if (playLegit)
                {
                    if (Environment.TickCount > lastE + legitCastDelay)
                    {
                        spells[Spells.E].CastOnUnit(target);
                        lastE = Environment.TickCount;
                    }
                }
                else
                {
                    spells[Spells.E].Cast(target);
                    lastE = Environment.TickCount;
                }
            }

            if (useW && spells[Spells.W].IsReady() && Environment.TickCount > lastQ + spells[Spells.Q].Delay * 1000)
            {
                var prediction = spells[Spells.W].GetPrediction(target);
                if ((Player.ServerPosition.Distance(prediction.CastPosition) < spells[Spells.W].Range))
                {
                    spells[Spells.W].CastIfHitchanceEquals(target, this.CustomHitChance);
                }
            }

            if (useR && spells[Spells.R].IsReady() && spells[Spells.R].IsInRange(rtarget))
            {
                var prediction = spells[Spells.R].GetPrediction(rtarget);
                if (prediction.Hitchance >= HitChance.VeryHigh
                    && Player.CountEnemiesInRange(spells[Spells.R].Range) >= countEnemies)
                {
                    spells[Spells.R].Cast(rtarget);
                }
            }

            if (Player.Distance(target) <= 600 && this.IgniteDamage(target) >= target.Health && useI)
            {
                Player.Spellbook.CastSpell(Ignite, target);
            }
        }

        private void OnDraw(EventArgs args)
        {
            var drawOff = this.Menu.Item("ElEasy.Cassio.Draw.off").IsActive();
            var drawQ = this.Menu.Item("ElEasy.Cassio.Draw.Q").GetValue<Circle>();
            var drawW = this.Menu.Item("ElEasy.Cassio.Draw.W").GetValue<Circle>();
            var drawE = this.Menu.Item("ElEasy.Cassio.Draw.E").GetValue<Circle>();
            var drawR = this.Menu.Item("ElEasy.Cassio.Draw.R").GetValue<Circle>();

            if (drawOff)
            {
                return;
            }

            if (drawQ.Active)
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.Q].Range, Color.White);
                }
            }

            if (drawW.Active)
            {
                if (spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.W].Range, Color.White);
                }
            }

            if (drawE.Active)
            {
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.E].Range, Color.White);
                }
            }

            if (drawR.Active)
            {
                if (spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.R].Range, Color.White);
                }
            }
        }

        private void OnHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            var rtarget = TargetSelector.GetTarget(spells[Spells.R].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid || rtarget == null || !rtarget.IsValid)
            {
                return;
            }

            var useQ = this.Menu.Item("ElEasy.Cassio.Harass.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Cassio.Harass.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Cassio.Harass.E").IsActive();
            var playerMana = this.Menu.Item("ElEasy.Cassio.Harass.Mana").GetValue<Slider>().Value;

            if (Player.ManaPercent < playerMana)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target))
            {
                if ((Player.ServerPosition.Distance(spells[Spells.Q].GetPrediction(target, true).CastPosition)
                     < spells[Spells.Q].Range))
                {
                    spells[Spells.Q].CastIfHitchanceEquals(target, this.CustomHitChance);
                }
            }

            if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
            {
                if (!target.HasBuffOfType(BuffType.Poison))
                {
                    return;
                }

                spells[Spells.E].Cast(target);
            }

            if (useW && spells[Spells.W].IsReady())
            {
                if ((Player.ServerPosition.Distance(spells[Spells.W].GetPrediction(target, true).CastPosition)
                     < spells[Spells.W].Range))
                {
                    spells[Spells.W].CastIfHitchanceEquals(target, this.CustomHitChance);
                }
            }
        }

        private void OnJungleclear()
        {
            var mana = this.Menu.Item("ElEasy.Cassio.LaneClear.Mana").GetValue<Slider>().Value;
            if (Player.Mana < mana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                spells[Spells.E].Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            if (minions.Count <= 0)
            {
                return;
            }

            var useQ = this.Menu.Item("ElEasy.Cassio.JungleClear.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Cassio.JungleClear.E").IsActive();
            var useE = this.Menu.Item("ElEasy.Cassio.JungleClear.W").IsActive();

            if (useQ && spells[Spells.Q].IsReady())
            {
                var farmLocation = spells[Spells.Q].GetCircularFarmLocation(minions);
                spells[Spells.Q].Cast(farmLocation.Position);
            }

            if (useE && spells[Spells.E].IsReady())
            {
                var etarget =
                    minions.Where(x => x.Distance(Player) < spells[Spells.E].Range && x.HasBuffOfType(BuffType.Poison))
                        .OrderByDescending(x => x.Health)
                        .FirstOrDefault();

                if (etarget != null)
                {
                    spells[Spells.E].Cast(etarget.ServerPosition);
                }
            }

            if (useW && spells[Spells.W].IsReady())
            {
                var farmLocation = spells[Spells.W].GetCircularFarmLocation(minions);
                spells[Spells.W].Cast(farmLocation.Position);
            }
        }

        private void OnLaneclear()
        {
            var mana = this.Menu.Item("ElEasy.Cassio.LaneClear.Mana").GetValue<Slider>().Value;
            if (Player.ManaPercent < mana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.Q].Range);
            if (minions.Count <= 0)
            {
                return;
            }

            var useQ = this.Menu.Item("ElEasy.Cassio.LaneClear.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Cassio.LaneClear.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Cassio.LaneClear.E").IsActive();

            if (useQ && spells[Spells.Q].IsReady())
            {
                if (minions.Count <= 1)
                {
                    return;
                }

                var farmLocation = spells[Spells.Q].GetCircularFarmLocation(minions);
                spells[Spells.Q].Cast(farmLocation.Position);
            }

            if (useE && spells[Spells.E].IsReady())
            {
                var etarget =
                    minions.Where(
                        x =>
                        x.Distance(Player) < spells[Spells.E].Range
                        && x.Health <= ObjectManager.Player.GetSpellDamage(x, SpellSlot.E)
                        && x.HasBuffOfType(BuffType.Poison))
                        .OrderByDescending(x => x.Health)
                        .FirstOrDefault(
                            y =>
                            y.HPRegenRate + y.Health <= spells[Spells.E].GetDamage(y)
                            && HealthPrediction.GetHealthPrediction(
                                y,
                                (int)spells[Spells.E].Delay,
                                (int)spells[Spells.E].Speed) <= spells[Spells.E].GetDamage(y));

                spells[Spells.E].Cast(etarget);
            }

            if (useW && spells[Spells.W].IsReady())
            {
                var farmLocation = spells[Spells.W].GetCircularFarmLocation(minions);
                if (farmLocation.MinionsHit >= 1)
                {
                    spells[Spells.W].Cast(farmLocation.Position);
                }
            }
        }

        private void OnLasthit()
        {
            var minions = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.Q].Range);
            if (minions.Count <= 0)
            {
                return;
            }

            var useE = this.Menu.Item("ElEasy.Cassio.LastHit.E").IsActive();

            if (useE && spells[Spells.E].IsReady())
            {
                var etarget =
                    minions.Where(
                        x =>
                        x.Distance(Player) < spells[Spells.E].Range
                        && x.Health <= ObjectManager.Player.GetSpellDamage(x, SpellSlot.E)
                        && x.HasBuffOfType(BuffType.Poison))
                        .OrderByDescending(x => x.Health)
                        .FirstOrDefault(
                            y =>
                            y.HPRegenRate + y.Health <= spells[Spells.E].GetDamage(y)
                            && HealthPrediction.GetHealthPrediction(
                                y,
                                (int)spells[Spells.E].Delay,
                                (int)spells[Spells.E].Speed) <= spells[Spells.E].GetDamage(y));

                spells[Spells.E].Cast(etarget);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    this.OnCombo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    this.OnHarass();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    this.OnLaneclear();
                    this.OnJungleclear();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    this.OnLasthit();
                    break;
            }

            var autoHarass = this.Menu.Item("ElEasy.Cassio.AutoHarass.Activated", true).GetValue<KeyBind>().Active;
            if (autoHarass)
            {
                this.OnAutoHarass();
            }

            this.KillSteal();
        }

        private void OrbwalkingBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var target = args.Target;
            if (!target.IsValidTarget() || !(args.Target is Obj_AI_Base)
                || Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            var t = (Obj_AI_Base)target;
            if (spells[Spells.E].IsReady() && t.HasBuffOfType(BuffType.Poison)
                && target.IsValidTarget(spells[Spells.E].Range))
            {
                args.Process = false;
            }
        }

        #endregion
    }
}