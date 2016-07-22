namespace ElRengarRevamped
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using TargetSelector = SFXTargetSelector.TargetSelector;

    public enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal class Rengar : Standards
    {
        #region Properties

        private static IEnumerable<Obj_AI_Hero> Enemies => HeroManager.Enemies;

        #endregion

        #region Public Methods and Operators

    
        public static void OnLoad(EventArgs args)
        {
            if (Player.ChampionName != "Rengar")
            {
                return;
            }

            try
            {
                Youmuu = new Items.Item(3142, 0f);

                Ignite = Player.GetSpellSlot("summonerdot");
                Game.PrintChat(
                    "[00:01] <font color='#CC0000'>HEEEEEEY!</font> Use ElUtilitySuite for optimal results! xo jQuery!!");

                spells[Spells.E].SetSkillshot(0.25f, 70f, 1500f, true, SkillshotType.SkillshotLine);

                MenuInit.Initialize();
                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                CustomEvents.Unit.OnDash += OnDash;
                Drawing.OnEndScene += OnDrawEndScene;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
                Orbwalking.AfterAttack += AfterAttack;
                Orbwalking.BeforeAttack += BeforeAttack;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Methods

        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            try
            {
                var enemy = target as Obj_AI_Base;
                if (!unit.IsMe || enemy == null || !(target is Obj_AI_Hero))
                {
                    return;
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                    || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    if (Player.CountEnemiesInRange(Player.AttackRange + Player.BoundingRadius + 100) != 0)
                    {
                        spells[Spells.Q].Cast();
                        ActiveModes.CastItems(enemy);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            try
            {
                if (!IsActive("Combo.Use.QQ"))
                {
                    return;
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && !HasPassive && spells[Spells.Q].IsReady()
                    && !(IsListActive("Combo.Prio").SelectedIndex == 0
                         || IsListActive("Combo.Prio").SelectedIndex == 1 && Ferocity == 5))
                {

                    var targets = TargetSelector.GetTargets(
                                Orbwalking.GetRealAutoAttackRange(null) * 1.25f, DamageType.Physical);
                    if (targets != null)
                    {
                        var target = targets.FirstOrDefault(Orbwalking.InAutoAttackRange);
                        if (target != null)
                        {
                            Orbwalker.ForceTarget(target);
                            args.Process = false;
                            spells[Spells.Q].Cast();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void Heal()
        {
            try
            {
                if (RengarR || Player.IsRecalling() || Player.InFountain() || Ferocity != 5)
                {
                    return;
                }

                if (Player.CountEnemiesInRange(1000) > 1 && spells[Spells.W].IsReady())
                {
                    if (IsActive("Heal.AutoHeal")
                        && (Player.Health / Player.MaxHealth) * 100
                        <= MenuInit.Menu.Item("Heal.HP").GetValue<Slider>().Value)
                    {
                        spells[Spells.W].Cast();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void KillstealHandler()
        {
            try
            {
                if (!IsActive("Killsteal.On") || Player.IsRecalling())
                {
                    return;
                }

                var target = Enemies.FirstOrDefault(x => x.IsValidTarget(spells[Spells.E].Range));
                if (target == null)
                {
                    return;
                }

                if (RengarR)
                {
                    return;
                }

                if (spells[Spells.W].GetDamage(target) > target.Health && target.IsValidTarget(spells[Spells.W].Range))
                {
                    spells[Spells.W].Cast();
                }

                if (spells[Spells.E].GetDamage(target) > target.Health && target.IsValidTarget(spells[Spells.E].Range))
                {
                    var prediction = spells[Spells.E].GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.VeryHigh)
                    {
                        spells[Spells.E].Cast(prediction.CastPosition);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            try
            {
                if (!sender.IsMe || Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                {
                    return;
                }

                var target = TargetSelector.GetTarget(1500f, DamageType.Physical);
                if (!target.IsValidTarget())
                {
                    return;
                }

                if (!RengarR)
                {
                    ActiveModes.CastItems(target);
                }

                if (Ferocity == 5)
                {
                    switch (IsListActive("Combo.Prio").SelectedIndex)
                    {
                        case 0:
                            if (spells[Spells.E].IsReady())
                            {
                                var targetE = TargetSelector.GetTarget(
                                    spells[Spells.E].Range,
                                    DamageType.Physical);
                                if (targetE.IsValidTarget())
                                {
                                    var pred = spells[Spells.E].GetPrediction(targetE);
                                    if (pred.Hitchance >= HitChance.High)
                                    {
                                        spells[Spells.E].Cast(pred.CastPosition);
                                    }
                                }
                            }
                            break;
                        case 2:
                            if (spells[Spells.Q].IsReady()
                                && Player.CountEnemiesInRange(Player.AttackRange + Player.BoundingRadius + 100) != 0)
                            {
                                spells[Spells.Q].Cast();
                            }
                            break;
                    }
                }
                else
                {
                    if (IsListActive("Combo.Prio").SelectedIndex != 0)
                    {
                        if (spells[Spells.E].IsReady())
                        {
                            var targetE = TargetSelector.GetTarget(
                                spells[Spells.E].Range,
                                DamageType.Physical);
                            if (targetE.IsValidTarget(spells[Spells.E].Range))
                            {
                                var pred = spells[Spells.E].GetPrediction(targetE);
                                if (pred.Hitchance >= HitChance.Medium)
                                {
                                    spells[Spells.E].Cast(pred.CastPosition);
                                }
                            }
                        }
                    }
                }

                switch (IsListActive("Combo.Prio").SelectedIndex)
                {
                    case 0:
                        if (spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
                        {
                            var pred = spells[Spells.E].GetPrediction(target);
                            spells[Spells.E].Cast(pred.CastPosition);
                        }
                        break;

                    case 2:
                        if (IsActive("Beta.Cast.Q1") && RengarR)
                        {
                            spells[Spells.Q].Cast();
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            try
            {
                var drawW = MenuInit.Menu.Item("Misc.Drawings.W").GetValue<Circle>();
                var drawE = MenuInit.Menu.Item("Misc.Drawings.E").GetValue<Circle>();
                var drawExclamation = MenuInit.Menu.Item("Misc.Drawings.Exclamation").GetValue<Circle>();
                //Exclamation mark

                var drawSearchRange = MenuInit.Menu.Item("Beta.Search.Range").GetValue<Circle>();
                var searchrange = MenuInit.Menu.Item("Beta.searchrange").GetValue<Slider>().Value;

                var drawsearchrangeQ = MenuInit.Menu.Item("Beta.Search.QCastRange").GetValue<Circle>();
                var searchrangeQCastRange = MenuInit.Menu.Item("Beta.searchrange.Q").GetValue<Slider>().Value;

                if (IsActive("Misc.Drawings.Off"))
                {
                    return;
                }

                if (IsActive("Beta.Cast.Q1"))
                {
                    if (drawSearchRange.Active && spells[Spells.R].Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, searchrange, Color.Orange);
                    }

                    if (drawsearchrangeQ.Active && spells[Spells.R].Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, searchrangeQCastRange, Color.Orange);
                    }
                }

                if (RengarR && drawExclamation.Active)
                {
                    if (spells[Spells.R].Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, 1450f, Color.DeepSkyBlue);
                    }
                }

                if (drawW.Active)
                {
                    if (spells[Spells.W].Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.W].Range, Color.Purple);
                    }
                }

                if (drawE.Active)
                {
                    if (spells[Spells.E].Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.E].Range, Color.White);
                    }
                }

                if (IsActive("Misc.Drawings.Prioritized"))
                {
                    switch (IsListActive("Combo.Prio").SelectedIndex)
                    {
                        case 0:
                            Drawing.DrawText(
                                Drawing.Width * 0.70f,
                                Drawing.Height * 0.95f,
                                Color.Yellow,
                                "Prioritized spell: E");
                            break;
                        case 1:
                            Drawing.DrawText(
                                Drawing.Width * 0.70f,
                                Drawing.Height * 0.95f,
                                Color.White,
                                "Prioritized spell: W");
                            break;
                        case 2:
                            Drawing.DrawText(
                                Drawing.Width * 0.70f,
                                Drawing.Height * 0.95f,
                                Color.White,
                                "Prioritized spell: Q");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnDrawEndScene(EventArgs args)
        {
            try
            {
                if (Player.IsDead)
                {
                    return;
                }

                if (IsActive("Misc.Drawings.Minimap") && spells[Spells.R].Level > 0)
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, spells[Spells.R].Range, Color.White, 1, 23, true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (sender.IsMe)
                {
                    switch (args.SData.Name.ToLower())
                    {
                        case "RengarR":
                            if (Items.HasItem(3142) && Items.CanUseItem(3142))
                            {
                                Utility.DelayAction.Add(2000, () => Items.UseItem(3142));
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            try
            {
                if (Player.IsDead)
                {
                    return;
                }

                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        ActiveModes.Combo();
                        break;

                    case Orbwalking.OrbwalkingMode.LaneClear:
                        ActiveModes.Laneclear();
                        ActiveModes.Jungleclear();
                        break;

                    case Orbwalking.OrbwalkingMode.Mixed:
                        ActiveModes.Harass();
                        break;
                }

                SwitchCombo();
                Heal();
                KillstealHandler();

                // E on Immobile targets
                if (IsActive("Misc.Root") && spells[Spells.E].IsReady())
                {
                    if (RengarR)
                    {
                        return;
                    }

                    var target = HeroManager.Enemies.FirstOrDefault(h => h.IsValidTarget(spells[Spells.E].Range));
                    if (target != null)
                    {
                        if (Ferocity != 5)
                        {
                            return;
                        }

                        spells[Spells.E].CastIfHitchanceEquals(target, HitChance.Immobile);
                    }
                }

                if (IsActive("Beta.Cast.Q1") && IsListActive("Combo.Prio").SelectedIndex == 2)
                {
                    if (Ferocity != 5)
                    {
                        return;
                    }

                    var searchrange = MenuInit.Menu.Item("Beta.searchrange").GetValue<Slider>().Value;
                    var target = HeroManager.Enemies.FirstOrDefault(h => h.IsValidTarget(searchrange, false));
                    if (!target.IsValidTarget())
                    {
                        return;
                    }

                    // Check if Rengar is in ultimate
                    if (RengarR)
                    {
                        // Check if the player distance <= than the set search range
                        if (Player.Distance(target) <= MenuInit.Menu.Item("Beta.searchrange.Q").GetValue<Slider>().Value)
                        {
                            // Cast Q with the set delay
                            Utility.DelayAction.Add(
                                MenuInit.Menu.Item("Beta.Cast.Q1.Delay").GetValue<Slider>().Value,
                                () => spells[Spells.Q].Cast());
                        }
                    }
                }

                spells[Spells.R].Range = 1000 + spells[Spells.R].Level * 1000;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void SwitchCombo()
        {
            try
            {
                var switchTime = Utils.GameTimeTickCount - LastSwitch;
                if (MenuInit.Menu.Item("Combo.Switch").GetValue<KeyBind>().Active && switchTime >= 350)
                {
                    switch (IsListActive("Combo.Prio").SelectedIndex)
                    {
                        case 0:
                            MenuInit.Menu.Item("Combo.Prio").SetValue(new StringList(new[] { "E", "W", "Q" }, 2));
                            LastSwitch = Utils.GameTimeTickCount;
                            break;
                        case 1:
                            MenuInit.Menu.Item("Combo.Prio").SetValue(new StringList(new[] { "E", "W", "Q" }, 0));
                            LastSwitch = Utils.GameTimeTickCount;
                            break;

                        default:
                            MenuInit.Menu.Item("Combo.Prio").SetValue(new StringList(new[] { "E", "W", "Q" }, 0));
                            LastSwitch = Utils.GameTimeTickCount;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
    }
}