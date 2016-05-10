namespace ElEasy.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    public class Ryze : IPlugin
    {
        #region Static Fields

        private static readonly Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                                       {
                                                                           { Spells.Q, new Spell(SpellSlot.Q, 900) },
                                                                           { Spells.W, new Spell(SpellSlot.W, 600) },
                                                                           { Spells.E, new Spell(SpellSlot.E, 600) },
                                                                           { Spells.R, new Spell(SpellSlot.R) }
                                                                       };

        private static SpellSlot Ignite;

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
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        private Menu Menu { get; set; }

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
            this.Menu = new Menu("ElRyze", "ElRyze");
            {
                var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
                Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
                this.Menu.AddSubMenu(orbwalkerMenu);

                var targetSelector = new Menu("Target Selector", "TargetSelector");
                TargetSelector.AddToMenu(targetSelector);
                this.Menu.AddSubMenu(targetSelector);

                var comboMenu = new Menu("Combo", "Combo");
                {
                    comboMenu.AddItem(new MenuItem("ElEasy.Ryze.Combo.Q", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Ryze.Combo.W", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Ryze.Combo.E", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Ryze.Combo.R", "Use R").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Ryze.Combo.R.HP", "Use R when HP").SetValue(new Slider(100)));
                    comboMenu.AddItem(new MenuItem("ElEasy.Ryze.Combo.Ignite", "Use Ignite").SetValue(true));
                }

                this.Menu.AddSubMenu(comboMenu);

                var harassMenu = new Menu("Harass", "Harass");
                {
                    harassMenu.AddItem(new MenuItem("ElEasy.Ryze.Harass.Q", "Use Q").SetValue(true));
                    harassMenu.AddItem(new MenuItem("ElEasy.Ryze.Harass.W", "Use W").SetValue(true));
                    harassMenu.AddItem(new MenuItem("ElEasy.Ryze.Harass.E", "Use E").SetValue(true));
                    harassMenu.AddItem(
                        new MenuItem("ElEasy.Ryze.Harass.Player.Mana", "Minimum Mana").SetValue(new Slider(55)));

                    harassMenu.SubMenu("Harass")
                        .SubMenu("AutoHarass settings")
                        .AddItem(
                            new MenuItem("ElEasy.Ryze.AutoHarass.Activated", "Auto harass", true).SetValue(
                                new KeyBind("L".ToCharArray()[0], KeyBindType.Toggle)));
                    harassMenu.SubMenu("Harass")
                        .SubMenu("AutoHarass settings")
                        .AddItem(new MenuItem("ElEasy.Ryze.AutoHarass.Q", "Use Q").SetValue(true));
                    harassMenu.SubMenu("Harass")
                        .SubMenu("AutoHarass settings")
                        .AddItem(new MenuItem("ElEasy.Ryze.AutoHarass.W", "Use W").SetValue(true));

                    harassMenu.SubMenu("Harass")
                        .SubMenu("AutoHarass settings")
                        .AddItem(new MenuItem("ElEasy.Ryze.AutoHarass.E", "Use E").SetValue(true));

                    harassMenu.SubMenu("Harass")
                        .SubMenu("AutoHarass settings")
                        .AddItem(new MenuItem("ElEasy.Ryze.AutoHarass.Mana", "Minimum mana").SetValue(new Slider(55)));
                }

                this.Menu.AddSubMenu(harassMenu);

                var clearMenu = new Menu("Clear", "Clear");
                {
                    clearMenu.SubMenu("Lasthit").AddItem(new MenuItem("ElEasy.Ryze.Lasthit.Q", "Use Q").SetValue(true));
                    clearMenu.SubMenu("Lasthit").AddItem(new MenuItem("ElEasy.Ryze.Lasthit.W", "Use E").SetValue(true));
                    clearMenu.SubMenu("Lasthit").AddItem(new MenuItem("ElEasy.Ryze.Lasthit.E", "Use W").SetValue(true));

                    clearMenu.SubMenu("Laneclear")
                        .AddItem(new MenuItem("ElEasy.Ryze.LaneClear.Q", "Use Q").SetValue(true));
                    clearMenu.SubMenu("Laneclear")
                        .AddItem(new MenuItem("ElEasy.Ryze.LaneClear.W", "Use W").SetValue(true));
                    clearMenu.SubMenu("Laneclear")
                        .AddItem(new MenuItem("ElEasy.Ryze.LaneClear.E", "Use E").SetValue(true));
                    clearMenu.SubMenu("Jungleclear")
                        .AddItem(new MenuItem("ElEasy.Ryze.JungleClear.Q", "Use Q").SetValue(true));
                    clearMenu.SubMenu("Jungleclear")
                        .AddItem(new MenuItem("ElEasy.Ryze.JungleClear.W", "Use W").SetValue(true));
                    clearMenu.SubMenu("Jungleclear")
                        .AddItem(new MenuItem("ElEasy.Ryze.JungleClear.E", "Use E").SetValue(true));
                    clearMenu.AddItem(
                        new MenuItem("ElEasy.Ryze.Clear.Player.Mana", "Minimum Mana for clear").SetValue(new Slider(55)));
                }

                this.Menu.AddSubMenu(clearMenu);

                var miscellaneousMenu = new Menu("Miscellaneous", "Miscellaneous");
                {
                    miscellaneousMenu.AddItem(
                        new MenuItem("ElEasy.Ryze.GapCloser.Activated", "Anti gapcloser").SetValue(true));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Ryze.AA", "Don't use AA in combo").SetValue(false));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Ryze.Draw.off", "Turn drawings off").SetValue(true));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Ryze.Draw.Q", "Draw Q").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Ryze.Draw.W", "Draw W").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Ryze.Draw.E", "Draw E").SetValue(new Circle()));

                    var dmgAfterE = new MenuItem("ElEasy.Ryze.DrawComboDamage", "Draw combo damage").SetValue(true);
                    var drawFill =
                        new MenuItem("ElEasy.Ryze.DrawColour", "Fill colour", true).SetValue(
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
                }

                this.Menu.AddSubMenu(miscellaneousMenu);

            }

            rootMenu.AddSubMenu(this.Menu);
        }

        public void Load()
        {
            Console.WriteLine("Loaded Ryze");
            Ignite = this.Player.GetSpellSlot("summonerdot");
            spells[Spells.Q].SetSkillshot(
                spells[Spells.Q].Instance.SData.SpellCastTime,
                spells[Spells.Q].Instance.SData.LineWidth,
                spells[Spells.Q].Instance.SData.MissileSpeed,
                true,
                SkillshotType.SkillshotLine);

            Game.OnUpdate += this.OnUpdate;
            Drawing.OnDraw += this.OnDraw;
            Orbwalking.BeforeAttack += this.OrbwalkingBeforeAttack;
            AntiGapcloser.OnEnemyGapcloser += this.AntiGapcloser_OnEnemyGapcloser;
        }

        #endregion

        #region Methods

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (this.Menu.Item("ElEasy.Ryze.Interrupt.Activated").IsActive() && spells[Spells.W].IsReady()
                && gapcloser.Sender.Distance(this.Player) < spells[Spells.W].Range)
            {
                spells[Spells.W].CastOnUnit(gapcloser.Sender);
            }
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (spells[Spells.Q].IsReady())
            {
                damage += this.Player.GetSpellDamage(enemy, SpellSlot.Q);
            }

            if (spells[Spells.W].IsReady())
            {
                damage += this.Player.GetSpellDamage(enemy, SpellSlot.W);
            }

            if (spells[Spells.E].IsReady())
            {
                damage += this.Player.GetSpellDamage(enemy, SpellSlot.E);
            }

            if (spells[Spells.R].IsReady())
            {
                damage += this.Player.GetSpellDamage(enemy, SpellSlot.R);
            }

            return (float)damage;
        }

        private float IgniteDamage(Obj_AI_Hero target)
        {
            if (Ignite == SpellSlot.Unknown || this.Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)this.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private void OnAutoHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = this.Menu.Item("ElEasy.Ryze.AutoHarass.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Ryze.AutoHarass.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Ryze.AutoHarass.E").IsActive();
            var mana = this.Menu.Item("ElEasy.Ryze.AutoHarass.Mana").GetValue<Slider>().Value;

            if (this.Player.ManaPercent < mana)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target))
            {
                var prediction = spells[Spells.Q].GetPrediction(target);
                if (prediction.Hitchance != HitChance.Impossible && prediction.Hitchance != HitChance.OutOfRange
                    && prediction.Hitchance != HitChance.Collision)
                {
                    spells[Spells.Q].Cast(target);
                }
            }

            if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
            {
                spells[Spells.W].CastOnUnit(target);
            }

            if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
            {
                spells[Spells.E].CastOnUnit(target);
            }
        }

        private void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.W].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = this.Menu.Item("ElEasy.Ryze.Combo.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Ryze.Combo.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Ryze.Combo.E").IsActive();
            var useR = this.Menu.Item("ElEasy.Ryze.Combo.R").IsActive();
            var rHp = this.Menu.Item("ElEasy.Ryze.Combo.R.HP").GetValue<Slider>().Value;
            var useI = this.Menu.Item("ElEasy.Ryze.Combo.Ignite").IsActive();

            if (this.Player.Buffs.Count(buf => buf.Name == "RyzePassiveStack") == 1)
            {
                var prediction = spells[Spells.Q].GetPrediction(target);
                if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                {
                    spells[Spells.Q].Cast(target);
                }
                if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
                {
                    spells[Spells.W].CastOnUnit(target);
                }
                if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
                {
                    spells[Spells.E].CastOnUnit(target);
                }
                if (useR && spells[Spells.R].IsReady())
                {
                    spells[Spells.R].Cast();
                }
            }

            if (this.Player.Buffs.Count(buf => buf.Name == "RyzePassiveStack") <= 2)
            {
                var prediction = spells[Spells.Q].GetPrediction(target);
                if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                {
                    spells[Spells.Q].Cast(target);
                }

                if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
                {
                    spells[Spells.E].CastOnUnit(target);
                }
                if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
                {
                    spells[Spells.W].CastOnUnit(target);
                }

                if (useR && spells[Spells.R].IsReady() && this.Player.HealthPercent <= rHp)
                {
                    spells[Spells.R].Cast(this.Player);
                }
            }
            else if (this.Player.Buffs.Count(buf => buf.Name == "RyzePassiveStack") == 3)
            {
                if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
                {
                    spells[Spells.W].CastOnUnit(target);
                }

                if (useQ && spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target))
                {
                    var prediction = spells[Spells.Q].GetPrediction(target);
                    if (prediction.Hitchance != HitChance.Impossible && prediction.Hitchance != HitChance.OutOfRange
                        && prediction.Hitchance != HitChance.Collision)
                    {
                        spells[Spells.Q].Cast(target);
                    }
                }

                if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
                {
                    spells[Spells.E].CastOnUnit(target);
                }

                if (useQ && spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target))
                {
                    var prediction = spells[Spells.Q].GetPrediction(target);
                    if (prediction.Hitchance != HitChance.Impossible && prediction.Hitchance != HitChance.OutOfRange
                        && prediction.Hitchance != HitChance.Collision)
                    {
                        spells[Spells.Q].Cast(target);
                    }
                }

                if (useR && spells[Spells.R].IsReady() && this.Player.HealthPercent <= rHp)
                {
                    spells[Spells.R].Cast(this.Player);
                }
            }
            else if (this.Player.Buffs.Count(buf => buf.Name == "RyzePassiveStack") == 4)
            {
                if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
                {
                    spells[Spells.W].CastOnUnit(target);
                }

                if (useQ && spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target))
                {
                    var prediction = spells[Spells.Q].GetPrediction(target);
                    if (prediction.Hitchance != HitChance.Impossible && prediction.Hitchance != HitChance.OutOfRange
                        && prediction.Hitchance != HitChance.Collision)
                    {
                        spells[Spells.Q].Cast(target);
                    }
                }

                if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
                {
                    spells[Spells.E].CastOnUnit(target);
                }
            }

            if (this.Player.Distance(target) <= 600 && this.IgniteDamage(target) >= target.Health && useI)
            {
                this.Player.Spellbook.CastSpell(Ignite, target);
            }
        }

        private void OnDraw(EventArgs args)
        {
            var drawOff = this.Menu.Item("ElEasy.Ryze.Draw.off").IsActive();
            var drawQ = this.Menu.Item("ElEasy.Ryze.Draw.Q").GetValue<Circle>();
            var drawW = this.Menu.Item("ElEasy.Ryze.Draw.W").GetValue<Circle>();
            var drawE = this.Menu.Item("ElEasy.Ryze.Draw.E").GetValue<Circle>();

            if (drawOff)
            {
                return;
            }

            if (drawQ.Active)
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(this.Player.Position, spells[Spells.Q].Range, Color.White);
                }
            }

            if (drawE.Active)
            {
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(this.Player.Position, spells[Spells.E].Range, Color.White);
                }
            }

            if (drawW.Active)
            {
                if (spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(this.Player.Position, spells[Spells.W].Range, Color.White);
                }
            }
        }

        private void OnHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = this.Menu.Item("ElEasy.Ryze.Harass.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Ryze.Harass.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Ryze.Harass.E").IsActive();
            var mana = this.Menu.Item("ElEasy.Ryze.Harass.Player.Mana").GetValue<Slider>().Value;

            if (this.Player.ManaPercent < mana)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target))
            {
                var pred = spells[Spells.Q].GetPrediction(target);
                if (pred.Hitchance >= HitChance.High && pred.CollisionObjects.Count == 0)
                {
                    spells[Spells.Q].Cast(target);
                }
            }

            if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
            {
                spells[Spells.W].CastOnUnit(target);
            }

            if (useE && spells[Spells.E].IsReady() && spells[Spells.E].IsInRange(target))
            {
                spells[Spells.E].CastOnUnit(target);
            }
        }

        private void OnJungleclear()
        {
            var useQ = this.Menu.Item("ElEasy.Ryze.JungleClear.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Ryze.JungleClear.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Ryze.JungleClear.E").IsActive();
            var mana = this.Menu.Item("ElEasy.Ryze.Clear.Player.Mana").GetValue<Slider>().Value;

            if (this.Player.ManaPercent < mana)
            {
                return;
            }

            var minions =
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    spells[Spells.Q].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (minions == null)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady() && minions.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(minions);
            }

            if (useW && spells[Spells.W].IsReady() && minions.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast(minions);
            }

            if (useE && spells[Spells.E].IsReady() && minions.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(minions);
            }
        }

        private void OnLaneclear()
        {
            var useQ = this.Menu.Item("ElEasy.Ryze.LaneClear.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Ryze.LaneClear.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Ryze.LaneClear.E").IsActive();
            var mana = this.Menu.Item("ElEasy.Ryze.Clear.Player.Mana").GetValue<Slider>().Value;

            if (this.Player.ManaPercent < mana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(this.Player.ServerPosition, spells[Spells.W].Range).FirstOrDefault();
            if (minions == null)
            {
                return;
            }

            if (useW && spells[Spells.W].IsReady())
            {
                spells[Spells.W].CastOnUnit(minions);
            }

            if (useQ && spells[Spells.Q].IsReady())
            {
                if (HealthPrediction.GetHealthPrediction(minions, (int)0.25)
                    <= this.Player.GetSpellDamage(minions, SpellSlot.Q))
                {
                    spells[Spells.Q].Cast(minions);
                }
            }

            if (useE && spells[Spells.E].IsReady())
            {
                spells[Spells.E].Cast(minions);
            }
        }

        private void OnLasthit()
        {
            var useQ = this.Menu.Item("ElEasy.Ryze.Lasthit.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Ryze.Lasthit.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Ryze.Lasthit.E").IsActive();

            var minions = MinionManager.GetMinions(this.Player.ServerPosition, spells[Spells.W].Range).FirstOrDefault();
            if (minions == null)
            {
                return;
            }

            if (spells[Spells.Q].IsReady() && useQ)
            {
                if (HealthPrediction.GetHealthPrediction(minions, (int)0.25)
                    <= this.Player.GetSpellDamage(minions, SpellSlot.Q))
                {
                    spells[Spells.Q].Cast(minions);
                }
            }

            if (spells[Spells.W].IsReady() && useW)
            {
                var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, spells[Spells.W].Range);
                {
                    foreach (var minion in
                        allMinions.Where(
                            minion => minion.Health <= ObjectManager.Player.GetSpellDamage(minion, SpellSlot.W)))
                    {
                        if (minion.IsValidTarget(spells[Spells.W].Range))
                        {
                            spells[Spells.W].CastOnUnit(minion);
                            return;
                        }
                    }
                }
            }

            if (spells[Spells.E].IsReady() && useE)
            {
                var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, spells[Spells.E].Range);
                {
                    foreach (var minion in
                        allMinions.Where(
                            minion => minion.Health <= ObjectManager.Player.GetSpellDamage(minion, SpellSlot.E)))
                    {
                        if (minion.IsValidTarget())
                        {
                            spells[Spells.E].CastOnUnit(minion);
                            return;
                        }
                    }
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.Player.IsDead)
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    this.OnCombo();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    this.OnLasthit();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    this.OnLaneclear();
                    this.OnJungleclear();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    this.OnHarass();
                    break;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (this.Player.Buffs.Count(buf => buf.Name == "Muramana") == 0)
                    {
                        var muramana = ItemData.Muramana.GetItem();
                        if (muramana.IsOwned(this.Player))
                        {
                            muramana.Cast();
                        }
                    }
                    break;
                default:
                    if (this.Player.Buffs.Count(buf => buf.Name == "Muramana") != 0)
                    {
                        var muramana = ItemData.Muramana.GetItem();
                        if (muramana.IsOwned(this.Player))
                        {
                            muramana.Cast();
                        }
                    }
                    break;
            }

            if (this.Menu.Item("ElEasy.Ryze.AutoHarass.Activated", true).GetValue<KeyBind>().Active)
            {
                this.OnAutoHarass();
            }
        }

        private void OrbwalkingBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (this.Menu.Item("ElEasy.Ryze.AA").IsActive() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                args.Process = false;
            }
            else
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    args.Process =
                        !(spells[Spells.Q].IsReady() || spells[Spells.W].IsReady() || spells[Spells.E].IsReady()
                          || this.Player.Distance(args.Target) >= 1000);
                }
            }
        }

        #endregion
    }
}