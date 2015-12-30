namespace ElAlistarReborn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    internal enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal static class Alistar
    {
        #region Static Fields

        public static Orbwalking.Orbwalker Orbwalker;

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                             {
                                                                 { Spells.Q, new Spell(SpellSlot.Q, 365) },
                                                                 { Spells.W, new Spell(SpellSlot.W, 650) },
                                                                 { Spells.E, new Spell(SpellSlot.E, 575) },
                                                                 { Spells.R, new Spell(SpellSlot.R, 0) }
                                                             };

        private static SpellSlot flashSlot;

        private static SpellSlot ignite;

        #endregion

        #region Properties

        private static Obj_AI_Hero Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #endregion

        #region Public Methods and Operators

        public static void OnLoad(EventArgs args)
        {
            if (ObjectManager.Player.CharData.BaseSkinName != "Alistar")
            {
                return;
            }

            spells[Spells.W].SetTargetted(0.5f, 1.5f);

            ignite = Player.GetSpellSlot("summonerdot");
            flashSlot = Player.GetSpellSlot("summonerflash");

            Game.PrintChat(
                "[00:01] <font color='#f9eb0b'>HEEEEEEY!</font> Use ElUtilitySuite for optimal results! xo jQuery");

            ElAlistarMenu.Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawings.OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        #endregion

        #region Methods

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var gapCloserActive = ElAlistarMenu.Menu.Item("ElAlistar.Interrupt").GetValue<bool>();

            if (gapCloserActive && spells[Spells.W].IsReady()
                && gapcloser.Sender.Distance(Player) < spells[Spells.W].Range)
            {
                spells[Spells.W].Cast(gapcloser.Sender);
            }

            if (gapCloserActive && !spells[Spells.W].IsReady() && spells[Spells.Q].IsReady()
                && gapcloser.Sender.Distance(Player) < spells[Spells.Q].Range)
            {
                spells[Spells.Q].Cast(gapcloser.Sender);
            }
        }

        private static float GetWDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (spells[Spells.W].IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);
            }

            return (float)damage;
        }

        private static void HealManager()
        {
            var useHeal = ElAlistarMenu.Menu.Item("ElAlistar.Heal.Activated").IsActive();
            var useHealAlly = ElAlistarMenu.Menu.Item("ElAlistar.Heal.Ally.Activated").IsActive();
            var playerMana = ElAlistarMenu.Menu.Item("ElAlistar.Heal.Player.Mana").GetValue<Slider>().Value;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Player.IsRecalling() || Player.InFountain()
                || Player.Mana < playerMana)
            {
                return;
            }

            if (!spells[Spells.E].IsReady() || !useHeal)
            {
                return;
            }

            var playerHp = ElAlistarMenu.Menu.Item("ElAlistar.Heal.Player.HP").GetValue<Slider>().Value;
            var allyHp = ElAlistarMenu.Menu.Item("ElAlistar.Heal.Ally.HP").GetValue<Slider>().Value;

            if ((Player.Health / Player.MaxHealth) * 100 < playerHp)
            {
                spells[Spells.E].Cast();
            }

            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsAlly && !h.IsMe && !h.IsDead))
            {
                if (useHealAlly && (hero.Health / hero.MaxHealth) * 100 <= allyHp && spells[Spells.E].IsInRange(hero))
                {
                    spells[Spells.E].Cast();
                }
            }
        }

        private static float IgniteDamage(Obj_AI_Hero target)
        {
            if (ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static void Interrupter2_OnInterruptableTarget(
            Obj_AI_Hero sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel != Interrupter2.DangerLevel.High || sender.Distance(Player) > spells[Spells.Q].Range)
            {
                return;
            }

            if (sender.IsValidTarget(spells[Spells.Q].Range) && args.DangerLevel == Interrupter2.DangerLevel.High
                && spells[Spells.Q].IsReady())
            {
                spells[Spells.Q].Cast(sender);
            }

            if (sender.IsValidTarget(spells[Spells.W].Range) && args.DangerLevel == Interrupter2.DangerLevel.High
                && !spells[Spells.Q].IsReady() && spells[Spells.W].IsReady())
            {
                spells[Spells.W].Cast(sender);
            }
        }

        private static void OnCombo()
        {
            // ReSharper disable once ConvertConditionalTernaryToNullCoalescing
            var target = TargetSelector.GetSelectedTarget() != null
                             ? TargetSelector.GetSelectedTarget()
                             : TargetSelector.GetTarget(spells[Spells.W].Range, TargetSelector.DamageType.Physical);

            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            var useQ = ElAlistarMenu.Menu.Item("ElAlistar.Combo.Q").IsActive();
            var useW = ElAlistarMenu.Menu.Item("ElAlistar.Combo.W").IsActive();
            var useR = ElAlistarMenu.Menu.Item("ElAlistar.Combo.R").IsActive();
            var useI = ElAlistarMenu.Menu.Item("ElAlistar.Combo.Ignite").IsActive();
            var enemiesInRange = ElAlistarMenu.Menu.Item("ElAlistar.Combo.Count.Enemies").GetValue<Slider>().Value;
            var rHealth = ElAlistarMenu.Menu.Item("ElAlistar.Combo.HP.Enemies").GetValue<Slider>().Value;

            var qmana = Player.Spellbook.GetSpell(SpellSlot.Q);
            var wmana = Player.Spellbook.GetSpell(SpellSlot.W);

            if (target.IsValidTarget(spells[Spells.W].Range))
            {
                if (useQ && useW && spells[Spells.Q].IsReady() && spells[Spells.W].IsReady()
                    && Player.Mana > qmana.ManaCost + wmana.ManaCost)
                {
                    if (spells[Spells.W].Cast(target).IsCasted())
                    {
                        var comboTime = Math.Max(0, Player.Distance(target) - 365) / 1.2f - 25;
                        Utility.DelayAction.Add((int)comboTime, () => spells[Spells.Q].Cast());
                    }
                }
            }

            if (useR && Player.CountEnemiesInRange(spells[Spells.W].Range) >= enemiesInRange
                && (Player.Health / Player.MaxHealth) * 100 >= rHealth)
            {
                spells[Spells.R].Cast();
            }

            if (spells[Spells.W].IsReady() && !spells[Spells.Q].IsReady() && spells[Spells.W].IsInRange(target)
                && GetWDamage(target) > target.Health)
            {
                spells[Spells.W].Cast();
            }

            if (Player.Distance(target) <= 600 && IgniteDamage(target) >= target.Health && useI)
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static void OnHarass()
        {
            var target = TargetSelector.GetSelectedTarget() != null
                             ? TargetSelector.GetSelectedTarget()
                             : TargetSelector.GetTarget(spells[Spells.W].Range, TargetSelector.DamageType.Physical);

            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            var useQ = ElAlistarMenu.Menu.Item("ElAlistar.Harass.Q").IsActive();

            if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(target);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    OnCombo();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    OnHarass();
                    break;
            }

            if (ElAlistarMenu.Menu.Item("ElAlistar.Combo.FlashQ").GetValue<KeyBind>().Active
                && spells[Spells.Q].IsReady())
            {
                Orbwalk(Game.CursorPos);

                var target = ElAlistarMenu.Menu.Item("ElAlistar.Combo.Click").GetValue<bool>()
                                 ? TargetSelector.GetSelectedTarget()
                                 : TargetSelector.GetTarget(spells[Spells.W].Range, TargetSelector.DamageType.Magical);

                if (!target.IsValidTarget(spells[Spells.W].Range))
                {
                    return;
                }

                Player.Spellbook.CastSpell(flashSlot, target.ServerPosition);
                Utility.DelayAction.Add(50, () => spells[Spells.Q].Cast());
            }

            HealManager();
        }

        private static void Orbwalk(Vector3 pos, Obj_AI_Hero target = null)
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, pos);
        }

        #endregion
    }
}