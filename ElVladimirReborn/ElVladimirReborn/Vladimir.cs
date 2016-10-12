namespace ElVladimirReborn
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

    internal static class Vladimir
    {
        #region Static Fields

        public static Orbwalking.Orbwalker Orbwalker;

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                             {
                                                                     { Spells.Q, new Spell(SpellSlot.Q, 600) },
                                                                     { Spells.W, new Spell(SpellSlot.W) },
                                                                     { Spells.E, new Spell(SpellSlot.E, 600) },
                                                                     { Spells.R, new Spell(SpellSlot.R, 700) }
                                                             };

        private static SpellSlot ignite;

        #endregion

        #region Properties

        private static Obj_AI_Hero Player => ObjectManager.Player;

        #endregion

        #region Public Methods and Operators

        public static void OnLoad(EventArgs args)
        {
            if (ObjectManager.Player.CharData.BaseSkinName != "Vladimir") return;

            spells[Spells.Q].SetTargetted(0.25f, spells[Spells.Q].Instance.SData.MissileSpeed);
            spells[Spells.R].SetSkillshot(0.25f, 175, 700, false, SkillshotType.SkillshotCircle);
            ignite = Player.GetSpellSlot("summonerdot");

            ElVladimirMenu.Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawings.OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        #endregion

        #region Methods

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var gapCloserActive = ElVladimirMenu.Menu.Item("ElVladimir.Settings.AntiGapCloser.Active").GetValue<bool>();

            if (gapCloserActive && spells[Spells.W].IsReady()
                && (gapcloser.Sender.Distance(Player) < spells[Spells.W].Range)
                && (Player.CountEnemiesInRange(spells[Spells.Q].Range) >= 1)) spells[Spells.W].Cast();
        }

        private static void AreaOfEffectUltimate()
        {
            if (CheckMenu("ElVladimir.Combo.R") && spells[Spells.R].IsReady())
            {
                var target = TargetSelector.GetTarget(spells[Spells.R].Range, TargetSelector.DamageType.Magical);
                if (target == null)
                {
                    return;
                }

                var hits = HeroManager.Enemies.Where(x => x.Distance(target) <= 400f).ToList();
                if (
                    hits.Any(
                        hit =>
                            hits.Count
                            >= ElVladimirMenu.Menu.Item("ElVladimir.Combo.Count.R").GetValue<Slider>().Value))
                {
                    var pred = spells[Spells.R].GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        spells[Spells.R].Cast(pred.CastPosition);
                    }
                }
            }
        }

        private static bool CheckMenu(string menuName)
        {
            return ElVladimirMenu.Menu.Item(menuName).IsActive();
        }

        /// <summary>
        ///     Gets the targets that can be hit with E
        /// </summary>
        /// <returns></returns>
        private static Tuple<int, List<Obj_AI_Hero>> GetEHits()
        {
            var hits =
                    HeroManager.Enemies.Where(
                        e =>
                            (e.IsValidTarget() && (e.Distance(ObjectManager.Player) < spells[Spells.E].Width * 0.8f))
                            || (e.Distance(ObjectManager.Player) < spells[Spells.E].Width)).ToList();

            return new Tuple<int, List<Obj_AI_Hero>>(hits.Count, hits);
        }

        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null) return;

            if (CheckMenu("ElVladimir.Combo.Q") && spells[Spells.Q].IsReady()
                && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].CastOnUnit(target);
            }

            if (CheckMenu("ElVladimir.Combo.E"))
            {
                if (spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range + 200))
                {
                    spells[Spells.E].Cast();
                }

                // add minimum duration to hold the E
                // duration before auto cast is 1500MS
                if (Player.HasBuff("VladimirE")
                    && (target.IsValidTarget(spells[Spells.E].Range) || (GetEHits().Item1 > 0)))
                {
                    Player.Spellbook.UpdateChargedSpell(spells[Spells.E].Slot, Player.Position, true, false);
                    Player.Spellbook.CastSpell(spells[Spells.E].Slot, Player.Position, false);
                }
            }

            if (CheckMenu("ElVladimir.Combo.W") && spells[Spells.W].IsReady()
                && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast(Game.CursorPos);
            }

            if (CheckMenu("ElVladimir.Combo.R.Killable"))
                if (CheckMenu("ElVladimir.Combo.SmartUlt"))
                {
                    if (spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range)
                        && (spells[Spells.Q].GetDamage(target) >= target.Health))
                    {
                        spells[Spells.Q].Cast();
                    }

                    if (spells[Spells.R].IsReady() && (spells[Spells.R].GetDamage(target) >= target.Health)
                        && !target.IsDead)
                    {
                        var pred = spells[Spells.R].GetPrediction(target);
                        if (pred.Hitchance >= HitChance.VeryHigh)
                        {
                            spells[Spells.R].Cast(pred.CastPosition);
                        }
                    }
                }

            if (CheckMenu("ElVladimir.Combo.Ignite") && (Player.Distance(target) <= 600)
                && (Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) >= target.Health))
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static void OnHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null) return;

            if (CheckMenu("ElVladimir.Harass.Q") && spells[Spells.Q].IsReady()
                && target.IsValidTarget(spells[Spells.Q].Range)) spells[Spells.Q].CastOnUnit(target);

            if (CheckMenu("ElVladimir.Harass.E") && spells[Spells.E].IsReady() && target.IsValidTarget(800)) // 
                if (Player.Distance(target) < 800) spells[Spells.E].StartCharging(Game.CursorPos);
        }

        private static void OnJungleClear()
        {
            var useQ = ElVladimirMenu.Menu.Item("ElVladimir.JungleClear.Q").GetValue<bool>();
            var useE = ElVladimirMenu.Menu.Item("ElVladimir.JungleClear.E").GetValue<bool>();
            var playerHp = ElVladimirMenu.Menu.Item("ElVladimir.WaveClear.Health.E").GetValue<Slider>().Value;

            if (spells[Spells.Q].IsReady() && useQ)
            {
                var allMinions = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    spells[Spells.W].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);
                {
                    foreach (var minion in
                        allMinions.Where(minion => minion.IsValidTarget()))
                    {
                        spells[Spells.Q].CastOnUnit(minion);
                        return;
                    }
                }
            }

            if (spells[Spells.E].IsReady() && (Player.Health / Player.MaxHealth * 100 >= playerHp) && useE)
            {
                var minions = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    spells[Spells.W].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);
                if (minions.Count <= 0) return;

                if (minions.Count > 1) spells[Spells.E].StartCharging();
            }
        }

        private static void OnLaneClear()
        {
            if (spells[Spells.Q].IsReady() && ElVladimirMenu.Menu.Item("ElVladimir.WaveClear.Q").GetValue<bool>())
            {
                var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, spells[Spells.Q].Range);
                {
                    foreach (var minion in
                        allMinions.Where(
                            minion =>
                                (minion.Health <= spells[Spells.Q].GetDamage(minion))
                                && minion.IsValidTarget(spells[Spells.Q].Range)))
                    {
                        spells[Spells.Q].CastOnUnit(minion);
                        return;
                    }
                }
            }

            if (spells[Spells.E].IsReady()
                && (Player.Health / Player.MaxHealth * 100
                    >= ElVladimirMenu.Menu.Item("ElVladimir.WaveClear.Health.E").GetValue<Slider>().Value)
                && ElVladimirMenu.Menu.Item("ElVladimir.WaveClear.E").GetValue<bool>())
            {
                var minions = MinionManager.GetMinions(Player.ServerPosition, 800);
                if (minions.Count <= 0) return;

                if (minions.Count > 1) spells[Spells.E].StartCharging();
            }
        }

        private static void OnLasthit()
        {
            var minion =
                MinionManager.GetMinions(spells[Spells.Q].Range)
                    .Where(obj => spells[Spells.Q].IsKillable(obj))
                    .MinOrDefault(obj => obj.Health);

            if ((minion != null) && ElVladimirMenu.Menu.Item("ElVladimir.WaveClear.Q.Last").GetValue<bool>()) spells[Spells.Q].Cast(minion);
        }

        private static void OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    OnCombo();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    OnHarass();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    OnLaneClear();
                    OnJungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    OnLasthit();
                    break;
            }

            AreaOfEffectUltimate();
        }

        #endregion
    }
}