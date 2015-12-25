namespace ElRengarRevamped
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

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

        public static Obj_AI_Base SelectedEnemy;

        public static bool JustDoIt;

        public static int LastQ, LastE, LastW;

        private static int lastSpell;

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
                Notifications.AddNotification(string.Format("ElRengarRevamped by jQuery v{0}", ScriptVersion), 6000);
                Game.PrintChat(
                    "[00:00] <font color='#CC0000'>DATABASE!</font> Make sure to update in the database! (only if you liked it!)");
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
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (unit.IsMe && spells[Spells.Q].IsReady() && target is Obj_AI_Hero && target.IsValidTarget(spells[Spells.Q].Range))
                {
                    spells[Spells.Q].Cast();
                }
            }
        }

        private static void Heal()
        {
            if (Player.IsRecalling() || Player.InFountain() || Ferocity <= 4 || RengarR)
            {
                return;
            }

            if (IsActive("Heal.AutoHeal")
                && (Player.Health / Player.MaxHealth) * 100 <= MenuInit.Menu.Item("Heal.HP").GetValue<Slider>().Value
                && spells[Spells.W].IsReady())
            {
                spells[Spells.W].Cast();
            }
        }

        private static void OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (!sender.IsMe)
                return;

            var target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }

            if (sender.IsMe)
            {
                Orbwalking.LastAATick = Utils.GameTimeTickCount - Game.Ping / 2 - (int)Player.AttackCastDelay * 1000 + args.Duration;
            }

            if (sender.IsMe && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (Ferocity == 5 && Player.IsDashing())
                {
                    switch (IsListActive("Combo.Prio").SelectedIndex)
                    {
                        case 0:
                            if (spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
                            {
                                spells[Spells.E].Cast(target.ServerPosition);
                            }
                            break;
                        case 2:
                            if (spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                            {
                                spells[Spells.Q].Cast();
                            }
                            break;
                    }
                }

                if (JustDoIt)
                {
                    if (spells[Spells.E].IsReady())
                    {
                        spells[Spells.E].Cast(target.ServerPosition);
                    }

                    if (!spells[Spells.E].IsReady() && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                    {
                        spells[Spells.Q].Cast();
                    }
                }

                switch (IsListActive("Combo.Prio").SelectedIndex)
                {
                    case 0:
                        if (Ferocity == 5)
                        {
                            if (spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
                            {
                                spells[Spells.E].Cast(target.ServerPosition);
                            }
                        }
                        break;

                    case 1:
                        if (IsActive("Beta.Cast.Q") && RengarR)
                        {
                            spells[Spells.Q].Cast();
                        }

                        spells[Spells.E].Cast(target);

                        if (target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            Utility.DelayAction.Add(
                                50,
                                () =>
                                    {
                                        if (target.IsValidTarget(spells[Spells.W].Range))
                                        {
                                            spells[Spells.W].Cast();
                                        }

                                        spells[Spells.E].Cast(target);
                                        UseHydra();
                                    });
                        }
                        
                        break;
                }

                if (target.IsValidTarget(spells[Spells.W].Range))
                {
                    UseHydra();
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            var drawW = MenuInit.Menu.Item("Misc.Drawings.W").GetValue<Circle>();
            var drawE = MenuInit.Menu.Item("Misc.Drawings.E").GetValue<Circle>();
            var drawExclamation = MenuInit.Menu.Item("Misc.Drawings.Exclamation").GetValue<Circle>(); //Exclamation mark


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
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, 1250f, Color.DeepSkyBlue);
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

        private static void OnDrawEndScene(EventArgs args)
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


        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "RengarR")
                {
                    if (Items.CanUseItem(3142))
                    {
                        Utility.DelayAction.Add(1500, () => Items.UseItem(3142));
                    }
                }

                if (args.SData.Name.Contains("Attack") && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    //Tiamat Hydra cast after AA  - Credits to Kurisu 
                    Utility.DelayAction.Add(
                        50 + (int)(Player.AttackDelay * 100) + Game.Ping / 2 + 10,
                        delegate
                            {
                                if (Items.CanUseItem(3077))
                                {
                                    Items.UseItem(3077);
                                }
                                if (Items.CanUseItem(3074))
                                {
                                    Items.UseItem(3074);
                                }
                            });
                }

                switch (args.SData.Name.ToLower())
                {
                    case "rengarq":
                        LastQ = Environment.TickCount;
                        lastSpell = Environment.TickCount;
                        break;

                    case "rengare":
                        LastE = Environment.TickCount;
                        lastSpell = Environment.TickCount;
                        break;

                    case "rengarw":
                        LastW = Environment.TickCount;
                        lastSpell = Environment.TickCount;
                        break;
                }
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
                SwitchCombo();
                Heal();
                KillstealHandler();
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

                if (IsActive("Beta.Cast.Q") && IsListActive("Combo.Prio").SelectedIndex == 2)
                {
                    if (IsActive("Beta.Cast.Youmuu") && !Items.HasItem(3142))
                    {
                        return;
                    }

                    var searchrange = MenuInit.Menu.Item("Beta.searchrange").GetValue<Slider>().Value;
                    var target =
                        ObjectManager.Get<Obj_AI_Hero>()
                            .FirstOrDefault(h => h.IsEnemy && !h.IsDead && !h.IsZombie && h.IsValidTarget(searchrange));

                    if (!target.IsValidTarget())
                    {
                        return;
                    }

                    if (Ferocity == 5 && RengarR)
                    {
                        if (target.IsValidTarget(MenuInit.Menu.Item("Beta.searchrange.Q").GetValue<Slider>().Value))
                        {
                            Utility.DelayAction.Add(MenuInit.Menu.Item("Beta.Cast.Q.Delay").GetValue<Slider>().Value, () => spells[Spells.Q].Cast());
                            JustDoIt = true;
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


        private static IEnumerable<Obj_AI_Hero> Enemies
        {
            get
            {
                return HeroManager.Enemies;
            }
        }

        private static void KillstealHandler()
        {
            if (!IsActive("Killsteal.On"))
            {
                return;
            }

            var target = Enemies.FirstOrDefault(x => x.IsValidTarget(spells[Spells.W].Range) && x.Health < spells[Spells.W].GetDamage(x));
            if (target != null)
            {
                spells[Spells.W].Cast();
            }
        }

        private static void SwitchCombo()
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

        #endregion
    }
}