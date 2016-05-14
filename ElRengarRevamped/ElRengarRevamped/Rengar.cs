namespace ElRengarRevamped
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    public enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal class Rengar : Standards
    {
        #region Static Fields

        public static int LastAutoAttack, Lastrengarq;

        public static int LastQ, LastE, LastW, LastSpell;

        public static Obj_AI_Base SelectedEnemy;

        #endregion

        #region Properties

        private static IEnumerable<Obj_AI_Hero> Enemies => HeroManager.Enemies;

        #endregion

        #region Public Methods and Operators

        public static void OnClick(WndEventArgs args)
        {
            if (args.Msg != (uint)WindowsMessages.WM_LBUTTONDOWN)
            {
                return;
            }
            var unit2 =
                ObjectManager.Get<Obj_AI_Base>()
                    .FirstOrDefault(
                        a =>
                        (a.IsValid<Obj_AI_Hero>()) && a.IsEnemy && a.Distance(Game.CursorPos) < a.BoundingRadius + 80
                        && a.IsValidTarget());
            if (unit2 != null)
            {
                SelectedEnemy = unit2;
            }
        }

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
                    "[00:01] <font color='#f9eb0b'>HEEEEEEY!</font> Use ElUtilitySuite for optimal results! xo jQuery");
                spells[Spells.E].SetSkillshot(0.25f, 70f, 1500f, true, SkillshotType.SkillshotLine);

                MenuInit.Initialize();
                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                CustomEvents.Unit.OnDash += OnDash;
                Drawing.OnEndScene += OnDrawEndScene;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
                Orbwalking.AfterAttack += AfterAttack;
                Orbwalking.BeforeAttack += BeforeAttack;
                Game.OnWndProc += OnClick;
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
                    if (target.IsValidTarget(spells[Spells.Q].Range))
                    {
                        spells[Spells.Q].Cast();
                    }
                }
                ActiveModes.CastItems(enemy);
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
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && !HasPassive && spells[Spells.Q].IsReady()
                    && !(IsListActive("Combo.Prio").SelectedIndex == 0
                         || IsListActive("Combo.Prio").SelectedIndex == 1 && Ferocity == 5))
                {
                    var x = Prediction.GetPrediction(args.Target as Obj_AI_Base, Player.AttackCastDelay * 1000);
                    if (Player.Position.To2D().Distance(x.UnitPosition.To2D())
                        >= Player.BoundingRadius + Player.AttackRange + args.Target.BoundingRadius)
                    {
                        args.Process = false;
                        spells[Spells.Q].Cast();
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
                if (Player.IsRecalling() || Player.InFountain() || Ferocity <= 4 || RengarR)
                {
                    return;
                }

                if (IsActive("Heal.AutoHeal")
                    && (Player.Health / Player.MaxHealth) * 100
                    <= MenuInit.Menu.Item("Heal.HP").GetValue<Slider>().Value && spells[Spells.W].IsReady())
                {
                    spells[Spells.W].Cast();
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
                if (!IsActive("Killsteal.On"))
                {
                    return;
                }

                var target =
                    Enemies.FirstOrDefault(
                        x => x.IsValidTarget(spells[Spells.W].Range) && x.Health < spells[Spells.W].GetDamage(x));

                if (target != null)
                {
                    spells[Spells.W].Cast();
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

                var target = TargetSelector.GetTarget(1500f, TargetSelector.DamageType.Physical);
                if (!target.IsValidTarget())
                {
                    return;
                }

                if (Ferocity == 5)
                {
                    switch (IsListActive("Combo.Prio").SelectedIndex)
                    {
                        case 0:
                            if (spells[Spells.E].IsReady())
                            {
                                var targetE = TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Physical);
                                if (targetE.IsValidTarget())
                                {
                                    spells[Spells.E].Cast(targetE);
                                }
                            }
                            break;
                        case 2:
                            if (spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                            {
                                spells[Spells.Q].Cast();
                            }

                            if (target.IsValidTarget(spells[Spells.Q].Range))
                            {
                                ActiveModes.CastItems(target);
                                Utility.DelayAction.Add(
                                    50,
                                    () =>
                                        {
                                            if (target.IsValidTarget(spells[Spells.W].Range))
                                            {
                                                spells[Spells.W].Cast();
                                            }

                                            spells[Spells.E].Cast(target);
                                        });
                            }

                            break;
                    }
                }

                ActiveModes.CastItems(target);

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
                        if (IsActive("Beta.Cast.Q") && RengarR)
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

                if (SelectedEnemy.IsValidTarget() && SelectedEnemy.IsVisible && !SelectedEnemy.IsDead)
                {
                    Drawing.DrawText(
                        Drawing.WorldToScreen(SelectedEnemy.Position).X - 40,
                        Drawing.WorldToScreen(SelectedEnemy.Position).Y + 10,
                        Color.White,
                        "Selected Target");
                }

                if (IsActive("Misc.Drawings.Off"))
                {
                    return;
                }

                if (IsActive("Beta.Cast.Q"))
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
                            if (Items.CanUseItem(3142))
                            {
                                Utility.DelayAction.Add(2000, () => Items.UseItem(3142));
                            }
                            break;

                        case "RengarQ":
                            LastQ = Environment.TickCount;
                            break;

                        case "RengarE":
                            LastE = Environment.TickCount;
                            break;

                        case "RengarW":
                            LastW = Environment.TickCount;
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
                SmiteCombo();
                Heal();
                KillstealHandler();

                if (MenuInit.Menu.Item("Combo.TripleQ").GetValue<KeyBind>().Active)
                {
                    Orbwalking.Orbwalk(null, Game.CursorPos);

                    var target = TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Physical);
                    if (!target.IsValidTarget())
                    {
                        return;
                    }

                    Orbwalking.Orbwalk(target, Game.CursorPos);

                    if (RengarR)
                    {
                        if (Ferocity == 5 && Player.Distance(target) <= spells[Spells.Q].Range)
                        {
                            spells[Spells.Q].Cast();
                        }
                    }
                    else
                    {
                        spells[Spells.Q].Cast();
                    }

                    if (Ferocity <= 4)
                    {
                        if (Player.Distance(target) <= spells[Spells.Q].Range)
                        {
                            spells[Spells.Q].Cast();
                        }
                        if (Player.Distance(target) <= spells[Spells.W].Range)
                        {
                            spells[Spells.W].Cast();
                        }
                        if (Player.Distance(target) <= spells[Spells.E].Range)
                        {
                            spells[Spells.E].Cast(target);
                        }
                    }
                }

                if (IsActive("Beta.Cast.Q") && IsListActive("Combo.Prio").SelectedIndex == 2)
                {
                    if (IsActive("Beta.Cast.Youmuu") && !Items.HasItem(3142))
                    {
                        return;
                    }

                    var searchrange = MenuInit.Menu.Item("Beta.searchrange").GetValue<Slider>().Value;
                    var target =
                        ObjectManager.Get<Obj_AI_Hero>()
                            .FirstOrDefault(h => h.IsEnemy && h.IsValidTarget(searchrange, false));
                    if (!target.IsValidTarget())
                    {
                        return;
                    }

                    if (Ferocity == 5 && RengarR)
                    {
                        if (target.Distance(Player.ServerPosition)
                            <= MenuInit.Menu.Item("Beta.searchrange.Q").GetValue<Slider>().Value)
                        {
                            Utility.DelayAction.Add(
                                MenuInit.Menu.Item("Beta.Cast.Q.Delay").GetValue<Slider>().Value,
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