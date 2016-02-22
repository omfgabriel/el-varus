namespace ElNamiBurrito
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal class Nami
    {
        #region Static Fields

        public static Orbwalking.Orbwalker Orbwalker;

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                             {
                                                                 { Spells.Q, new Spell(SpellSlot.Q, 875) },
                                                                 { Spells.W, new Spell(SpellSlot.W, 725) },
                                                                 { Spells.E, new Spell(SpellSlot.E, 800) },
                                                                 { Spells.R, new Spell(SpellSlot.R, 2750) }
                                                             };

        private static SpellSlot _ignite;

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

        public static void Game_OnGameLoad(EventArgs args)
        {
            if (!Player.ChampionName.Equals("Nami", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            Notifications.AddNotification("ElNamiReborn by jQuery v1.0.0.2", 5000);

            spells[Spells.Q].SetSkillshot(1f, 150f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            spells[Spells.R].SetSkillshot(0.5f, 260f, 850f, false, SkillshotType.SkillshotLine);

            _ignite = Player.GetSpellSlot("summonerdot");

            ElNamiMenu.Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawings.Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        #endregion

        #region Methods

        private static void AllyHealing()
        {
            if (ObjectManager.Player.IsRecalling() || ObjectManager.Player.InFountain())
            {
                return;
            }

            var useHeal = ElNamiMenu._menu.Item("ElNamiReborn.Heal.Ally.HP").GetValue<bool>();
            var allyHp = ElNamiMenu._menu.Item("ElNamiReborn.Heal.Ally.HP.Percentage").GetValue<Slider>().Value;
            var minumumMana = ElNamiMenu._menu.Item("ElNamiReborn.Heal.Mana").GetValue<Slider>().Value;

            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsAlly && !h.IsMe))
            {
                if (hero.IsRecalling() || hero.InFountain())
                {
                    return;
                }

                if (useHeal && (hero.Health / hero.MaxHealth) * 100 <= allyHp && spells[Spells.W].IsReady()
                    && hero.Distance(Player.ServerPosition) <= spells[Spells.W].Range
                    && Player.ManaPercent >= minumumMana)
                {
                    spells[Spells.W].Cast(hero);
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!gapcloser.Sender.IsValidTarget(spells[Spells.Q].Range))
            {
                return;
            }

            if (gapcloser.Sender.Distance(Player) > spells[Spells.Q].Range)
            {
                return;
            }

            var useQ = ElNamiMenu._menu.Item("ElNamiReborn.Interupt.Q").IsActive();
            var useR = ElNamiMenu._menu.Item("ElNamiReborn.Interupt.R").IsActive();

            if (gapcloser.Sender.IsValidTarget(spells[Spells.Q].Range))
            {
                if (useQ && spells[Spells.Q].IsReady())
                {
                    spells[Spells.Q].Cast(gapcloser.Sender);
                }

                if (useR && !spells[Spells.Q].IsReady() && spells[Spells.R].IsReady())
                {
                    spells[Spells.R].Cast(gapcloser.Sender);
                }
            }
        }

        private static void Combo(Obj_AI_Base target)
        {
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            var useQ = ElNamiMenu._menu.Item("ElNamiReborn.Combo.Q").IsActive();
            var useW = ElNamiMenu._menu.Item("ElNamiReborn.Combo.W").IsActive();
            var useE = ElNamiMenu._menu.Item("ElNamiReborn.Combo.E").IsActive();
            var useR = ElNamiMenu._menu.Item("ElNamiReborn.Combo.R").IsActive();
            var useIgnite = ElNamiMenu._menu.Item("ElNamiReborn.Combo.Ignite").IsActive();
            var countR = ElNamiMenu._menu.Item("ElNamiReborn.Combo.R.Count").GetValue<Slider>().Value;

            if (useQ && spells[Spells.Q].IsReady())
            {
                var prediction = spells[Spells.Q].GetPrediction(target);

                if (prediction.Hitchance >= HitChance.High)
                {
                    spells[Spells.Q].Cast(target);
                }
            }

            if (useE && spells[Spells.E].IsReady())
            {
                var selectedAlly =
                    HeroManager.Allies.Where(
                        hero =>
                        hero.IsAlly
                        && ElNamiMenu._menu.Item("ElNamiReborn.Settings.E1" + hero.CharData.BaseSkinName).IsActive())
                        .OrderBy(closest => closest.Distance(target))
                        .FirstOrDefault();

                if (spells[Spells.E].IsInRange(selectedAlly) && spells[Spells.E].IsReady())
                {
                    spells[Spells.E].CastOnUnit(selectedAlly);
                }
                else
                {
                    spells[Spells.E].Cast();
                }
            }

            if (useW && spells[Spells.W].IsReady())
            {
                spells[Spells.W].Cast(target);
            }


            if (useR && spells[Spells.R].IsReady())
            {
                foreach (
                    var x in
                        HeroManager.Enemies.Where((hero => !hero.IsDead && hero.IsValidTarget(spells[Spells.R].Range))))
                {
                    var pred = spells[Spells.R].GetPrediction(x);
                    if (pred.AoeTargetsHitCount >= countR)
                    {
                        spells[Spells.R].Cast(pred.CastPosition);
                    }
                }
            }

            if (Player.Distance(target) <= 600 && IgniteDamage(target) >= target.Health && useIgnite)
            {
                Player.Spellbook.CastSpell(_ignite, target);
            }
        }

        private static void Harass(Obj_AI_Base target)
        {
            if (target == null || !target.IsValidTarget() || target.IsMinion)
            {
                return;
            }

            var useQ = ElNamiMenu._menu.Item("ElNamiReborn.Harass.Q").IsActive();
            var useW = ElNamiMenu._menu.Item("ElNamiReborn.Harass.W").IsActive();
            var useE = ElNamiMenu._menu.Item("ElNamiReborn.Harass.E").IsActive();
            var checkMana = ElNamiMenu._menu.Item("ElNamiReborn.Harass.Mana").GetValue<Slider>().Value;

            if (Player.ManaPercent < checkMana)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady())
            {
                var prediction = spells[Spells.Q].GetPrediction(target);
                if (prediction.Hitchance >= HitChance.High)
                {
                    spells[Spells.Q].Cast(prediction.CastPosition);
                }
            }

            if (useE && spells[Spells.E].IsReady())
            {
                var selectedAlly =
                    HeroManager.Allies.Where(
                        hero =>
                        hero.IsAlly
                        && ElNamiMenu._menu.Item("ElNamiReborn.Settings.E1" + hero.CharData.BaseSkinName).IsActive())
                        .OrderBy(closest => closest.Distance(target))
                        .FirstOrDefault();

                if (spells[Spells.E].IsInRange(selectedAlly) && spells[Spells.E].IsReady())
                {
                    spells[Spells.E].CastOnUnit(selectedAlly);
                }
            }

            if (useW && spells[Spells.W].IsReady())
            {
                spells[Spells.W].Cast(target);
            }
        }

        private static float IgniteDamage(Obj_AI_Base target)
        {
            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
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
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo(target);
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass(target);
                    break;
            }

            PlayerHealing();
            AllyHealing();
        }

        private static void PlayerHealing()
        {
            if (Player.IsRecalling() || Player.InFountain())
            {
                return;
            }

            var useHeal = ElNamiMenu._menu.Item("ElNamiReborn.Heal.Activate").IsActive();
            var playerHp = ElNamiMenu._menu.Item("ElNamiReborn.Heal.Player.HP").GetValue<Slider>().Value;
            var minumumMana = ElNamiMenu._menu.Item("ElNamiReborn.Heal.Mana").GetValue<Slider>().Value;

            if (useHeal && (Player.Health / Player.MaxHealth) * 100 <= playerHp && spells[Spells.W].IsReady()
                && ObjectManager.Player.ManaPercent >= minumumMana)
            {
                spells[Spells.W].Cast();
            }
        }

        #endregion
    }
}