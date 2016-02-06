namespace ElTristana
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    #region

    internal enum Spells
    {
        Q,

        E,

        R
    }

    #endregion

    internal static class Tristana
    {
        #region Static Fields

        public static Orbwalking.Orbwalker Orbwalker;

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>()
                                                             {
                                                                 { Spells.Q, new Spell(SpellSlot.Q, 550) },
                                                                 { Spells.E, new Spell(SpellSlot.E, 625) },
                                                                 { Spells.R, new Spell(SpellSlot.R, 700) },
                                                             };

        #endregion

        #region Public Properties

        public static String ScriptVersion
        {
            get
            {
                return typeof(Tristana).Assembly.GetName().Version.ToString();
            }
        }

        #endregion

        #region Properties

        private static Obj_AI_Hero Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        /// <summary>
        ///     Gets or sets the random.
        /// </summary>
        /// <value>
        ///     The random.
        /// </value>
        private static Random Random { get; set; }

        #endregion

        #region Public Methods and Operators

        public static void OnLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Tristana")
            {
                return;
            }

            try
            {
                Console.WriteLine("Injected ElTristana AMK");
                Notifications.AddNotification(String.Format("ElTristana by jQuery v{0}", ScriptVersion), 8000);
                Game.PrintChat(
                    "[00:00] <font color='#f9eb0b'>HEEEEEEY!</font> Use ElUtilitySuite for optimal results! xo jQuery");

                MenuInit.Initialize();
                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                Spellbook.OnCastSpell += OnSpellCast;
                AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
                Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Methods

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (IsActive("ElTristana.Antigapcloser"))
            {
                if (gapcloser.Sender.IsValidTarget(250f) && spells[Spells.R].IsReady())
                {
                    spells[Spells.R].Cast(gapcloser.Sender);
                }
            }
        }

        private static BuffInstance GetECharge(this Obj_AI_Base target)
        {
            return target.Buffs.Find(x => x.DisplayName == "TristanaECharge");
        }

        private static void Interrupter2_OnInterruptableTarget(
            Obj_AI_Hero sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!IsActive("ElTristana.Interrupter"))
            {
                return;
            }

            if (sender.Distance(Player) < spells[Spells.R].Range)
            {
                spells[Spells.R].Cast(sender);
            }
        }

        private static bool IsActive(string menuItem)
        {
            return MenuInit.Menu.Item(menuItem).GetValue<bool>();
        }

        private static bool IsECharged(this Obj_AI_Base target)
        {
            return target.GetECharge() != null;
        }

        private static void OnCombo()
        {
            var eTarget =
                HeroManager.Enemies.Find(
                    x => x.HasBuff("TristanaECharge") && x.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)));
            var target = eTarget ?? TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Physical);

            if (!target.IsValidTarget())
            {
                return;
            }

            if (eTarget != null && IsActive("ElTristana.Combo.Focus.E"))
            {
                TargetSelector.SetTarget(target);
                Orbwalker.ForceTarget(target);
            }

            if (spells[Spells.E].IsReady() && IsActive("ElTristana.Combo.E")
                && Player.ManaPercent > MenuInit.Menu.Item("ElTristana.Combo.E.Mana").GetValue<Slider>().Value)
            {
                foreach (var hero in HeroManager.Enemies.OrderByDescending(x => x.Health))
                {
                    if (hero.IsEnemy)
                    {
                        var getEnemies = MenuInit.Menu.Item("ElTristana.E.On" + hero.CharData.BaseSkinName);
                        if (getEnemies != null && getEnemies.GetValue<bool>())
                        {
                            spells[Spells.E].Cast(hero);
                        }

                        if (getEnemies != null && !getEnemies.GetValue<bool>() && Player.CountEnemiesInRange(1500) == 1)
                        {
                            spells[Spells.E].Cast(hero);
                        }
                    }
                }
            }

            UseItems(target);

            if (spells[Spells.R].IsReady() && IsActive("ElTristana.Combo.R"))
            {
                if (spells[Spells.R].GetDamage(target) > target.Health)
                {
                    spells[Spells.R].Cast(target);
                }
            }

            if (IsECharged(target) && IsActive("ElTristana.Combo.Always.RE"))
            {
                if (spells[Spells.R].GetDamage(target)
                    + spells[Spells.E].GetDamage(target) * ((0.3 * target.GetBuffCount("TristanaECharge") + 1))
                    > target.Health)
                {
                    spells[Spells.R].Cast(target);
                }
            }

            if (spells[Spells.Q].IsReady() && IsActive("ElTristana.Combo.Q")
                && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.Q].Cast();
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (IsActive("ElTristana.Draw.off"))
            {
                return;
            }

            if (MenuInit.Menu.Item("ElTristana.Draw.Q").GetValue<Circle>().Active)
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.Q].Range, Color.White);
                }
            }

            if (MenuInit.Menu.Item("ElTristana.Draw.E").GetValue<Circle>().Active)
            {
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.E].Range, Color.White);
                }
            }

            if (MenuInit.Menu.Item("ElTristana.Draw.R").GetValue<Circle>().Active)
            {
                if (spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.R].Range, Color.White);
                }
            }
        }

        private static void OnHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            if (spells[Spells.E].IsReady() && IsActive("ElTristana.Harass.E")
                && Player.ManaPercent > MenuInit.Menu.Item("ElTristana.Harass.E.Mana").GetValue<Slider>().Value)
            {
                foreach (var hero in HeroManager.Enemies.OrderByDescending(x => x.Health))
                {
                    if (hero.IsEnemy)
                    {
                        var getEnemies = MenuInit.Menu.Item("ElTristana.E.On.Harass" + hero.CharData.BaseSkinName);
                        if (getEnemies != null && getEnemies.GetValue<bool>())
                        {
                            spells[Spells.E].Cast(hero);
                        }
                    }
                }
            }

            if (spells[Spells.Q].IsReady() && IsActive("ElTristana.Harass.Q")
                && target.IsValidTarget(spells[Spells.E].Range))
            {
                if (IsECharged(target) && IsActive("ElTristana.Harass.QE"))
                {
                    spells[Spells.Q].Cast();
                }
                else if (!IsActive("ElTristana.Harass.QE"))
                {
                    spells[Spells.Q].Cast();
                }
            }
        }

        private static void OnJungleClear()
        {
            var minions =
                MinionManager.GetMinions(
                    spells[Spells.Q].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (!minions.IsValidTarget() || minions == null)
            {
                return;
            }

            if (spells[Spells.E].IsReady() && IsActive("ElTristana.JungleClear.E")
                && Player.ManaPercent > MenuInit.Menu.Item("ElTristana.JungleClear.E.Mana").GetValue<Slider>().Value)
            {
                spells[Spells.E].CastOnUnit(minions);
            }

            if (spells[Spells.Q].IsReady() && IsActive("ElTristana.JungleClear.Q"))
            {
                spells[Spells.Q].Cast();
            }
        }

        private static void OnLaneClear()
        {
            if (IsActive("ElTristana.LaneClear.Tower"))
            {
                foreach (var tower in ObjectManager.Get<Obj_AI_Turret>())
                {
                    if (!tower.IsDead && tower.Health > 100 && tower.IsEnemy && tower.IsValidTarget()
                        && Player.ServerPosition.Distance(tower.ServerPosition)
                        < Orbwalking.GetRealAutoAttackRange(Player))
                    {
                        spells[Spells.E].Cast(tower);
                    }
                }
            }

            var minions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                spells[Spells.E].Range,
                MinionTypes.All,
                MinionTeam.NotAlly,
                MinionOrderTypes.MaxHealth);

            if (minions.Count <= 0)
            {
                return;
            }

            if (spells[Spells.E].IsReady() && IsActive("ElTristana.LaneClear.E") && minions.Count > 2
                && Player.ManaPercent > MenuInit.Menu.Item("ElTristana.LaneClear.E.Mana").GetValue<Slider>().Value)
            {
                foreach (var minion in
                    ObjectManager.Get<Obj_AI_Minion>().OrderByDescending(m => m.Health))
                {
                    spells[Spells.E].Cast(minion);
                }
            }

            var eminion =
                minions.Find(x => x.HasBuff("TristanaECharge") && x.IsValidTarget(1000));

            if (eminion != null)
            {
                Orbwalker.ForceTarget(eminion);
            }

            if (spells[Spells.Q].IsReady() && IsActive("ElTristana.LaneClear.Q"))
            {
                var eMob = minions.FindAll(x => x.IsValidTarget() && x.HasBuff("TristanaECharge")).FirstOrDefault();
                if (eMob != null)
                {
                    Orbwalker.ForceTarget(eMob);
                    spells[Spells.Q].Cast();
                }
            }
        }

        private static void OnSpellCast(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None && args.Slot == SpellSlot.W
                && IsActive("ElTristana.DumbRetards"))
            {
                args.Process = false;
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            try
            {
                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        OnCombo();
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        OnLaneClear();
                        OnJungleClear();
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        OnHarass();
                        break;
                }

                spells[Spells.Q].Range = 550 + 9 * (Player.Level - 1);
                spells[Spells.E].Range = 625 + 9 * (Player.Level - 1);
                spells[Spells.R].Range = 517 + 9 * (Player.Level - 1);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (!Player.IsWindingUp)
            {
                damage += (float)ObjectManager.Player.GetAutoAttackDamage(enemy, true);
            }

            if (enemy.HasBuff("tristanaecharge"))
            {
                damage += (float)(spells[Spells.E].GetDamage(enemy) * (enemy.GetBuffCount("tristanaecharge") * 0.30)) +
                          spells[Spells.E].GetDamage(enemy);
            }

            if (spells[Spells.R].IsReady())
            {
                damage += spells[Spells.R].GetDamage(enemy);
            }

            return damage;
        }

        private static void UseItems(Obj_AI_Base target)
        {
            var botrk = ItemData.Blade_of_the_Ruined_King.GetItem();
            var ghost = ItemData.Youmuus_Ghostblade.GetItem();
            var cutlass = ItemData.Bilgewater_Cutlass.GetItem();

            var useBladeEhp = MenuInit.Menu.Item("ElTristana.Items.Blade.EnemyEHP").GetValue<Slider>().Value;
            var useBladeMhp = MenuInit.Menu.Item("ElTristana.Items.Blade.EnemyMHP").GetValue<Slider>().Value;

            if (botrk.IsReady() && botrk.IsOwned(Player) && botrk.IsInRange(target)
                && target.HealthPercent <= useBladeEhp && IsActive("ElTristana.Items.Blade"))
            {
                botrk.Cast(target);
            }

            if (botrk.IsReady() && botrk.IsOwned(Player) && botrk.IsInRange(target)
                && Player.HealthPercent <= useBladeMhp && IsActive("ElTristana.Items.Blade"))
            {
                botrk.Cast(target);
            }

            if (cutlass.IsReady() && cutlass.IsOwned(Player) && cutlass.IsInRange(target)
                && target.HealthPercent <= useBladeEhp && IsActive("ElTristana.Items.Blade"))
            {
                cutlass.Cast(target);
            }

            if (ghost.IsReady() && ghost.IsOwned(Player) && target.IsValidTarget(spells[Spells.Q].Range)
                && IsActive("ElTristana.Items.Youmuu"))
            {
                ghost.Cast();
            }
        }

        #endregion
    }
}